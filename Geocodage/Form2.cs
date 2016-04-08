using System;
using System.Configuration;
using System.Windows.Forms;

namespace Geocodage
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            AcceptButton = button1;
            try {
                textBox1.Text = ConfigurationManager.AppSettings["separateur"].ToCharArray(0, 1)[0].ToString();
                textBox2.Text = ConfigurationManager.AppSettings["adresse"];
                textBox3.Text = ConfigurationManager.AppSettings["codepostal"];
                textBox4.Text = ConfigurationManager.AppSettings["suffixe"];
            }
            catch (Exception)
            {
                //valeurs par défaut
                textBox1.Text = ",";
                textBox2.Text = "adresse";
                textBox3.Text = "codepostal";
                textBox4.Text = "2";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Open App.Config of executable
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            if (textBox1.Text.Length == 1)
            {
                config.AppSettings.Settings.Remove("separateur");
                config.AppSettings.Settings.Add("separateur", textBox1.Text);
                config.AppSettings.Settings.Remove("adresse");
                config.AppSettings.Settings.Add("adresse", textBox2.Text);
                config.AppSettings.Settings.Remove("codepostal");
                config.AppSettings.Settings.Add("codepostal", textBox3.Text);
                config.AppSettings.Settings.Remove("suffixe");
                config.AppSettings.Settings.Add("suffixe", textBox4.Text);
                // Save the configuration file.
                config.Save(ConfigurationSaveMode.Modified);

                // Force a reload of a changed section.
                ConfigurationManager.RefreshSection("appSettings");

                Close();
            }
            else
                MessageBox.Show("Le séparateur doit contenir un seul caractère !");
        }
    }
}
