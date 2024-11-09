using System;
using System.Windows;
using System.Windows.Controls;
using Npgsql;
using System.Collections.ObjectModel;
using System.Linq;

namespace TarifRehberi
{
    public partial class TarifGuncelleWindow : Window
    {
        private string connString = "Host=localhost;Username=postgres;Password=admin;Database=tarif";
        private int TarifID;
        private ObservableCollection<MalzemeMiktar> malzemeMiktarListesi = new ObservableCollection<MalzemeMiktar>();

        public TarifGuncelleWindow(int tarifID)
        {
            InitializeComponent();
            TarifID = tarifID;
            LoadCategories();
            LoadTarifDetails();
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

        
        private void LoadTarifDetails()
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                
                NpgsqlCommand cmd = new NpgsqlCommand("SELECT tarifadi, kategoriid, hazirlamasuresi, talimatlar FROM tarifler WHERE tarifid = @tarifid", conn);
                cmd.Parameters.AddWithValue("tarifid", TarifID);
                NpgsqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    TarifAdiTextBox.Text = reader.GetString(0);
                    int kategoriID = reader.GetInt32(1);
                    HazirlamaSuresiTextBox.Text = reader.GetInt32(2).ToString();
                    TalimatlarTextBox.Text = reader.GetString(3);

                    
                    foreach (ComboBoxItem item in KategoriComboBox.Items)
                    {
                        if ((int)item.Tag == kategoriID)
                        {
                            item.IsSelected = true;
                            break;
                        }
                    }
                }
                reader.Close();
            
                NpgsqlCommand malzemeCmd = new NpgsqlCommand(@"
                    SELECT m.malzemeid, m.malzemeadi, m.malzemebirim, tm.malzememiktar
                    FROM tarifmalzemeler tm
                    INNER JOIN malzemeler m ON tm.malzemeid = m.malzemeid
                    WHERE tm.tarifid = @tarifid", conn);
                malzemeCmd.Parameters.AddWithValue("tarifid", TarifID);
                NpgsqlDataReader malzemeReader = malzemeCmd.ExecuteReader();

                while (malzemeReader.Read())
                {
                    malzemeMiktarListesi.Add(new MalzemeMiktar
                    {
                        MalzemeID = malzemeReader.GetInt32(0),
                        MalzemeAdi = malzemeReader.GetString(1),
                        Birim = malzemeReader.GetString(2),
                        Miktar = malzemeReader.GetDouble(3)
                    });
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
        
        private void OnTarifGuncelleClick(object sender, RoutedEventArgs e)
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

                NpgsqlCommand guncelleCmd = new NpgsqlCommand("UPDATE tarifler SET tarifadi = @tarifadi, kategoriid = @kategoriid, hazirlamasuresi = @hazirlamasuresi, talimatlar = @talimatlar WHERE tarifid = @tarifid", conn);
                guncelleCmd.Parameters.AddWithValue("tarifadi", tarifAdi);
                guncelleCmd.Parameters.AddWithValue("kategoriid", ((ComboBoxItem)KategoriComboBox.SelectedItem).Tag);
                guncelleCmd.Parameters.AddWithValue("hazirlamasuresi", hazirlamaSuresi);
                guncelleCmd.Parameters.AddWithValue("talimatlar", talimatlar);
                guncelleCmd.Parameters.AddWithValue("tarifid", TarifID);
                guncelleCmd.ExecuteNonQuery();

                
                NpgsqlCommand silCmd = new NpgsqlCommand("DELETE FROM tarifmalzemeler WHERE tarifid = @tarifid", conn);
                silCmd.Parameters.AddWithValue("tarifid", TarifID);
                silCmd.ExecuteNonQuery();

                
                foreach (var malzemeMiktar in malzemeMiktarListesi)
                {
                    NpgsqlCommand malzemeCmd = new NpgsqlCommand("INSERT INTO tarifmalzemeler (tarifid, malzemeid, malzememiktar) VALUES (@tarifid, @malzemeid, @malzememiktar)", conn);
                    malzemeCmd.Parameters.AddWithValue("tarifid", TarifID);
                    malzemeCmd.Parameters.AddWithValue("malzemeid", malzemeMiktar.MalzemeID);
                    malzemeCmd.Parameters.AddWithValue("malzememiktar", malzemeMiktar.Miktar);
                    malzemeCmd.ExecuteNonQuery();
                }

                MessageBox.Show("Tarif başarıyla güncellendi.");
                this.DialogResult = true;
                this.Close();
            }
        }
    }
}
