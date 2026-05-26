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
    public partial class AssesementForm : Form
    {
        private int selectedComponentId = 0;
        public AssesementForm()
        {
            InitializeComponent();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void AssesementForm_Load(object sender, EventArgs e)
        {
            LoadAssignments();
        }

        private void LoadAssignments()
        {
            using (SqlConnection conn = Koneksi.GetConnection())
            {
                // Query mengambil ID Assignment, digabung dengan Nama Kelas dan Nama Mapel
                string query = @"
                    SELECT ta.AssignmentID, 
                           c.ClassName AS AssignmentName
                    FROM TeachingAssignments ta
                    INNER JOIN Classes c ON ta.ClassID = c.ClassID
                    WHERE ta.TeacherID = @TeacherID";

                string queris = @"
                    SELECT ta.AssignmentID, 
                           s.SubjectName AS AssignmentName
                    FROM TeachingAssignments ta
                    INNER JOIN Subjects s ON ta.SubjectID = s.SubjectID
                    WHERE ta.TeacherID = @TeacherID";

                SqlCommand cmds = new SqlCommand(queris, conn);
                cmds.Parameters.AddWithValue("@TeacherID", Session.UserID);

                SqlDataAdapter das = new SqlDataAdapter(cmds);
                DataTable dts = new DataTable();
                das.Fill(dts);

                comboBox1.DataSource = dts;
                comboBox1.DisplayMember = "AssignmentName";
                comboBox1.ValueMember = "AssignmentID";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@TeacherID", Session.UserID);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                comboBox2.DataSource = dt;
                comboBox2.DisplayMember = "AssignmentName";
                comboBox2.ValueMember = "AssignmentID";
            }
        }

        // 2. Memuat daftar komponen penilaian berdasarkan Assignment yang dipilih
        private void LoadComponents()
        {
            if (comboBox1.SelectedValue == null) return;

            // Coba parsing SelectedValue untuk menghindari error saat inisialisasi awal
            if (!int.TryParse(comboBox1.SelectedValue.ToString(), out int assignmentId)) return;

            using (SqlConnection conn = Koneksi.GetConnection())
            {
                string query = "SELECT ComponentID, ComponentName, Weight FROM AssessmentComponents WHERE AssignmentID = @AssignmentID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@AssignmentID", assignmentId);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dataGridView1.DataSource = dt;
                CalculateTotalWeight();
                ClearInput();
            }
        }

       
        private void CalculateTotalWeight()
        {
            decimal totalWeight = 0;

            // Kita hitung langsung dari baris yang tampil di DataGridView
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                // Pastikan baris tersebut bukan baris kosong di bawah tabel
                if (!row.IsNewRow)
                {
                    // Pastikan sel Weight tidak kosong (null / DBNull)
                    var selWeight = row.Cells["Weight"].Value;
                    if (selWeight != null && selWeight != DBNull.Value)
                    {
                        totalWeight += Convert.ToDecimal(selWeight);
                    }
                }
            }
            
                // Pastikan nilai tidak melebihi batas maximum NumericUpDown (biasanya 100)
                if (totalWeight <= total.Maximum)
                {
                    total.Value = totalWeight;
                } else
                {
                    total.Value = total.Maximum; // Set ke nilai maksimum jika melebihi
                }

            
        }
        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(componentName.Text) || string.IsNullOrWhiteSpace(weight.Text))
            {
                MessageBox.Show("Nama Komponen dan Bobot tidak boleh kosong!");
                return false;
            }

            if (!decimal.TryParse(weight.Text, out _))
            {
                MessageBox.Show("Bobot harus berupa angka valid!");
                return false;
            }
            return true;
        }

        // Helper: Mengosongkan form
        private void ClearInput()
        {
            selectedComponentId = 0;
            componentName.Text = "";
            weight.Text = "";
        }

            private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
            {
                LoadComponents();
            }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadComponents();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!ValidateInput()) return;

            int assignmentId = Convert.ToInt32(comboBox1.SelectedValue);
            decimal eight = Convert.ToDecimal(weight.Text);

            using (SqlConnection conn = Koneksi.GetConnection())
            {
                conn.Open();
                string query = "INSERT INTO AssessmentComponents (AssignmentID, ComponentName, Weight) VALUES (@AssignmentID, @Name, @Weight)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@AssignmentID", assignmentId);
                cmd.Parameters.AddWithValue("@Name", componentName.Text);
                cmd.Parameters.AddWithValue("@Weight", eight);
                cmd.ExecuteNonQuery();
            }
            MessageBox.Show("Komponen berhasil ditambahkan!");
            LoadComponents();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (selectedComponentId == 0)
            {
                MessageBox.Show("Pilih komponen dari tabel terlebih dahulu!");
                return;
            }
            if (!ValidateInput()) return;

            decimal eight = Convert.ToDecimal(weight.Text);
            int idComponent = Convert.ToInt32(componentID.Text);

            using (SqlConnection conn = Koneksi.GetConnection())
            {
                conn.Open();
                string query = "UPDATE AssessmentComponents SET ComponentName = @Name, Weight = @Weight WHERE ComponentID = @ComponentID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Name", componentName.Text);
                cmd.Parameters.AddWithValue("@Weight", eight);
                cmd.Parameters.AddWithValue("@ComponentID", idComponent);
                cmd.ExecuteNonQuery();
            }
            MessageBox.Show("Komponen berhasil diubah!");
            LoadComponents();
        }

        private void button3_Click(object sender, EventArgs e)
        {
           
            if (selectedComponentId == 0)
            {
                MessageBox.Show("Pilih komponen dari tabel terlebih dahulu!");
                return;
            }

            DialogResult dialog = MessageBox.Show(
                "Apakah Anda yakin ingin menghapus komponen ini?\n\nPERINGATAN: Menghapus komponen ini juga akan MENGHAPUS SEMUA NILAI SISWA yang terkait!",
                "Konfirmasi Hapus Berbahaya",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (dialog == DialogResult.Yes)
            {
                using (SqlConnection conn = Koneksi.GetConnection())
                {
                    try
                    {
                        conn.Open();

                       
                        string queryHapusNilai = "DELETE FROM StudentScores WHERE ComponentID = @ComponentID";
                        using (SqlCommand cmdNilai = new SqlCommand(queryHapusNilai, conn))
                        {
                            cmdNilai.Parameters.AddWithValue("@ComponentID", selectedComponentId);
                            cmdNilai.ExecuteNonQuery(); // Eksekusi penghapusan nilai
                        }

                       
                        string queryHapusKomponen = "DELETE FROM AssessmentComponents WHERE ComponentID = @ComponentID";
                        using (SqlCommand cmdKomponen = new SqlCommand(queryHapusKomponen, conn))
                        {
                            cmdKomponen.Parameters.AddWithValue("@ComponentID", selectedComponentId);
                            cmdKomponen.ExecuteNonQuery(); // Eksekusi penghapusan komponen
                        }

                        MessageBox.Show("Komponen beserta nilai siswa yang terkait berhasil dihapus!");
                        LoadComponents(); // Refresh tabel
                    }
                    catch (Exception ex)
                    {
                        
                        MessageBox.Show("Gagal menghapus data: " + ex.Message, "Error Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        
            }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                selectedComponentId = Convert.ToInt32(row.Cells["ComponentID"].Value);
                componentID.Text = row.Cells["ComponentID"].Value.ToString();
                componentName.Text = row.Cells["ComponentName"].Value.ToString();
                weight.Text = row.Cells["Weight"].Value.ToString();
                
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (dataGridView1.DataSource is DataTable dt)
            {
                if (string.IsNullOrWhiteSpace(txtCari.Text))
                {
                    dt.DefaultView.RowFilter = string.Empty;
                }
                else
                {
                    dt.DefaultView.RowFilter = $"ComponentName LIKE '%{txtCari.Text}%'";
                }
            }
        }
    }
}
