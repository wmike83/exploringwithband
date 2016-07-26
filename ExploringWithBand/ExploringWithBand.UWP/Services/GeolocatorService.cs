using System;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace ExploringWithBand.UWP.Services
{
    public class GeolocatorService
    {
        #region Singleton Instance
        private static GeolocatorService _instance = null;
        public static GeolocatorService Instance
        { get { return _instance ?? (_instance = new GeolocatorService()); } }
        #endregion

        #region Constructor
        private GeolocatorService()
        {
        }
        #endregion

        private Geolocator locator = null;
        private Geoposition lastPosition = null;
        private GeolocationAccessStatus accessStatus = GeolocationAccessStatus.Unspecified;
        private TimeSpan refreshTime = new TimeSpan(0, 5, 0); // 5 minutes

        private async Task Init()
        {
            // Ask user for access to GPS
            accessStatus = await Geolocator.RequestAccessAsync();

            // If allowed then initialize geolocator
            if(accessStatus == GeolocationAccessStatus.Allowed)
            {
                locator = new Geolocator() { DesiredAccuracyInMeters = 5 };
            }
        }

        public async Task<Geoposition> GetCurrentLocationAsync()
        {
            if(accessStatus == GeolocationAccessStatus.Unspecified)
            {
                await Init();
            }

            if (accessStatus == GeolocationAccessStatus.Allowed)
            {
                // If difference in time is greater than refreshTime then get new locatoin
                if (lastPosition == null || (DateTime.Now - lastPosition.Coordinate.Timestamp) > refreshTime)
                {
                    lastPosition = await locator.GetGeopositionAsync();
                }

                return lastPosition;
            }

            return null;
        }
    }
}
