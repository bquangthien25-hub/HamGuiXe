using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ParkingApp.UI
{
    /// <summary>
    /// Modern Theme Manager - Quản lý color palette và theme settings (LIGHT THEME)
    /// </summary>
    public static class ModernTheme
    {
        // Primary Colors
        public static readonly Color Primary = ColorTranslator.FromHtml("#6366f1");      // Indigo
        public static readonly Color PrimaryDark = ColorTranslator.FromHtml("#4f46e5");
        public static readonly Color PrimaryLight = ColorTranslator.FromHtml("#818cf8");
        
        public static readonly Color Secondary = ColorTranslator.FromHtml("#8b5cf6");    // Purple
        public static readonly Color SecondaryDark = ColorTranslator.FromHtml("#7c3aed");
        
        // LIGHT MODE Backgrounds - Changed to white/light colors
        public static readonly Color BackgroundDark = Color.White;                       // Main background - WHITE
        public static readonly Color BackgroundCard = ColorTranslator.FromHtml("#f8f9fa"); // Cards - Light gray
        public static readonly Color BackgroundHover = ColorTranslator.FromHtml("#e9ecef"); // Hover state
        
        // Status Colors
        public static readonly Color Success = ColorTranslator.FromHtml("#10b981");      // Green
        public static readonly Color Warning = ColorTranslator.FromHtml("#f59e0b");      // Orange
        public static readonly Color Danger = ColorTranslator.FromHtml("#ef4444");       // Red
        public static readonly Color Info = ColorTranslator.FromHtml("#3b82f6");         // Blue
        
        // Text Colors - Changed to dark for light background
        public static readonly Color TextPrimary = ColorTranslator.FromHtml("#1a1a1a");  // Almost black
        public static readonly Color TextSecondary = ColorTranslator.FromHtml("#4a5568"); // Dark gray
        public static readonly Color TextMuted = ColorTranslator.FromHtml("#718096");     // Medium gray
        
        // Borders & Shadows
        public static readonly Color BorderColor = ColorTranslator.FromHtml("#e2e8f0");   // Light border
        public static readonly Color ShadowColor = Color.FromArgb(20, 0, 0, 0);          // Subtle shadow
        
        // Fonts
        public static readonly Font FontRegular = new Font("Segoe UI", 10F, FontStyle.Regular);
        public static readonly Font FontBold = new Font("Segoe UI", 10F, FontStyle.Bold);
        public static readonly Font FontTitle = new Font("Segoe UI", 14F, FontStyle.Bold);
        public static readonly Font FontHeader = new Font("Segoe UI", 18F, FontStyle.Bold);
        public static readonly Font FontSmall = new Font("Segoe UI", 8.5F, FontStyle.Regular);
    }

    /// <summary>
    /// Modern Button with gradient, hover effects, rounded corners
    /// </summary>
    public class ModernButton : Button
    {
        private Color _baseColor = ModernTheme.Primary;
        private Color _hoverColor = ModernTheme.PrimaryDark;
        private bool _isHovered = false;
        private int _borderRadius = 8;

        public ModernButton()
        {
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            BackColor = _baseColor;
            ForeColor = Color.White;  // Keep white text on colored buttons
            Font = ModernTheme.FontBold;
            Cursor = Cursors.Hand;
            Height = 40;
            
            MouseEnter += (s, e) => { _isHovered = true; Invalidate(); };
            MouseLeave += (s, e) => { _isHovered = false; Invalidate(); };
        }

        public int BorderRadius
        {
            get => _borderRadius;
            set { _borderRadius = value; Invalidate(); }
        }

        public Color BaseColor
        {
            get => _baseColor;
            set { _baseColor = value; Invalidate(); }
        }

        public Color HoverColor
        {
            get => _hoverColor;
            set { _hoverColor = value; }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Create rounded rectangle path
            GraphicsPath path = GetRoundedRectangle(ClientRectangle, _borderRadius);
            
            // Fill with gradient
            Color currentColor = _isHovered ? _hoverColor : _baseColor;
            using (LinearGradientBrush brush = new LinearGradientBrush(
                ClientRectangle, 
                currentColor, 
                ControlPaint.Dark(currentColor, 0.1f),
                LinearGradientMode.Vertical))
            {
                e.Graphics.FillPath(brush, path);
            }
            
            // Draw text
            TextRenderer.DrawText(e.Graphics, Text, Font, ClientRectangle, ForeColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
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
    }

    /// <summary>
    /// Modern TextBox with border effects and placeholder text
    /// </summary>
    public class ModernTextBox : TextBox
    {
        private string _placeholderText = "";
        private Color _placeholderColor = ModernTheme.TextMuted;
        private Color _borderColor = ModernTheme.BorderColor;
        private Color _focusBorderColor = ModernTheme.Primary;
        private bool _isFocused = false;

        public ModernTextBox()
        {
            BorderStyle = BorderStyle.FixedSingle;
            BackColor = Color.White;
            ForeColor = ModernTheme.TextPrimary;
            Font = ModernTheme.FontRegular;
            Padding = new Padding(10, 8, 10, 8);
            Height = 35;
            
            GotFocus += (s, e) => { _isFocused = true; Parent?.Invalidate(); };
            LostFocus += (s, e) => { _isFocused = false; Parent?.Invalidate(); };
        }

        public string PlaceholderText
        {
            get => _placeholderText;
            set
            {
                _placeholderText = value;
                Invalidate();
            }
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            
            if (m.Msg == 0xF) // WM_PAINT
            {
                using (Graphics g = CreateGraphics())
                {
                    // Draw border
                    Color borderColor = _isFocused ? _focusBorderColor : _borderColor;
                    using (Pen pen = new Pen(borderColor, 2))
                    {
                        g.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
                    }
                    
                    // Draw placeholder
                    if (string.IsNullOrEmpty(Text) && !_isFocused && !string.IsNullOrEmpty(_placeholderText))
                    {
                        TextRenderer.DrawText(g, _placeholderText, Font, 
                            new Rectangle(5, 8, Width, Height), _placeholderColor);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Modern Panel with shadow, rounded corners, optional glassmorphism
    /// </summary>
    public class ModernPanel : Panel
    {
        private int _borderRadius = 12;
        private bool _showShadow = true;
        private Color _backgroundColor = ModernTheme.BackgroundCard;

        public ModernPanel()
        {
            BackColor = _backgroundColor;
            Padding = new Padding(15);
            DoubleBuffered = true;
        }

        public int BorderRadius
        {
            get => _borderRadius;
            set { _borderRadius = value; Invalidate(); }
        }

        public bool ShowShadow
        {
            get => _showShadow;
            set { _showShadow = value; Invalidate(); }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // Draw shadow
            if (_showShadow)
            {
                Rectangle shadowRect = new Rectangle(5, 5, Width - 10, Height - 10);
                GraphicsPath shadowPath = GetRoundedRectangle(shadowRect, _borderRadius);
                using (PathGradientBrush brush = new PathGradientBrush(shadowPath))
                {
                    brush.CenterColor = ModernTheme.ShadowColor;
                    brush.SurroundColors = new[] { Color.Transparent };
                    e.Graphics.FillPath(brush, shadowPath);
                }
            }

            // Draw panel background
            Rectangle panelRect = new Rectangle(0, 0, Width - 1, Height - 1);
            GraphicsPath path = GetRoundedRectangle(panelRect, _borderRadius);
            
            using (SolidBrush brush = new SolidBrush(_backgroundColor))
            {
                e.Graphics.FillPath(brush, path);
            }

            // Draw border
            using (Pen pen = new Pen(ModernTheme.BorderColor, 1))
            {
                e.Graphics.DrawPath(pen, path);
            }

            base.OnPaint(e);
        }

        private GraphicsPath GetRoundedRectangle(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int diameter = radius * 2;
            
            if (diameter > rect.Width) diameter = rect.Width;
            if (diameter > rect.Height) diameter = rect.Height;
            
            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();
            
            return path;
        }
    }

    /// <summary>
    /// Modern DataGridView with styled appearance
    /// </summary>
    public class ModernDataGridView : DataGridView
    {
        public ModernDataGridView()
        {
            // Appearance
            BackgroundColor = ModernTheme.BackgroundCard;
            BorderStyle = BorderStyle.None;
            
            // Grid styling
            GridColor = ModernTheme.BorderColor;
            
            // Column header styling
            EnableHeadersVisualStyles = false;
            ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            ColumnHeadersDefaultCellStyle.BackColor = ModernTheme.Primary;
            ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            ColumnHeadersDefaultCellStyle.Font = ModernTheme.FontBold;
            ColumnHeadersDefaultCellStyle.Padding = new Padding(10, 8, 10, 8);
            ColumnHeadersHeight = 40;
            
            // Row styling
            DefaultCellStyle.BackColor = ModernTheme.BackgroundCard;
            DefaultCellStyle.ForeColor = ModernTheme.TextPrimary;
            DefaultCellStyle.Font = ModernTheme.FontRegular;
            DefaultCellStyle.SelectionBackColor = ModernTheme.Primary;
            DefaultCellStyle.SelectionForeColor = Color.White;
            DefaultCellStyle.Padding = new Padding(5, 4, 5, 4);
            RowTemplate.Height = 35;
            
            // Alternating row color
            AlternatingRowsDefaultCellStyle.BackColor = ModernTheme.BackgroundHover;
            
            // Row header
            RowHeadersVisible = false;
            
            // Selection
            SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            MultiSelect = false;
            ReadOnly = true;
            AllowUserToAddRows = false;
            AllowUserToDeleteRows = false;
            AllowUserToResizeRows = false;
            
            // Auto size
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            
            // Other
            CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        }
    }

    /// <summary>
    /// Statistics Card for Dashboard
    /// </summary>
    public class StatCard : ModernPanel
    {
        private Label lblTitle;
        private Label lblValue;
        private Label lblIcon;

        public StatCard(string title, string value, string icon, Color accentColor)
        {
            Width = 200;
            Height = 100;
            ShowShadow = true;

            // Icon
            lblIcon = new Label
            {
                Text = icon,
                Font = new Font("Segoe UI", 24F),
                ForeColor = accentColor,
                Location = new Point(15, 15),
                AutoSize = true
            };

            // Title
            lblTitle = new Label
            {
                Text = title,
                Font = ModernTheme.FontSmall,
                ForeColor = ModernTheme.TextSecondary,
                Location = new Point(15, 55),
                AutoSize = true
            };

            // Value
            lblValue = new Label
            {
                Text = value,
                Font = ModernTheme.FontTitle,
                ForeColor = ModernTheme.TextPrimary,
                Location = new Point(15, 70),
                AutoSize = true
            };

            Controls.Add(lblIcon);
            Controls.Add(lblTitle);
            Controls.Add(lblValue);
        }

        public void UpdateValue(string newValue)
        {
            lblValue.Text = newValue;
        }
    }
}
