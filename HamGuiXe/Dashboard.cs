using ParkingApp.Forms;
using ParkingApp.Utils;
using ParkingApp.UI;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace ParkingApp
{
    public class DashboardForm : Form
    {
        private Panel menuPanel, contentPanel, headerPanel, statsPanel;
        private Button btnXe, btnVe, btnKhach, btnRaVao, btnNhanVien, btnDoanhThu, btnViTriDoXe, btnBaoCao, btnLogout;
        private Label lblWelcome, lblDateTime;
        private Timer refreshTimer, animationTimer;
        private int animationStep = 0;
        
        // Stat cards - ADDED BACK
        private StatCard cardXeDangGui, cardDoanhThu, cardTyLeLayDay, cardLuotVaoRa;

        public DashboardForm()
        {
            Text = "Hệ thống Quản lý Hầm Gửi Xe V2.0";
            WindowState = FormWindowState.Maximized;
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = ModernTheme.BackgroundDark;

            BuildUI();
            ApplyRole();
            LoadDashboard();
            
            // Auto refresh every 30 seconds
            refreshTimer = new Timer();
            refreshTimer.Interval = 30000; // 30 seconds
            refreshTimer.Tick += (s, e) => RefreshStats();
            refreshTimer.Start();

            // Welcome animation - fade in then fade out
            StartWelcomeAnimation();
        }

        private void StartWelcomeAnimation()
        {
            animationStep = 0;
            headerPanel.Visible = true;
            
            animationTimer = new Timer();
            animationTimer.Interval = 50; // 50ms per step
            animationTimer.Tick += AnimationTimer_Tick;
            animationTimer.Start();
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            animationStep++;

            if (animationStep <= 10)
            {
                // Fade in: 0 → 10 (500ms)
                // No built-in opacity for Panel, so we'll just show it
                headerPanel.Visible = true;
            }
            else if (animationStep <= 70)
            {
                // Hold: stay visible for 3 seconds (60 steps * 50ms = 3000ms)
                // Do nothing, keep visible
            }
            else if (animationStep <= 80)
            {
                // Fade out: will hide after
                // No built-in opacity, so just keep visible
            }
            else
            {
                // Hide completely
                headerPanel.Visible = false;
                animationTimer.Stop();
            }
        }

        private void BuildUI()
        {
            // ===== HEADER PANEL ===== (Dark for contrast - SMALLER)
            headerPanel = new Panel()
            {
                Dock = DockStyle.Top,
                Height = 50,  // Reduced from 70 to 50
                BackColor = ColorTranslator.FromHtml("#2d3748") // Dark header
            };

            lblWelcome = new Label()
            {
                Text = $"Xin chào, {LoginForm.CurrentUser}",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold), // Smaller font
                ForeColor = Color.White,
                Location = new Point(20, 10),  // Adjusted position
                AutoSize = true
            };

            lblDateTime = new Label()
            {
                Font = new Font("Segoe UI", 8.5F),  // Smaller font
                ForeColor = ModernTheme.TextSecondary,
                Location = new Point(20, 30),  // Adjusted position
                AutoSize = true
            };
            UpdateDateTime();

            // Update time every second
            Timer timeTimer = new Timer();
            timeTimer.Interval = 1000;
            timeTimer.Tick += (s, e) => UpdateDateTime();
            timeTimer.Start();

            ModernButton btnRefresh = new ModernButton()
            {
                Text = "🔄 Làm mới",
                Width = 120,
                Height = 35,
                BaseColor = ModernTheme.Info
            };
            btnRefresh.Location = new Point(headerPanel.Width - 140, 7);  // Adjusted for 50px header
            btnRefresh.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnRefresh.Click += (s, e) => RefreshStats();

            headerPanel.Controls.Add(lblWelcome);
            headerPanel.Controls.Add(lblDateTime);
            headerPanel.Controls.Add(btnRefresh);

            Controls.Add(headerPanel);

            // ===== CONTENT PANEL =====
            contentPanel = new Panel()
            {
                Dock = DockStyle.Fill,
                BackColor = ModernTheme.BackgroundDark,
                AutoScroll = true,
                Padding = new Padding(20)
            };
            Controls.Add(contentPanel);

            // ===== MENU PANEL ===== (Keep dark for contrast)
            menuPanel = new Panel()
            {
                Dock = DockStyle.Left,
                Width = 250,
                BackColor = ColorTranslator.FromHtml("#2d3748") // Dark sidebar
            };
            Controls.Add(menuPanel);

            // Logo
            Label lblLogo = new Label()
            {
                Text = "🅿️ PARKING V3",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = ModernTheme.Primary,
                Dock = DockStyle.Top,
                Height = 60,
                TextAlign = ContentAlignment.MiddleCenter
            };
            menuPanel.Controls.Add(lblLogo);

            // ===== MENU BUTTONS =====
            btnXe = CreateMenuButton("🚗 Quản lý xe", "Quản lý thông tin xe");
            btnVe = CreateMenuButton("🎫 Quản lý vé", "Vé tháng, vé lượt");
            btnKhach = CreateMenuButton("👥 Khách hàng", "Quản lý khách hàng");
            btnRaVao = CreateMenuButton("🚪 Ra / Vào", "Quản lý lượt vào/ra");
            btnViTriDoXe = CreateMenuButton("🅿️ Vị trí đỗ", "Sơ đồ bãi xe");
            btnNhanVien = CreateMenuButton("👔 Nhân viên", "Quản lý nhân viên");
            btnDoanhThu = CreateMenuButton("💰 Doanh thu", "Báo cáo doanh thu");
            btnBaoCao = CreateMenuButton("📊 Báo cáo", "Báo cáo tổng hợp");
            btnLogout = CreateMenuButton("🚪 Đăng xuất", "Thoát khỏi hệ thống");

            // ===== EVENTS =====
            btnXe.Click += (s, e) => LoadContent(new XeForm());
            btnVe.Click += (s, e) => LoadContent(new VeForm());
            btnKhach.Click += (s, e) => LoadContent(new KhachForm());
            btnRaVao.Click += (s, e) => LoadContent(new RaVaoForm());
            btnViTriDoXe.Click += (s, e) => LoadContent(new ViTriDoXeForm());
            btnNhanVien.Click += (s, e) => LoadContent(new NhanVienForm());
            btnDoanhThu.Click += (s, e) => LoadContent(new DoanhThuForm());
            btnBaoCao.Click += (s, e) => LoadContent(new BaoCaoForm());
            btnLogout.Click += BtnLogout_Click;

            menuPanel.Controls.Add(btnXe);
            menuPanel.Controls.Add(btnVe);
            menuPanel.Controls.Add(btnKhach);
            menuPanel.Controls.Add(btnRaVao);
            menuPanel.Controls.Add(btnViTriDoXe);
            menuPanel.Controls.Add(btnNhanVien);
            menuPanel.Controls.Add(btnDoanhThu);
            menuPanel.Controls.Add(btnBaoCao);
            menuPanel.Controls.Add(btnLogout);
        }

        private Button CreateMenuButton(string text, string tooltip)
        {
            Button btn = new Button()
            {
                Text = text,
                Dock = DockStyle.Top,
                Height = 55,
                BackColor = ModernTheme.BackgroundCard,
                ForeColor = ModernTheme.Primary,  // Blue text instead of white
                FlatStyle = FlatStyle.Flat,
                Font = ModernTheme.FontBold,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(20, 0, 0, 0),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.MouseEnter += (s, e) => {
                btn.BackColor = ModernTheme.BackgroundHover;
                btn.ForeColor = ModernTheme.PrimaryLight;  // Lighter blue on hover
            };
            btn.MouseLeave += (s, e) => {
                btn.BackColor = ModernTheme.BackgroundCard;
                btn.ForeColor = ModernTheme.Primary;  // Back to blue
            };

            ToolTip tt = new ToolTip();
            tt.SetToolTip(btn, tooltip);

            return btn;
        }

        private void LoadContent(Form frm)
        {
            contentPanel.Controls.Clear();

            frm.TopLevel = false;
            frm.FormBorderStyle = FormBorderStyle.None;
            frm.Width = contentPanel.Width - 40;
            frm.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            frm.BackColor = ModernTheme.BackgroundDark;

            contentPanel.Controls.Add(frm);
            frm.Show();
        }

        private void LoadDashboard()
        {
            contentPanel.Controls.Clear();

            // Stats Panel
            statsPanel = new Panel()
            {
                Location = new Point(20, 20),
                Size = new Size(contentPanel.Width - 60, 120),
                BackColor = Color.Transparent,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            // Create stat cards
            cardXeDangGui = new StatCard("Xe đang gửi", "0", "🚗", ModernTheme.Info);
            cardXeDangGui.Location = new Point(0, 0);

            cardDoanhThu = new StatCard("Doanh thu hôm nay", "0 đ", "💰", ModernTheme.Success);
            cardDoanhThu.Location = new Point(220, 0);

            cardTyLeLayDay = new StatCard("Tỷ lệ lấp đầy", "0%", "📊", ModernTheme.Warning);
            cardTyLeLayDay.Location = new Point(440, 0);

            cardLuotVaoRa = new StatCard("Lượt vào/ra", "0", "🚪", ModernTheme.Primary);
            cardLuotVaoRa.Location = new Point(660, 0);

            statsPanel.Controls.Add(cardXeDangGui);
            statsPanel.Controls.Add(cardDoanhThu);
            statsPanel.Controls.Add(cardTyLeLayDay);
            statsPanel.Controls.Add(cardLuotVaoRa);

            contentPanel.Controls.Add(statsPanel);

            // Charts Panel
            Panel chartsPanel = new Panel()
            {
                Location = new Point(20, 160),
                Size = new Size(contentPanel.Width - 60, 400),
                BackColor = Color.Transparent,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            // Revenue Chart (Left)
            ModernPanel revenuePanel = new ModernPanel()
            {
                Location = new Point(0, 0),
                Size = new Size(550, 400)
            };

            Label lblRevenueChart = new Label()
            {
                Text = "Doanh thu 7 ngày qua",
                Font = ModernTheme.FontBold,
                ForeColor = Color.White,
                Location = new Point(15, 15),
                AutoSize = true
            };
            revenuePanel.Controls.Add(lblRevenueChart);

            Chart revenueChart = CreateRevenueChart();
            revenueChart.Location = new Point(10, 50);
            revenueChart.Size = new Size(530, 330);
            revenuePanel.Controls.Add(revenueChart);

            chartsPanel.Controls.Add(revenuePanel);

            // Activity Panel (Right)
            ModernPanel activityPanel = new ModernPanel()
            {
                Location = new Point(570, 0),
                Size = new Size(400, 400),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            Label lblActivity = new Label()
            {
                Text = "Hoạt động gần đây",
                Font = ModernTheme.FontBold,
                ForeColor = Color.White,
                Location = new Point(15, 15),
                AutoSize = true
            };
            activityPanel.Controls.Add(lblActivity);

            ListBox lstActivity = new ListBox()
            {
                Location = new Point(15, 50),
                Size = new Size(370, 330),
                BackColor = ModernTheme.BackgroundDark,
                ForeColor = Color.White,
                BorderStyle = BorderStyle.None,
                Font = ModernTheme.FontSmall
            };
            LoadRecentActivity(lstActivity);
            activityPanel.Controls.Add(lstActivity);

            chartsPanel.Controls.Add(activityPanel);

            contentPanel.Controls.Add(chartsPanel);

            // Load statistics
            RefreshStats();
        }

        private Chart CreateRevenueChart()
        {
            Chart chart = new Chart();
            chart.BackColor = ModernTheme.BackgroundDark;
            
            ChartArea chartArea = new ChartArea();
            chartArea.BackColor = ModernTheme.BackgroundDark;
            chartArea.AxisX.LabelStyle.ForeColor = Color.White;
            chartArea.AxisY.LabelStyle.ForeColor = Color.White;
            chartArea.AxisX.LineColor = ModernTheme.BorderColor;
            chartArea.AxisY.LineColor = ModernTheme.BorderColor;
            chartArea.AxisX.MajorGrid.LineColor = ModernTheme.BorderColor;
            chartArea.AxisY.MajorGrid.LineColor = ModernTheme.BorderColor;
            chart.ChartAreas.Add(chartArea);

            Series series = new Series();
            series.ChartType = SeriesChartType.Column;
            series.Color = ModernTheme.Primary;
            series.BorderWidth = 2;
            
            // Load data from database
            try
            {
                using (SqlConnection conn = Database.GetConnection())
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("EXEC SP_GetRevenue7Days", conn);
                    SqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        DateTime ngay = Convert.ToDateTime(dr["Ngay"]);
                        int tongTien = Convert.ToInt32(dr["TongTien"]);
                        series.Points.AddXY(ngay.ToString("dd/MM"), tongTien);
                    }
                }
            }
            catch { }

            chart.Series.Add(series);
            
            return chart;
        }

        private void RefreshStats()
        {
            try
            {
                using (SqlConnection conn = Database.GetConnection())
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("EXEC SP_GetDashboardStats", conn);
                    SqlDataReader dr = cmd.ExecuteReader();

                    if (dr.Read())
                    {
                        int xeDangGui = Convert.ToInt32(dr["XeDangGui"]);
                        int doanhThu = Convert.ToInt32(dr["DoanhThuHomNay"]);
                        decimal tyLe = Convert.ToDecimal(dr["TyLeLayDay"]);
                        int luotVaoRa = Convert.ToInt32(dr["LuotVaoRa"]);

                        cardXeDangGui.UpdateValue(xeDangGui.ToString());
                        cardDoanhThu.UpdateValue($"{doanhThu:N0} đ");
                        cardTyLeLayDay.UpdateValue($"{tyLe:F1}%");
                        cardLuotVaoRa.UpdateValue(luotVaoRa.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle error silently in refresh
            }
        }

        private void LoadRecentActivity(ListBox listBox)
        {
            try
            {
                using (SqlConnection conn = Database.GetConnection())
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(
                        @"SELECT TOP 10 HanhDong, ChiTiet, ThoiGian
                          FROM NhatKyHeThong
                          ORDER BY ThoiGian DESC", conn);
                    
                    SqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        string time = Convert.ToDateTime(dr["ThoiGian"]).ToString("HH:mm dd/MM");
                        string action = dr["HanhDong"].ToString();
                        listBox.Items.Add($"[{time}] {action}");
                    }
                }
            }
            catch { }
        }

        private void UpdateDateTime()
        {
            lblDateTime.Text = DateTime.Now.ToString("dddd, dd/MM/yyyy HH:mm:ss");
        }

        private void BtnLogout_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Bạn có chắc muốn đăng xuất?",
                "Xác nhận",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                // Stop all timers
                refreshTimer?.Stop();
                refreshTimer?.Dispose();
                animationTimer?.Stop();
                animationTimer?.Dispose();
                
                // Close this form - will trigger Application.Exit()
                this.Close();
            }
        }

        private void ApplyRole()
        {
            if (LoginForm.CurrentRole == "Staff")
            {
                btnNhanVien.Visible = false;
                btnBaoCao.Visible = false;
            }
        }
    }
}
