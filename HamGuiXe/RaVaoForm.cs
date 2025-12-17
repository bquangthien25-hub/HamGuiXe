using ParkingApp.Utils;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using HamGuiXe;   // Form nhận diện biển số

namespace ParkingApp.Forms
{
    public class RaVaoForm : Form
    {
        TextBox txtBienSo;
        Button btnVao, btnRa, btnNhanDien;
        CheckBox chkMatThe;
        DataGridView dgv;

        public RaVaoForm()
        {
            Text = "Quản lý lượt vào / ra";
            Size = new Size(1100, 700);
            StartPosition = FormStartPosition.CenterScreen;

            BuildUI();
            LoadData();
        }

        // ================= UI =================
        private void BuildUI()
        {
            Panel topPanel = new Panel()
            {
                Dock = DockStyle.Top,
                Height = 200,
                BackColor = Color.WhiteSmoke
            };

            Label lblBS = new Label()
            {
                Text = "Biển số xe:",
                Left = 20,
                Top = 25,
                AutoSize = true
            };

            txtBienSo = new TextBox()
            {
                Left = 120,
                Top = 22,
                Width = 220
            };

            btnNhanDien = new Button()
            {
                Text = "📷 Nhận Diện Biển Số",
                Left = 120,
                Top = 55,
                Width = 220,

                BackColor = Color.LightSkyBlue,          // màu nền
                ForeColor = Color.Black,          // màu chữ
                FlatStyle = FlatStyle.Flat,       // style phẳng
                UseVisualStyleBackColor = false,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            btnNhanDien.Click += BtnNhanDien_Click;

            btnVao = new Button()
            {
                Text = "Xe vào",
                Left = 120,
                Top = 95,
                Width = 100,

                BackColor = Color.LightSkyBlue,          // màu nền
                ForeColor = Color.Black,          // màu chữ
                FlatStyle = FlatStyle.Flat,       // style phẳng
                UseVisualStyleBackColor = false,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            btnVao.Click += BtnVao_Click;

            btnRa = new Button()
            {
                Text = "Xe ra",
                Left = 240,
                Top = 95,
                Width = 100,

                BackColor = Color.LightPink,          // màu nền
                ForeColor = Color.Black,          // màu chữ
                FlatStyle = FlatStyle.Flat,       // style phẳng
                UseVisualStyleBackColor = false,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)

            };
            btnRa.Click += BtnRa_Click;

            chkMatThe = new CheckBox()
            {
                Text = "Mất thẻ (phạt 50.000đ)",
                Left = 120,
                Top = 130,
                AutoSize = true
            };

            topPanel.Controls.Add(lblBS);
            topPanel.Controls.Add(txtBienSo);
            topPanel.Controls.Add(btnNhanDien);
            topPanel.Controls.Add(btnVao);
            topPanel.Controls.Add(btnRa);
            topPanel.Controls.Add(chkMatThe);

            dgv = new DataGridView()
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
            dgv.CellClick += Dgv_CellClick;

            Controls.Add(dgv);
            Controls.Add(topPanel);
        }

        // ================= CLICK DÒNG → ĐỔ BIỂN SỐ =================
        private void Dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow row = dgv.Rows[e.RowIndex];
            if (row.Cells["BienSo"].Value != null)
            {
                txtBienSo.Text = row.Cells["BienSo"].Value.ToString();
            }
        }

        // ================= MỞ FORM NHẬN DIỆN =================
        private void BtnNhanDien_Click(object sender, EventArgs e)
        {
            NhanDienBienSoForm frm = new NhanDienBienSoForm();
            frm.BienSoNhanDienThanhCong += XuLyBienSoNhanDien;
            frm.ShowDialog();
        }

        // ================= XỬ LÝ BIỂN SỐ (TỪ OCR / NHẬP TAY) =================
        private void XuLyBienSoNhanDien(string bienSo)
        {
            txtBienSo.Text = bienSo;

            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();

                // kiểm tra xe đang gửi
                SqlCommand check = new SqlCommand(
                    @"SELECT COUNT(*) FROM LichSuRaVao
                      WHERE BienSo = @bs AND TrangThai = N'Đang gửi'", conn);
                check.Parameters.AddWithValue("@bs", bienSo);

                int exists = (int)check.ExecuteScalar();
                if (exists > 0)
                {
                    MessageBox.Show("⚠ Xe này đang ở trong hầm!");
                    return;
                }

                string loaiXe = PhanLoaiXe(bienSo);

                SqlCommand cmd = new SqlCommand(
                    @"INSERT INTO LichSuRaVao
                      (BienSo, LoaiXe, ThoiGianVao, TrangThai)
                      VALUES (@bs, @lx, GETDATE(), N'Đang gửi')", conn);

                cmd.Parameters.AddWithValue("@bs", bienSo);
                cmd.Parameters.AddWithValue("@lx", loaiXe);
                cmd.ExecuteNonQuery();
            }

            LoadData();
        }

        // ================= LOAD DATA =================
        private void LoadData()
        {
            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();
                SqlDataAdapter da = new SqlDataAdapter(
                @"SELECT 
                    MaLuot,
                    BienSo,
                    LoaiXe,
                    ThoiGianVao,
                    ThoiGianRa,
                    TrangThai,
                    TienThu
                  FROM LichSuRaVao
                  ORDER BY MaLuot DESC", conn);

                DataTable dt = new DataTable();
                da.Fill(dt);
                dgv.DataSource = dt;
            }
        }

        // ================= PHÂN LOẠI XE =================
        private string PhanLoaiXe(string bienSo)
        {
            bienSo = bienSo.Replace(" ", "").ToUpper();

            if (Regex.IsMatch(bienSo, @"^\d{2}[A-Z].*-\d+"))
                return "Ô tô";

            return "Xe máy";
        }

        // ================= XE VÀO (NHẬP TAY) =================
        private void BtnVao_Click(object sender, EventArgs e)
        {
            if (txtBienSo.Text.Trim() == "")
            {
                MessageBox.Show("Nhập biển số xe!");
                return;
            }

            XuLyBienSoNhanDien(txtBienSo.Text.Trim());
        }

        // ================= XE RA =================
        private void BtnRa_Click(object sender, EventArgs e)
        {
            if (dgv.CurrentRow == null)
            {
                MessageBox.Show("Chọn một lượt xe!");
                return;
            }

            int maLuot = Convert.ToInt32(dgv.CurrentRow.Cells["MaLuot"].Value);
            string loaiXe = dgv.CurrentRow.Cells["LoaiXe"].Value.ToString();

            int tien = (loaiXe == "Ô tô") ? 50000 : 3000;
            if (chkMatThe.Checked) tien += 50000;

            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(
                @"UPDATE LichSuRaVao
                  SET ThoiGianRa = GETDATE(),
                      TrangThai = N'Đã ra',
                      TienThu = @tien
                  WHERE MaLuot = @id", conn);

                cmd.Parameters.AddWithValue("@tien", tien);
                cmd.Parameters.AddWithValue("@id", maLuot);
                cmd.ExecuteNonQuery();
            }

            MessageBox.Show($"Xe đã ra – Thu {tien:N0} đ");
            chkMatThe.Checked = false;
            LoadData();
        }
    }
}
