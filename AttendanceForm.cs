using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AplikasiSekolah
{
    public partial class AttendanceForm : Form
    {
        public AttendanceForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
           
            // 1. Validasi apakah DataGridView benar-benar ada isinya
            if (dataGridView1.Rows.Count == 0 || (dataGridView1.Rows.Count == 1 && dataGridView1.Rows[0].IsNewRow))
            {
                MessageBox.Show("Tidak ada data siswa untuk disimpan! Pastikan Anda sudah memilih kelas.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Pastikan kursor yang sedang aktif di tabel menyimpan perubahannya
            dataGridView1.EndEdit();

            int jumlahTersimpan = 0; // Variabel untuk menghitung data yang sukses masuk

            try
            {
                // Menggunakan blok 'using' agar koneksi otomatis ditutup
                using (SqlConnection conn = Koneksi.GetConnection())
                {
                    conn.Open();
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        if (row.IsNewRow) continue;

                        // Pastikan StudentID tidak null sebelum dikonversi
                        if (row.Cells["StudentID"].Value == null) continue;

                        int studentId = Convert.ToInt32(row.Cells["StudentID"].Value);
                        string status = row.Cells["Status"].Value?.ToString() ?? "Alpha";

                        string query = "INSERT INTO Attendance (StudentID, Date, Status, RecordedBy) VALUES (@StudentId, @Date, @Status, @RecordedBy)";

                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@StudentId", studentId);
                            cmd.Parameters.AddWithValue("@Date", dateTimePicker1.Value.Date);
                            cmd.Parameters.AddWithValue("@Status", status);
                            cmd.Parameters.AddWithValue("@RecordedBy", Session.UserID);

                            // ExecuteNonQuery menghasilkan jumlah baris yang terpengaruh (masuk database)
                            int result = cmd.ExecuteNonQuery();
                            jumlahTersimpan += result;
                        }
                    }
                }

                MessageBox.Show($"Proses selesai! {jumlahTersimpan} data absensi berhasil masuk ke database.", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal menyimpan data: " + ex.Message, "Error Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        

        private void AttendanceForm_Load(object sender, EventArgs e)
        {

            LoadClasses();
            LoadStudents(0); 
        }

        private void LoadClasses()
        {
            SqlConnection conn = Koneksi.GetConnection();
            // Sesuaikan nama kolom dengan struktur tabel Classes di database Anda
            string query = "SELECT ClassID, ClassName FROM Classes";
            SqlDataAdapter da = new SqlDataAdapter(query, conn);
            DataTable dt = new DataTable();
            da.Fill(dt);

            comboBox1.DataSource = dt;
            comboBox1.DisplayMember = "ClassName"; // Teks yang tampil di dropdown
            comboBox1.ValueMember = "ClassID";     // Nilai ID yang tersimpan di balik teks
        
    }
        private void LoadStudents(int classId)
        {
            SqlConnection conn = Koneksi.GetConnection();
            // Mengambil data siswa (Role = 'Siswa' / 'Student' menyesuaikan isi tabel Users)
            string query = "SELECT UserID AS StudentID, FullName AS StudentName FROM Users WHERE Role = 'Siswa' AND ClassID = @ClassID";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@ClassID", classId);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            // Reset dan atur kolom DataGridView
            dataGridView1.Columns.Clear();
            dataGridView1.AutoGenerateColumns = false;

            // 1. Kolom StudentID (Disembunyikan, hanya untuk kebutuhan simpan data)
            DataGridViewTextBoxColumn colId = new DataGridViewTextBoxColumn();
            colId.Name = "StudentID";
            colId.DataPropertyName = "StudentID";
            colId.Visible = false;
            dataGridView1.Columns.Add(colId);

            // 2. Kolom Nama Siswa (Hanya bisa dibaca / ReadOnly)
            DataGridViewTextBoxColumn colName = new DataGridViewTextBoxColumn();
            colName.HeaderText = "Student Name";
            colName.Name = "StudentName";
            colName.DataPropertyName = "StudentName";
            colName.ReadOnly = true;
            colName.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns.Add(colName);

            // 3. Kolom Status Absensi (Berupa Dropdown)
            DataGridViewComboBoxColumn colStatus = new DataGridViewComboBoxColumn();
            colStatus.HeaderText = "Status";
            colStatus.Name = "Status";
            colStatus.Items.AddRange("Hadir", "Sakit", "Izin", "Alpha");
            colStatus.DefaultCellStyle.NullValue = "Hadir"; // Default siswa dianggap Hadir
            dataGridView1.Columns.Add(colStatus);

            // Masukkan data ke DataGridView
            dataGridView1.DataSource = dt;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedValue != null)
            {
                // Coba konversi ValueMember (ClassID) menjadi integer
                if (int.TryParse(comboBox1.SelectedValue.ToString(), out int selectedClassId))
                {
                    LoadStudents(selectedClassId);
                }
            }
        }
    }
}
