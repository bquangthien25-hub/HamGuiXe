using ParkingApp.Forms;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ParkingApp
{
    public class DashboardForm : Form
    {
        private Panel menuPanel, contentPanel;
        private Button btnXe, btnVe, btnKhach, btnRaVao, btnNhanVien, btnDoanhThu, btnLogout;

        public DashboardForm()
        {
            Text = "Hệ thống quản lý hầm xe";
            WindowState = FormWindowState.Maximized;
            StartPosition = FormStartPosition.CenterScreen;

            BuildUI();
            ApplyRole();
        }

        private void BuildUI()
        {
            // ===== PANEL NỘI DUNG =====
            contentPanel = new Panel()
            {
                Dock = DockStyle.Fill,
                BackColor = Color.WhiteSmoke,
                AutoScroll = true
            };
            Controls.Add(contentPanel);

            // ===== PANEL MENU =====
            menuPanel = new Panel()
            {
                Dock = DockStyle.Left,
                Width = 220,
                BackColor = Color.FromArgb(40, 40, 40)
            };
            Controls.Add(menuPanel);

            // ===== BUTTON MENU =====
            btnXe = CreateMenuButton("Quản lý xe");
            btnVe = CreateMenuButton("Quản lý vé");
            btnKhach = CreateMenuButton("Khách hàng");
            btnRaVao = CreateMenuButton("Lượt vào / ra");
            btnNhanVien = CreateMenuButton("Nhân viên");
            btnDoanhThu = CreateMenuButton("Doanh thu");
            btnLogout = CreateMenuButton("Đăng xuất");

            // ===== GÁN SỰ KIỆN =====
            btnXe.Click += (s, e) => LoadContent(new XeForm());
            btnVe.Click += (s, e) => LoadContent(new VeForm());
            btnKhach.Click += (s, e) => LoadContent(new KhachForm());
            btnRaVao.Click += (s, e) => LoadContent(new RaVaoForm());
            btnNhanVien.Click += (s, e) => LoadContent(new NhanVienForm());
            btnDoanhThu.Click += (s, e) => LoadContent(new DoanhThuForm());
            btnLogout.Click += BtnLogout_Click;

            // ===== ADD BUTTON VÀO MENU =====
            menuPanel.Controls.Add(btnXe);
            menuPanel.Controls.Add(btnVe);
            menuPanel.Controls.Add(btnKhach);
            menuPanel.Controls.Add(btnRaVao);
            menuPanel.Controls.Add(btnNhanVien);
            menuPanel.Controls.Add(btnDoanhThu);
            menuPanel.Controls.Add(btnLogout);
        }

        private Button CreateMenuButton(string text)
        {
            Button btn = new Button()
            {
                Text = text,
                Dock = DockStyle.Top,
                Height = 55,
                BackColor = Color.FromArgb(63, 63, 65),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12, FontStyle.Bold)
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.MouseEnter += (s, e) => btn.BackColor = Color.FromArgb(80, 80, 80);
            btn.MouseLeave += (s, e) => btn.BackColor = Color.FromArgb(63, 63, 65);
            return btn;
        }

        private void LoadContent(Form frm)
        {
            contentPanel.Controls.Clear();

            frm.TopLevel = false;
            frm.FormBorderStyle = FormBorderStyle.None;
            frm.Width = contentPanel.Width;
            frm.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            contentPanel.Controls.Add(frm);
            frm.Show();
        }

        private void BtnLogout_Click(object sender, EventArgs e)
        {
            Hide();
            new LoginForm().Show();
        }

        private void ApplyRole()
        {
            if (LoginForm.CurrentRole == "Staff")
            {
                btnNhanVien.Visible = false;
                btnDoanhThu.Visible = false;
            }
        }
    }
}
