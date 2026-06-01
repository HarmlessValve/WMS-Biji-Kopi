using System.Drawing;

namespace CoffeeWMS.Theme
{
    public static class DesignTokens
    {
        // Colors
        public static readonly Color Primary = ColorTranslator.FromHtml("#2C5F2E");
        public static readonly Color PrimaryLight = ColorTranslator.FromHtml("#4A7C4E");
        public static readonly Color PrimaryDark = ColorTranslator.FromHtml("#1A3D1C");
        public static readonly Color Accent = ColorTranslator.FromHtml("#6F4E37");
        public static readonly Color AccentLight = ColorTranslator.FromHtml("#A07855");
        public static readonly Color Background = ColorTranslator.FromHtml("#F5F5F0");
        public static readonly Color Surface = ColorTranslator.FromHtml("#FFFFFF");
        public static readonly Color SurfaceAlt = ColorTranslator.FromHtml("#F0F7F0");
        public static readonly Color Border = ColorTranslator.FromHtml("#CCCCCC");
        public static readonly Color BorderFocus = ColorTranslator.FromHtml("#2C5F2E");
        
        public static readonly Color TextPrimary = ColorTranslator.FromHtml("#1A1A1A");
        public static readonly Color TextSecondary = ColorTranslator.FromHtml("#555555");
        public static readonly Color TextDisabled = ColorTranslator.FromHtml("#AAAAAA");
        
        public static readonly Color Success = ColorTranslator.FromHtml("#28A745");
        public static readonly Color Warning = ColorTranslator.FromHtml("#FFC107");
        public static readonly Color Error = ColorTranslator.FromHtml("#DC3545");
        public static readonly Color Info = ColorTranslator.FromHtml("#17A2B8");

        // Typography (Using Segoe UI or Arial)
        public const string FontFamily = "Segoe UI";
        public const string FontFallback = "Arial";

        // Properties returning new Font objects to avoid ObjectDisposedException
        public static Font TitleFont => new Font(FontFamily, 18, FontStyle.Bold);
        public static Font HeadingFont => new Font(FontFamily, 14, FontStyle.Bold);
        public static Font SubheadingFont => new Font(FontFamily, 12, FontStyle.Bold);
        public static Font BodyFont => new Font(FontFamily, 10, FontStyle.Regular);
        public static Font SmallFont => new Font(FontFamily, 9, FontStyle.Regular);
        public static Font ButtonFont => new Font(FontFamily, 10, FontStyle.Bold);

        // Sidebar and Header layout
        public const int SidebarWidth = 220;
        public const int HeaderHeight = 56;
    }
}
