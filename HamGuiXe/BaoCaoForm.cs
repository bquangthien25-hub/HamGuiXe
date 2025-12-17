using ParkingApp.Utils;
using ParkingApp.UI;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace ParkingApp.Forms
{
    /// <summary>
    /// Form báo cáo tổng hợp - Tạo và quản lý các báo cáo
    /// </summary>
    public class BaoCaoForm : Form
    {
        private ModernDataGridView dgv;
        private ComboBox cmbLoaiBaoCao;
        private DateTimePicker dtpTuNgay, dtpDenNgay;
        private ModernButton btnTaoBaoCao, btnXem, btnExportExcel;
        private Panel previewPanel;

        public BaoCaoForm()
        {
            Text = "Báo cáo Tổng hợp";
            Size = new Size(1200, 800);
            BackColor = ModernTheme.BackgroundDark;

            BuildUI();
            LoadBaoCaoList();
        }

        private void BuildUI()
        {
            // Header
            Label lblTitle = new Label
            {
                Text = "📊 BÁO CÁO TỔNG HỢP",
                Font = ModernTheme.FontHeader,
                ForeColor = Color.Black,  // Changed to black
                Location = new Point(20, 20),
                AutoSize = true
            };
            Controls.Add(lblTitle);

            // Control panel
            ModernPanel controlPanel = new ModernPanel
            {
                Location = new Point(20, 70),
                Size = new Size(1150, 140)
            };

            Label lblLoai = new Label
            {
                Text = "Loại báo cáo:",
                Location = new Point(15, 20),
                ForeColor = Color.Black,  // Changed to black
                Font = ModernTheme.FontBold,
                AutoSize = true
            };

            cmbLoaiBaoCao = new ComboBox
            {
                Location = new Point(130, 17),
                Width = 300,
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = ModernTheme.BackgroundDark,
                ForeColor = Color.White,
                Font = ModernTheme.FontRegular
            };
            cmbLoaiBaoCao.Items.AddRange(new string[]
            {
                "Báo cáo doanh thu",
                "Báo cáo tình trạng bãi xe",
                "Báo cáo hoạt động nhân viên",
                "Báo cáo khách hàng VIP",
                "Báo cáo lượt vào/ra"
            });
            cmbLoaiBaoCao.SelectedIndex = 0;

            Label lblTuNgay = new Label
            {
                Text = "Từ ngày:",
                Location = new Point(15, 60),
                ForeColor = Color.Black,  // Changed to black
                Font = ModernTheme.FontBold,
                AutoSize = true
            };

            dtpTuNgay = new DateTimePicker
            {
                Location = new Point(130, 57),
                Width = 200,
                Format = DateTimePickerFormat.Short
            };
            dtpTuNgay.Value = DateTime.Now.AddDays(-30);

            Label lblDenNgay = new Label
            {
                Text = "Đến ngày:",
                Location = new Point(350, 60),
                ForeColor = Color.Black,  // Changed to black
                Font = ModernTheme.FontBold,
                AutoSize = true
            };

            dtpDenNgay = new DateTimePicker
            {
                Location = new Point(450, 57),
                Width = 200,
                Format = DateTimePickerFormat.Short
            };

            btnTaoBaoCao = new ModernButton
            {
                Text = "📊 Tạo báo cáo",
                Location = new Point(15, 95),
                Width = 150,
                Height = 35,
                BaseColor = ModernTheme.Primary
            };
            btnTaoBaoCao.Click += BtnTaoBaoCao_Click;

            btnExportExcel = new ModernButton
            {
                Text = "📄 Export Excel",
                Location = new Point(180, 95),
                Width = 150,
                Height = 35,
                BaseColor = ModernTheme.Success
            };
            btnExportExcel.Click += BtnExportExcel_Click;

            controlPanel.Controls.Add(lblLoai);
            controlPanel.Controls.Add(cmbLoaiBaoCao);
            controlPanel.Controls.Add(lblTuNgay);
            controlPanel.Controls.Add(dtpTuNgay);
            controlPanel.Controls.Add(lblDenNgay);
            controlPanel.Controls.Add(dtpDenNgay);
            controlPanel.Controls.Add(btnTaoBaoCao);
            controlPanel.Controls.Add(btnExportExcel);

            Controls.Add(controlPanel);

            // Preview panel
            previewPanel = new Panel
            {
                Location = new Point(20, 230),
                Size = new Size(1150, 350),
                BackColor = ModernTheme.BackgroundCard,
                AutoScroll = true
            };

            Label lblPreview = new Label
            {
                Text = "Kết quả báo cáo:",
                Font = ModernTheme.FontBold,
                ForeColor = Color.Black,  // Changed to black
                Location = new Point(10, 10),
                AutoSize = true
            };
            previewPanel.Controls.Add(lblPreview);

            dgv = new ModernDataGridView
            {
                Location = new Point(10, 40),
                Size = new Size(1120, 290)
            };
            previewPanel.Controls.Add(dgv);

            Controls.Add(previewPanel);

            // History panel
            ModernPanel historyPanel = new ModernPanel
            {
                Location = new Point(20, 600),
                Size = new Size(1150, 160)
            };

            Label lblHistory = new Label
            {
                Text = "Lịch sử báo cáo:",
                Font = ModernTheme.FontBold,
                ForeColor = Color.Black,  // Changed to black
                Location = new Point(15, 15),
                AutoSize = true
            };
            historyPanel.Controls.Add(lblHistory);

            ListBox lstHistory = new ListBox
            {
                Location = new Point(15, 45),
                Size = new Size(1110, 100),
                BackColor = ModernTheme.BackgroundDark,
                ForeColor = Color.White,
                BorderStyle = BorderStyle.None,
                Font = ModernTheme.FontSmall
            };
            LoadRecentReports(lstHistory);
            historyPanel.Controls.Add(lstHistory);

            Controls.Add(historyPanel);
        }

        private void BtnTaoBaoCao_Click(object sender, EventArgs e)
        {
            string loaiBaoCao = cmbLoaiBaoCao.Text;
            DateTime tuNgay = dtpTuNgay.Value.Date;
            DateTime denNgay = dtpDenNgay.Value.Date;

            try
            {
                DataTable dt = new DataTable();

                using (SqlConnection conn = Database.GetConnection())
                {
                    conn.Open();
                    string sql = "";

                    switch (cmbLoaiBaoCao.SelectedIndex)
                    {
                        case 0: // Doanh thu
                            sql = @"SELECT 
                                        CAST(ThoiGianRa AS DATE) AS Ngay,
                                        COUNT(*) AS SoLuot,
                                        SUM(TienThu) AS TongTien,
                                        AVG(TienThu) AS TrungBinh
                                    FROM LichSuRaVao
                                    WHERE ThoiGianRa >= @TuNgay AND ThoiGianRa <= @DenNgay
                                      AND TienThu IS NOT NULL
                                    GROUP BY CAST(ThoiGianRa AS DATE)
                                    ORDER BY Ngay DESC";
                            break;

                        case 1: // Tình trạng bãi xe
                            sql = @"SELECT 
                                        KhuVuc,
                                        LoaiXe,
                                        COUNT(*) AS TongViTri,
                                        SUM(CASE WHEN TrangThai = N'Trống' THEN 1 ELSE 0 END) AS Trong,
                                        SUM(CASE WHEN TrangThai = N'Đang sử dụng' THEN 1 ELSE 0 END) AS DangDung,
                                        SUM(CASE WHEN TrangThai = N'Bảo trì' THEN 1 ELSE 0 END) AS BaoTri
                                    FROM ViTriDoXe
                                    GROUP BY KhuVuc, LoaiXe";
                            break;

                        case 2: // Hoạt động nhân viên
                            sql = @"SELECT 
                                        N.HoTen,
                                        V.TenVaiTro,
                                        COUNT(NK.MaNhatKy) AS SoHoatDong,
                                        MAX(NK.ThoiGian) AS LanCuoi
                                    FROM NguoiDung N
                                    JOIN VaiTro V ON N.MaVaiTro = V.MaVaiTro
                                    LEFT JOIN NhatKyHeThong NK ON N.MaND = NK.MaNguoiDung
                                        AND NK.ThoiGian >= @TuNgay AND NK.ThoiGian <= @DenNgay
                                    GROUP BY N.HoTen, V.TenVaiTro
                                    ORDER BY SoHoatDong DESC";
                            break;

                        case 3: // Khách hàng VIP
                            sql = @"SELECT 
                                        K.TenKH,
                                        K.SDT,
                                        COUNT(DISTINCT X.MaXe) AS SoXe,
                                        COUNT(V.MaVe) AS SoVe,
                                        ISNULL(SUM(V.GiaTien), 0) AS TongChiTieu
                                    FROM KhachHang K
                                    LEFT JOIN Xe X ON K.MaKH = X.MaKH
                                    LEFT JOIN Ve V ON K.MaKH = V.MaKH
                                        AND V.NgayBatDau >= @TuNgay AND V.NgayBatDau <= @DenNgay
                                   GROUP BY K.TenKH, K.SDT
                                    HAVING COUNT(V.MaVe) > 0
                                    ORDER BY TongChiTieu DESC";
                            break;

                        case 4: // Lượt vào/ra
                            sql = @"SELECT 
                                        BienSo,
                                        LoaiXe,
                                        ThoiGianVao,
                                        ThoiGianRa,
                                        TrangThai,
                                        TienThu
                                    FROM LichSuRaVao
                                    WHERE ThoiGianVao >= @TuNgay AND ThoiGianVao <= @DenNgay
                                    ORDER BY ThoiGianVao DESC";
                            break;
                    }

                    SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                    da.SelectCommand.Parameters.AddWithValue("@TuNgay", tuNgay);
                    da.SelectCommand.Parameters.AddWithValue("@DenNgay", denNgay.AddDays(1));
                    da.Fill(dt);

                    dgv.DataSource = dt;

                    // Save to database
                    SqlCommand saveCmd = new SqlCommand(
                        @"INSERT INTO BaoCao (LoaiBaoCao, NguoiTao, TuNgay, DenNgay)
                          VALUES (@Loai, @NguoiTao, @TuNgay, @DenNgay)", conn);
                    saveCmd.Parameters.AddWithValue("@Loai", loaiBaoCao);
                    saveCmd.Parameters.AddWithValue("@NguoiTao", LoginForm.CurrentUserID);
                    saveCmd.Parameters.AddWithValue("@TuNgay", tuNgay);
                    saveCmd.Parameters.AddWithValue("@DenNgay", denNgay);
                    saveCmd.ExecuteNonQuery();

                    MessageBox.Show($"Đã tạo báo cáo với {dt.Rows.Count} dòng dữ liệu!", "Thành công",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tạo báo cáo: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnExportExcel_Click(object sender, EventArgs e)
        {
            if (dgv.Rows.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu để export!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // NOTE: Để export Excel thực sự, cần cài thêm thư viện EPPlus
            // Tạm thời hiển thị thông báo
            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "Excel Files|*.xlsx",
                FileName = $"BaoCao_{cmbLoaiBaoCao.Text}_{DateTime.Now:yyyyMMdd}.xlsx"
            };

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                MessageBox.Show($"Tính năng export sẽ lưu file vào:\n{sfd.FileName}\n\n" +
                    "Để sử dụng tính năng này, cần cài NuGet package 'EPPlus' vào project.",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // TODO: Implement actual Excel export using EPPlus
                // Example code (requires EPPlus):
                // using (ExcelPackage package = new ExcelPackage())
                // {
                //     ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Report");
                //     worksheet.Cells["A1"].LoadFromDataTable(dgv.DataSource as DataTable, true);
                //     package.SaveAs(new FileInfo(sfd.FileName));
                // }
            }
        }

        private void LoadBaoCaoList()
        {
            // Placeholder - list would be loaded from BaoCao table
        }

        private void LoadRecentReports(ListBox listBox)
        {
            try
            {
                using (SqlConnection conn = Database.GetConnection())
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(
                        @"SELECT TOP 5 
                            B.LoaiBaoCao, 
                            B.ThoiGianTao,
                            N.HoTen
                          FROM BaoCao B
                          JOIN NguoiDung N ON B.NguoiTao = N.MaND
                          ORDER BY B.ThoiGianTao DESC", conn);

                    SqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        string time = Convert.ToDateTime(dr["ThoiGianTao"]).ToString("dd/MM/yyyy HH:mm");
                        string loai = dr["LoaiBaoCao"].ToString();
                        string nguoi = dr["HoTen"].ToString();
                        listBox.Items.Add($"[{time}] {loai} - bởi {nguoi}");
                    }
                }
            }
            catch { }
        }
    }
}
