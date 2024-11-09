using System;
using System.Windows;
using System.Windows.Controls;
using Npgsql;

namespace TarifRehberi
{
    public partial class MalzemeEkleWindow : Window
    {
        private string connString = "Host=localhost;Username=postgres;Password=admin;Database=tarif";
        public MalzemeMiktar? SecilenMalzeme { get; private set; }

        public MalzemeEkleWindow()
        {
            InitializeComponent();
            LoadIngredients();
        }

        private void LoadIngredients()
        {
            MalzemeComboBox.Items.Clear();

            using (NpgsqlConnection conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                NpgsqlCommand cmd = new NpgsqlCommand("SELECT malzemeid, malzemeadi, malzemebirim FROM malzemeler", conn);
                NpgsqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    ComboBoxItem item = new ComboBoxItem
                    {
                        Content = reader.GetString(1),
                        Tag = new MalzemeMiktar
                        {
                            MalzemeID = reader.GetInt32(0),
                            MalzemeAdi = reader.GetString(1),
                            Birim = reader.GetString(2)
                        }
                    };
                    MalzemeComboBox.Items.Add(item);
                }
            }
        }

        private void OnEkleClick(object sender, RoutedEventArgs e)
        {
            if (MalzemeComboBox.SelectedItem == null)
            {
                MessageBox.Show("Lütfen bir malzeme seçin.");
                return;
            }

            if (!double.TryParse(MiktarTextBox.Text.Trim(), out double miktar))
            {
                MessageBox.Show("Miktar geçerli bir sayı olmalıdır.");
                return;
            }

            MalzemeMiktar? selectedMalzeme = ((ComboBoxItem)MalzemeComboBox.SelectedItem).Tag as MalzemeMiktar;

            if (selectedMalzeme == null)
            {
                MessageBox.Show("Seçilen malzeme geçerli değil.");
                return;
            }

            selectedMalzeme.Miktar = miktar;

            SecilenMalzeme = selectedMalzeme;
            this.DialogResult = true;
            this.Close();
        }
    }
}
