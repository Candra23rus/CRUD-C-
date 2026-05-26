using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AplikasiSekolah
{
    public partial class ScoreForm : Form
    {
        private Dictionary<int, decimal> componentWeights = new Dictionary<int, decimal>();
        private int currentAssignmentId = 0;
        public ScoreForm()
        {
            InitializeComponent();
        }
        private void LoadComponents()
        {
            // Pastikan kedua combobox sudah dipilih
            if (comboBox1.SelectedValue == null || comboBox2.SelectedValue == null) return;

            // Gunakan int.TryParse agar aman dari error saat inisialisasi form
            if (!int.TryParse(comboBox1.SelectedValue.ToString(), out int classId) ||
                !int.TryParse(comboBox2.SelectedValue.ToString(), out int subjectId))
            {
                return;
            }

            using (SqlConnection conn = Koneksi.GetConnection())
            {
                conn.Open();

                // 1. Cari Assignment ID
                string queryCari = "SELECT AssignmentID FROM TeachingAssignments WHERE ClassID = @ClassID AND SubjectID = @SubjectID";
                SqlCommand cmdCari = new SqlCommand(queryCari, conn);
                cmdCari.Parameters.AddWithValue("@ClassID", classId);
                cmdCari.Parameters.AddWithValue("@SubjectID", subjectId);

                object result = cmdCari.ExecuteScalar();

                // JIKA JADWAL TIDAK DITEMUKAN
                if (result == null)
                {
                    // Bersihkan tabel secara otomatis daripada memunculkan pesan error berulang kali
                    dataGridView1.Columns.Clear();
                    componentWeights.Clear();
                    currentAssignmentId = 0;
                    return;
                }

                currentAssignmentId = Convert.ToInt32(result);

                // ... (KODE ANDA SELANJUTNYA KE BAWAH TETAP SAMA) ...

                // 2. Ambil Komponen Penilaian untuk membuat Kolom Tabel
                string queryComp = "SELECT ComponentID, ComponentName, Weight FROM AssessmentComponents WHERE AssignmentID = @AssignmentID";
                SqlCommand cmdComp = new SqlCommand(queryComp, conn);
                cmdComp.Parameters.AddWithValue("@AssignmentID", currentAssignmentId);
                SqlDataAdapter daComp = new SqlDataAdapter(cmdComp);
                DataTable dtComp = new DataTable();
                daComp.Fill(dtComp);

                // Reset DataGridView dan Dictionary Bobot
                dataGridView1.Columns.Clear();
                componentWeights.Clear();
                dataGridView1.AllowUserToAddRows = false;

                // Buat Kolom Tetap (ID & Nama Siswa)
                dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { Name = "StudentID", HeaderText = "StudentID", Visible = false });
                dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { Name = "StudentName", HeaderText = "Student Name", ReadOnly = true, AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });

                // Buat Kolom Dinamis berdasarkan Komponen di Database
                foreach (DataRow row in dtComp.Rows)
                {
                    int compId = Convert.ToInt32(row["ComponentID"]);
                    decimal weight = Convert.ToDecimal(row["Weight"]);
                    string compName = row["ComponentName"].ToString();

                    // Simpan bobot ke memory untuk hitung final score nanti
                    componentWeights.Add(compId, weight);

                    // Tambahkan kolom. Nama kolom diberi prefix "Comp_" + ID agar mudah dilacak
                    dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
                    {
                        Name = "Comp_" + compId,
                        HeaderText = $"{compName}\n({weight}%)" // Menampilkan persen di header
                    });
                }

                // Tambahkan Kolom Akhir (Final Score)
                dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { Name = "FinalScore", HeaderText = "Final Score", ReadOnly = true });

                // 3. Ambil Data Siswa dan Masukkan ke Tabel
                string querySiswa = "SELECT UserID, FullName FROM Users WHERE Role = 'Siswa' AND ClassID = @ClassID ORDER BY FullName ASC";
                SqlCommand cmdSiswa = new SqlCommand(querySiswa, conn);
                cmdSiswa.Parameters.AddWithValue("@ClassID", classId);
                SqlDataAdapter daSiswa = new SqlDataAdapter(cmdSiswa);
                DataTable dtSiswa = new DataTable();
                daSiswa.Fill(dtSiswa);

                foreach (DataRow siswa in dtSiswa.Rows)
                {
                    int indexBaris = dataGridView1.Rows.Add();
                    DataGridViewRow row = dataGridView1.Rows[indexBaris];

                    int studentId = Convert.ToInt32(siswa["UserID"]);
                    row.Cells["StudentID"].Value = studentId;
                    row.Cells["StudentName"].Value = siswa["FullName"].ToString();

                    // --- MENGAMBIL NILAI YANG SUDAH ADA DI DATABASE ---
                    string queryNilai = "SELECT ComponentID, Score FROM StudentScores WHERE StudentID = @StudentID";
                    SqlCommand cmdNilai = new SqlCommand(queryNilai, conn);
                    cmdNilai.Parameters.AddWithValue("@StudentID", studentId);
                    SqlDataAdapter daNilai = new SqlDataAdapter(cmdNilai);
                    DataTable dtNilai = new DataTable();
                    daNilai.Fill(dtNilai);

                    foreach (DataRow nilai in dtNilai.Rows)
                    {
                        string colName = "Comp_" + nilai["ComponentID"].ToString();
                        if (dataGridView1.Columns.Contains(colName))
                        {
                            row.Cells[colName].Value = nilai["Score"];
                        }
                    }
                }

                // Hitung Rata-rata & Statistik di awal load
                RecalculateAll();
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadComponents();
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            RecalculateAll();
        }
        private void RecalculateAll()
        {
            decimal totalFinalScoreSemuaSiswa = 0;
            decimal nilaiMin = 100;
            decimal nilaiMax = 0;
            int countA = 0, countB = 0, countC = 0, countD = 0, countE = 0;
            int jumlahSiswa = dataGridView1.Rows.Count;

            if (jumlahSiswa == 0) return;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                decimal finalScoreSiswa = 0;

                // Hitung Final Score per siswa berdasarkan bobot komponen
                foreach (var comp in componentWeights)
                {
                    string colName = "Comp_" + comp.Key;
                    if (row.Cells[colName].Value != null && decimal.TryParse(row.Cells[colName].Value.ToString(), out decimal scoreInput))
                    {
                        // Rumus LKS: (Nilai * Bobot) / 100
                        finalScoreSiswa += (scoreInput * comp.Value) / 100m;
                    }
                }

                // Tampilkan Final Score di kolom paling kanan
                row.Cells["FinalScore"].Value = finalScoreSiswa.ToString("0.00");

                // --- KALKULASI STATISTIK KESELURUHAN KELAS ---
                totalFinalScoreSemuaSiswa += finalScoreSiswa;
                if (finalScoreSiswa < nilaiMin) nilaiMin = finalScoreSiswa;
                if (finalScoreSiswa > nilaiMax) nilaiMax = finalScoreSiswa;

                // Kategori Grade sesuai soal PDF
                if (finalScoreSiswa >= 85) countA++;
                else if (finalScoreSiswa >= 75) countB++;
                else if (finalScoreSiswa >= 65) countC++;
                else if (finalScoreSiswa >= 50) countD++;
                else countE++;
            }

            // Tampilkan ke Label
            Average.Text = (totalFinalScoreSemuaSiswa / jumlahSiswa).ToString("0.00");
            min.Text = (nilaiMin == 100 ? "0" : nilaiMin.ToString("0.00"));
            max.Text = nilaiMax.ToString("0.00");

            txtA.Text = $"{countA}";
            txtB.Text = $"{countB}";
            txtC.Text = $"{countC}";
            txtD.Text = $"{countD}";
            txtE.Text = $"{countE}";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count == 0) return;

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "CSV File|*.csv";
            sfd.FileName = "Data_Nilai_Siswa.csv";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                StringBuilder sb = new StringBuilder();

                // 1. Ambil nama Header Kolom (Abaikan StudentID yang tersembunyi)
                var headers = dataGridView1.Columns.Cast<DataGridViewColumn>()
                                .Where(c => c.Visible)
                                .Select(c => c.HeaderText.Replace("\n", " "));
                sb.AppendLine(string.Join(",", headers));

                // 2. Ambil isi datanya
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    var cells = row.Cells.Cast<DataGridViewCell>()
                                .Where(c => c.OwningColumn.Visible)
                                .Select(c => c.Value != null ? c.Value.ToString() : "");
                    sb.AppendLine(string.Join(",", cells));
                }

                // 3. Buat filenya
                File.WriteAllText(sfd.FileName, sb.ToString());
                MessageBox.Show("Data berhasil diekspor ke Excel (CSV)!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void ScoreForm_Load(object sender, EventArgs e)
        {
            LoadAssignments();
        }
        private void LoadAssignments()
        {
            using (SqlConnection conn = Koneksi.GetConnection())
            {
                conn.Open();

                // 1. Ambil Semua Data Kelas (Unik)
                string queryKelas = "SELECT ClassID, ClassName FROM Classes ORDER BY ClassName ASC";
                SqlCommand cmdKelas = new SqlCommand(queryKelas, conn);
                SqlDataAdapter daKelas = new SqlDataAdapter(cmdKelas);
                DataTable dtKelas = new DataTable();
                daKelas.Fill(dtKelas);

                // Matikan event sementara agar tidak error saat data baru dimasukkan
                comboBox1.SelectedIndexChanged -= comboBox1_SelectedIndexChanged;

                comboBox1.DataSource = dtKelas;
                comboBox1.DisplayMember = "ClassName";
                comboBox1.ValueMember = "ClassID";
                comboBox1.SelectedIndex = -1; // Biarkan kosong di awal

                // Nyalakan event kembali
                comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;

                // 2. Ambil Semua Data Mata Pelajaran (Unik)
                string queryMapel = "SELECT SubjectID, SubjectName FROM Subjects ORDER BY SubjectName ASC";
                SqlCommand cmdMapel = new SqlCommand(queryMapel, conn);
                SqlDataAdapter daMapel = new SqlDataAdapter(cmdMapel);
                DataTable dtMapel = new DataTable();
                daMapel.Fill(dtMapel);

                // Matikan event sementara
                comboBox2.SelectedIndexChanged -= comboBox2_SelectedIndexChanged;

                comboBox2.DataSource = dtMapel;
                comboBox2.DisplayMember = "SubjectName";
                comboBox2.ValueMember = "SubjectID";
                comboBox2.SelectedIndex = -1; // Biarkan kosong di awal

                // Nyalakan event kembali
                comboBox2.SelectedIndexChanged += comboBox2_SelectedIndexChanged;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("Silakan pilih Kelas & Mata Pelajaran, lalu klik Search terlebih dahulu sebelum mengimpor nilai!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "CSV File|*.csv";
            ofd.Title = "Pilih File Nilai CSV";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // 2. Baca seluruh baris teks dari file CSV
                    string[] lines = System.IO.File.ReadAllLines(ofd.FileName);

                    // Jika file kosong atau hanya berisi judul kolom (header)
                    if (lines.Length <= 1)
                    {
                        MessageBox.Show("File CSV kosong atau tidak memiliki data nilai.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // 3. Kumpulkan daftar nama kolom komponen nilai saja (mengabaikan nama siswa & final score)
                    var componentColumns = dataGridView1.Columns.Cast<DataGridViewColumn>()
                                            .Where(c => c.Visible && c.Name.StartsWith("Comp_"))
                                            .ToList();

                    // 4. Looping untuk membaca data CSV (Mulai dari index 1 untuk melewati baris Header)
                    for (int i = 1; i < lines.Length; i++)
                    {
                        // Pisahkan teks berdasarkan tanda koma
                        string[] data = lines[i].Split(',');

                        if (data.Length == 0) continue;

                        // Index 0 di file hasil Export kita adalah Nama Siswa
                        string studentName = data[0];

                        // 5. Cari baris di DataGridView yang nama siswanya cocok dengan di CSV
                        foreach (DataGridViewRow row in dataGridView1.Rows)
                        {
                            if (row.Cells["StudentName"].Value != null && row.Cells["StudentName"].Value.ToString() == studentName)
                            {
                                // Jika namanya cocok, masukkan nilai-nilai komponennya
                                // data[1] untuk nilai komponen 1, data[2] untuk komponen 2, dst.
                                for (int j = 0; j < componentColumns.Count; j++)
                                {
                                    // Cegah error jika jumlah kolom CSV lebih sedikit dari kolom tabel
                                    if (data.Length > j + 1)
                                    {
                                        string colName = componentColumns[j].Name;
                                        row.Cells[colName].Value = data[j + 1];
                                    }
                                }
                                break; // Siswa sudah ketemu dan diisi, lanjut ke baris CSV berikutnya
                            }
                        }
                    }

                    // 6. Panggil ulang fungsi perhitungan agar Rata-rata & Grade otomatis terupdate!
                    RecalculateAll();

                    MessageBox.Show("Data nilai berhasil diimpor ke tabel!\n\nJANGAN LUPA: Klik tombol 'Save' untuk menyimpannya ke database.", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Terjadi kesalahan saat membaca file CSV: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        
    }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadComponents();
        }
    }
}
