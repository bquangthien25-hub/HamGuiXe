using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using SD = System.Drawing;
using OpenCvSharp;
using Tesseract;

namespace HamGuiXe
{
    public partial class NhanDienBienSoForm : Form
    {
        string duongDanAnh = "";

        PictureBox picAnhGoc;
        PictureBox picXuLy;
        Button btnChonAnh;
        Button btnNhanDien;
        TextBox txtKetQua;
        Label lblKetQua;

        // 🔥 EVENT GỬI BIỂN SỐ RA NGOÀI
        public event Action<string> BienSoNhanDienThanhCong;

        public NhanDienBienSoForm()
        {
            InitializeComponent();
        }

        // ================== UI ==================
        private void InitializeComponent()
        {
            Text = "Nhận Diện Biển Số";
            Size = new SD.Size(900, 500);
            StartPosition = FormStartPosition.CenterScreen;

            picAnhGoc = new PictureBox
            {
                Location = new SD.Point(30, 30),
                Size = new SD.Size(350, 200),
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.Zoom
            };

            picXuLy = new PictureBox
            {
                Location = new SD.Point(420, 30),
                Size = new SD.Size(350, 200),
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.Zoom
            };

            btnChonAnh = new Button
            {
                Text = "Chọn ảnh",
                Location = new SD.Point(150, 250),
                Width = 120
            };
            btnChonAnh.Click += btnChonAnh_Click;

            btnNhanDien = new Button
            {
                Text = "Nhận diện",
                Location = new SD.Point(520, 250),
                Width = 120
            };
            btnNhanDien.Click += btnNhanDien_Click;

            lblKetQua = new Label
            {
                Text = "Kết quả biển số:",
                Location = new SD.Point(30, 320),
                AutoSize = true
            };

            txtKetQua = new TextBox
            {
                Location = new SD.Point(150, 315),
                Width = 300,
                Font = new SD.Font("Consolas", 12, SD.FontStyle.Bold),
                ReadOnly = true
            };

            Controls.AddRange(new Control[]
            {
                picAnhGoc, picXuLy,
                btnChonAnh, btnNhanDien,
                lblKetQua, txtKetQua
            });
        }

        // ================== CHỌN ẢNH ==================
        private void btnChonAnh_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png"
            };

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                duongDanAnh = ofd.FileName;
                picAnhGoc.Image = SD.Image.FromFile(duongDanAnh);
                picXuLy.Image = null;
                txtKetQua.Clear();
            }
        }

        // ================== NHẬN DIỆN ==================
        private void btnNhanDien_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(duongDanAnh))
            {
                MessageBox.Show("Vui lòng chọn ảnh trước!");
                return;
            }

            byte[] bytes = File.ReadAllBytes(duongDanAnh);
            Mat img = Cv2.ImDecode(bytes, ImreadModes.Color);

            Mat gray = new Mat();
            Cv2.CvtColor(img, gray, ColorConversionCodes.BGR2GRAY);
            Cv2.GaussianBlur(gray, gray, new OpenCvSharp.Size(3, 3), 0);
            Cv2.EqualizeHist(gray, gray);

            Mat thresh = new Mat();
            Cv2.Threshold(gray, thresh, 0, 255, ThresholdTypes.Binary | ThresholdTypes.Otsu);

            Mat kernel = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(2, 2));
            Cv2.MorphologyEx(thresh, thresh, MorphTypes.Close, kernel);

            SD.Bitmap bmpXuLy = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(thresh);
            picXuLy.Image = bmpXuLy;

            string raw = RunOCR(bmpXuLy);
            string bienSo = FormatBienSo(raw);
            txtKetQua.Text = bienSo;

            // 🔥 PHÁT EVENT
            if (!string.IsNullOrWhiteSpace(bienSo))
            {
                BienSoNhanDienThanhCong?.Invoke(bienSo);
                Close();
            }
            else
            {
                MessageBox.Show("Không nhận diện được biển số!");
            }
        }

        // ================== OCR ==================
        private string RunOCR(SD.Bitmap bmp)
        {
            try
            {
                using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default))
                {
                    engine.SetVariable("tessedit_char_whitelist", "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789.-");
                    engine.DefaultPageSegMode = PageSegMode.SingleLine;

                    using (var page = engine.Process(bmp))
                    {
                        return page.GetText()
                                   .ToUpper()
                                   .Replace("\n", "")
                                   .Replace("\r", "")
                                   .Replace(" ", "")
                                   .Trim();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "LOI: Chua cai dat Tesseract OCR!\n\n" +
                    "Tinh nang nhan dien bien so can:\n" +
                    "1. Tai file 'eng.traineddata' tu:\n" +
                    "   https://github.com/tesseract-ocr/tessdata\n\n" +
                    "2. Tao thu muc 'tessdata' trong thu muc chua file .exe\n\n" +
                    "3. Copy file 'eng.traineddata' vao thu muc 'tessdata'\n\n" +
                    $"Chi tiet loi: {ex.Message}",
                    "Loi Tesseract OCR",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return "";
            }
        }

        // ================== FORMAT BIỂN SỐ ==================
        private string FormatBienSo(string raw)
        {
            raw = Regex.Replace(raw, @"[^A-Z0-9.-]", "");

            Match m = Regex.Match(raw, @"(\d{2})([A-Z]\d)(\d{3}\.\d{2})");
            if (m.Success)
            {
                return $"{m.Groups[1].Value}-{m.Groups[2].Value} {m.Groups[3].Value}";
            }

            return raw;
        }
    }
}
