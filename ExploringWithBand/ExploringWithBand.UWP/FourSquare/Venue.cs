using System.Collections.Generic;
using System.Linq;

namespace ExploringWithBand.UWP.FourSquare
{
    public class Venue
    {
        private string _venueName, _lat, _lon, _dist, _shortDescription, _longDescription;
        private List<string> _categories;
        public Venue(Newtonsoft.Json.Linq.JToken v)
        {
            Id = v["id"].ToString();
            _venueName = v["name"].ToString();
            _lat = v["location"]["lat"].ToString();
            _lon = v["location"]["lng"].ToString();
            _dist = v["location"]["distance"].ToString();
            _categories = v["categories"].ToList().Select(c => c["name"].ToString()).ToList();
        }
        public string Name {
            get { return _venueName; }
        }
        public string Latitude
        {
            get { return _lat; }
        }
        public string Longitude
        {
            get { return _lon; }
        }
        public string Distance
        {
            get { return _dist; }
        }
        public List<string> Categories
        {
            get { return _categories; }
        }
        public string Id { get; set; }
        public string Category
        {
            get {
                if (_categories.Count > 0)
                {
                    return _categories[0];
                }
                else
                {
                    return "Place";
                }
                   
            }
        }
    }
}
