using System;
using System.Windows.Forms;

namespace Geocodage
{
    public partial class Form1 : Form
    {
        TraitementDonnees traitement = new TraitementDonnees();


        public Form1()
        {
            InitializeComponent();

            //ajoute dynamiquement le même évènement à tous les textBox
            foreach (Control control in Controls)
            {
                if (control.GetType() == typeof(TextBox))
                {
                    control.GotFocus += new EventHandler(textBox_GotFocus);
                }
            }

            if (traitement.lectureFichierConfig()== false)
            {
                MessageBox.Show("erreur de lecture du fichier de configuraton\n"
                    +"Chargement des valeurs par défaut.");
            }

            //autorise le drag and drop
            AllowDrop = true;

            //valide directement quand on valide avec entrée dans un TextBox
            //plus besoin de gérer un évènement par textbox
            AcceptButton = button1;
        }

        //gestion du drag&drop
        void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        //quand un fichier est déposé dans la fenêtre
        void Form1_DragDrop(object sender, DragEventArgs e)
        {
            //liste des fichiers déposés
            string[] lFichiers = (string[])e.Data.GetData(DataFormats.FileDrop);

            foreach (string fichier in lFichiers)
            {
                try
                {
                    if (traitement.traiterFichier(fichier))
                    {
                        MessageBox.Show("fin du traitement de " + fichier);
                    }
                    else
                    {
                        MessageBox.Show("erreur durant le traitement de " + fichier);
                    }
                }
                catch (Exception e2)
                {
                    // Let the user know what went wrong.
                    // à supprimer ??
                    MessageBox.Show("Erreur avec le fichier "+ fichier);
                    MessageBox.Show(e2.Message);
                }
            }
        }

        private void lancerTraitement()
        {
            int codePostal;
            //vérification ddes champs
            if (textBox1.Text != "" && textBox2.Text != "" && Int32.TryParse(textBox2.Text, out codePostal))
            {
                try
                {
                    traitement.traiterFormulaire(textBox1.Text, textBox2.Text);
                    label1.Text = traitement.location.Latitude.ToString();
                    label2.Text = traitement.location.Longitude.ToString();
                    label3.Text = traitement.location.label;
                }
                catch (Exception)
                {
                    MessageBox.Show("Impossible de géocoder l'adresse. Un des champs doit être incorrect !");
                }
            }
            else
            {
                if (textBox1.Text == "")
                {
                    MessageBox.Show("Le champs adresse est vide !");
                }
                if (textBox2.Text == "")
                {
                    MessageBox.Show("Le champs code postal est vide !");
                }
                else if (!Int32.TryParse(textBox2.Text, out codePostal))
                {
                    MessageBox.Show("Le champs code postal est incorrect !");
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            lancerTraitement();
        }

        //
        private void textBox_GotFocus(object sender, EventArgs e)
        {
            TextBox box = sender as TextBox;
            box.Text = "";
        }
      
        private void aProposToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Application écrite en C# par Marie-Lyse Briffaud.\nCette application utilise l'api de http://adresse.data.gouv.fr/ pour géocoder une adresse ou un lot d'adresses.");
        }

        private void aideToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Entrez une adresse et un code postal valide et vous obtiendrez la latitude et la longitude.\nSi vous glissez-déposez un fichier csv, chaque ligne sera géocodée (trois champs seront ajoutés au fichier : label qui reprend l'adresse géocodée, latitude et longitude).\nLe fichier csv doit contenir une première ligne d'entête et vous pouvez configurer le séparateur de champs ainsi que les noms de champs dans les options de configuration.", "Aide");
        }

        private void configurationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 fConfig = new Form2();
            fConfig.ShowDialog();
            if (!traitement.lectureFichierConfig())
            {
                MessageBox.Show("Erreur de lecture du fichier de configuration.\nChargement des valeurs par défaut");
            }
        }
    }
}
