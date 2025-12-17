using ParkingApp.Utils;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace ParkingApp.Forms
{
    public class KhachForm : Form
    {
        DataGridView dgv;     // bảng khách hàng
        DataGridView dgvXe;   // bảng xe thuộc khách

        TextBox txtTen, txtSDT, txtDiaChi, txtTim, txtBienSo;
        ComboBox cmbLoaiXe;

        public KhachForm()
        {
            this.Text = "Quản lý khách hàng";
            this.Size = new Size(1000, 650);

            BuildUI();
            LoadData();
        }

        private void BuildUI()
        {
            // ====================== BẢNG KHÁCH HÀNG ======================
            dgv = new DataGridView()
            {
                Dock = DockStyle.Top,
                Height = 300,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.CellClick += Dgv_CellClick;
            Controls.Add(dgv);
            // Tránh NULL gây crash
            dgv.AllowUserToAddRows = false;


            // ====================== BẢNG XE CỦA KHÁCH ======================
            dgvXe = new DataGridView()
            {
                Dock = DockStyle.Bottom,
                Height = 180,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            dgvXe.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvXe.CellClick += DgvXe_CellClick;
            Controls.Add(dgvXe);

            // ====================== INPUT THÔNG TIN KHÁCH ======================
            Label lblTen = new Label() { Text = "Tên KH:", Left = 20, Top = 320 };
            txtTen = new TextBox() { Left = 120, Top = 320, Width = 250 };

            Label lblSDT = new Label() { Text = "Số ĐT:", Left = 20, Top = 360 };
            txtSDT = new TextBox() { Left = 120, Top = 360, Width = 250 };

            Label lblDiaChi = new Label() { Text = "Địa chỉ:", Left = 20, Top = 400 };
            txtDiaChi = new TextBox() { Left = 120, Top = 400, Width = 250 };

            Controls.Add(lblTen);
            Controls.Add(txtTen);
            Controls.Add(lblSDT);
            Controls.Add(txtSDT);
            Controls.Add(lblDiaChi);
            Controls.Add(txtDiaChi);

            // ====================== BUTTON CRUD KHÁCH ======================
            Button btnThem = new Button() { Text = "Thêm", Left = 400, Top = 320, Width = 120 };
            Button btnSua = new Button() { Text = "Sửa", Left = 530, Top = 320, Width = 120 };
            Button btnXoa = new Button() { Text = "Xóa", Left = 660, Top = 320, Width = 120 };

            btnThem.Click += BtnThem_Click;
            btnSua.Click += BtnSua_Click;
            btnXoa.Click += BtnXoa_Click;

            Controls.Add(btnThem);
            Controls.Add(btnSua);
            Controls.Add(btnXoa);

            // ====================== TÌM KIẾM KHÁCH ======================
            txtTim = new TextBox() { Left = 400, Top = 360, Width = 250 };
            Button btnTim = new Button() { Text = "Tìm", Left = 660, Top = 360, Width = 120 };
            btnTim.Click += BtnTim_Click;

            Controls.Add(txtTim);
            Controls.Add(btnTim);

            // ======================= QUẢN LÝ XE CỦA KHÁCH =====================
            Label lblBS = new Label() { Text = "Biển số:", Left = 20, Top = 440 };
            txtBienSo = new TextBox() { Left = 120, Top = 440, Width = 200 };

            Label lblLoai = new Label() { Text = "Loại xe:", Left = 350, Top = 440, AutoSize = true };
            cmbLoaiXe = new ComboBox()
            {
                Left = 420,
                Top = 440,
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbLoaiXe.Items.AddRange(new string[] { "Xe máy", "Xe đạp", "Ô tô" });

            Button btnThemXe = new Button() { Text = "Thêm xe", Left = 650, Top = 440, Width = 120 };
            Button btnSuaXe = new Button() { Text = "Sửa xe", Left = 780, Top = 440, Width = 120 };
            Button btnXoaXe = new Button() { Text = "Xóa xe", Left = 910, Top = 440, Width = 120 };

            btnThemXe.Click += (s, e) => ThemXe();
            btnSuaXe.Click += (s, e) => SuaXe();
            btnXoaXe.Click += (s, e) => XoaXe();

            Controls.Add(lblBS);
            Controls.Add(txtBienSo);
            Controls.Add(lblLoai);
            Controls.Add(cmbLoaiXe);
            Controls.Add(btnThemXe);
            Controls.Add(btnSuaXe);
            Controls.Add(btnXoaXe);
        }

        // ======================= LOAD DANH SÁCH KHÁCH =======================
        private void LoadData()
        {
            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();
                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM KhachHang ORDER BY MaKH DESC", conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgv.DataSource = dt;

                dgvXe.DataSource = new DataTable(); // Clear danh sách xe dưới khi chưa chọn KH
            }
        }

        // ======================= CLICK KHÁCH -> LOAD XE ======================
        private void Dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            txtTen.Text = dgv.Rows[e.RowIndex].Cells["TenKH"].Value.ToString();
            txtSDT.Text = dgv.Rows[e.RowIndex].Cells["SDT"].Value.ToString();
            txtDiaChi.Text = dgv.Rows[e.RowIndex].Cells["DiaChi"].Value.ToString();

            int maKH = Convert.ToInt32(dgv.Rows[e.RowIndex].Cells["MaKH"].Value);
            LoadXeTheoKhach(maKH);
        }

        // ======================= LOAD XE THEO KHÁCH ========================
        private void LoadXeTheoKhach(int maKH)
        {
            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();
                SqlDataAdapter da = new SqlDataAdapter(
                    @"SELECT BienSo, LoaiXe 
                      FROM Xe 
                      WHERE MaKH=@id", conn);

                da.SelectCommand.Parameters.AddWithValue("@id", maKH);

                DataTable dt = new DataTable();
                da.Fill(dt);
                dgvXe.DataSource = dt;
            }
        }

        // ====================== CLICK XE -> LOAD TEXTBOX ===================
        private void DgvXe_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            txtBienSo.Text = dgvXe.Rows[e.RowIndex].Cells["BienSo"].Value.ToString();
            cmbLoaiXe.Text = dgvXe.Rows[e.RowIndex].Cells["LoaiXe"].Value.ToString();
        }

        // ======================== THÊM KHÁCH ===============================
        private void BtnThem_Click(object sender, EventArgs e)
        {
            if (txtTen.Text == "" || txtSDT.Text == "")
            {
                MessageBox.Show("Tên và SĐT không được để trống!");
                return;
            }

            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(
                    "INSERT INTO KhachHang (TenKH, SDT, DiaChi) VALUES (@t, @s, @d)", conn);

                cmd.Parameters.AddWithValue("@t", txtTen.Text);
                cmd.Parameters.AddWithValue("@s", txtSDT.Text);
                cmd.Parameters.AddWithValue("@d", txtDiaChi.Text);
                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Thêm thành công!");
            LoadData();
        }

        // ======================== SỬA KHÁCH ===============================
        private void BtnSua_Click(object sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count == 0) return;

            int maKhach = Convert.ToInt32(dgv.SelectedRows[0].Cells["MaKH"].Value);

            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(
                    "UPDATE KhachHang SET TenKH=@t, SDT=@s, DiaChi=@d WHERE MaKH=@id", conn);

                cmd.Parameters.AddWithValue("@t", txtTen.Text);
                cmd.Parameters.AddWithValue("@s", txtSDT.Text);
                cmd.Parameters.AddWithValue("@d", txtDiaChi.Text);
                cmd.Parameters.AddWithValue("@id", maKhach);
                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Sửa thành công!");
            LoadData();
        }

        // ======================== XÓA KHÁCH ===============================
        private void BtnXoa_Click(object sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count == 0) return;

            int maKhach = Convert.ToInt32(dgv.SelectedRows[0].Cells["MaKH"].Value);

            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM KhachHang WHERE MaKH=@id", conn);
                cmd.Parameters.AddWithValue("@id", maKhach);
                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Xóa thành công!");
            LoadData();
        }

        // ======================== THÊM XE ===============================
        private void ThemXe()
        {
            if (dgv.SelectedRows.Count == 0)
            {
                MessageBox.Show("Chọn khách trước khi thêm xe!");
                return;
            }

            if (txtBienSo.Text.Trim() == "")
            {
                MessageBox.Show("Nhập biển số xe!");
                return;
            }

            int maKH = Convert.ToInt32(dgv.SelectedRows[0].Cells["MaKH"].Value);

            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(
                    @"INSERT INTO Xe (BienSo, LoaiXe, MaKH)
                      VALUES (@bs, @lx, @kh)", conn);

                cmd.Parameters.AddWithValue("@bs", txtBienSo.Text);
                cmd.Parameters.AddWithValue("@lx", cmbLoaiXe.Text);
                cmd.Parameters.AddWithValue("@kh", maKH);

                try
                {
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Thêm xe thành công!");
                }
                catch
                {
                    MessageBox.Show("Biển số đã tồn tại hoặc dữ liệu sai!");
                }
            }

            LoadXeTheoKhach(maKH);
        }

        // ======================== SỬA XE ===============================
        private void SuaXe()
        {
            if (dgvXe.SelectedRows.Count == 0)
            {
                MessageBox.Show("Chọn xe cần sửa!");
                return;
            }

            string oldBienSo = dgvXe.SelectedRows[0].Cells["BienSo"].Value.ToString();
            int maKH = Convert.ToInt32(dgv.SelectedRows[0].Cells["MaKH"].Value);

            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(
                    @"UPDATE Xe SET BienSo=@bs, LoaiXe=@lx 
                      WHERE BienSo=@old", conn);

                cmd.Parameters.AddWithValue("@bs", txtBienSo.Text);
                cmd.Parameters.AddWithValue("@lx", cmbLoaiXe.Text);
                cmd.Parameters.AddWithValue("@old", oldBienSo);

                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Sửa xe thành công!");
            LoadXeTheoKhach(maKH);
        }

        // ======================== XÓA XE ===============================
        private void XoaXe()
        {
            if (dgvXe.SelectedRows.Count == 0)
            {
                MessageBox.Show("Chọn xe cần xóa!");
                return;
            }

            string bienSo = dgvXe.SelectedRows[0].Cells["BienSo"].Value.ToString();
            int maKH = Convert.ToInt32(dgv.SelectedRows[0].Cells["MaKH"].Value);

            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(
                    "DELETE FROM Xe WHERE BienSo=@bs", conn);

                cmd.Parameters.AddWithValue("@bs", bienSo);
                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Xóa xe thành công!");
            LoadXeTheoKhach(maKH);
        }

        // ======================== TÌM KHÁCH ===============================
        private void BtnTim_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();

                SqlDataAdapter da = new SqlDataAdapter(
                    "SELECT * FROM KhachHang WHERE TenKH LIKE @t OR SDT LIKE @t", conn);

                da.SelectCommand.Parameters.AddWithValue("@t", "%" + txtTim.Text + "%");

                DataTable dt = new DataTable();
                da.Fill(dt);
                dgv.DataSource = dt;
            }
        }
    }
}
