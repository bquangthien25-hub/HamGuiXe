using ParkingApp.Utils;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace ParkingApp.Forms
{
    public class DoanhThuForm : Form
    {
        DateTimePicker dtFrom, dtTo;
        Button btnThongKe;
        Label lblTongTien, lblLuotXe;

        public DoanhThuForm()
        {
            this.Text = "Báo cáo doanh thu";
            this.Size = new Size(650, 400);

            BuildUI();
        }

        private void BuildUI()
        {
            Label lbl1 = new Label() { Text = "Từ ngày:", Left = 20, Top = 30, AutoSize = true };
            dtFrom = new DateTimePicker() { Left = 100, Top = 25, Width = 150 };

            Label lbl2 = new Label() { Text = "Đến ngày:", Left = 270, Top = 30, AutoSize = true};
            dtTo = new DateTimePicker() { Left = 350, Top = 25, Width = 150 };

            btnThongKe = new Button()
            {
                Text = "Thống kê",
                Left = 510,
                Top = 25,
                Width = 100
            };
            btnThongKe.Click += BtnThongKe_Click;

            lblTongTien = new Label()
            {
                Text = "Tổng doanh thu: 0 VND",
                Left = 20,
                Top = 100,
                Width = 500,
                Font = new Font(FontFamily.GenericSansSerif, 12, FontStyle.Bold)
            };

            lblLuotXe = new Label()
            {
                Text = "Tổng lượt xe: 0",
                Left = 20,
                Top = 140,
                Width = 500,
                Font = new Font(FontFamily.GenericSansSerif, 12, FontStyle.Bold)
            };

            Controls.Add(lbl1);
            Controls.Add(lbl2);
            Controls.Add(dtFrom);
            Controls.Add(dtTo);
            Controls.Add(btnThongKe);
            Controls.Add(lblTongTien);
            Controls.Add(lblLuotXe);
        }

        private void BtnThongKe_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();

                SqlCommand cmd1 = new SqlCommand(
                    @"SELECT ISNULL(SUM(SoTien),0) 
                      FROM DoanhThu 
                      WHERE NgayThu BETWEEN @d1 AND @d2", conn);

                cmd1.Parameters.AddWithValue("@d1", dtFrom.Value.Date);
                cmd1.Parameters.AddWithValue("@d2", dtTo.Value.Date.AddDays(1)); // đảm bảo bao trùm cả ngày

                int tongTien = Convert.ToInt32(cmd1.ExecuteScalar());

                SqlCommand cmd2 = new SqlCommand(
                    @"SELECT COUNT(*) 
                      FROM DoanhThu 
                      WHERE NgayThu BETWEEN @d1 AND @d2", conn);

                cmd2.Parameters.AddWithValue("@d1", dtFrom.Value.Date);
                cmd2.Parameters.AddWithValue("@d2", dtTo.Value.Date.AddDays(1));

                int soLuot = Convert.ToInt32(cmd2.ExecuteScalar());

                lblTongTien.Text = $"Tổng doanh thu: {tongTien:#,##0} VND";
                lblLuotXe.Text = $"Tổng lượt xe: {soLuot}";
            }
        }
    }
}
