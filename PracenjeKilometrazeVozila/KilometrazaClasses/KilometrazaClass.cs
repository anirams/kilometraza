using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracenjeKilometrazeVozila.KilometrazaClasses
{
    class KilometrazaClass
    {
        public int Id { get; set; }
        public string RegOznaka { get; set; }
        public DateTime DatumIVrijeme { get; set; }
        public int PocetnaKilometraza { get; set; }
        public int ZavrsnaKilometraza { get; set; }
        public int PrijedjeniKm { get; set; }

        static string connString = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;

        //-----------------------------------------------uzimanje podataka iz baze (pretrazivanje forma)-----------------------------------
        public DataTable Select(string regOznakaString, DateTime vrijemeOd, DateTime vrijemeDo)
        {
            SqlConnection conn = new SqlConnection(connString);
            DataTable dt = new DataTable();
            try
            {
                string sql = "SELECT * FROM Kilometrazas WHERE RegOznaka=@RegOznaka AND DatumIVrijeme<=@vrijemeDo AND DatumIVrijeme>=@vrijemeOd";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@RegOznaka", regOznakaString);
                cmd.Parameters.AddWithValue("@vrijemeOd", vrijemeOd);
                cmd.Parameters.AddWithValue("@vrijemeDo", vrijemeDo);
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

            return dt;
        }

        // --------------------------------------------------------------UMETANJE NOVOG ROW-A U BAZU,VALIDACIJA---------------------------------

        public bool Insert(KilometrazaClass kilometraza)
        {
            bool isSuccess = false;
            SqlConnection conn = new SqlConnection(connString);

            //-------------------------------------------validacija (FORMAT REG. OZNAKE I kilometraze) ------------------------------------------------- 
            if (kilometraza.ZavrsnaKilometraza < kilometraza.PocetnaKilometraza || 
                kilometraza.RegOznaka == "^[a-zA-Z]{2}[0-9]{3,4}[a-zA-Z]{1,2}$")
            {
                return isSuccess;
            }

            try
            {
                string sql = "INSERT INTO Kilometrazas (RegOznaka, DatumIVrijeme, PocetnaKilometraza, ZavrsnaKilometraza, PrijedjeniKm) VALUES (@RegOznaka, @DatumIVrijeme, @PocetnaKilometraza, @ZavrsnaKilometraza, @PrijedjeniKm)";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@RegOznaka", kilometraza.RegOznaka);
                cmd.Parameters.AddWithValue("@DatumIvrijeme", kilometraza.DatumIVrijeme);
                cmd.Parameters.AddWithValue("@PocetnaKilometraza", kilometraza.PocetnaKilometraza);
                cmd.Parameters.AddWithValue("@ZavrsnaKilometraza", kilometraza.ZavrsnaKilometraza);
                cmd.Parameters.AddWithValue("@PrijedjeniKm", kilometraza.PrijedjeniKm);
                conn.Open();
                int rows = cmd.ExecuteNonQuery();
                if (rows > 0)
                {
                    isSuccess = true;
                }
                else
                {
                    isSuccess = false;
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            finally
            {
                conn.Close();
            }
            return isSuccess;
        }
    }
}
