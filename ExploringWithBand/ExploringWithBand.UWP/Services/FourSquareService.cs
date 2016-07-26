using ExploringWithBand.UWP.FourSquare;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ExploringWithBand.UWP.Services
{
    public class FourSquareService
    {
        private HttpClient client;
        private string clientId = "Z3D0JJHBZCV1LPSQOJ41E42MHFEE25IUIDXD0JESXBRK5W1L";
        private string clientSecret = "U1SF0GS0FDHNT2NGUR5CXZ1AKDTCKWS4QGLILPBILKG4NBD5";
        private string v = "20160725";

        private string baseAddress;

        public FourSquareService()
        {
            client = new HttpClient();
            // https://api.foursquare.com/v2/venues/search?
            // client_id=Z3D0JJHBZCV1LPSQOJ41E42MHFEE25IUIDXD0JESXBRK5W1L&
            // client_secret=U1SF0GS0FDHNT2NGUR5CXZ1AKDTCKWS4QGLILPBILKG4NBD5&
            // v=20160725
            UriBuilder uri = new UriBuilder("https://api.foursquare.com/v2/venues/search");

            StringBuilder sb = new StringBuilder();
            sb.Append("client_id=");
            sb.Append(clientId);
            sb.Append("&client_secret=");
            sb.Append(clientSecret);
            sb.Append("&v=");
            sb.Append(v);

            uri.Query = sb.ToString();

            baseAddress = uri.ToString();
        }

        public async Task<List<Venue>> FetchVenuesAsync(double lat, double lon, IList<string> CategoryFilter = null)
        {
            string filter = string.Empty;
            if (CategoryFilter != null)
            {
                filter = "&categoryId=" + String.Join(",", CategoryFilter);
            }

            var response = await client.GetAsync(baseAddress + "&ll=" + lat + "," + lon + filter);

            var content = await response.Content.ReadAsStringAsync();
            var json = JObject.Parse(content);
            var vens = json["response"]["venues"].ToList();

            var lis = vens.Select(v => new Venue(v)).ToList();
            return lis;
        }
    }
}
