using System;
using System.Windows.Forms;

namespace Geocodage
{
    public partial class Form1 : Form
    {
        FichierConfiguration conf = new FichierConfiguration();

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

            try {
                conf.lecture();
            }
            catch(Exception)
            {
                MessageBox.Show("erreur de lecture du fichier de configuration\n"
                    +"Chargement des valeurs par défaut.");
                conf.ecriture();
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
            int erreur;
            foreach (string fichier in lFichiers)
            {
                try
                {
                    erreur = traitement.traiterFichier(fichier);
                    if (erreur==0)
                    {
                        MessageBox.Show("fin du traitement de " + fichier);
                    }
                    else
                    {
                        messageErreur(erreur,fichier);
                    }
                }
                catch (Exception e2)
                {
                    // à supprimer ??
                    MessageBox.Show("Erreur avec le fichier "+ fichier);
                    MessageBox.Show(e2.Message);
                }
            }
        }

        private void messageErreur(int codeErreur, string fichier)
        {
            string code = Convert.ToString(-codeErreur, 2);
            string message = "";
            int lettre;

            for (int i = 0; i < code.Length; i++)
            {
                Int32.TryParse(code.Substring(i, 1), out lettre);
                if (lettre == 1)
                {
                    int puissance = (int)Math.Pow(2, code.Length - 1 - i);
                    switch (puissance)
                    {
                        case 1: message += "Séparateur incorrect.\n";break;
                        case 2: message += "Entête adresse incorrect.\n"; break;
                        case 4: message += "Entête codepostal incorrect.\n"; break;
                        case 8: message += "Fichier vide.\n"; break;
                        case 16: message += "Fichier ne contient que l'entête.\n"; break;
                        case 32: message += "Au moins une adresse impossible à géocoder.\n"; break;
                        default:
                            break;
                    }
                }
            }
            MessageBox.Show("erreur(s) durant le traitement de " + fichier);

            MessageBox.Show(message);
        }

        private void lancerTraitement()
        {
            int codePostal;
            //vérification ddes champs

            if( (textBox1.Text == "") || (textBox2.Text == ""))
            {
                MessageBox.Show("L'un des champs est vide !");
            }
            else if (!Int32.TryParse(textBox2.Text, out codePostal))
            {
                MessageBox.Show("Le champs code postal est incorrect !");
            }
            else {
                try
                {
                    traitement.traiterFormulaire(textBox1.Text, textBox2.Text);
                    label1.Text = traitement.Latitude.ToString();
                    label2.Text = traitement.Longitude.ToString();
                    label3.Text = traitement.Label;
                }
                catch (Exception)
                {
                    MessageBox.Show("Impossible de géocoder l'adresse. Un des champs doit être incorrect !");
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
        }
    }
}
