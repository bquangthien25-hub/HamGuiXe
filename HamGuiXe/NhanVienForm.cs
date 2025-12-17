using ParkingApp.Utils;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace ParkingApp.Forms
{
    public class NhanVienForm : Form
    {
        DataGridView dgv;
        TextBox txtEmail, txtPass, txtHoTen, txtTim;
        ComboBox cmbRole;
        Button btnThem, btnSua, btnXoa, btnReset, btnTim, btnDelAccount;

        public NhanVienForm()
        {
            this.Text = "Quản lý tài khoản";
            this.Size = new Size(950, 600);

            BuildUI();
            LoadRole();
            LoadData();
        }

        private void BuildUI()
        {
            dgv = new DataGridView()
            {
                Dock = DockStyle.Top,
                Height = 350,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.CellClick += Dgv_CellClick;

            Controls.Add(dgv);

            Label lblEmail = new Label() { Text = "Email:", Left = 20, Top = 370 };
            txtEmail = new TextBox() { Left = 120, Top = 370, Width = 250 };

            Label lblPass = new Label() { Text = "Mật khẩu:", Left = 20, Top = 410 };
            txtPass = new TextBox() { Left = 120, Top = 410, Width = 250 };

            Label lblHoTen = new Label() { Text = "Họ tên:", Left = 20, Top = 450 };
            txtHoTen = new TextBox() { Left = 120, Top = 450, Width = 250 };

            Label lblRole = new Label() { Text = "Vai trò:", Left = 20, Top = 490 };
            cmbRole = new ComboBox()
            {
                Left = 120,
                Top = 490,
                Width = 250,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            btnThem = new Button() { Text = "Thêm", Left = 400, Top = 370, Width = 120 };
            btnSua = new Button() { Text = "Sửa", Left = 400, Top = 410, Width = 120 };
            btnXoa = new Button() { Text = "Khóa/Mở", Left = 400, Top = 450, Width = 120 };
            btnReset = new Button() { Text = "Reset mật khẩu", Left = 400, Top = 490, Width = 120 };

            btnThem.Click += BtnThem_Click;
            btnSua.Click += BtnSua_Click;
            btnXoa.Click += BtnXoa_Click;
            btnReset.Click += BtnReset_Click;

            // ============ NÚT XÓA VĨNH VIỄN ==============
            btnDelAccount = new Button()
            {
                Text = "Xóa tài khoản",
                Left = 550,
                Top = 450,
                Width = 120
            };
            btnDelAccount.Click += BtnXoaTaiKhoan_Click;

            txtTim = new TextBox() { Left = 550, Top = 370, Width = 250 };
            btnTim = new Button() { Text = "Tìm", Left = 820, Top = 370, Width = 120 };
            btnTim.Click += BtnTim_Click;

            Controls.Add(lblEmail);
            Controls.Add(txtEmail);
            Controls.Add(lblPass);
            Controls.Add(txtPass);
            Controls.Add(lblHoTen);
            Controls.Add(txtHoTen);
            Controls.Add(lblRole);
            Controls.Add(cmbRole);

            Controls.Add(btnThem);
            Controls.Add(btnSua);
            Controls.Add(btnXoa);
            Controls.Add(btnReset);
            Controls.Add(btnDelAccount);

            Controls.Add(txtTim);
            Controls.Add(btnTim);
        }

        private void LoadRole()
        {
            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();

                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM VaiTro", conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                cmbRole.DataSource = dt;
                cmbRole.DisplayMember = "TenVaiTro";
                cmbRole.ValueMember = "MaVaiTro";
            }
        }

        private void LoadData()
        {
            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();

                SqlDataAdapter da = new SqlDataAdapter(
                    @"SELECT N.MaND, N.Email, N.MatKhau, N.HoTen, N.TrangThai, V.TenVaiTro
                      FROM NguoiDung N
                      JOIN VaiTro V ON N.MaVaiTro = V.MaVaiTro", conn);

                DataTable dt = new DataTable();
                da.Fill(dt);

                dgv.DataSource = dt;

                // Only Admin can see password column
                if (dgv.Columns["MatKhau"] != null)
                {
                    if (LoginForm.CurrentRole == "Admin")
                    {
                        dgv.Columns["MatKhau"].HeaderText = "Mat khau (hashed)";
                        dgv.Columns["MatKhau"].Visible = true;
                    }
                    else
                    {
                        dgv.Columns["MatKhau"].Visible = false;
                    }
                }
            }
        }

        private void Dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                txtEmail.Text = dgv.Rows[e.RowIndex].Cells["Email"].Value.ToString();
                txtPass.Text = dgv.Rows[e.RowIndex].Cells["MatKhau"].Value.ToString();
                txtHoTen.Text = dgv.Rows[e.RowIndex].Cells["HoTen"].Value.ToString();
                cmbRole.Text = dgv.Rows[e.RowIndex].Cells["TenVaiTro"].Value.ToString();
            }
        }

        private void BtnThem_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();

                SqlCommand check = new SqlCommand("SELECT COUNT(*) FROM NguoiDung WHERE Email=@e", conn);
                check.Parameters.AddWithValue("@e", txtEmail.Text);

                if ((int)check.ExecuteScalar() > 0)
                {
                    MessageBox.Show("Email đã tồn tại!");
                    return;
                }

                // Hash password before saving to database
                string hashedPassword = SecurityHelper.HashPassword(txtPass.Text);

                SqlCommand cmd = new SqlCommand(
                    @"INSERT INTO NguoiDung (Email, MatKhau, HoTen, MaVaiTro, TrangThai)
                      VALUES (@e, @p, @h, @r, 1)", conn);

                cmd.Parameters.AddWithValue("@e", txtEmail.Text);
                cmd.Parameters.AddWithValue("@p", hashedPassword);  // Save hashed password
                cmd.Parameters.AddWithValue("@h", txtHoTen.Text);
                cmd.Parameters.AddWithValue("@r", cmbRole.SelectedValue);

                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Thêm tài khoản thành công!");
            LoadData();
        }

        private void BtnSua_Click(object sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count == 0) return;

            int id = Convert.ToInt32(dgv.SelectedRows[0].Cells["MaND"].Value);

            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();

                string sql;
                SqlCommand cmd;

                // If password field is empty, don't update password
                if (string.IsNullOrEmpty(txtPass.Text))
                {
                    sql = @"UPDATE NguoiDung SET Email=@e, HoTen=@h, MaVaiTro=@r 
                            WHERE MaND=@id";
                    cmd = new SqlCommand(sql, conn);
                }
                else
                {
                    // Hash new password before saving
                    string hashedPassword = SecurityHelper.HashPassword(txtPass.Text);
                    
                    sql = @"UPDATE NguoiDung SET Email=@e, MatKhau=@p, HoTen=@h, MaVaiTro=@r 
                            WHERE MaND=@id";
                    cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@p", hashedPassword);
                }

                cmd.Parameters.AddWithValue("@e", txtEmail.Text);
                cmd.Parameters.AddWithValue("@h", txtHoTen.Text);
                cmd.Parameters.AddWithValue("@r", cmbRole.SelectedValue);
                cmd.Parameters.AddWithValue("@id", id);

                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Sua tai khoan thanh cong!");
            LoadData();
        }

        private void BtnXoa_Click(object sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count == 0) return;

            int id = Convert.ToInt32(dgv.SelectedRows[0].Cells["MaND"].Value);
            int trangThai = Convert.ToInt32(dgv.SelectedRows[0].Cells["TrangThai"].Value);

            int newStatus = (trangThai == 1) ? 0 : 1;

            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(
                    "UPDATE NguoiDung SET TrangThai=@t WHERE MaND=@id", conn);
                cmd.Parameters.AddWithValue("@t", newStatus);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }

            MessageBox.Show(newStatus == 1 ? "Đã mở khóa!" : "Đã khóa tài khoản!");
            LoadData();
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count == 0) return;

            int id = Convert.ToInt32(dgv.SelectedRows[0].Cells["MaND"].Value);

            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(
                    "UPDATE NguoiDung SET MatKhau='123' WHERE MaND=@id", conn);

                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Đặt lại mật khẩu thành '123'!");
            LoadData();
        }

        // ============== XÓA VĨNH VIỄN =================
        private void BtnXoaTaiKhoan_Click(object sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count == 0)
            {
                MessageBox.Show("Chọn tài khoản cần xóa!");
                return;
            }

            int id = Convert.ToInt32(dgv.SelectedRows[0].Cells["MaND"].Value);

            var confirm = MessageBox.Show(
                "Bạn có chắc muốn xóa tài khoản này vĩnh viễn?\nHành động không thể hoàn tác!",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (confirm != DialogResult.Yes) return;

            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(
                    "DELETE FROM NguoiDung WHERE MaND=@id", conn);

                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Xóa tài khoản thành công!");
            LoadData();
        }


        private void BtnTim_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();

                string sql =
                    @"SELECT N.MaND, N.Email, N.MatKhau, N.HoTen, N.TrangThai, V.TenVaiTro
                      FROM NguoiDung N
                      JOIN VaiTro V ON N.MaVaiTro = V.MaVaiTro
                      WHERE N.Email LIKE @tk OR N.HoTen LIKE @tk";

                SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                da.SelectCommand.Parameters.AddWithValue("@tk", "%" + txtTim.Text + "%");

                DataTable dt = new DataTable();
                da.Fill(dt);

                dgv.DataSource = dt;
            }
        }
    }
}
