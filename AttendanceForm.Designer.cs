namespace AplikasiSekolah
{
    partial class AttendanceForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Hadir = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Sakit = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Izin = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Alpha = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(12, 25);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(265, 28);
            this.comboBox1.TabIndex = 0;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Location = new System.Drawing.Point(326, 27);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(282, 26);
            this.dateTimePicker1.TabIndex = 1;
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Hadir,
            this.Sakit,
            this.Izin,
            this.Alpha});
            this.dataGridView1.Location = new System.Drawing.Point(12, 89);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersWidth = 62;
            this.dataGridView1.RowTemplate.Height = 28;
            this.dataGridView1.Size = new System.Drawing.Size(983, 391);
            this.dataGridView1.TabIndex = 2;
            // 
            // Hadir
            // 
            this.Hadir.HeaderText = "Hadir";
            this.Hadir.MinimumWidth = 8;
            this.Hadir.Name = "Hadir";
            this.Hadir.Width = 150;
            // 
            // Sakit
            // 
            this.Sakit.HeaderText = "Sakit";
            this.Sakit.MinimumWidth = 8;
            this.Sakit.Name = "Sakit";
            this.Sakit.Width = 150;
            // 
            // Izin
            // 
            this.Izin.HeaderText = "Izin";
            this.Izin.MinimumWidth = 8;
            this.Izin.Name = "Izin";
            this.Izin.Width = 150;
            // 
            // Alpha
            // 
            this.Alpha.HeaderText = "Alpha";
            this.Alpha.MinimumWidth = 8;
            this.Alpha.Name = "Alpha";
            this.Alpha.Width = 150;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(691, 16);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(243, 44);
            this.button1.TabIndex = 3;
            this.button1.Text = "Save";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // AttendanceForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1007, 505);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.dateTimePicker1);
            this.Controls.Add(this.comboBox1);
            this.Name = "AttendanceForm";
            this.Text = "AttendanceForm";
            this.Load += new System.EventHandler(this.AttendanceForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Hadir;
        private System.Windows.Forms.DataGridViewTextBoxColumn Sakit;
        private System.Windows.Forms.DataGridViewTextBoxColumn Izin;
        private System.Windows.Forms.DataGridViewTextBoxColumn Alpha;
        private System.Windows.Forms.Button button1;
    }
}