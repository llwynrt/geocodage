using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Geocodage
{
    class TraitementDonnees
    {
        private string fichier;
        private StreamReader sr;
        private List<string> lLignes;
        private Indice indice;
        private FichierConfiguration conf;
        private Geoloc location;
        private int erreur;

        public double Latitude { get { return location.Latitude; } }
        public double Longitude { get { return location.Longitude; } }
        public string Label { get { return location.label; } }

        /**
         * Géocode une adresse seule
         * @param adresse : adresse à géocoder (sans le code postal ni la ville)
         * @param sCodePotal : code postal à géocoder
         */
        public void traiterFormulaire(string adresse, string sCodePostal)
        {
            location = new Geoloc(adresse, sCodePostal);
        }

        /**
         * lit le fichier ligne par ligne et geocode chaque adresse dans le fichier
         * @param pFichier : le fichier à traiter
         * return code d'erreur
         */
        public int traiterFichier(string sFichier)
        {
            conf = new FichierConfiguration();
            conf.lecture();
            erreur = 0;

            fichier = sFichier;
            lectureFichier();
            
            //on écrit le fichier s'il n'y a pas d'erreur de mise en forme
            if (erreur == 0 || erreur == 32)
            {
                ecritureFichier();
            }
            return erreur;
        }

        /**
         * lit le fichier ligne par ligne
         * en premier l'entête pour connaitre les champs à utiliser
         * ensuite les lignes qui sont stockées dans une liste de string
         * @return code d'erreur : 0 si ok, nombre négatif sinon
         */
        private void lectureFichier()
        {
            lLignes = new List<string>();

            using (sr = new StreamReader(fichier, Encoding.GetEncoding("utf-8")))
            {
                lireEntete();
                /*string code = Convert.ToString(-erreur, 2);
                code=code.PadLeft(6, '0');*/

                if (erreur==0)
                {
                    lireDonnees();
                }
            }
        }

        /**
         * lit la premiere ligne du fichier et récupère l'indice des champs adresse et code postal
         * return un objet indice contenant les indices de adresse et codepostal
         * todo : lire l'entête et permettre de choisir les colonnes à utiliser
         */
        private void lireEntete()
        {
            string ligne;
            indice = new Indice();

            //si le fichier n'est pas vide
            if ((ligne = sr.ReadLine()) != null)
            {
                //récupère les différents champs dans un tableau
                string[] tEntete = ligne.Split(conf.separateur);
                if (tEntete.Length == 1)
                {
                    erreur -= 1;
                }
                else {
                    erreur -= 2;
                    erreur -= 4;
                    //parcourt le tableau pour chercher adresse et codepostal
                    for (int i = 0; i < tEntete.Length; i++)
                    {
                        tEntete[i] = tEntete[i].Trim();
                        if (tEntete[i] == conf.adresse)
                        {
                            indice.adresse = i;
                            erreur += 2;
                        }
                        else if (tEntete[i] == conf.codepostal)
                        {
                            indice.codepostal = i;
                            erreur += 4;
                        }
                    }
                    //ajout des nouveaux champs à l'entête
                    ligne += conf.separateur + " libellé" + conf.separateur + " latitude" + conf.separateur 
                        + " longitude";
                    lLignes.Add(ligne);
                }
            }
            else
            {
                //fichier vide
                erreur -= 8;
            }
        }

        private void lireDonnees()
        {
            string ligne;
            bool erreurAdresse = false;
            int nombreLignes = 0;
            //lecture du fichier ligne par ligne
            while ((ligne = sr.ReadLine()) != null)
            {
                nombreLignes++;
                string coordonnees;
                try
                {
                    coordonnees = geocoderLigne(ligne);
                    lLignes.Add(ligne + coordonnees);
                }
                catch (Exception)
                {
                    lLignes.Add(ligne + conf.separateur + "adresse impossible à géocoder");
                    erreurAdresse = true;
                }
            }
            if (nombreLignes == 0)
            {
                erreur -= 16;
            }
            if (erreurAdresse)
            {
                erreur -= 32;
            }
        }

        /**
         * lit l'adresse et le code postal d'une ligne pour les convertir en coordonnées à ajouter à la ligne
         * @param line : ligne du fichier en cours à traiter
         * @param indiceCodePostal
         * @param indiceAdresse
         * return string contenant libellé et coordonnées à ajouter à la ligne
         */
        private string geocoderLigne(string line)
        { 
            /*change le format des nombres pour avoir un point et non une virgule comme séparateur
              afin de ne pas avoir de problème si le séparateur du fichier est une virgule */
            NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;
            nfi.NumberDecimalDigits = 6;
            //récupère chaque item de la ligne dans un tableau
            string[] TlineTemp = line.Split(conf.separateur);
            TlineTemp[indice.codepostal] = TlineTemp[indice.codepostal].Trim();
            //récupère les coordonnées
            location = new Geoloc(TlineTemp[indice.adresse], TlineTemp[indice.codepostal]);
            return conf.separateur + location.label + conf.separateur + location.Latitude.ToString("N", nfi) 
                + conf.separateur + location.Longitude.ToString("N", nfi);
        }

        /**
         * écrit les lignes modifiées dans un nouveau fichier : nomfichiersuffixe.extension
         */
        private void ecritureFichier()
        {
            using (StreamWriter sw = new StreamWriter(
                  Path.GetDirectoryName(fichier) + "\\"
                + Path.GetFileNameWithoutExtension(fichier) + conf.suffixe
                + Path.GetExtension(fichier)))
            {
                foreach (string ligne in lLignes)
                    sw.WriteLine(ligne);
            }
        }
    }

    class Indice
    {
        public int adresse { get; set; }
        public int codepostal { get; set; }

        public Indice()
        {
            adresse = -1;
            codepostal = -1;
        }
    }
}
