using ParkingApp.Utils;              // Chứa Database.GetConnection() để kết nối SQL Server
using ParkingApp.UI;                 // Chứa ModernTheme, ModernButton, ModernDataGridView (UI tùy biến)
using System;                        // Các kiểu dữ liệu cơ bản, Exception, Convert...
using System.Data;                   // DataTable, DataAdapter...
using System.Data.SqlClient;         // SqlConnection, SqlCommand, SqlDataReader...
using System.Drawing;                // Color, Font, Point, Size...
using System.Windows.Forms;          // Form, Panel, Label, ComboBox, Button, MessageBox...

namespace ParkingApp.Forms
{
    /// <summary>
    /// Form quản lý vị trí đỗ xe - hiển thị sơ đồ bãi xe trực quan
    /// </summary>
    public class ViTriDoXeForm : Form
    {
        private Panel mapPanel;                          // Panel chứa các nút vị trí đỗ (sơ đồ bãi)
        private ModernDataGridView dgv;                  // Bảng dữ liệu chi tiết vị trí
        private Label lblStats;                          // Label thống kê tổng/trống/đang dùng/bảo trì
        private ComboBox cmbKhuVuc, cmbTrangThai;        // Bộ lọc khu vực và trạng thái
        private Button[][] slotButtons;                  // Mảng 2 chiều nút slot (hiện đang chưa dùng)

        public ViTriDoXeForm()
        {
            Text = "Quản lý Vị trí Đỗ Xe";                // Tiêu đề form
            Size = new Size(1200, 800);                   // Kích thước form
            BackColor = ModernTheme.BackgroundDark;       // Nền tối theo theme

            BuildUI();                                    // Tạo giao diện
            LoadData();                                   // Load dữ liệu + vẽ sơ đồ
        }

        private void BuildUI()
        {
            // Header (tiêu đề)
            Label lblTitle = new Label
            {
                Text = "SO DO BAI XE",                    // Tiêu đề không dấu
                Font = ModernTheme.FontHeader,            // Font header
                ForeColor = Color.Black,                  // Màu chữ
                Location = new Point(20, 20),             // Vị trí
                AutoSize = true                           // Tự co giãn theo nội dung
            };
            Controls.Add(lblTitle);                       // Thêm vào form

            // Stats (thống kê)
            lblStats = new Label
            {
                Font = ModernTheme.FontRegular,           // Font thường
                ForeColor = Color.Black,                  // Màu chữ
                Location = new Point(20, 60),             // Vị trí
                AutoSize = true                           // Tự co giãn
            };
            Controls.Add(lblStats);                       // Thêm vào form

            // Filter panel (khung lọc)
            Panel filterPanel = new Panel
            {
                Location = new Point(20, 90),             // Vị trí
                Size = new Size(1150, 50),                // Kích thước
                BackColor = ModernTheme.BackgroundCard    // Nền card
            };

            // Label "Khu vực"
            Label lblKhuVuc = new Label
            {
                Text = "Khu vuc:",                        // Text không dấu
                Location = new Point(10, 15),             // Vị trí
                ForeColor = Color.Black,                  // Màu chữ
                AutoSize = true                           // Tự co giãn
            };

            // ComboBox chọn khu vực
            cmbKhuVuc = new ComboBox
            {
                Location = new Point(80, 12),             // Vị trí
                Width = 200,                              // Chiều rộng
                DropDownStyle = ComboBoxStyle.DropDownList,// Chỉ chọn, không cho nhập
                BackColor = ModernTheme.BackgroundDark,   // Nền tối
                ForeColor = Color.Black,                  // Chữ trắng
                FlatStyle = FlatStyle.Flat                // Phẳng
            };
            cmbKhuVuc.Items.AddRange(new string[] { "Tất cả", "Khu A - Xe Máy", "Khu B - Ô Tô" }); // Danh sách khu vực
            cmbKhuVuc.SelectedIndex = 0;                  // Mặc định "Tất cả"
            cmbKhuVuc.SelectedIndexChanged += (s, e) => LoadData(); // Thay đổi lọc -> load lại dữ liệu

            // Label "Trạng thái"
            Label lblTrangThai = new Label
            {
                Text = "Trang thai:",                     // Text không dấu
                Location = new Point(300, 15),            // Vị trí
                ForeColor = Color.Black,                  // Màu chữ
                AutoSize = true                           // Tự co giãn
            };

            // ComboBox chọn trạng thái
            cmbTrangThai = new ComboBox
            {
                Location = new Point(385, 12),            // Vị trí
                Width = 150,                              // Chiều rộng
                DropDownStyle = ComboBoxStyle.DropDownList,// Chỉ chọn
                BackColor = ModernTheme.BackgroundDark,   // Nền tối
                ForeColor = Color.White,                  // Chữ trắng
                FlatStyle = FlatStyle.Flat                // Phẳng
            };
            cmbTrangThai.Items.AddRange(new string[] { "Tat ca", "Trong", "Dang su dung", "Bao tri" }); // Danh sách trạng thái (không dấu)
            cmbTrangThai.SelectedIndex = 0;               // Mặc định "Tất cả"
            cmbTrangThai.SelectedIndexChanged += (s, e) => LoadData(); // Thay đổi lọc -> load lại dữ liệu

            // Nút làm mới
            ModernButton btnRefresh = new ModernButton
            {
                Text = "🔄 Làm mới",                      // Text nút
                Location = new Point(560, 8),             // Vị trí
                Width = 120,                              // Rộng
                Height = 35,                              // Cao
                BaseColor = ModernTheme.Info              // Màu theo theme
            };
            btnRefresh.Click += (s, e) => LoadData();      // Click -> load lại dữ liệu

            // Nút đặt bảo trì
            ModernButton btnBaoTri = new ModernButton
            {
                Text = "🔧 Đặt bảo trì",                  // Text nút
                Location = new Point(700, 8),             // Vị trí
                Width = 120,                              // Rộng
                Height = 35,                              // Cao
                BaseColor = ModernTheme.Warning           // Màu cảnh báo
            };
            btnBaoTri.Click += BtnBaoTri_Click;           // Click -> xử lý đặt bảo trì cho dòng đang chọn

            // Add controls vào filterPanel
            filterPanel.Controls.Add(lblKhuVuc);          // Thêm label khu vực
            filterPanel.Controls.Add(cmbKhuVuc);          // Thêm combobox khu vực
            filterPanel.Controls.Add(lblTrangThai);       // Thêm label trạng thái
            filterPanel.Controls.Add(cmbTrangThai);       // Thêm combobox trạng thái
            filterPanel.Controls.Add(btnRefresh);         // Thêm nút làm mới
            filterPanel.Controls.Add(btnBaoTri);          // Thêm nút bảo trì

            Controls.Add(filterPanel);                    // Thêm panel lọc vào form

            // Legend (chú thích màu)
            Panel legendPanel = new Panel
            {
                Location = new Point(900, 90),            // Vị trí
                Size = new Size(270, 50),                 // Kích thước
                BackColor = ModernTheme.BackgroundCard    // Nền card
            };

            AddLegendItem(legendPanel, "● Trống", ModernTheme.Success, 10, 10);       // Chú thích màu Trống
            AddLegendItem(legendPanel, "● Đang dùng", ModernTheme.Danger, 90, 10);    // Chú thích màu Đang dùng
            AddLegendItem(legendPanel, "● Bảo trì", ModernTheme.Warning, 10, 30);     // Chú thích màu Bảo trì

            Controls.Add(legendPanel);                    // Thêm legend vào form

            // Map Panel - hiển thị lưới slot trực quan
            mapPanel = new Panel
            {
                Location = new Point(20, 160),            // Vị trí
                Size = new Size(1150, 300),               // Kích thước
                BackColor = ModernTheme.BackgroundCard,   // Nền card
                AutoScroll = true                         // Cho phép scroll nếu nhiều slot
            };
            Controls.Add(mapPanel);                       // Thêm mapPanel vào form

            // DataGridView - bảng dữ liệu chi tiết
            dgv = new ModernDataGridView
            {
                Location = new Point(20, 480),            // Vị trí
                Size = new Size(1150, 280),               // Kích thước
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None // Không auto size để tránh tràn cột
            };
            dgv.CellDoubleClick += Dgv_CellDoubleClick;   // Double click dòng -> đổi trạng thái / thao tác

            Controls.Add(dgv);                            // Thêm dgv vào form

            LoadParkingMap();                             // Vẽ sơ đồ bãi xe ngay sau khi dựng UI
        }

        private void AddLegendItem(Panel parent, string text, Color color, int x, int y)
        {
            Label lbl = new Label                         // Tạo label chú thích
            {
                Text = text,                              // Nội dung chú thích
                ForeColor = color,                        // Màu chữ theo trạng thái
                Location = new Point(x, y),               // Vị trí trong legendPanel
                AutoSize = true,                          // Tự co giãn
                Font = ModernTheme.FontSmall              // Font nhỏ
            };
            parent.Controls.Add(lbl);                     // Thêm label vào panel cha
        }

        private void LoadParkingMap()
        {
            mapPanel.Controls.Clear();                    // Xóa sơ đồ cũ để vẽ lại

            try
            {
                using (SqlConnection conn = Database.GetConnection()) // Tạo kết nối DB (tự đóng sau using)
                {
                    conn.Open();                          // Mở kết nối

                    SqlCommand cmd = new SqlCommand(      // Tạo câu lệnh SQL lấy danh sách vị trí
                        @"SELECT MaViTri, TenViTri, KhuVuc, TrangThai
                          FROM ViTriDoXe
                          ORDER BY TenViTri", conn);

                    SqlDataReader dr = cmd.ExecuteReader();// Thực thi và lấy dữ liệu dạng đọc tuần tự

                    int x = 10, y = 10;                   // Tọa độ bắt đầu vẽ nút
                    int col = 0;                          // Đếm số cột hiện tại

                    while (dr.Read())                     // Lặp từng dòng dữ liệu vị trí
                    {
                        string tenViTri = dr["TenViTri"].ToString();           // Lấy tên vị trí
                        string trangThai = dr["TrangThai"].ToString();         // Lấy trạng thái
                        int maViTri = Convert.ToInt32(dr["MaViTri"]);          // Lấy mã vị trí

                        Button btnSlot = new Button       // Tạo nút đại diện cho 1 slot đỗ xe
                        {
                            Text = tenViTri,              // Hiển thị tên vị trí
                            Size = new Size(70, 50),      // Kích thước nút
                            Location = new Point(x, y),   // Vị trí trên mapPanel
                            FlatStyle = FlatStyle.Flat,   // Style phẳng
                            Font = new Font("Segoe UI", 9F, FontStyle.Bold), // Font chữ
                            Tag = maViTri,                // Lưu MaViTri trong Tag để xử lý khi click
                            Cursor = Cursors.Hand         // Con trỏ bàn tay khi hover
                        };
                        btnSlot.FlatAppearance.BorderSize = 2; // Độ dày viền nút

                        switch (trangThai)                // Đổi màu dựa trên trạng thái
                        {
                            case "Trống":
                                btnSlot.BackColor = ModernTheme.Success;          // Màu xanh
                                btnSlot.ForeColor = Color.White;                  // Chữ trắng
                                btnSlot.FlatAppearance.BorderColor = ModernTheme.Success; // Viền xanh
                                break;

                            case "Đang sử dụng":
                                btnSlot.BackColor = ModernTheme.Danger;           // Màu đỏ
                                btnSlot.ForeColor = Color.White;                  // Chữ trắng
                                btnSlot.FlatAppearance.BorderColor = ModernTheme.Danger; // Viền đỏ
                                break;

                            case "Bảo trì":
                                btnSlot.BackColor = ModernTheme.Warning;          // Màu vàng
                                btnSlot.ForeColor = Color.Black;                  // Chữ đen
                                btnSlot.FlatAppearance.BorderColor = ModernTheme.Warning; // Viền vàng
                                break;
                        }

                        btnSlot.Click += BtnSlot_Click;    // Click vào slot -> hiện chi tiết vị trí
                        mapPanel.Controls.Add(btnSlot);     // Thêm nút vào mapPanel

                        x += 80;                            // Dịch sang phải cho slot tiếp theo
                        col++;                              // Tăng số cột

                        if (col >= 10)                      // Nếu đủ 10 slot 1 hàng thì xuống dòng
                        {
                            col = 0;                        // Reset cột
                            x = 10;                         // Reset X về đầu hàng
                            y += 60;                        // Tăng Y xuống hàng mới
                        }
                    }
                }
            }
            catch (Exception ex)                             // Bắt lỗi nếu có sự cố DB hoặc UI
            {
                MessageBox.Show($"Lỗi tải sơ đồ: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error); // Thông báo lỗi
            }
        }

        private void BtnSlot_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;                    // Ép kiểu sender về Button
            int maViTri = Convert.ToInt32(btn.Tag);         // Lấy mã vị trí từ Tag

            try
            {
                using (SqlConnection conn = Database.GetConnection()) // Mở kết nối DB
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(        // Query lấy chi tiết vị trí + thông tin xe (nếu có)
                        @"SELECT v.*, l.BienSo, l.ThoiGianVao
                          FROM ViTriDoXe v
                          LEFT JOIN LichSuRaVao l ON v.MaLuot = l.MaLuot
                          WHERE v.MaViTri = @id", conn);
                    cmd.Parameters.AddWithValue("@id", maViTri); // Truyền tham số để tránh sai / injection

                    SqlDataReader dr = cmd.ExecuteReader(); // Thực thi query

                    if (dr.Read())                          // Nếu có dữ liệu
                    {
                        string info = $"Vị trí: {dr["TenViTri"]}\n";          // Dòng thông tin vị trí
                        info += $"Khu vực: {dr["KhuVuc"]}\n";                // Dòng thông tin khu vực
                        info += $"Loại xe: {dr["LoaiXe"]}\n";                // Dòng thông tin loại xe
                        info += $"Trạng thái: {dr["TrangThai"]}\n";          // Dòng thông tin trạng thái

                        if (dr["BienSo"] != DBNull.Value)                   // Nếu có biển số (đang có xe)
                        {
                            info += $"\nBiển số: {dr["BienSo"]}\n";          // Thêm biển số
                            info += $"Thời gian vào: {dr["ThoiGianVao"]}";   // Thêm thời gian vào
                        }

                        MessageBox.Show(info, "Chi tiết vị trí", MessageBoxButtons.OK, MessageBoxIcon.Information); // Hiển thị chi tiết
                    }
                }
            }
            catch (Exception ex)                             // Bắt lỗi
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error); // Thông báo lỗi
            }
        }

        private void LoadData()
        {
            try
            {
                using (SqlConnection conn = Database.GetConnection()) // Kết nối DB
                {
                    conn.Open();                                      // Mở kết nối

                    string sql = "SELECT MaViTri, TenViTri, KhuVuc, LoaiXe, TrangThai FROM ViTriDoXe WHERE 1=1"; // Query gốc

                    if (cmbKhuVuc.SelectedIndex > 0)                  // Nếu không chọn "Tất cả"
                    {
                        sql += $" AND KhuVuc = N'{cmbKhuVuc.Text}'";  // Lọc theo khu vực đã chọn
                    }

                    if (cmbTrangThai.SelectedIndex > 0)              // Nếu không chọn "Tất cả"
                    {
                        sql += $" AND TrangThai = N'{cmbTrangThai.Text}'"; // Lọc theo trạng thái đã chọn
                    }

                    sql += " ORDER BY TenViTri";                      // Sắp xếp theo tên vị trí

                    SqlDataAdapter da = new SqlDataAdapter(sql, conn);// Adapter để fill DataTable
                    DataTable dt = new DataTable();                   // Tạo bảng dữ liệu
                    da.Fill(dt);                                      // Đổ dữ liệu vào dt
                    dgv.DataSource = dt;                              // Gán dt cho DataGridView

                    if (dgv.Columns.Count > 0)                        // Nếu đã có cột
                    {
                        dgv.Columns["MaViTri"].Width = 80;            // Đặt width cột mã vị trí
                        dgv.Columns["MaViTri"].HeaderText = "Ma vi tri";

                        dgv.Columns["TenViTri"].Width = 100;          // Đặt width cột tên vị trí
                        dgv.Columns["TenViTri"].HeaderText = "Ten vi tri";

                        dgv.Columns["KhuVuc"].Width = 200;            // Đặt width cột khu vực
                        dgv.Columns["KhuVuc"].HeaderText = "Khu vuc";

                        dgv.Columns["LoaiXe"].Width = 120;            // Đặt width cột loại xe
                        dgv.Columns["LoaiXe"].HeaderText = "Loai xe";

                        dgv.Columns["TrangThai"].Width = 150;         // Đặt width cột trạng thái
                        dgv.Columns["TrangThai"].HeaderText = "Trang thai";
                    }

                    SqlCommand cmdStats = new SqlCommand(             // Query thống kê số lượng theo trạng thái
                        @"SELECT 
                            COUNT(*) AS Tong,
                            SUM(CASE WHEN TrangThai = N'Trống' THEN 1 ELSE 0 END) AS Trong,
                            SUM(CASE WHEN TrangThai = N'Đang sử dụng' THEN 1 ELSE 0 END) AS DangDung,
                            SUM(CASE WHEN TrangThai = N'Bảo trì' THEN 1 ELSE 0 END) AS BaoTri
                          FROM ViTriDoXe", conn);

                    SqlDataReader dr = cmdStats.ExecuteReader();      // Thực thi thống kê
                    if (dr.Read())                                    // Đọc kết quả
                    {
                        int tong = Convert.ToInt32(dr["Tong"]);       // Tổng số vị trí
                        int trong = Convert.ToInt32(dr["Trong"]);     // Số vị trí trống
                        int dangDung = Convert.ToInt32(dr["DangDung"]);// Số vị trí đang dùng
                        int baoTri = Convert.ToInt32(dr["BaoTri"]);   // Số vị trí bảo trì

                        lblStats.Text = $"Tong: {tong} | Trong: {trong} | Dang dung: {dangDung} | Bao tri: {baoTri}"; // Hiển thị thống kê
                    }
                }

                LoadParkingMap();                                     // Sau khi load data -> vẽ lại sơ đồ theo trạng thái mới
            }
            catch (Exception ex)                                      // Bắt lỗi
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error); // Thông báo lỗi
            }
        }

        private void Dgv_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;                               // Nếu click header thì bỏ qua

            int maViTri = Convert.ToInt32(dgv.Rows[e.RowIndex].Cells["MaViTri"].Value); // Lấy mã vị trí từ dòng
            string tenViTri = dgv.Rows[e.RowIndex].Cells["TenViTri"].Value.ToString(); // Lấy tên vị trí
            string trangThai = dgv.Rows[e.RowIndex].Cells["TrangThai"].Value.ToString(); // Lấy trạng thái hiện tại

            if (trangThai == "Đang sử dụng")                          // Nếu đang có xe thì không cho đổi trạng thái
            {
                MessageBox.Show("Vị trí này đang được sử dụng!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show(                    // Hỏi xác nhận đổi trạng thái
                $"Chuyển trạng thái vị trí {tenViTri}?",
                "Xác nhận",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)                           // Nếu đồng ý
            {
                ToggleStatus(maViTri, trangThai);                     // Đổi trạng thái trong DB
            }
        }

        private void BtnBaoTri_Click(object sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count == 0)                          // Nếu chưa chọn dòng nào
            {
                MessageBox.Show("Chọn vị trí cần đặt bảo trì!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int maViTri = Convert.ToInt32(dgv.SelectedRows[0].Cells["MaViTri"].Value); // Lấy mã vị trí dòng đang chọn
            string trangThai = dgv.SelectedRows[0].Cells["TrangThai"].Value.ToString(); // Lấy trạng thái hiện tại

            ToggleStatus(maViTri, trangThai);                          // Đổi trạng thái (Trống <-> Bảo trì)
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();                                     // Tạm dừng layout để tối ưu hiệu năng khi init
            this.ClientSize = new System.Drawing.Size(274, 229);      // Kích thước mặc định (thường do designer tạo)
            this.Name = "ViTriDoXeForm";                              // Tên form
            this.Load += new System.EventHandler(this.ViTriDoXeForm_Load); // Gắn sự kiện Load
            this.ResumeLayout(false);                                 // Tiếp tục layout
        }

        private void ViTriDoXeForm_Load(object sender, EventArgs e)
        {
            // (Hiện tại trống) - Nếu muốn có thể đặt LoadData/LoadParkingMap tại đây
        }

        private void ToggleStatus(int maViTri, string currentStatus)
        {
            try
            {
                string newStatus = currentStatus == "Bảo trì" ? "Trống" : "Bảo trì"; // Nếu đang bảo trì -> chuyển trống, ngược lại -> bảo trì

                using (SqlConnection conn = Database.GetConnection()) // Mở kết nối DB
                {
                    conn.Open();                                      // Mở kết nối

                    SqlCommand cmd = new SqlCommand(                  // Lệnh update trạng thái vị trí
                        "UPDATE ViTriDoXe SET TrangThai = @status WHERE MaViTri = @id", conn);
                    cmd.Parameters.AddWithValue("@status", newStatus);// Truyền trạng thái mới
                    cmd.Parameters.AddWithValue("@id", maViTri);      // Truyền mã vị trí
                    cmd.ExecuteNonQuery();                            // Thực thi update
                }

                LoadData();                                           // Reload lại dữ liệu + vẽ lại sơ đồ
                MessageBox.Show("Cập nhật thành công!", "Thành công",
                    MessageBoxButtons.OK, MessageBoxIcon.Information); // Thông báo thành công
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);      // Thông báo lỗi
            }
        }
    }
}
