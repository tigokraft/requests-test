using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Windows.Forms;

namespace requests_test
{
    public partial class MainForm : Form
    {
        public class Expense
        {
            public int ExpenseId { get; set; }
            public decimal Amount { get; set; }
            public string Description { get; set; }
            public DateTime Date { get; set; }
            public string Tags { get; set; }
            public string CategoryName { get; set; }
        }

        public class ExpenseResponse
        {
            public decimal TotalMonthlySpent { get; set; }
            public decimal TotalAllTimeSpent { get; set; }
            public List<Expense> Expenses { get; set; }
        }


        private readonly HttpClient _http;
        private readonly FlowLayoutPanel _expenseList;

        public MainForm(HttpClient httpClient)
        {
            _http = httpClient;

            Text = "Expenses Viewer";
            Width = 800;
            Height = 600;

            _expenseList = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false
            };

            Controls.Add(_expenseList);

            Load += async (s, e) =>
            {
                string token = Prompt.ShowDialog("Enter JWT Token:", "Authorization");

                if (!IsTokenValid(token))
                {
                    MessageBox.Show("Invalid token.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Close();
                    return;
                }

                _http.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                await LoadExpensesAsync();
            };
        }

        private async System.Threading.Tasks.Task LoadExpensesAsync()
        {
            var response = await _http.GetAsync("api/expense/summary");
            if (!response.IsSuccessStatusCode)
            {
                MessageBox.Show("Failed to fetch expenses.");
                return;
            }

            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var data = JsonSerializer.Deserialize<ExpenseResponse>(json, options);

            _expenseList.Controls.Clear();

            foreach (var exp in data.Expenses)
            {
                var panel = new Panel
                {
                    Width = _expenseList.Width - 30,
                    Height = 100,
                    Margin = new Padding(5),
                    BorderStyle = BorderStyle.FixedSingle
                };

                var title = new Label
                {
                    Text = $"{exp.Description} - {exp.Amount:C}",
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    AutoSize = true,
                    Location = new Point(10, 10)
                };

                var info = new Label
                {
                    Text = $"Date: {exp.Date:yyyy-MM-dd HH:mm} | Tags: {exp.Tags} | Category: {exp.CategoryName}",
                    AutoSize = true,
                    Location = new Point(10, 35)
                };

                panel.Controls.Add(title);
                panel.Controls.Add(info);

                _expenseList.Controls.Add(panel);
            }
        }

        private bool IsTokenValid(string token)
        {
            try
            {
                var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(token);
                var expClaim = jwt.Claims.FirstOrDefault(c => c.Type == "exp");

                if (expClaim != null && long.TryParse(expClaim.Value, out long exp))
                {
                    var expDate = DateTimeOffset.FromUnixTimeSeconds(exp);
                    return expDate > DateTimeOffset.UtcNow;
                }
            }
            catch
            {
                return false;
            }

            return false;
        }
    }
}
