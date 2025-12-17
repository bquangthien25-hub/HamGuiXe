using ParkingApp.Utils;
using ParkingApp.UI;
using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ParkingApp.Forms
{
    public partial class LoginForm : Form
    {
        public static string CurrentUser = "";
        public static string CurrentRole = "";
        public static int CurrentUserID = 0;

        private TextBox txtUser, txtPass;
        private Button btnLogin;
        private Panel loginPanel;
        private CheckBox chkRemember;

        public LoginForm()
        {
            InitializeComponent();
            BuildModernUI();
        }

        private void BuildModernUI()
        {
            // Form settings
            this.Text = "Hệ thống Quản lý Hầm Gửi Xe V2";
            this.Size = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = ModernTheme.BackgroundDark;
            this.FormBorderStyle = FormBorderStyle.None;

            // Add close button
            Button btnClose = new Button
            {
                Text = "✕",
                Size = new Size(40, 40),
                Location = new Point(this.Width - 45, 5),
                BackColor = Color.Transparent,
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 16F),
                Cursor = Cursors.Hand
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) => Application.Exit();
            btnClose.MouseEnter += (s, e) => btnClose.BackColor = ModernTheme.Danger;
            btnClose.MouseLeave += (s, e) => btnClose.BackColor = Color.Transparent;
            Controls.Add(btnClose);

            // Left side - Branding
            Panel brandingPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 500,
                BackColor = ModernTheme.BackgroundDark
            };

            Label lblBranding = new Label
            {
                Text = "🅿️",
                Font = new Font("Segoe UI", 72F, FontStyle.Bold),
                ForeColor = ModernTheme.Primary,
                Location = new Point(170, 150),
                AutoSize = true
            };

            Label lblTitle = new Label
            {
                Text = "PARKING\nMANAGEMENT",
                Font = new Font("Segoe UI", 28F, FontStyle.Bold),
                ForeColor = Color.LightSkyBlue,
                Location = new Point(100, 280),
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleCenter
            };

            Label lblVersion = new Label
            {
                Text = "Professional Edition V2.0",
                Font = ModernTheme.FontRegular,
                ForeColor = ModernTheme.TextSecondary,
                Location = new Point(150, 500),
                AutoSize = true
            };

            Label lblSubtitle = new Label
            {
                Text = "Hệ Thống Quản Lý Bãi Xe Thông Minh",
                Font = ModernTheme.FontSmall,
                ForeColor = ModernTheme.TextMuted,
                Location = new Point(100, 440),
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleCenter
            };

            brandingPanel.Controls.Add(lblBranding);
            brandingPanel.Controls.Add(lblTitle);
            brandingPanel.Controls.Add(lblVersion);
            brandingPanel.Controls.Add(lblSubtitle);
            Controls.Add(brandingPanel);

            // Right side - Login form
            loginPanel = new Panel
            {
                Location = new Point(550, 100),
                Size = new Size(400, 400),
                BackColor = ModernTheme.BackgroundCard
            };
            loginPanel.Paint += LoginPanel_Paint; // Custom paint for rounded corners

            Label lblLoginTitle = new Label
            {
                Text = "Đăng Nhập",
                Font = ModernTheme.FontHeader,
                ForeColor = Color.White,
                Location = new Point(30, 30),
                AutoSize = true
            };

            Label lblLoginSubtitle = new Label
            {
                Text = "Nhập thông tin để tiếp tục",
                Font = ModernTheme.FontSmall,
                ForeColor = ModernTheme.TextSecondary,
                Location = new Point(30, 70),
                AutoSize = true
            };

            // Email label & input
            Label lblEmail = new Label
            {
                Text = "Email",
                Font = ModernTheme.FontBold,
                ForeColor = ModernTheme.TextSecondary,
                Location = new Point(30, 120),
                AutoSize = true
            };

            txtUser = new TextBox
            {
                Location = new Point(30, 145),
                Width = 340,
                Height = 40,
                BackColor = Color.White,
                ForeColor = Color.Black,
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 11F),
                Text = "admin@parking.com" // Pre-fill for testing
            };
            Panel userBorder = CreateInputBorder(txtUser);

            // Password label & input
            Label lblPassword = new Label
            {
                Text = "Mật khẩu",
                Font = ModernTheme.FontBold,
                ForeColor = ModernTheme.TextSecondary,
                Location = new Point(30, 210),
                AutoSize = true
            };

            txtPass = new TextBox
            {
                Location = new Point(30, 235),
                Width = 340,
                Height = 40,
                BackColor = Color.White,
                ForeColor = Color.Black,
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 11F),
                PasswordChar = '●',
                Text = "123456" // Pre-fill for testing
            };
            Panel passBorder = CreateInputBorder(txtPass);

            // Remember me checkbox
            chkRemember = new CheckBox
            {
                Text = "Ghi nhớ đăng nhập",
                Location = new Point(30, 290),
                ForeColor = ModernTheme.TextSecondary,
                Font = ModernTheme.FontSmall,
                AutoSize = true
            };

            // Login button
            ModernButton btnLoginModern = new ModernButton
            {
                Text = "ĐĂNG NHẬP",
                Location = new Point(30, 330),
                Width = 340,
                Height = 45,
                BaseColor = ModernTheme.Primary,
                HoverColor = ModernTheme.PrimaryDark
            };
            btnLoginModern.Click += BtnLogin_Click;

            loginPanel.Controls.Add(lblLoginTitle);
            loginPanel.Controls.Add(lblLoginSubtitle);
            loginPanel.Controls.Add(lblEmail);
            loginPanel.Controls.Add(userBorder);
            loginPanel.Controls.Add(lblPassword);
            loginPanel.Controls.Add(passBorder);
            loginPanel.Controls.Add(chkRemember);
            loginPanel.Controls.Add(btnLoginModern);

            Controls.Add(loginPanel);

            // Allow Enter key to login
            txtPass.KeyPress += (s, e) =>
            {
                if (e.KeyChar == (char)13) // Enter key
                {
                    btnLoginModern.PerformClick();
                }
            };
        }

        private Panel CreateInputBorder(TextBox textBox)
        {
            Panel border = new Panel
            {
                Location = new Point(textBox.Left - 2, textBox.Top - 2),
                Size = new Size(textBox.Width + 4, textBox.Height + 4),
                BackColor = ModernTheme.BorderColor
            };

            textBox.Parent = border;
            textBox.Location = new Point(2, 2);

            textBox.GotFocus += (s, e) => border.BackColor = ModernTheme.Primary;
            textBox.LostFocus += (s, e) => border.BackColor = ModernTheme.BorderColor;

            return border;
        }

        private void LoginPanel_Paint(object sender, PaintEventArgs e)
        {
            // Draw rounded corners and shadow
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle rect = new Rectangle(0, 0, loginPanel.Width - 1, loginPanel.Height - 1);
            int radius = 15;

            using (GraphicsPath path = GetRoundedRectangle(rect, radius))
            {
                loginPanel.Region = new Region(path);

                using (SolidBrush brush = new SolidBrush(ModernTheme.BackgroundCard))
                {
                    e.Graphics.FillPath(brush, path);
                }
            }
        }

        private GraphicsPath GetRoundedRectangle(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int diameter = radius * 2;

            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();

            return path;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ClientSize = new System.Drawing.Size(1000, 600);
            this.Name = "LoginForm";
            this.Load += new System.EventHandler(this.LoginForm_Load);
            this.ResumeLayout(false);
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            // Form load event
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string email = txtUser.Text.Trim();
            string password = txtPass.Text;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SqlConnection conn = Database.GetConnection())
                {
                    conn.Open();

                    // First, try to login with plain password (for backward compatibility)
                    string sql = @"SELECT MaND, Email, HoTen, V.TenVaiTro, MatKhau
                                   FROM NguoiDung N 
                                   JOIN VaiTro V ON N.MaVaiTro = V.MaVaiTro
                                   WHERE Email=@email AND TrangThai=1";

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@email", email);

                    SqlDataReader dr = cmd.ExecuteReader();

                    if (dr.Read())
                    {
                        string storedPassword = dr["MatKhau"].ToString();
                        bool isValidPassword = false;

                        // Check if password is already hashed (length 64 for SHA256)
                        if (storedPassword.Length == 64)
                        {
                            // Verify hashed password
                            isValidPassword = SecurityHelper.VerifyPassword(password, storedPassword);
                        }
                        else
                        {
                            // Plain text password (legacy), check directly
                            if (storedPassword == password)
                            {
                                isValidPassword = true;

                                // Update to hashed password
                                int userId = Convert.ToInt32(dr["MaND"]);
                                dr.Close();

                                string hashedPassword = SecurityHelper.HashPassword(password);
                                SqlCommand updateCmd = new SqlCommand(
                                    "UPDATE NguoiDung SET MatKhau=@hash WHERE MaND=@id", conn);
                                updateCmd.Parameters.AddWithValue("@hash", hashedPassword);
                                updateCmd.Parameters.AddWithValue("@id", userId);
                                updateCmd.ExecuteNonQuery();

                                // Re-read user data
                                cmd = new SqlCommand(sql, conn);
                                cmd.Parameters.AddWithValue("@email", email);
                                dr = cmd.ExecuteReader();
                                dr.Read();
                            }
                        }

                        if (isValidPassword)
                        {
                            CurrentUserID = Convert.ToInt32(dr["MaND"]);
                            CurrentUser = dr["Email"].ToString();
                            CurrentRole = dr["TenVaiTro"].ToString();
                            string fullName = dr["HoTen"].ToString();

                            dr.Close();

                            // Log activity
                            SqlCommand logCmd = new SqlCommand(
                                "EXEC SP_LogActivity @MaNguoiDung, @HanhDong, @ChiTiet, @DiaChiIP", conn);
                            logCmd.Parameters.AddWithValue("@MaNguoiDung", CurrentUserID);
                            logCmd.Parameters.AddWithValue("@HanhDong", "Đăng nhập");
                            logCmd.Parameters.AddWithValue("@ChiTiet", $"User {fullName} đăng nhập thành công");
                            logCmd.Parameters.AddWithValue("@DiaChiIP", "127.0.0.1");
                            logCmd.ExecuteNonQuery();

                            MessageBox.Show($"Xin chào, {fullName}!\nĐăng nhập thành công.", "Thành công",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Open dashboard - when dashboard closes, app exits
                            DashboardForm dash = new DashboardForm();
                            dash.FormClosed += (s, args) => Application.Exit(); // Exit app when dashboard closes
                            dash.Show();
                            
                            // Just hide login form, don't close it
                            this.Hide();
                        }
                        else
                        {
                            dr.Close();
                            MessageBox.Show("Mật khẩu không đúng!", "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Email không tồn tại hoặc tài khoản đã bị khóa!", "Lỗi",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi kết nối: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
