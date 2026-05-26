using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Collections.Specialized.BitVector32;

namespace AplikasiSekolah
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }
        private void MainForm_Load(object sender, EventArgs e)
        {
            // Contoh: Jika bukan Homeroom Teacher (Wali Kelas), sembunyikan menu Attendance
            if (Session.Role != "Guru")
            {
                attendanceToolStripMenuItem.Visible = false;
            }
        }

        private void attendanceToolStripMenuItem_Click(object sender, EventArgs e)
        {
           
        }

        private void MainForm_Load_1(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
            label1.Text = "Welcome, " + Session.FullName;
        }

        private void attendanceToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            AttendanceForm at = new AttendanceForm();
            at.MdiParent = this;
            at.Show();
        }

        private void logoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Apakah Anda yakin ingin keluar dari akun ini?", "Konfirmasi Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            // 2. Jika user memilih "Yes"
            if (dialogResult == DialogResult.Yes)
            {
                // 3. Bersihkan data di Class Session (Kembalikan ke nilai awal)
                Session.UserID = 0;
                Session.FullName = string.Empty;
                Session.Role = string.Empty;

                // 4. Restart aplikasi agar kembali bersih ke Form awal (LoginForm)
                Application.Restart();
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dialog = MessageBox.Show("Apakah Anda yakin ingin menutup aplikasi Esemka School?", "Konfirmasi Keluar", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            // 2. Jika pengguna mengeklik tombol "Yes"
            if (dialog == DialogResult.Yes)
            {
                // 3. Matikan seluruh proses aplikasi
                Application.Exit();
            }
        }

        private void attendanceToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            DashboardForm df = new DashboardForm();
            df.MdiParent = this;
            df.Show();
        }

        private void assesementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AssesementForm ass = new AssesementForm();
            ass.MdiParent = this;
            ass.Show();
        }

        private void scoresToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ScoreForm sc = new ScoreForm();
            sc.MdiParent = this;
            sc.Show();
        }
    }
}
