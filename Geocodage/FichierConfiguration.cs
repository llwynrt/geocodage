using System.Configuration;

namespace Geocodage
{
    class FichierConfiguration
    {
        //valeurs par défaut
        public char separateur;
        public string adresse;
        public string codepostal;
        public string suffixe;

        public FichierConfiguration()
        {
            //valeurs par défaut
            separateur = ',';
            adresse = "adresse";
            codepostal = "codepostal";
            suffixe = "suffixe";
        }

        public void lecture()
        {
            //premier caractère de la chaine "separateur"
            separateur = ConfigurationManager.AppSettings["separateur"].ToCharArray(0, 1)[0];
            adresse = ConfigurationManager.AppSettings["adresse"];
            codepostal = ConfigurationManager.AppSettings["codepostal"];
            suffixe = ConfigurationManager.AppSettings["suffixe"];
        }

        public void ecriture(char separateur, string adresse, string codepostal, string suffixe)
        {
            this.separateur = separateur;
            this.adresse = adresse;
            this.codepostal = codepostal;
            this.suffixe = suffixe;

            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            config.AppSettings.Settings.Remove("separateur");
            config.AppSettings.Settings.Add("separateur", separateur.ToString());
            config.AppSettings.Settings.Remove("adresse");
            config.AppSettings.Settings.Add("adresse", adresse);
            config.AppSettings.Settings.Remove("codepostal");
            config.AppSettings.Settings.Add("codepostal", codepostal);
            config.AppSettings.Settings.Remove("suffixe");
            config.AppSettings.Settings.Add("suffixe", suffixe);

            // Enregistre les modifications du fichier
            config.Save(ConfigurationSaveMode.Modified);

            // Recharge les valeurs
            ConfigurationManager.RefreshSection("appSettings");
        }
    }
}
