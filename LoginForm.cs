using System;
using System.Data;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace AplikasiSekolah
{
    public partial class LoginForm : Form
    {
        Label lblTitle, lblEmail, lblPassword;
        TextBox txtEmail, txtPassword;

        public LoginForm()
        {
            InitializeComponent();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            SqlConnection conn = Koneksi.GetConnection();
            conn.Open();

            {
                string query = @"
                SELECT UserID, FullName, Role 
                FROM Users
                WHERE Email = @email
                AND PasswordHash = @password";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@email", txtEmail.Text);
                cmd.Parameters.AddWithValue("@password", txtPassword.Text);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    Session.UserID = Convert.ToInt32(dt.Rows[0]["UserID"]);
                    Session.FullName = dt.Rows[0]["FullName"].ToString();
                    Session.Role = dt.Rows[0]["Role"].ToString();

                    MessageBox.Show("Login berhasil", "Info",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Hide();
                    MainForm mf = new MainForm();
                    mf.Show();
                }
                else
                {
                    MessageBox.Show("Email atau Password salah", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
