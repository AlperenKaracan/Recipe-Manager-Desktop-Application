using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Npgsql;
using System.Collections.ObjectModel;
using System.Linq;
using WpfApp1;

namespace TarifRehberi
{
    public partial class MainWindow : Window
    {
        private string connString = "Host=localhost;Username=postgres;Password=admin;Database=tarif";

        public MainWindow()
        {
            InitializeComponent();
            LoadCategories();
            LoadIngredients();
            LoadRecipes();
            RecipeList.SelectionChanged += OnRecipeSelected;
        }

        private void LoadRecipes()
        {
            ObservableCollection<Recipe> recipeList = new ObservableCollection<Recipe>();

            using (NpgsqlConnection conn = new NpgsqlConnection(connString))
            {
                try
                {
                    conn.Open();
                    int? selectedCategoryID = null;
                    if (FilterCategory != null && FilterCategory.SelectedItem != null)
                    {
                        ComboBoxItem selectedItem = (ComboBoxItem)FilterCategory.SelectedItem;
                        selectedCategoryID = selectedItem.Tag != null ? (int?)Convert.ToInt32(selectedItem.Tag) : null;
                    }

                    int maxSure = 0;
                    if (FilterTime != null && !string.IsNullOrWhiteSpace(FilterTime.Text) && FilterTime.Text != "Max Süre")
                    {
                        int.TryParse(FilterTime.Text, out maxSure);
                    }

                    double maxMaliyet = 0;
                    if (FilterCost != null && !string.IsNullOrWhiteSpace(FilterCost.Text) && FilterCost.Text != "Max Maliyet")
                    {
                        double.TryParse(FilterCost.Text, out maxMaliyet);
                    }

                    string orderByClause = "ORDER BY t.tarifadi";
                    if (SortOption != null && SortOption.SelectedItem != null)
                    {
                        ComboBoxItem selectedSortItem = (ComboBoxItem)SortOption.SelectedItem;
                        string sortValue = selectedSortItem.Tag != null ? selectedSortItem.Tag.ToString() : "Varsayilan";
                        if (sortValue == "SureArtan")
                        {
                            orderByClause = "ORDER BY t.hazirlamasuresi ASC";
                        }
                        else if (sortValue == "SureAzalan")
                        {
                            orderByClause = "ORDER BY t.hazirlamasuresi DESC";
                        }
                        else if (sortValue == "MaliyetArtan")
                        {
                            orderByClause = "ORDER BY toplam_maliyet ASC";
                        }
                        else if (sortValue == "MaliyetAzalan")
                        {
                            orderByClause = "ORDER BY toplam_maliyet DESC";
                        }
                    }

                    string query = $@"
                        SELECT t.tarifid, t.tarifadi, k.kategoriadi, t.hazirlamasuresi,
                               COALESCE(SUM(m.birimfiyat * tm.malzememiktar), 0) AS toplam_maliyet
                        FROM tarifler t
                        INNER JOIN kategoriler k ON t.kategoriid = k.kategoriid
                        LEFT JOIN tarifmalzemeler tm ON t.tarifid = tm.tarifid
                        LEFT JOIN malzemeler m ON tm.malzemeid = m.malzemeid
                        WHERE ( @kategoriID IS NULL OR t.kategoriid = @kategoriID )
                        AND ( @maxSure = 0 OR t.hazirlamasuresi <= @maxSure )
                        GROUP BY t.tarifid, t.tarifadi, k.kategoriadi, t.hazirlamasuresi
                        HAVING ( @maxMaliyet = 0 OR COALESCE(SUM(m.birimfiyat * tm.malzememiktar), 0) <= @maxMaliyet )
                        {orderByClause}";

                    NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
                    var kategoriParam = new NpgsqlParameter("kategoriID", NpgsqlTypes.NpgsqlDbType.Integer);
                    kategoriParam.Value = selectedCategoryID.HasValue ? (object)selectedCategoryID.Value : DBNull.Value;
                    cmd.Parameters.Add(kategoriParam);

                    var maxSureParam = new NpgsqlParameter("maxSure", NpgsqlTypes.NpgsqlDbType.Integer);
                    maxSureParam.Value = maxSure;
                    cmd.Parameters.Add(maxSureParam);

                    var maxMaliyetParam = new NpgsqlParameter("maxMaliyet", NpgsqlTypes.NpgsqlDbType.Double);
                    maxMaliyetParam.Value = maxMaliyet;
                    cmd.Parameters.Add(maxMaliyetParam);

                    NpgsqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        int tarifID = reader.GetInt32(0);
                        string tarifAdi = !reader.IsDBNull(1) ? reader.GetString(1) : string.Empty;
                        string kategoriAdi = !reader.IsDBNull(2) ? reader.GetString(2) : string.Empty;
                        int hazirlamaSuresi = !reader.IsDBNull(3) ? reader.GetInt32(3) : 0;
                        double toplamMaliyet = !reader.IsDBNull(4) ? reader.GetDouble(4) : 0.0;

                        recipeList.Add(new Recipe
                        {
                            TarifID = tarifID,
                            TarifAdi = tarifAdi,
                            KategoriAdi = kategoriAdi,
                            HazirlamaSuresi = hazirlamaSuresi,
                            ToplamMaliyet = toplamMaliyet
                        });
                    }

                    if (RecipeList != null)
                    {
                        RecipeList.ItemsSource = recipeList;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Tarifler yüklenirken bir hata oluştu: {ex.Message}\n{ex.StackTrace}");
                }
            }
        }

        private void LoadCategories()
        {
            FilterCategory.Items.Clear();
            ComboBoxItem defaultItem = new ComboBoxItem
            {
                Content = "Tüm Kategoriler",
                Tag = null
            };
            FilterCategory.Items.Add(defaultItem);

            using (NpgsqlConnection conn = new NpgsqlConnection(connString))
            {
                try
                {
                    conn.Open();
                    NpgsqlCommand cmd = new NpgsqlCommand("SELECT kategoriid, kategoriadi FROM kategoriler", conn);
                    NpgsqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        ComboBoxItem item = new ComboBoxItem
                        {
                            Content = reader.GetString(1),
                            Tag = reader.GetInt32(0)
                        };
                        FilterCategory.Items.Add(item);
                    }
                    FilterCategory.SelectedIndex = 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Kategoriler yüklenirken bir hata oluştu: {ex.Message}");
                }
            }
        }

        private void LoadIngredients()
        {
            IngredientList.Items.Clear();
            using (NpgsqlConnection conn = new NpgsqlConnection(connString))
            {
                try
                {
                    conn.Open();
                    NpgsqlCommand cmd = new NpgsqlCommand("SELECT malzemeid, malzemeadi FROM malzemeler", conn);
                    NpgsqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        CheckBox ingredientCheckbox = new CheckBox
                        {
                            Content = reader.GetString(1),
                            Tag = reader.GetInt32(0)
                        };
                        IngredientList.Items.Add(ingredientCheckbox);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Malzemeler yüklenirken bir hata oluştu: {ex.Message}");
                }
            }
        }

        private void OnSuggestRecipeClick(object sender, RoutedEventArgs e)
        {
            ObservableCollection<int> selectedIngredients = new ObservableCollection<int>();

            foreach (CheckBox item in IngredientList.Items)
            {
                if (item.IsChecked == true)
                {
                    selectedIngredients.Add((int)item.Tag);
                }
            }

            using (NpgsqlConnection conn = new NpgsqlConnection(connString))
            {
                try
                {
                    conn.Open();
                    NpgsqlCommand cmd = new NpgsqlCommand(@"
                        SELECT t.tarifid, t.tarifadi, k.kategoriadi,
                               COUNT(tm.malzemeid) AS toplam_malzeme,
                               SUM(CASE WHEN tm.malzemeid = ANY(@selectedIngredients) THEN 1 ELSE 0 END) AS mevcut_malzeme,
                               COALESCE(SUM(CASE WHEN tm.malzemeid != ANY(@selectedIngredients) THEN m.birimfiyat * tm.malzememiktar ELSE 0 END), 0) AS eksik_malzeme_maliyeti
                        FROM tarifler t
                        INNER JOIN kategoriler k ON t.kategoriid = k.kategoriid
                        INNER JOIN tarifmalzemeler tm ON t.tarifid = tm.tarifid
                        INNER JOIN malzemeler m ON tm.malzemeid = m.malzemeid
                        GROUP BY t.tarifid, t.tarifadi, k.kategoriadi
                        ORDER BY mevcut_malzeme DESC, toplam_malzeme", conn);
                    cmd.Parameters.AddWithValue("selectedIngredients", selectedIngredients.ToArray());
                    NpgsqlDataReader reader = cmd.ExecuteReader();

                    ObservableCollection<RecipeSuggestion> suggestedRecipes = new ObservableCollection<RecipeSuggestion>();

                    while (reader.Read())
                    {
                        int mevcutMalzemeSayisi = reader.GetInt32(4);
                        int toplamMalzemeSayisi = reader.GetInt32(3);

                        suggestedRecipes.Add(new RecipeSuggestion
                        {
                            TarifID = reader.GetInt32(0),
                            TarifAdi = reader.GetString(1),
                            KategoriAdi = reader.GetString(2),
                            MevcutMalzemeSayisi = mevcutMalzemeSayisi,
                            ToplamMalzemeSayisi = toplamMalzemeSayisi,
                            EksikMalzemeMaliyeti = reader.GetDouble(5)
                        });
                    }

                    SuggestedRecipeList.ItemsSource = suggestedRecipes;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Tarif önerileri alınırken bir hata oluştu: {ex.Message}");
                }
            }
        }

        private void OnSearchClick(object sender, RoutedEventArgs e)
        {
            string searchTerm = SearchBox.Text.ToLower();

            ObservableCollection<Recipe> searchResults = new ObservableCollection<Recipe>();

            using (NpgsqlConnection conn = new NpgsqlConnection(connString))
            {
                try
                {
                    conn.Open();
                    NpgsqlCommand cmd = new NpgsqlCommand(@"
                        SELECT t.tarifid, t.tarifadi, k.kategoriadi, t.hazirlamasuresi,
                               COALESCE(SUM(m.birimfiyat * tm.malzememiktar), 0) AS toplam_maliyet
                        FROM tarifler t
                        INNER JOIN kategoriler k ON t.kategoriid = k.kategoriid
                        LEFT JOIN tarifmalzemeler tm ON t.tarifid = tm.tarifid
                        LEFT JOIN malzemeler m ON tm.malzemeid = m.malzemeid
                        WHERE LOWER(t.tarifadi) LIKE @searchTerm
                        GROUP BY t.tarifid, t.tarifadi, k.kategoriadi, t.hazirlamasuresi", conn);
                    cmd.Parameters.AddWithValue("searchTerm", "%" + searchTerm + "%");
                    NpgsqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        int tarifID = reader.GetInt32(0);
                        string tarifAdi = !reader.IsDBNull(1) ? reader.GetString(1) : string.Empty;
                        string kategoriAdi = !reader.IsDBNull(2) ? reader.GetString(2) : string.Empty;
                        int hazirlamaSuresi = !reader.IsDBNull(3) ? reader.GetInt32(3) : 0;
                        double toplamMaliyet = !reader.IsDBNull(4) ? reader.GetDouble(4) : 0.0;

                        searchResults.Add(new Recipe
                        {
                            TarifID = tarifID,
                            TarifAdi = tarifAdi,
                            KategoriAdi = kategoriAdi,
                            HazirlamaSuresi = hazirlamaSuresi,
                            ToplamMaliyet = toplamMaliyet
                        });
                    }

                    RecipeList.ItemsSource = searchResults;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Tarif aranırken bir hata oluştu: {ex.Message}");
                }
            }
        }

        private void OnAddRecipeClick(object sender, RoutedEventArgs e)
        {
            TarifEkleWindow tarifEkleWindow = new TarifEkleWindow();
            if (tarifEkleWindow.ShowDialog() == true)
            {
                LoadRecipes();
            }
        }

        private void OnUpdateRecipeClick(object sender, RoutedEventArgs e)
        {
            if (RecipeList.SelectedItem == null)
            {
                MessageBox.Show("Güncellemek için bir tarif seçmelisiniz.");
                return;
            }

            Recipe selectedRecipe = (Recipe)RecipeList.SelectedItem;

            TarifGuncelleWindow guncelleWindow = new TarifGuncelleWindow(selectedRecipe.TarifID);
            if (guncelleWindow.ShowDialog() == true)
            {
                LoadRecipes();
            }
        }

        private void OnDeleteRecipeClick(object sender, RoutedEventArgs e)
        {
            if (RecipeList.SelectedItem == null)
            {
                MessageBox.Show("Silmek için bir tarif seçmelisiniz.");
                return;
            }

            Recipe selectedRecipe = (Recipe)RecipeList.SelectedItem;

            using (NpgsqlConnection conn = new NpgsqlConnection(connString))
            {
                try
                {
                    conn.Open();
                    NpgsqlCommand deleteIngredientsCmd = new NpgsqlCommand("DELETE FROM tarifmalzemeler WHERE tarifid = @tarifid", conn);
                    deleteIngredientsCmd.Parameters.AddWithValue("tarifid", selectedRecipe.TarifID);
                    deleteIngredientsCmd.ExecuteNonQuery();

                    NpgsqlCommand deleteRecipeCmd = new NpgsqlCommand("DELETE FROM tarifler WHERE tarifid = @tarifid", conn);
                    deleteRecipeCmd.Parameters.AddWithValue("tarifid", selectedRecipe.TarifID);
                    deleteRecipeCmd.ExecuteNonQuery();

                    MessageBox.Show("Tarif başarıyla silindi.");
                    LoadRecipes();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Tarif silinirken bir hata oluştu: {ex.Message}");
                }
            }
        }

        private void OnRecipeSelected(object sender, SelectionChangedEventArgs e)
        {
            if (RecipeList.SelectedItem == null)
                return;

            Recipe selectedRecipe = (Recipe)RecipeList.SelectedItem;
            RecipeDetailWindow detailWindow = new RecipeDetailWindow(selectedRecipe.TarifID);
            detailWindow.Show();
        }

        private void OnFilterChanged(object sender, RoutedEventArgs e)
        {
            LoadRecipes();
        }

        private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (SearchBox.Text == "Tarif Arama")
            {
                SearchBox.Text = "";
                SearchBox.Foreground = Brushes.Black;
            }
        }

        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchBox.Text))
            {
                SearchBox.Text = "Tarif Arama";
                SearchBox.Foreground = Brushes.Gray;
            }
        }

        private void FilterTime_GotFocus(object sender, RoutedEventArgs e)
        {
            if (FilterTime.Text == "Max Süre")
            {
                FilterTime.Text = "";
                FilterTime.Foreground = Brushes.Black;
            }
        }

        private void FilterTime_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FilterTime.Text))
            {
                FilterTime.Text = "Max Süre";
                FilterTime.Foreground = Brushes.Gray;
            }
        }

        private void FilterCost_GotFocus(object sender, RoutedEventArgs e)
        {
            if (FilterCost.Text == "Max Maliyet")
            {
                FilterCost.Text = "";
                FilterCost.Foreground = Brushes.Black;
            }
        }

        private void FilterCost_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FilterCost.Text))
            {
                FilterCost.Text = "Max Maliyet";
                FilterCost.Foreground = Brushes.Gray;
            }
        }
    }

    public class Recipe
    {
        public int TarifID { get; set; }
        public string TarifAdi { get; set; } = string.Empty;
        public string KategoriAdi { get; set; } = string.Empty;
        public int HazirlamaSuresi { get; set; }
        public double ToplamMaliyet { get; set; }
    }

    public class RecipeSuggestion
    {
        public int TarifID { get; set; }
        public string TarifAdi { get; set; } = string.Empty;
        public string KategoriAdi { get; set; } = string.Empty;
        public int MevcutMalzemeSayisi { get; set; }
        public int ToplamMalzemeSayisi { get; set; }
        public double EksikMalzemeMaliyeti { get; set; }
        public Brush Renk
        {
            get
            {
                return MevcutMalzemeSayisi == ToplamMalzemeSayisi ? Brushes.LightGreen : Brushes.OrangeRed;
            }
        }
    }
}
