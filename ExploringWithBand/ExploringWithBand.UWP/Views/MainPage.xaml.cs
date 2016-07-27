using ExploringWithBand.UWP.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Devices.Geolocation;
using ExploringWithBand.UWP.FourSquare;
using Windows.Devices.Geolocation.Geofencing;
using Windows.UI.Xaml.Navigation;
using Microsoft.Band;
using Microsoft.Band.Tiles;
using Windows.UI.Core;
using Microsoft.Band.Tiles.Pages;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ExploringWithBand.UWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private bool isGeofenceUp = false;
        private const string CURRENT_GEOFENCE_ID = "__current";
        private Guid tileGuid = Guid.Empty;
        private IBandClient bandClient = null;

        public MainPage()
        {
            this.InitializeComponent();

            Loaded += MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            ConnectToBand();

            //GeolocatorService.Instance.OnPositionChanged += OnPositionChanged;

            //RefreshData();
        }

        private async void ConnectToBand()
        {
            IBandInfo[] pairedBands = await BandClientManager.Instance.GetBandsAsync();

            try
            {
                if (pairedBands.Length > 0)
                {
                    bandClient = await BandClientManager.Instance.ConnectAsync(pairedBands[0]);

                    // do work after successful connect
                    try
                    {
                        try
                        {
                            await CreateTilePage();
                        }
                        catch (BandException ex)
                        {
                            // handle a Band connection exception
                        }
                    }
                    catch (BandException ex)
                    {
                        // handle a Band connection exception
                    }

                }
            }
            catch (BandException ex)
            {
                // handle a Band connection exception
            }
        }

        private async Task CreateTilePage()
        {
            try
            {
                var tiles = await bandClient.TileManager.GetTilesAsync();
                if(tiles.Any())
                {
                    tileGuid = tiles.First().TileId;
                    SendBandNotification();
                    return;
                }
            }
            catch (Exception ex)
            {

            }

            // create a new Guid for the tile
            tileGuid = Guid.NewGuid();

            // Create the small and tile icons from writable bitmaps.
            // Small icons are 24x24 pixels.
            WriteableBitmap smallIconBitmap = new WriteableBitmap(24, 24);
            BandIcon smallIcon = smallIconBitmap.ToBandIcon();
            // Tile icons are 46x46 pixels for Microsoft Band 1, and 48x48 pixels
            // for Microsoft Band 2.
            WriteableBitmap tileIconBitmap = new WriteableBitmap(48, 48);
            BandIcon tileIcon = tileIconBitmap.ToBandIcon();

            // create a new tile
            BandTile tile = new BandTile(tileGuid)
            {
                // set the name
                Name = "MyTile",
                // set the icons
                SmallIcon = smallIcon,
                TileIcon = tileIcon
            };

            // Our layout doesn’t use Icons, but if it did, we would
            // add our Icons to the tile by calling tile.AdditionalIcons.Add
            // to add our (up to 8) additional BandIcons.

            // Create a scrollable vertical panel that will hold 2 text messages.
            ScrollFlowPanel panel = new ScrollFlowPanel()
            {
                Rect = new PageRect(0, 0, 245, 102)
            };

            // add the text block to contain the first message
            panel.Elements.Add(
                new WrappedTextBlock
                {
                    ElementId = (short)TileMessagesLayoutElementId.Message1,
                    Rect = new PageRect(0, 0, 245, 102),
                    // left, top, right, bottom margins
                    Margins = new Margins(15, 0, 15, 0),
                    Color = new BandColor(0xFF, 0xFF, 0xFF),
                    Font = WrappedTextBlockFont.Small
                });
            // add the text block to contain the second message
            panel.Elements.Add(
                new WrappedTextBlock
                {
                    ElementId = (short)TileMessagesLayoutElementId.Message2,
                    Rect = new PageRect(0, 0, 245, 102),
                    // left, top, right, bottom margins
                    Margins = new Margins(15, 0, 15, 0),
                    Color = new BandColor(0xFF, 0xFF, 0xFF),
                    Font = WrappedTextBlockFont.Small
                });

            // create the page layout
            PageLayout layout = new PageLayout(panel);

            try
            {
                // add the layout to the tile
                tile.PageLayouts.Add(layout);
            }
            catch (BandException ex)
            {
                // handle an error adding the layout
            }

            // prerequisite: bandClient has successfully connected to a Band  
            try
            {
                await bandClient.NotificationManager.VibrateAsync(Microsoft.Band.Notifications.VibrationType.TwoToneHigh);

                // add the tile to the Band
                if (await bandClient.TileManager.AddTileAsync(tile))
                {
                    // tile was successfully added
                    // can proceed to set tile content with SetPagesAsync
                    // create a new Guid for the messages page
                    Guid messagesPageGuid = Guid.NewGuid();
                    // create the object that contains the page content to be set
                    PageData pageContent = new PageData(
                        messagesPageGuid,
                        // specify which layout to use for this page
                        (int)TileLayoutIndex.MessagesLayout,
                        new WrappedTextBlockData((Int16)TileMessagesLayoutElementId.Message1,
                        "This is the text of the first message"), new WrappedTextBlockData(
                    (Int16)TileMessagesLayoutElementId.Message2,
                    "This is the text of the second message"));
                    try
                    {     // set the page content on the Band
                        if (await bandClient.TileManager.SetPagesAsync(tileGuid, pageContent))
                        {
                            // page content successfully set on Band
                        }
                        else
                        {
                            // unable to set content to the Band
                        }
                    }
                    catch (BandException ex)
                    {
                        // handle a Band connection exception
                    }
                }
                else
                {         // tile failed to be added, handle error
                }
            }
            catch (BandException ex)
            {     // handle a Band connection exception
            }
        }

        // Define symbolic constants for indexes to each layout that
        // the tile has. The index of the first layout is 0. Because only
        // 5 layouts are allowed, the max index value is 4.
        internal enum TileLayoutIndex
        {
            MessagesLayout = 0,
        };

        // Define symbolic constants to uniquely (in MessagesLayout)
        // identify each of the elements of our layout
        // that contain content that the app will set
        // (that is, these Ids will be used when calling APIs
        // to set the page content).
        internal enum TileMessagesLayoutElementId : short
        {
            Message1 = 1, // Id for the 1st message text block
            Message2 = 2, // Id for the 2nd message text block
        };

        private async void RefreshData()
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High,
                async () =>
                {
                    var data = await LoadDataAsync();
                    lstHub.ItemsSource = data;
                });
        }

        #region GPS Position Event
        private void OnPositionChanged(Geoposition obj)
        {
            RefreshData();
        }

        private async void SendBandNotification()
        {
            if (bandClient == null || tileGuid == Guid.Empty || !bandClient.TileManager.TileInstalledAndOwned(tileGuid, System.Threading.CancellationToken.None))
            {
                return;
            }
            try
            {
                // send a dialog to the Band for one of our tiles
                await bandClient.NotificationManager.ShowDialogAsync(tileGuid, "Dialog title", "Dialog body");
            }
            catch (BandException ex)
            {
                // handle a Band connection exception
            }
        }
        #endregion

        private void SetupGeofences(List<Venue> venues, Geoposition position)
        {
            var geofences = GeofenceMonitor.Current.Geofences;

            if (!isGeofenceUp)
            {
                isGeofenceUp = true;
                GeofenceMonitor.Current.GeofenceStateChanged += OnGeofenceStateChanged;
                GeofenceMonitor.Current.StatusChanged += OnGeofenceStatusChanged;
            }
            
            CreateAllGeofences(venues, position);
        }

        private void CreateAllGeofences(List<Venue> venues, Geoposition currentPos)
        {
            int i = 0;
            foreach (Venue v in venues)
            {
                var position = new BasicGeoposition();
                position.Latitude = Double.Parse(v.Latitude);
                position.Longitude = Double.Parse(v.Longitude);
                var geocircle = new Geocircle(position, 50);
                var geofence = new Geofence("geo" + i, geocircle);
                i++;
            }

            // create geofence from current position
            var basicCurrentPos = new BasicGeoposition();
            basicCurrentPos.Latitude = currentPos.Coordinate.Point.Position.Latitude;
            basicCurrentPos.Longitude = currentPos.Coordinate.Point.Position.Longitude;
            new Geofence(CURRENT_GEOFENCE_ID, new Geocircle(basicCurrentPos, 250));

        }

        private void TearDownGeofences()
        {
            // should work if it implements the interface?
            GeofenceMonitor.Current.Geofences.Clear();
        }

        #region Geofence events
        private void OnGeofenceStatusChanged(GeofenceMonitor sender, object args)
        {
            // ToDo: Don't know yet
        }

        private async void OnGeofenceStateChanged(GeofenceMonitor sender, object args)
        {
            var reports = sender.ReadReports();

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                foreach (GeofenceStateChangeReport report in reports)
                {
                    GeofenceState state = report.NewState;

                    Geofence geofence = report.Geofence;

                    if (state == GeofenceState.Removed)
                    {
                        // Remove the geofence from the geofences collection.
                        GeofenceMonitor.Current.Geofences.Remove(geofence);
                    }
                    else if (state == GeofenceState.Entered)
                    {

                    }
                    else if (state == GeofenceState.Exited)
                    {
                        if (geofence.Id == CURRENT_GEOFENCE_ID)
                        {
                            TearDownGeofences();
                            RefreshData();
                        }
                        else
                        {
                            // TODO send notification to Band
                        }
                    }
                }
            });

        }
        #endregion

        #region Load Data
        private async Task<List<Venue>> LoadDataAsync()
        {
            var position = await GeolocatorService.Instance.GetCurrentLocationAsync();

            var listOfSelectedCategories = (App.Current as App).SelectedCategories.SelectMany(kvp => kvp.Value).ToList();

            FourSquareService fs = new FourSquareService();
            var venues = await fs.FetchVenuesAsync(position.Coordinate.Point.Position.Latitude, position.Coordinate.Point.Position.Longitude, listOfSelectedCategories.Count > 0 ? listOfSelectedCategories : null);

            SetupGeofences(venues, position);

            return venues;
        }

        private IList<HubType> LoadDummyData()
        {
            List<HubType> dummyList = new List<HubType>();
            Random rand = new Random();
            int i = 0;
            dummyList.Add(new HubType() { Id = (i++).ToString(), Rating = (rand.NextDouble() * 10).ToString("N1"), DistanceInMiles = (rand.NextDouble() * 2).ToString("N1") + " mi", Name = "Chihuly Garden", CategoryName = "Museum", Description = "lkdsj flkds jlkfdsljf jdsfldslkj flkds jlkfjdsfjds jfld slkfljdsf jsd jfds jgoie jg jfd j;r jb;lkw;lk jlkf jlkds hfn lkdsj flkds jlkfdsljf jdsfldslkj flkds jlkfjdsfjds jfld slkfljdsf jsd jfds jgoie jg jfd j;r jb;lkw;lk jlkf jlkds lkdsj flkds jlkfdsljf jdsfldslkj flkds jlkfjdsfjds jfld slkfljdsf jsd jfds jgoie jg jfd j;r jb;lkw;lk jlkf jlkds lkdsj flkds jlkfdsljf jdsfldslkj flkds jlkfjdsfjds jfld slkfljdsf jsd jfds jgoie jg jfd j;r jb;lkw;lk jlkf jlkds lkdsj flkds jlkfdsljf jdsfldslkj flkds jlkfjdsfjds jfld slkfljdsf jsd jfds jgoie jg jfd j;r jb;lkw;lk jlkf jlkds" });
            dummyList.Add(new HubType() { Id = (i++).ToString(), Rating = (rand.NextDouble() * 10).ToString("N1"), DistanceInMiles = (rand.NextDouble() * 2).ToString("N1") + " mi", Name = "lksj fdsj fs", CategoryName = "sdlkfj lkdsjfds", Description = "lkdsj flkds jlkfdsljf jdsfldslkj flkds jlkfjdsfjds jfld slkfljdsf jsd jfds jgoie jg jfd j;r jb;lkw;lk jlkf jlkds hfn lkdsj flkds jlkfdsljf jdsfldslkj flkds jlkfjdsfjds jfld slkfljdsf jsd jfds jgoie jg jfd j;r jb;lkw;lk jlkf jlkds lkdsj flkds jlkfdsljf jdsfldslkj flkds jlkfjdsfjds jfld slkfljdsf jsd jfds jgoie jg jfd j;r jb;lkw;lk jlkf jlkds lkdsj flkds jlkfdsljf jdsfldslkj flkds jlkfjdsfjds jfld slkfljdsf jsd jfds jgoie jg jfd j;r jb;lkw;lk jlkf jlkds lkdsj flkds jlkfdsljf jdsfldslkj flkds jlkfjdsfjds jfld slkfljdsf jsd jfds jgoie jg jfd j;r jb;lkw;lk jlkf jlkds" });
            dummyList.Add(new HubType() { Id = (i++).ToString(), Rating = (rand.NextDouble() * 10).ToString("N1"), DistanceInMiles = (rand.NextDouble() * 2).ToString("N1") + " mi", Name = "lkejt eytl kre", CategoryName = "fj oireyr egre", Description = "lkdsj flkds jlkfdsljf jdsfldslkj flkds jlkfjdsfjds jfld slkfljdsf jsd jfds jgoie jg jfd j;r jb;lkw;lk jlkf jlkds hfn lkdsj flkds jlkfdsljf jdsfldslkj flkds jlkfjdsfjds jfld slkfljdsf jsd jfds jgoie jg jfd j;r jb;lkw;lk jlkf jlkds lkdsj flkds jlkfdsljf jdsfldslkj flkds jlkfjdsfjds jfld slkfljdsf jsd jfds jgoie jg jfd j;r jb;lkw;lk jlkf jlkds lkdsj flkds jlkfdsljf jdsfldslkj flkds jlkfjdsfjds jfld slkfljdsf jsd jfds jgoie jg jfd j;r jb;lkw;lk jlkf jlkds lkdsj flkds jlkfdsljf jdsfldslkj flkds jlkfjdsfjds jfld slkfljdsf jsd jfds jgoie jg jfd j;r jb;lkw;lk jlkf jlkds" });
            dummyList.Add(new HubType() { Id = (i++).ToString(), Rating = (rand.NextDouble() * 10).ToString("N1"), DistanceInMiles = (rand.NextDouble() * 2).ToString("N1") + " mi", Name = "rew qwe qew qw ewq req", CategoryName = "few few", Description = "lkdsj flkds jlkfdsljf jdsfldslkj flkds jlkfjdsfjds jfld slkfljdsf jsd jfds jgoie jg jfd j;r jb;lkw;lk jlkf jlkds hfn lkdsj flkds jlkfdsljf jdsfldslkj flkds jlkfjdsfjds jfld slkfljdsf jsd jfds jgoie jg jfd j;r jb;lkw;lk jlkf jlkds lkdsj flkds jlkfdsljf jdsfldslkj flkds jlkfjdsfjds jfld slkfljdsf jsd jfds jgoie jg jfd j;r jb;lkw;lk jlkf jlkds lkdsj flkds jlkfdsljf jdsfldslkj flkds jlkfjdsfjds jfld slkfljdsf jsd jfds jgoie jg jfd j;r jb;lkw;lk jlkf jlkds lkdsj flkds jlkfdsljf jdsfldslkj flkds jlkfjdsfjds jfld slkfljdsf jsd jfds jgoie jg jfd j;r jb;lkw;lk jlkf jlkds" });
            dummyList.Add(new HubType() { Id = (i++).ToString(), Rating = (rand.NextDouble() * 10).ToString("N1"), DistanceInMiles = (rand.NextDouble() * 2).ToString("N1") + " mi", Name = "ew qeq", CategoryName = "wefl kwjelkf jwefw", Description = "lkdsj flkds jlkfdsljf jdsfldslkj flkds jlkfjdsfjds jfld slkfljdsf jsd jfds jgoie jg jfd j;r jb;lkw;lk jlkf jlkds hfn lkdsj flkds jlkfdsljf jdsfldslkj flkds jlkfjdsfjds jfld slkfljdsf jsd jfds jgoie jg jfd j;r jb;lkw;lk jlkf jlkds lkdsj flkds jlkfdsljf jdsfldslkj flkds jlkfjdsfjds jfld slkfljdsf jsd jfds jgoie jg jfd j;r jb;lkw;lk jlkf jlkds lkdsj flkds jlkfdsljf jdsfldslkj flkds jlkfjdsfjds jfld slkfljdsf jsd jfds jgoie jg jfd j;r jb;lkw;lk jlkf jlkds lkdsj flkds jlkfdsljf jdsfldslkj flkds jlkfjdsfjds jfld slkfljdsf jsd jfds jgoie jg jfd j;r jb;lkw;lk jlkf jlkds" });
            dummyList.Add(new HubType() { Id = (i++).ToString(), Rating = (rand.NextDouble() * 10).ToString("N1"), DistanceInMiles = (rand.NextDouble() * 2).ToString("N1") + " mi", Name = "34", CategoryName = "wef m", Description = "lkdsj flkds jlkfdsljf jdsfldslkj flkds jlkfjdsfjds jfld slkfljdsf jsd jfds jgoie jg jfd j;r jb;lkw;lk jlkf jlkds hfn lkdsj flkds jlkfdsljf jdsfldslkj flkds jlkfjdsfjds jfld slkfljdsf jsd jfds jgoie jg jfd j;r jb;lkw;lk jlkf jlkds lkdsj flkds jlkfdsljf jdsfldslkj flkds jlkfjdsfjds jfld slkfljdsf jsd jfds jgoie jg jfd j;r jb;lkw;lk jlkf jlkds lkdsj flkds jlkfdsljf jdsfldslkj flkds jlkfjdsfjds jfld slkfljdsf jsd jfds jgoie jg jfd j;r jb;lkw;lk jlkf jlkds lkdsj flkds jlkfdsljf jdsfldslkj flkds jlkfjdsfjds jfld slkfljdsf jsd jfds jgoie jg jfd j;r jb;lkw;lk jlkf jlkds" });
            dummyList.Add(new HubType() { Id = (i++).ToString(), Rating = (rand.NextDouble() * 10).ToString("N1"), DistanceInMiles = (rand.NextDouble() * 2).ToString("N1") + " mi", Name = "carefe", CategoryName = "wef jge", Description = "lkdsj flkds jlkfdsljf jdsfldslkj flkds jlkfjdsfjds jfld slkfljdsf jsd jfds jgoie jg jfd j;r jb;lkw;lk jlkf jlkds hfn lkdsj flkds jlkfdsljf jdsfldslkj flkds jlkfjdsfjds jfld slkfljdsf jsd jfds jgoie jg jfd j;r jb;lkw;lk jlkf jlkds lkdsj flkds jlkfdsljf jdsfldslkj flkds jlkfjdsfjds jfld slkfljdsf jsd jfds jgoie jg jfd j;r jb;lkw;lk jlkf jlkds lkdsj flkds jlkfdsljf jdsfldslkj flkds jlkfjdsfjds jfld slkfljdsf jsd jfds jgoie jg jfd j;r jb;lkw;lk jlkf jlkds lkdsj flkds jlkfdsljf jdsfldslkj flkds jlkfjdsfjds jfld slkfljdsf jsd jfds jgoie jg jfd j;r jb;lkw;lk jlkf jlkds" });
            dummyList.Add(new HubType() { Id = (i++).ToString(), Rating = (rand.NextDouble() * 10).ToString("N1"), DistanceInMiles = (rand.NextDouble() * 2).ToString("N1") + " mi", Name = "dlkfs jr", CategoryName = "sdlfk jrehgr", Description = "lkdsj flkds jlkfdsljf jdsfldslkj flkds jlkfjdsfjds jfld slkfljdsf jsd jfds jgoie jg jfd j;r jb;lkw;lk jlkf jlkds hfn lkdsj flkds jlkfdsljf jdsfldslkj flkds jlkfjdsfjds jfld slkfljdsf jsd jfds jgoie jg jfd j;r jb;lkw;lk jlkf jlkds lkdsj flkds jlkfdsljf jdsfldslkj flkds jlkfjdsfjds jfld slkfljdsf jsd jfds jgoie jg jfd j;r jb;lkw;lk jlkf jlkds lkdsj flkds jlkfdsljf jdsfldslkj flkds jlkfjdsfjds jfld slkfljdsf jsd jfds jgoie jg jfd j;r jb;lkw;lk jlkf jlkds lkdsj flkds jlkfdsljf jdsfldslkj flkds jlkfjdsfjds jfld slkfljdsf jsd jfds jgoie jg jfd j;r jb;lkw;lk jlkf jlkds" });
            dummyList.Add(new HubType() { Id = (i++).ToString(), Rating = (rand.NextDouble() * 10).ToString("N1"), DistanceInMiles = (rand.NextDouble() * 2).ToString("N1") + " mi", Name = "slfk jewlktjfs fw rew", CategoryName = "sdfsgf sf sfs ", Description = "lkdsj flkds jlkfdsljf jdsfldslkj flkds jlkfjdsfjds jfld slkfljdsf jsd jfds jgoie jg jfd j;r jb;lkw;lk jlkf jlkds hfn lkdsj flkds jlkfdsljf jdsfldslkj flkds jlkfjdsfjds jfld slkfljdsf jsd jfds jgoie jg jfd j;r jb;lkw;lk jlkf jlkds lkdsj flkds jlkfdsljf jdsfldslkj flkds jlkfjdsfjds jfld slkfljdsf jsd jfds jgoie jg jfd j;r jb;lkw;lk jlkf jlkds lkdsj flkds jlkfdsljf jdsfldslkj flkds jlkfjdsfjds jfld slkfljdsf jsd jfds jgoie jg jfd j;r jb;lkw;lk jlkf jlkds lkdsj flkds jlkfdsljf jdsfldslkj flkds jlkfjdsfjds jfld slkfljdsf jsd jfds jgoie jg jfd j;r jb;lkw;lk jlkf jlkds" });
            dummyList.Add(new HubType() { Id = (i++).ToString(), Rating = (rand.NextDouble() * 10).ToString("N1"), DistanceInMiles = (rand.NextDouble() * 2).ToString("N1") + " mi", Name = "sdflk jsfkj lkds", CategoryName = "fds  g4ht jyukyu", Description = "lkdsj flkds jlkfdsljf jdsfldslkj flkds jlkfjdsfjds jfld slkfljdsf jsd jfds jgoie jg jfd j;r jb;lkw;lk jlkf jlkds hfn lkdsj flkds jlkfdsljf jdsfldslkj flkds jlkfjdsfjds jfld slkfljdsf jsd jfds jgoie jg jfd j;r jb;lkw;lk jlkf jlkds lkdsj flkds jlkfdsljf jdsfldslkj flkds jlkfjdsfjds jfld slkfljdsf jsd jfds jgoie jg jfd j;r jb;lkw;lk jlkf jlkds lkdsj flkds jlkfdsljf jdsfldslkj flkds jlkfjdsfjds jfld slkfljdsf jsd jfds jgoie jg jfd j;r jb;lkw;lk jlkf jlkds lkdsj flkds jlkfdsljf jdsfldslkj flkds jlkfjdsfjds jfld slkfljdsf jsd jfds jgoie jg jfd j;r jb;lkw;lk jlkf jlkds" });
            dummyList.Add(new HubType() { Id = (i++).ToString(), Rating = (rand.NextDouble() * 10).ToString("N1"), DistanceInMiles = (rand.NextDouble() * 2).ToString("N1") + " mi", Name = "sflkjew ljuth; dsgf", CategoryName = "ukm hree rtgrewge wrge", Description = "lkdsj flkds jlkfdsljf jdsfldslkj flkds jlkfjdsfjds jfld slkfljdsf jsd jfds jgoie jg jfd j;r jb;lkw;lk jlkf jlkds hfn lkdsj flkds jlkfdsljf jdsfldslkj flkds jlkfjdsfjds jfld slkfljdsf jsd jfds jgoie jg jfd j;r jb;lkw;lk jlkf jlkds lkdsj flkds jlkfdsljf jdsfldslkj flkds jlkfjdsfjds jfld slkfljdsf jsd jfds jgoie jg jfd j;r jb;lkw;lk jlkf jlkds lkdsj flkds jlkfdsljf jdsfldslkj flkds jlkfjdsfjds jfld slkfljdsf jsd jfds jgoie jg jfd j;r jb;lkw;lk jlkf jlkds lkdsj flkds jlkfdsljf jdsfldslkj flkds jlkfjdsfjds jfld slkfljdsf jsd jfds jgoie jg jfd j;r jb;lkw;lk jlkf jlkds" });
            dummyList.Add(new HubType() { Id = (i++).ToString(), Rating = (rand.NextDouble() * 10).ToString("N1"), DistanceInMiles = (rand.NextDouble() * 2).ToString("N1") + " mi", Name = "sd lkfjlkds jflk jslkgjkewg", CategoryName = "te ehr ", Description = "lkdsj flkds jlkfdsljf jdsfldslkj flkds jlkfjdsfjds jfld slkfljdsf jsd jfds jgoie jg jfd j;r jb;lkw;lk jlkf jlkds hfn lkdsj flkds jlkfdsljf jdsfldslkj flkds jlkfjdsfjds jfld slkfljdsf jsd jfds jgoie jg jfd j;r jb;lkw;lk jlkf jlkds lkdsj flkds jlkfdsljf jdsfldslkj flkds jlkfjdsfjds jfld slkfljdsf jsd jfds jgoie jg jfd j;r jb;lkw;lk jlkf jlkds lkdsj flkds jlkfdsljf jdsfldslkj flkds jlkfjdsfjds jfld slkfljdsf jsd jfds jgoie jg jfd j;r jb;lkw;lk jlkf jlkds lkdsj flkds jlkfdsljf jdsfldslkj flkds jlkfjdsfjds jfld slkfljdsf jsd jfds jgoie jg jfd j;r jb;lkw;lk jlkf jlkds" });

            return dummyList;
        }
        #endregion

        #region Expand/Collapse logic and event
        private void Image_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var img = sender as Image;
            var parentGrid = img.Parent as Grid;

            var details = parentGrid.FindName("Details") as Grid;

            if (details.Visibility == Visibility.Collapsed)
            {
                // Set to bottom and make hidden textblock visible and change icon
                details.Visibility = Visibility.Visible;
                img.Source = new BitmapImage(new Uri("ms-appx:///Assets/Collapse.png"));
            }
            else
            {
                details.Visibility = Visibility.Collapsed;
                img.Source = new BitmapImage(new Uri("ms-appx:///Assets/Expand.png"));
            }
        }
        #endregion

        private void Grid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var grid = sender as Grid;
            var id = grid.Tag.ToString();

            // ToDo: What ever we want to do here
        }

        #region Menu Events
        private void Image_Tapped_1(object sender, TappedRoutedEventArgs e)
        {
            BurgerToggle.IsChecked = false;
        }

        private void Image_Tapped_2(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(SettingsPage));
        }
        #endregion

        #region Navigation Overrides
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);

            GeolocatorService.Instance.OnPositionChanged -= OnPositionChanged;

            if (isGeofenceUp)
            {
                isGeofenceUp = false;
                GeofenceMonitor.Current.GeofenceStateChanged -= OnGeofenceStateChanged;
                GeofenceMonitor.Current.StatusChanged -= OnGeofenceStatusChanged;
            }
        }
        #endregion
    }

    public class HubType
    {
        public string Name { get; set; }
        public string CategoryName { get; set; }
        public string DistanceInMiles { get; set; }
        public string Description { get; set; }
        public string Rating { get; set; }
        public string Id { get; set; }
    }
}
