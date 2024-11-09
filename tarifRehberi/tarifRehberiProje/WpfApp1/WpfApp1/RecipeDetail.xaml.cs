using System;
using System.Windows;
using System.Windows.Controls;
using Npgsql;

namespace TarifRehberi
{
    public partial class RecipeDetailWindow : Window
    {
        private string connString = "Host=localhost;Username=postgres;Password=admin;Database=tarif";
        private int TarifID;

        public RecipeDetailWindow(int tarifID)
        {
            InitializeComponent();
            TarifID = tarifID;
            LoadRecipeDetails();
        }

        private void LoadRecipeDetails()
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connString))
            {
                conn.Open();

                
                NpgsqlCommand cmd = new NpgsqlCommand(@"
                    SELECT t.tarifadi, k.kategoriadi, t.hazirlamasuresi, t.talimatlar
                    FROM tarifler t
                    INNER JOIN kategoriler k ON t.kategoriid = k.kategoriid
                    WHERE t.tarifid = @tarifid", conn);
                cmd.Parameters.AddWithValue("tarifid", TarifID);
                NpgsqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    TarifAdiTextBlock.Text = reader.GetString(0);
                    KategoriTextBlock.Text = reader.GetString(1);
                    HazirlamaSuresiTextBlock.Text = $"{reader.GetInt32(2)} dakika";
                    TalimatlarTextBlock.Text = reader.GetString(3);
                }
                reader.Close();

                
                NpgsqlCommand malzemeCmd = new NpgsqlCommand(@"
                    SELECT m.malzemeadi, tm.malzememiktar, m.malzemebirim
                    FROM tarifmalzemeler tm
                    INNER JOIN malzemeler m ON tm.malzemeid = m.malzemeid
                    WHERE tm.tarifid = @tarifid", conn);
                malzemeCmd.Parameters.AddWithValue("tarifid", TarifID);
                NpgsqlDataReader malzemeReader = malzemeCmd.ExecuteReader();

                while (malzemeReader.Read())
                {
                    MalzemeListesi.Items.Add($"{malzemeReader.GetString(0)} - {malzemeReader.GetDouble(1)} {malzemeReader.GetString(2)}");
                }
            }
        }
    }
}
