using ParkingApp.Utils;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace ParkingApp.Forms
{
    public class VeForm : Form
    {
        DataGridView dgv;
        ComboBox cmbLoaiVe, cmbKhach;
        DateTimePicker dtpBatDau, dtpKetThuc;
        TextBox txtGia, txtTim;
        Button btnThem, btnSua, btnXoa, btnTim;

        public VeForm()
        {
            this.Text = "Quản lý vé";
            this.Size = new Size(950, 600);

            BuildUI();
            LoadKhachHang();
            LoadData();
        }

        private void BuildUI()
        {
            // ========== DATAGRID ==========
            dgv = new DataGridView()
            {
                Dock = DockStyle.Top,
                Height = 350,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.CellClick += Dgv_CellClick;

            Controls.Add(dgv);
            // Ngăn NULL gây crash 
            dgv.AllowUserToAddRows = false;


            // ===== LABELS + CONTROLS =====
            Label lblLoai = new Label() { Text = "Loại vé:", Left = 20, Top = 370 };
            cmbLoaiVe = new ComboBox()
            {
                Left = 120,
                Top = 370,
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbLoaiVe.Items.AddRange(new string[] { "Vé ngày", "Vé tháng", "Vé năm" });
            cmbLoaiVe.SelectedIndexChanged += CmbLoaiVe_SelectedIndexChanged;

            Label lblGia = new Label() { Text = "Giá tiền:", Left = 20, Top = 410 };
            txtGia = new TextBox() { Left = 120, Top = 410, Width = 200 };

            Label lblBD = new Label() { Text = "Ngày bắt đầu:", Left = 20, Top = 450 };
            dtpBatDau = new DateTimePicker()
            {
                Left = 120,
                Top = 450,
                Width = 200,
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd/MM/yyyy"
            };
            dtpBatDau.ValueChanged += DtB_ValueChanged;

            Label lblKT = new Label() { Text = "Ngày kết thúc:", Left = 20, Top = 490 };
            dtpKetThuc = new DateTimePicker()
            {
                Left = 120,
                Top = 490,
                Width = 200,
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd/MM/yyyy"
            };

            Label lblKhach = new Label() { Text = "Khách hàng:", Left = 350, Top = 370 };
            cmbKhach = new ComboBox()
            {
                Left = 450,
                Top = 370,
                Width = 250,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            // ===== BUTTONS =====
            btnThem = new Button() { Text = "Thêm", Left = 750, Top = 370, Width = 120 };
            btnSua = new Button() { Text = "Sửa", Left = 750, Top = 410, Width = 120 };
            btnXoa = new Button() { Text = "Xóa", Left = 750, Top = 450, Width = 120 };

            btnThem.Click += BtnThem_Click;
            btnSua.Click += BtnSua_Click;
            btnXoa.Click += BtnXoa_Click;

            // ===== TÌM KIẾM =====
            txtTim = new TextBox() { Left = 450, Top = 450, Width = 250 };
            btnTim = new Button() { Text = "Tìm", Left = 750, Top = 490, Width = 120 };
            btnTim.Click += BtnTim_Click;

            Controls.Add(lblLoai);
            Controls.Add(cmbLoaiVe);
            Controls.Add(lblGia);
            Controls.Add(txtGia);
            Controls.Add(lblBD);
            Controls.Add(dtpBatDau);
            Controls.Add(lblKT);
            Controls.Add(dtpKetThuc);
            Controls.Add(lblKhach);
            Controls.Add(cmbKhach);

            Controls.Add(btnThem);
            Controls.Add(btnSua);
            Controls.Add(btnXoa);

            Controls.Add(txtTim);
            Controls.Add(btnTim);
        }

        // ==================== LOAD KHÁCH HÀNG ====================
        private void LoadKhachHang()
        {
            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();

                SqlDataAdapter da = new SqlDataAdapter("SELECT MaKH, TenKH FROM KhachHang", conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                cmbKhach.DataSource = dt;
                cmbKhach.DisplayMember = "TenKH";
                cmbKhach.ValueMember = "MaKH";
            }
        }

        // ==================== LOAD DỮ LIỆU VÉ ====================
        private void LoadData()
        {
            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();

                SqlDataAdapter da = new SqlDataAdapter(
                    @"SELECT V.MaVe, V.LoaiVe, V.GiaTien, V.NgayBatDau, V.NgayKetThuc,
                      K.TenKH 
                      FROM Ve V 
                      JOIN KhachHang K ON V.MaKH = K.MaKH", conn);

                DataTable dt = new DataTable();
                da.Fill(dt);
                dgv.DataSource = dt;
            }
        }

        // ========== TỰ ĐỘNG TÍNH NGÀY KẾT THÚC ==========
        private void CmbLoaiVe_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cmbLoaiVe.Text)
            {
                case "Vé ngày":
                    dtpKetThuc.Value = dtpBatDau.Value.AddDays(1);
                    txtGia.Text = "5000";
                    break;

                case "Vé tháng":
                    dtpKetThuc.Value = dtpBatDau.Value.AddMonths(1);
                    txtGia.Text = "108000";
                    break;

                case "Vé năm":
                    dtpKetThuc.Value = dtpBatDau.Value.AddYears(1);
                    txtGia.Text = "1000000";
                    break;
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // VeForm
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Name = "VeForm";
            this.Load += new System.EventHandler(this.VeForm_Load);
            this.ResumeLayout(false);

        }

        private void VeForm_Load(object sender, EventArgs e)
        {

        }

        private void DtB_ValueChanged(object sender, EventArgs e)
        {
            CmbLoaiVe_SelectedIndexChanged(null, null);
        }

        // ==================== CLICK LẤY ROW ====================
        private void Dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                cmbLoaiVe.Text = dgv.Rows[e.RowIndex].Cells["LoaiVe"].Value.ToString();
                txtGia.Text = dgv.Rows[e.RowIndex].Cells["GiaTien"].Value.ToString();
                dtpBatDau.Value = Convert.ToDateTime(dgv.Rows[e.RowIndex].Cells["NgayBatDau"].Value);
                dtpKetThuc.Value = Convert.ToDateTime(dgv.Rows[e.RowIndex].Cells["NgayKetThuc"].Value);
                cmbKhach.Text = dgv.Rows[e.RowIndex].Cells["TenKH"].Value.ToString();
            }
        }

        // ==================== THÊM VÉ ====================
        private void BtnThem_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(
                    @"INSERT INTO Ve (LoaiVe, GiaTien, NgayBatDau, NgayKetThuc, MaKH)
                      VALUES (@lv, @gia, @bd, @kt, @kh)", conn);

                cmd.Parameters.AddWithValue("@lv", cmbLoaiVe.Text);
                cmd.Parameters.AddWithValue("@gia", txtGia.Text);
                cmd.Parameters.AddWithValue("@bd", dtpBatDau.Value);
                cmd.Parameters.AddWithValue("@kt", dtpKetThuc.Value);
                cmd.Parameters.AddWithValue("@kh", cmbKhach.SelectedValue);

                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Thêm vé thành công!");
            LoadData();
        }

        // ==================== SỬA VÉ ====================
        private void BtnSua_Click(object sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count == 0)
            {
                MessageBox.Show("Chọn vé cần sửa!");
                return;
            }

            int MaVe = Convert.ToInt32(dgv.SelectedRows[0].Cells["MaVe"].Value);

            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(
                    @"UPDATE Ve SET LoaiVe=@lv, GiaTien=@gia, 
                    NgayBatDau=@bd, NgayKetThuc=@kt, MaKH=@kh WHERE MaVe=@id", conn);

                cmd.Parameters.AddWithValue("@lv", cmbLoaiVe.Text);
                cmd.Parameters.AddWithValue("@gia", txtGia.Text);
                cmd.Parameters.AddWithValue("@bd", dtpBatDau.Value);
                cmd.Parameters.AddWithValue("@kt", dtpKetThuc.Value);
                cmd.Parameters.AddWithValue("@kh", cmbKhach.SelectedValue);
                cmd.Parameters.AddWithValue("@id", MaVe);

                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Sửa vé thành công!");
            LoadData();
        }

        // ==================== XÓA VÉ ====================
        private void BtnXoa_Click(object sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count == 0)
            {
                MessageBox.Show("Chọn vé cần xóa!");
                return;
            }

            int MaVe = Convert.ToInt32(dgv.SelectedRows[0].Cells["MaVe"].Value);

            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("DELETE FROM Ve WHERE MaVe=@id", conn);
                cmd.Parameters.AddWithValue("@id", MaVe);
                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Xóa vé thành công!");
            LoadData();
        }

        // ==================== TÌM KIẾM ====================
        private void BtnTim_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();

                string sql =
                    @"SELECT V.MaVe, V.LoaiVe, V.GiaTien, V.NgayBatDau, V.NgayKetThuc, K.TenKH 
                      FROM Ve V 
                      JOIN KhachHang K ON V.MaKH = K.MaKH
                      WHERE K.TenKH LIKE @tk OR V.LoaiVe LIKE @tk";

                SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                da.SelectCommand.Parameters.AddWithValue("@tk", "%" + txtTim.Text + "%");

                DataTable dt = new DataTable();
                da.Fill(dt);

                dgv.DataSource = dt;
            }
        }
    }
}
