using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Configuration;
using PracenjeKilometrazeVozila.KilometrazaClasses;
using System.Data;

namespace PracenjeKilometrazeVozila
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

        }

        static string connString = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;

        KilometrazaClass k = new KilometrazaClass();

        //---------------------------upisivanje forma, validacija-------------------------------------------------------------------------------

        private void UpisKilometrazeButton_Click(object sender, RoutedEventArgs e)
        {
            k.RegOznaka = regOznakaTextBox.Text;
            k.DatumIVrijeme = DateTime.Parse(datumPocetkaTextBox.Text);
            k.PocetnaKilometraza = Int32.Parse(pocetnaKilometrazaTextBox.Text);
            k.ZavrsnaKilometraza = Int32.Parse(zavrsnaKilometrazaTextBox.Text);
            k.PrijedjeniKm = Int32.Parse(zavrsnaKilometrazaTextBox.Text) - Int32.Parse(pocetnaKilometrazaTextBox.Text);

            SqlConnection conn = new SqlConnection(connString);
            DataTable dt = new DataTable();

            //--------------------------validacija- upis pocetne kilometraze za vozilo za koje je vec upisivana kilometraza-------------------
            try
            {
                
                string sql = "SELECT ZavrsnaKilometraza FROM Kilometrazas WHERE RegOznaka=@RegOznaka";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@RegOznaka", k.RegOznaka);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                conn.Open();
                adapter.Fill(dt);  
            }
            catch (Exception ex)
            {

                throw;
            }
            finally
            {
                conn.Close();
            }

            int kilometraza = 0;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i].Field<int>("ZavrsnaKilometraza") > kilometraza)
                {
                    kilometraza = dt.Rows[i].Field<int>("ZavrsnaKilometraza");
                }
            }

            if (k.ZavrsnaKilometraza < kilometraza)
            {
                MessageBox.Show("Posljednja unesena kilometraza za navedeno \n vozilo je bila " + kilometraza
                    + "\n te početna kilometraža ne smije biti manja od toga!" +
                    "\n ispravite unos!");

            }

            //-------------------------------------------------------------------------------------------------------------------------
            else
            {
                bool success = k.Insert(k);
                if (success == true)
                {
                    MessageBox.Show("Uspješan unos!");
                }
                else
                {
                    MessageBox.Show("Došlo je do pogreške! \n Završna kilometraža ne smije biti manja od početne! " +
                        "\n Registarska oznaka mora biti u formatu ab123cd, ab1234cd, ab1234c ili ab123c");
                }
                regOznakaTextBox.Text = "";
                datumPocetkaTextBox.Text = "";
                pocetnaKilometrazaTextBox.Text = "";
                zavrsnaKilometrazaTextBox.Text = "";

            }
        }

        //-------------------------------------- pretrazivanje forma -------------------------------------------------------

        private void PretragaButton_Click(object sender, RoutedEventArgs e)
        {
            string regOznakaString = pretragaRegOznakaTextBox.Text;
            DateTime vrijemeOd = DateTime.Parse(pretragaPocetniDatumTextBox.Text);
            DateTime vrijemeDo = DateTime.Parse(pretragaZavrsniDatumTextBox.Text);
            
            DataTable dt = k.Select(regOznakaString, vrijemeOd, vrijemeDo);
            podaciDataGrid.DataContext = dt.DefaultView;
            int ukupnoKm = 0;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                ukupnoKm += dt.Rows[i].Field<int>("PrijedjeniKm");
            }
            ukupnoKmTextBlock.Text = "Vozilo je prešlo ukupno " + ukupnoKm.ToString() + " u zadanom razdoblju!";

            regOznakaTextBox.Text = "";
            pretragaPocetniDatumTextBox.Text = "";
            pretragaZavrsniDatumTextBox.Text = "";
        }
    }
}
