using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Text;

namespace Geocodage
{
    class TraitementDonnees
    {
        char separateur;
        string adresse;
        string codepostal;
        string suffixe;
        StreamReader sr;
        List<string> lLignes;
        string fichier;
        public Geoloc location;

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
         * todo : renvoyer un code d'erreur personnalisé
         */
        public bool traiterFichier(string sFichier)
        {
            FichierConfiguration conf = new FichierConfiguration();
            conf.lecture();
            separateur = conf.separateur;
            adresse = conf.adresse;
            codepostal = conf.codepostal;
            suffixe = conf.suffixe;
            fichier = sFichier;

            //pas besoin de traiter le fichier s'il est vide ou contient juste la ligne d'entête
            if (lectureFichier() && lLignes.Count > 1)
            {
                ecritureFichier();
                return true;
            }
            else
            {
                return false;
            }
        }
        
        /**
         * lit le fichier ligne par ligne
         * en premier l'entête pour connaitre les champs à utiliser
         * ensuite les lignes qui sont stockées dans une liste de string
         * @return true si tout c'est bien passé, false si impossible de trouver les champs adresse ou codepostal dans l'entête
         * todo : retourner un code d'erreur personnalisé
         */
        private bool lectureFichier()
        {
            lLignes = new List<string>();

            using (sr = new StreamReader(fichier, Encoding.GetEncoding("utf-8")))
            {
                string ligne;
                //récupération entête
                int[] tIndice = lireEntete();
                if (tIndice[0] == -1 || tIndice[1] == -1)
                {
                    return false;
                }
                else
                {
                    //lecture du fichier ligne par ligne
                    while ((ligne = sr.ReadLine()) != null)
                    {
                        string coordonnees;
                        try
                        {
                            coordonnees = geocoderLigne(ligne, tIndice[1], tIndice[0]);
                            lLignes.Add(ligne + coordonnees);
                        }
                        catch (Exception)
                        {
                            lLignes.Add(ligne + separateur + "adresse impossible à géocoder");
                        }
                    }
                    return true;
                }
            }
        }

        /**
         * lit la premiere ligne du fichier et récupère l'indice des champs adresse et code postal
         * todo : lire l'entête et permettre de choisir les colonnes à utiliser
         * return un tableau contenant les indices de adresse et codepostal
         */
        private int[] lireEntete()
        {
            string ligne;
            int indiceAdresse = -1;
            int indiceCodePostal = -1;
            //si le fichier n'est pas vide
            if ((ligne = sr.ReadLine()) != null)
            {
                //récupère les différents champs dans un tableau
                string[] tEntete = ligne.Split(separateur);
                //parcourt le tableau pour chercher adresse et codepostal
                for (int i = 0; i < tEntete.Length; i++)
                {
                    tEntete[i] = tEntete[i].Trim();
                    if (tEntete[i] == adresse)
                    {
                        indiceAdresse = i;
                    }
                    else if (tEntete[i] == codepostal)
                    {
                        indiceCodePostal = i;
                    }
                }
                //ajout des nouveaux champs à l'entête
                ligne += separateur + " libellé" + separateur + " latitude" + separateur + " longitude";
                lLignes.Add(ligne);
            }
            return new int[] { indiceAdresse, indiceCodePostal };
        }

        /**
         * lit l'adresse et le code postal d'une ligne pour les convertir en coordonnées à ajouter à la ligne
         * @param line : ligne du fichier en cours à traiter
         * @param indiceCodePostal
         * @param indiceAdresse
         * return string contenant libellé et coordonnées à ajouter à la ligne
         */
        private string geocoderLigne(string line, int indiceCodePostal, int indiceAdresse)
        { 
            /*change le format des nombres pour avoir un point et non une virgule comme séparateur
              afin de ne pas avoir de problème si le séparateur du fichier est une virgule */
            NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;
            nfi.NumberDecimalDigits = 6;
            //récupère chaque item de la ligne dans un tableau
            string[] TlineTemp = line.Split(separateur);
            TlineTemp[indiceCodePostal] = TlineTemp[indiceCodePostal].Trim();
            //récupère les coordonnées
            location = new Geoloc(TlineTemp[indiceAdresse], TlineTemp[indiceCodePostal]);
            return separateur + location.label + separateur + location.Latitude.ToString("N", nfi) 
                + separateur + location.Longitude.ToString("N", nfi);
        }

        /**
         * écrit les lignes modifiées dans un nouveau fichier : nomfichiersuffixe.extension
         */
        private void ecritureFichier()
        {
            using (StreamWriter sw = new StreamWriter(
                  Path.GetDirectoryName(fichier) + "\\"
                + Path.GetFileNameWithoutExtension(fichier) + suffixe
                + Path.GetExtension(fichier)))
            {
                foreach (string ligne in lLignes)
                    sw.WriteLine(ligne);
            }
        }

        /**
         * lit le fichier de configuration 
         * lit des valeurs par défaut en cas d'erreur de lecture du fichier
         * @return true si la lecture a réussi et false si les valeurs par défaut sont utilisées
         */
       /* public bool lectureFichierConfig()
        {
            try {
                //récupère le premier caractère de la chaine separateur
                separateur = ConfigurationManager.AppSettings["separateur"].ToCharArray(0, 1)[0];
                adresse = ConfigurationManager.AppSettings["adresse"];
                codepostal = ConfigurationManager.AppSettings["codepostal"];
                suffixe = ConfigurationManager.AppSettings["suffixe"];
                return true;
            }
            catch(Exception)
            {
                separateur = ',';
                adresse = "adresse";
                codepostal = "codepostal";
                suffixe = "2";
                return false;
            }
        }*/
    }
}
