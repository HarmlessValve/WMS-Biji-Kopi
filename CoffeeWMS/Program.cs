namespace CoffeeWMS;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();
        
        var login = new CoffeeWMS.Forms.LoginForm();
        if (login.ShowDialog() == DialogResult.OK)
        {
            Application.Run(new CoffeeWMS.Forms.MainForm());
        }
    }    
}