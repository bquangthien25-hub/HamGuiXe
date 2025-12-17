using ParkingApp.Utils;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace ParkingApp.Forms
{
    public class XeForm : Form
    {
        DataGridView dgv;
        TextBox txtBienSo, txtTimKiem;
        ComboBox cmbLoaiXe;
        Button btnThem, btnSua, btnXoa, btnTim;

        public XeForm()
        {
            this.Text = "Quản lý xe";
            this.Size = new Size(900, 600);

            BuildUI();
            LoadData();
        }

        private void BuildUI()
        {
            // ======== DATAGRIDVIEW ==========
            dgv = new DataGridView()
            {
                Dock = DockStyle.Top,
                Height = 350,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true
            };
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.CellClick += Dgv_CellClick;
            Controls.Add(dgv);
            //Tránh NULL gây Crash
            dgv.AllowUserToAddRows = false;


            // ======== LABEL + INPUT ==========
            Label lblBienSo = new Label() { Text = "Biển số:", Left = 20, Top = 380 };
            txtBienSo = new TextBox() { Left = 120, Top = 380, Width = 200 };

            Label lblLoaiXe = new Label() { Text = "Loại xe:", Left = 20, Top = 420 };
            cmbLoaiXe = new ComboBox()
            {
                Left = 120,
                Top = 420,
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbLoaiXe.Items.AddRange(new string[] { "Xe máy", "Xe đạp", "Ô tô" });

            // ======== BUTTONS ==========
            btnThem = new Button() { Text = "Thêm", Left = 350, Top = 380, Width = 120 };
            btnSua = new Button() { Text = "Sửa", Left = 480, Top = 380, Width = 120 };
            btnXoa = new Button() { Text = "Xóa", Left = 610, Top = 380, Width = 120 };

            btnThem.Click += BtnThem_Click;
            btnSua.Click += BtnSua_Click;
            btnXoa.Click += BtnXoa_Click;

            // ======== TÌM KIẾM ==========
            txtTimKiem = new TextBox() { Left = 350, Top = 420, Width = 250 };
            btnTim = new Button() { Text = "Tìm", Left = 610, Top = 420, Width = 120 };
            btnTim.Click += BtnTim_Click;

            // Add Controls
            Controls.Add(lblBienSo);
            Controls.Add(txtBienSo);
            Controls.Add(lblLoaiXe);
            Controls.Add(cmbLoaiXe);

            Controls.Add(btnThem);
            Controls.Add(btnSua);
            Controls.Add(btnXoa);

            Controls.Add(txtTimKiem);
            Controls.Add(btnTim);
        }

        // ============= LOAD DATA =============
        private void LoadData()
        {
            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();
                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Xe", conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgv.DataSource = dt;
            }
        }

        // ============= CLICK VÀO ROW → LẤY DỮ LIỆU =============
        private void Dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                txtBienSo.Text = dgv.Rows[e.RowIndex].Cells["BienSo"].Value.ToString();
                cmbLoaiXe.Text = dgv.Rows[e.RowIndex].Cells["LoaiXe"].Value.ToString();
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // XeForm
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Name = "XeForm";
            this.Load += new System.EventHandler(this.XeForm_Load);
            this.ResumeLayout(false);

        }

        private void XeForm_Load(object sender, EventArgs e)
        {

        }

        // ============= TÌM KIẾM =============
        private void BtnTim_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();
                string query = "SELECT * FROM Xe WHERE BienSo LIKE @bs";
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                da.SelectCommand.Parameters.AddWithValue("@bs", "%" + txtTimKiem.Text + "%");

                DataTable dt = new DataTable();
                da.Fill(dt);
                dgv.DataSource = dt;
            }
        }

        // ============= THÊM XE =============
        private void BtnThem_Click(object sender, EventArgs e)
        {
            if (txtBienSo.Text == "" || cmbLoaiXe.SelectedIndex == -1)
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!");
                return;
            }

            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();

                // Check trùng biển số
                SqlCommand check = new SqlCommand("SELECT COUNT(*) FROM Xe WHERE BienSo=@bs", conn);
                check.Parameters.AddWithValue("@bs", txtBienSo.Text);
                int exists = (int)check.ExecuteScalar();

                if (exists > 0)
                {
                    MessageBox.Show("Biển số này đã tồn tại!");
                    return;
                }

                SqlCommand cmd = new SqlCommand(
                    "INSERT INTO Xe (BienSo, LoaiXe) VALUES (@bs, @lx)", conn);

                cmd.Parameters.AddWithValue("@bs", txtBienSo.Text);
                cmd.Parameters.AddWithValue("@lx", cmbLoaiXe.Text);

                cmd.ExecuteNonQuery();
                MessageBox.Show("Thêm thành công!");
            }

            LoadData();
        }

        // ============= SỬA XE =============
        private void BtnSua_Click(object sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count == 0)
            {
                MessageBox.Show("Chọn xe cần sửa!");
                return;
            }

            int MaXe = Convert.ToInt32(dgv.SelectedRows[0].Cells["MaXe"].Value);

            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(
                    "UPDATE Xe SET BienSo=@bs, LoaiXe=@lx WHERE MaXe=@id", conn);

                cmd.Parameters.AddWithValue("@bs", txtBienSo.Text);
                cmd.Parameters.AddWithValue("@lx", cmbLoaiXe.Text);
                cmd.Parameters.AddWithValue("@id", MaXe);

                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Sửa thành công!");
            LoadData();
        }

        // ============= XÓA XE =============
        private void BtnXoa_Click(object sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count == 0)
            {
                MessageBox.Show("Chọn xe cần xóa!");
                return;
            }

            int MaXe = Convert.ToInt32(dgv.SelectedRows[0].Cells["MaXe"].Value);

            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("DELETE FROM Xe WHERE MaXe=@id", conn);
                cmd.Parameters.AddWithValue("@id", MaXe);
                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Xóa thành công!");
            LoadData();
        }
    }
}
