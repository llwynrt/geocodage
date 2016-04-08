using Newtonsoft.Json;
using System.Collections.Generic;
using Json2;

namespace Geocodage
{
    public class Geoloc
    {
        Json json = new Json();
        string texte;
        Reponse m;

        public Geoloc(string rue, string postcode)
        {
            convertir(rue, postcode);
        }

        public void convertir(string rue, string postcode)
        {
            texte = json.GET("http://api-adresse.data.gouv.fr/search/?q=" + rue + "&postcode=" + postcode + "&limit=1");
            texte.Replace("Properties", "Properties2");
            m = JsonConvert.DeserializeObject<Reponse>(texte);
        }

        public double Latitude
        {
            get
            {
                return m.features[0].geometry.coordinates[1];
            }
        }

        public double Longitude
        {
            get
            {
                return m.features[0].geometry.coordinates[0];
            }
        }

        public string label
        {
            get
            {
                return m.features[0].properties.label;
            }
        }
    }


    public class Filters
    {
        public string postcode { get; set; }
    }

    public class Geometry
    {
        public string type { get; set; }
        public List<double> coordinates { get; set; }
    }

    public class Properties2
    {
        public string citycode { get; set; }
        public string street { get; set; }
        public string name { get; set; }
        public string housenumber { get; set; }
        public string city { get; set; }
        public string type { get; set; }
        public string context { get; set; }
        public double score { get; set; }
        public string label { get; set; }
        public string postcode { get; set; }
        public string id { get; set; }
    }

    public class Feature
    {
        public Geometry geometry { get; set; }
        public Properties2 properties { get; set; }
        public string type { get; set; }
    }

    public class Reponse
    {
        public int limit { get; set; }
        public Filters filters { get; set; }
        public string attribution { get; set; }
        public string version { get; set; }
        public string licence { get; set; }
        public string query { get; set; }
        public string type { get; set; }
        public List<Feature> features { get; set; }
    }
}