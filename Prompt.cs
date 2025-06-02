using System.Windows.Forms;
using System.Drawing;

public static class Prompt
{
    public static string ShowDialog(string text, string caption)
    {
        Form prompt = new Form()
        {
            Width = 400,
            Height = 150,
            Text = caption,
            StartPosition = FormStartPosition.CenterScreen
        };
        Label textLabel = new Label() { Left = 10, Top = 10, Text = text, AutoSize = true };
        TextBox inputBox = new TextBox() { Left = 10, Top = 35, Width = 360 };
        Button confirmation = new Button() { Text = "OK", Left = 290, Width = 80, Top = 65 };
        confirmation.Click += (sender, e) => { prompt.Close(); };
        prompt.Controls.Add(textLabel);
        prompt.Controls.Add(inputBox);
        prompt.Controls.Add(confirmation);
        prompt.AcceptButton = confirmation;

        prompt.ShowDialog();
        return inputBox.Text;
    }
}
