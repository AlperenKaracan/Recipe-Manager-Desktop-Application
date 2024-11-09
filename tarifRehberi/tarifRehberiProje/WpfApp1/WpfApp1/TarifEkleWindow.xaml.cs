using System;
using System.Windows;
using System.Windows.Controls;
using Npgsql;
using System.Collections.ObjectModel;

namespace TarifRehberi
{
    public partial class TarifEkleWindow : Window
    {
        private string connString = "Host=localhost;Username=postgres;Password=admin;Database=tarif";
        private ObservableCollection<MalzemeMiktar> malzemeMiktarListesi = new ObservableCollection<MalzemeMiktar>();

        public TarifEkleWindow()
        {
            InitializeComponent();
            LoadCategories();
            MalzemeListesi.ItemsSource = malzemeMiktarListesi;
        }

      
        private void LoadCategories()
        {
            KategoriComboBox.Items.Clear();

            using (NpgsqlConnection conn = new NpgsqlConnection(connString))
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
                    KategoriComboBox.Items.Add(item);
                }
            }
        }

        
        private void OnMalzemeEkleClick(object sender, RoutedEventArgs e)
        {
            MalzemeEkleWindow malzemeEkleWindow = new MalzemeEkleWindow();
            if (malzemeEkleWindow.ShowDialog() == true)
            {
                
                var mevcutMalzeme = malzemeMiktarListesi.FirstOrDefault(m => m.MalzemeID == malzemeEkleWindow.SecilenMalzeme.MalzemeID);
                if (mevcutMalzeme != null)
                {
                    MessageBox.Show("Bu malzeme zaten eklenmiş.");
                    return;
                }

                malzemeMiktarListesi.Add(malzemeEkleWindow.SecilenMalzeme);
            }
        }

        
        private void OnTarifEkleClick(object sender, RoutedEventArgs e)
        {
            string tarifAdi = TarifAdiTextBox.Text.Trim();
            string talimatlar = TalimatlarTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(tarifAdi))
            {
                MessageBox.Show("Tarif adı boş olamaz.");
                return;
            }

            if (KategoriComboBox.SelectedItem == null)
            {
                MessageBox.Show("Lütfen bir kategori seçin.");
                return;
            }

            if (!int.TryParse(HazirlamaSuresiTextBox.Text.Trim(), out int hazirlamaSuresi))
            {
                MessageBox.Show("Hazırlama süresi geçerli bir sayı olmalıdır.");
                return;
            }

            if (malzemeMiktarListesi.Count == 0)
            {
                MessageBox.Show("Lütfen en az bir malzeme ekleyin.");
                return;
            }

            
            using (NpgsqlConnection conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                NpgsqlCommand kontrolCmd = new NpgsqlCommand("SELECT COUNT(*) FROM tarifler WHERE tarifadi = @tarifadi", conn);
                kontrolCmd.Parameters.AddWithValue("tarifadi", tarifAdi);
                int count = Convert.ToInt32(kontrolCmd.ExecuteScalar());
                if (count > 0)
                {
                    MessageBox.Show("Bu isimde bir tarif zaten mevcut.");
                    return;
                }

                
                NpgsqlCommand ekleCmd = new NpgsqlCommand("INSERT INTO tarifler (tarifadi, kategoriid, hazirlamasuresi, talimatlar) VALUES (@tarifadi, @kategoriid, @hazirlamasuresi, @talimatlar) RETURNING tarifid", conn);
                ekleCmd.Parameters.AddWithValue("tarifadi", tarifAdi);
                ekleCmd.Parameters.AddWithValue("kategoriid", ((ComboBoxItem)KategoriComboBox.SelectedItem).Tag);
                ekleCmd.Parameters.AddWithValue("hazirlamasuresi", hazirlamaSuresi);
                ekleCmd.Parameters.AddWithValue("talimatlar", talimatlar);
                int yeniTarifID = Convert.ToInt32(ekleCmd.ExecuteScalar());

                
                foreach (var malzemeMiktar in malzemeMiktarListesi)
                {
                    NpgsqlCommand malzemeCmd = new NpgsqlCommand("INSERT INTO tarifmalzemeler (tarifid, malzemeid, malzememiktar) VALUES (@tarifid, @malzemeid, @malzememiktar)", conn);
                    malzemeCmd.Parameters.AddWithValue("tarifid", yeniTarifID);
                    malzemeCmd.Parameters.AddWithValue("malzemeid", malzemeMiktar.MalzemeID);
                    malzemeCmd.Parameters.AddWithValue("malzememiktar", malzemeMiktar.Miktar);
                    malzemeCmd.ExecuteNonQuery();
                }

                MessageBox.Show("Tarif başarıyla eklendi.");
                this.DialogResult = true;
                this.Close();
            }
        }
    }

    
    public class MalzemeMiktar
    {
        public int MalzemeID { get; set; }
        public string MalzemeAdi { get; set; } = string.Empty;
        public double Miktar { get; set; }
        public string Birim { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"{MalzemeAdi} - {Miktar} {Birim}";
        }
    }
}
