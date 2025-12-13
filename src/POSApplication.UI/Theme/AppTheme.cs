namespace POSApplication.UI.Theme;

public static class AppTheme
{
    public static Color PrimaryColor = Color.FromArgb(41, 128, 185);   // Blue
    public static Color SecondaryColor = Color.FromArgb(52, 73, 94); // Dark Blue/Gray
    public static Color SuccessColor = Color.FromArgb(39, 174, 96);   // Green
    public static Color DangerColor = Color.FromArgb(231, 76, 60);    // Red
    public static Color BackgroundColor = Color.FromArgb(236, 240, 241); // Light Gray
    public static Color PanelColor = Color.White;
    public static Color TextColor = Color.FromArgb(44, 62, 80);       // Dark Gray
    public static Color TextColorLight = Color.White;

    public static Font HeaderFont = new Font("Segoe UI", 14, FontStyle.Bold);
    public static Font SubHeaderFont = new Font("Segoe UI", 12, FontStyle.Bold);
    public static Font BodyFont = new Font("Segoe UI", 10, FontStyle.Regular);
    public static Font LargeFont = new Font("Segoe UI", 18, FontStyle.Bold);

    public static void ApplyButtonTheme(Button btn, Color backColor)
    {
        btn.FlatStyle = FlatStyle.Flat;
        btn.FlatAppearance.BorderSize = 0;
        btn.BackColor = backColor;
        btn.ForeColor = TextColorLight;
        btn.Font = BodyFont;
        btn.Cursor = Cursors.Hand;
    }
    
    public static void ApplySecondaryButtonTheme(Button btn)
    {
        btn.FlatStyle = FlatStyle.Flat;
        btn.FlatAppearance.BorderSize = 1;
        btn.FlatAppearance.BorderColor = SecondaryColor;
        btn.BackColor = PanelColor;
        btn.ForeColor = SecondaryColor;
        btn.Font = BodyFont;
        btn.Cursor = Cursors.Hand;
    }
}
