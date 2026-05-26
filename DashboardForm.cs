using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace AplikasiSekolah
{
    public partial class DashboardForm : Form
    {
        public DashboardForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadLowestScoringStudents();
            LoadHighestAbsenceStudents();
        }
        private void LoadLowestScoringStudents()
        {
            using (SqlConnection conn = Koneksi.GetConnection())
            {
                // Mengambil 10 siswa dengan rata-rata nilai (AVG) terendah (ASC)
                string query = @"
                    SELECT  
                        u.FullName        AS [Student Name],
                        c.ClassName       AS [Class],
                        s.SubjectName     AS [Subject],
                        ac.ComponentName  AS [Component],
                        ac.Weight         AS [Weight],
                        ss.Score          AS [Score]
                    FROM StudentScores ss
                    JOIN Users u 
                        ON ss.StudentID = u.UserID
                    JOIN AssessmentComponents ac 
                        ON ss.ComponentID = ac.ComponentID
                    JOIN TeachingAssignments ta 
                        ON ac.AssignmentID = ta.AssignmentID
                    JOIN Subjects s 
                        ON ta.SubjectID = s.SubjectID
                    JOIN Classes c 
                        ON ta.ClassID = c.ClassID
                    ORDER BY u.FullName, s.SubjectName";

                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dgvLowestScore.DataSource = dt;

                // Merapikan tampilan grid agar tidak bisa diedit dan memenuhi lebar
                dgvLowestScore.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dgvLowestScore.ReadOnly = true;
                dgvLowestScore.AllowUserToAddRows = false;
            }
        }

        // 2. Fungsi untuk menampilkan 10 absen terbanyak
        private void LoadHighestAbsenceStudents()
        {
            using (SqlConnection conn = Koneksi.GetConnection())
            {
                // Mengambil 10 siswa dengan jumlah (COUNT) status 'Alpha' terbanyak (DESC)
                string query = @"
                    SELECT TOP 10 
                        u.UserID AS StudentID, 
                        u.FullName AS [Student Name], 
                        COUNT(a.AttendanceID) AS [Total Absences]
                    FROM Users u
                    INNER JOIN Attendance a ON u.UserID = a.StudentID
                    WHERE u.Role = 'Siswa' AND a.Status = 'Alpha' 
                    GROUP BY u.UserID, u.FullName
                    ORDER BY [Total Absences] DESC";

                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dgvHighestAbsence.DataSource = dt;

                // Merapikan tampilan grid
                dgvHighestAbsence.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dgvHighestAbsence.ReadOnly = true;
                dgvHighestAbsence.AllowUserToAddRows = false;
            }
        }
    }
}
