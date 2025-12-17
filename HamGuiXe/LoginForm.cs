using ParkingApp.Utils;
using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace ParkingApp.Forms
{
    public partial class LoginForm : Form
    {
        public static string CurrentUser = "";
        public static string CurrentRole = "";

        TextBox txtUser, txtPass;
        Button btnLogin;

        public LoginForm()
        {
            this.Text = "Đăng nhập hệ thống";
            this.Size = new Size(400, 250);
            this.StartPosition = FormStartPosition.CenterScreen;

            Label lbl1 = new Label()
            {
                Text = "Email:",
                Location = new Point(30, 40),
                Width = 120,
                AutoSize = true
            };

            txtUser = new TextBox()
            {
                Location = new Point(120, 40),
                Width = 200
            };

            Label lbl2 = new Label()
            {
                Text = "Mật khẩu:",
                Location = new Point(30, 80),
                Width = 120,
                AutoSize = true
            };

            txtPass = new TextBox()
            {
                Location = new Point(120, 80),
                Width = 200,
                PasswordChar = '*'
            };

            btnLogin = new Button()
            {
                Text = "Đăng nhập",
                Location = new Point(120, 120),
                Width = 200
            };
            btnLogin.Click += BtnLogin_Click;

            Controls.Add(lbl1);
            Controls.Add(lbl2);
            Controls.Add(txtUser);
            Controls.Add(txtPass);
            Controls.Add(btnLogin);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // LoginForm
            // 
            this.ClientSize = new System.Drawing.Size(1173, 481);
            this.Name = "LoginForm";
            this.Load += new System.EventHandler(this.LoginForm_Load);
            this.ResumeLayout(false);

        }

        private void LoginForm_Load(object sender, EventArgs e)
        {

        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();
                string sql = @"SELECT N.Email, V.TenVaiTro 
                               FROM NguoiDung N 
                               JOIN VaiTro V ON N.MaVaiTro = V.MaVaiTro
                               WHERE Email=@email AND MatKhau=@pass AND TrangThai=1";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@email", txtUser.Text);
                cmd.Parameters.AddWithValue("@pass", txtPass.Text);

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    CurrentUser = dr["Email"].ToString();
                    CurrentRole = dr["TenVaiTro"].ToString();

                    MessageBox.Show("Đăng nhập thành công!");

                    DashboardForm dash = new DashboardForm();
                    dash.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Sai tài khoản hoặc mật khẩu!");
                }
            }
        }
    }
}
