
using System;
using System.Drawing;
using System.Windows.Forms;
using src.BUS;

namespace src.GUI.ThongKe
{
    public class ThongKe : Form
    {
        private readonly ThongKeBUS thongkeBUS;
        private TabControl tabbedPane;

        private readonly Color backgroundColor =
            Color.FromArgb(240, 247, 250);

        private readonly Color primaryColor =
            Color.FromArgb(41, 128, 185);

        private readonly Color selectedColor =
            Color.FromArgb(30, 105, 160);

        private readonly Color inactiveColor =
            Color.FromArgb(229, 238, 245);

        private ThongKeTongQuan tongquan;
        private ThongKeTonKho nhapxuat;
        private ThongKeKhachHang khachhang;
        private ThongKeNhaCungCap nhacungcap;
        private ThongKeDoanhThu doanhthu;

        public ThongKe()
        {
            thongkeBUS = new ThongKeBUS();
            InitComponent();
        }

        private void InitComponent()
        {
            // Cấu hình Form nhúng trong MainForm
            TopLevel = false;
            FormBorderStyle = FormBorderStyle.None;
            Dock = DockStyle.Fill;
            BackColor = backgroundColor;

            Font = new Font(
                "Segoe UI",
                10F,
                FontStyle.Regular
            );

            // Khởi tạo các màn hình thống kê
            tongquan = new ThongKeTongQuan(thongkeBUS);
            nhapxuat = new ThongKeTonKho(thongkeBUS);
            doanhthu = new ThongKeDoanhThu(thongkeBUS);
            nhacungcap = new ThongKeNhaCungCap(thongkeBUS);
            khachhang = new ThongKeKhachHang(thongkeBUS);

            // Tạo thanh tab
            tabbedPane = new TabControl
            {
                Dock = DockStyle.Fill,
                Font = new Font(
                    "Segoe UI",
                    10F,
                    FontStyle.Bold
                ),
                DrawMode = TabDrawMode.OwnerDrawFixed,
                SizeMode = TabSizeMode.Fixed,
                ItemSize = new Size(145, 42),
                Padding = new Point(12, 6),
                Appearance = TabAppearance.Normal
            };

            // Tự vẽ tab theo màu giao diện Desktop
            tabbedPane.DrawItem += TabbedPane_DrawItem;

            // Tạo 5 tab, giữ nguyên chức năng
            AddTab("Tổng quan", tongquan);
            AddTab("Tồn kho", nhapxuat);
            AddTab("Doanh thu", doanhthu);
            AddTab("Nhà cung cấp", nhacungcap);
            AddTab("Khách hàng", khachhang);

            Controls.Add(tabbedPane);
        }

        private void AddTab(string title, UserControl content)
        {
            TabPage page = new TabPage(title)
            {
                BackColor = backgroundColor,
                Padding = new Padding(8),
                UseVisualStyleBackColor = false
            };

            content.Dock = DockStyle.Fill;
            page.Controls.Add(content);

            tabbedPane.TabPages.Add(page);
        }

        // Vẽ giao diện thanh tab
        private void TabbedPane_DrawItem(
            object sender,
            DrawItemEventArgs e)
        {
            if (e.Index < 0 ||
                e.Index >= tabbedPane.TabPages.Count)
                return;

            Graphics g = e.Graphics;

            bool isSelected =
                tabbedPane.SelectedIndex == e.Index;

            Rectangle rect =
                tabbedPane.GetTabRect(e.Index);

            // Thu nhẹ vùng vẽ để các tab có khoảng cách
            rect.Inflate(-2, -2);

            Color bgColor = isSelected
                ? selectedColor
                : inactiveColor;

            Color textColor = isSelected
                ? Color.White
                : Color.FromArgb(55, 75, 95);

            // Nền tab
            using (SolidBrush brush =
                new SolidBrush(bgColor))
            {
                g.FillRectangle(brush, rect);
            }

            // Viền nhẹ
            using (Pen borderPen = new Pen(
                isSelected
                    ? primaryColor
                    : Color.FromArgb(208, 222, 233)))
            {
                g.DrawRectangle(
                    borderPen,
                    rect.X,
                    rect.Y,
                    rect.Width - 1,
                    rect.Height - 1
                );
            }

            // Nội dung chữ căn giữa
            string title =
                tabbedPane.TabPages[e.Index].Text;

            TextRenderer.DrawText(
                g,
                title,
                tabbedPane.Font,
                rect,
                textColor,
                TextFormatFlags.HorizontalCenter |
                TextFormatFlags.VerticalCenter |
                TextFormatFlags.SingleLine |
                TextFormatFlags.EndEllipsis
            );

            // Đường nhấn dưới tab đang chọn
            if (isSelected)
            {
                using (SolidBrush indicator =
                    new SolidBrush(
                        Color.FromArgb(255, 193, 7)))
                {
                    g.FillRectangle(
                        indicator,
                        rect.X,
                        rect.Bottom - 4,
                        rect.Width,
                        4
                    );
                }
            }
        }
    }
}
