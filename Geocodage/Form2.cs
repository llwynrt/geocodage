using System;
using System.Windows.Forms;

namespace Geocodage
{
    public partial class Form2 : Form
    {
        FichierConfiguration conf = new FichierConfiguration();

        public Form2()
        {
            InitializeComponent();
            AcceptButton = button1;
            try {
                conf.lecture();
                
            }
            catch (Exception)
            {
                MessageBox.Show("Erreur de lecture du fichier de configuration.\n"
                    + "Chargement des valeurs par défaut.");
            }
            finally
            {
                textBox1.Text = conf.separateur.ToString();
                textBox2.Text = conf.adresse;
                textBox3.Text = conf.codepostal;
                textBox4.Text = conf.suffixe;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            conf.ecriture(textBox1.Text.ToCharArray(0,1)[0], textBox2.Text, textBox3.Text, textBox4.Text);
            Close();
        }
    }
}
