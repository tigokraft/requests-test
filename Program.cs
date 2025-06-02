using System;
using System.Net.Http;
using System.Windows.Forms;

namespace requests_test
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            HttpClient httpClient = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:5034")
            };
            httpClient.DefaultRequestHeaders.Add("x-api-key", "12345-abcdef-67890");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm(httpClient));
        }
    }
}
