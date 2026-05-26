using System.Data.SqlClient;

class Koneksi
{
    public static SqlConnection GetConnection()
    {
        return new SqlConnection(@"Data Source=SKANADA\SQLEXPRESS;Initial Catalog=EsemkaSchool;Integrated Security=True");
    }
}
