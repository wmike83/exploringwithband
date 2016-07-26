using ExploringWithBand.UWP.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ExploringWithBand.UWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            Loaded += MainPage_Loaded;
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            lstHub.ItemsSource = await LoadDataAsync();
        }

        private async Task<IList<HubType>> LoadDataAsync()
        {
            var position = await GeolocatorService.Instance.GetCurrentLocationAsync();

            var listOfSelectedCategories = (App.Current as App).SelectedCategories.SelectMany(kvp => kvp.Value).ToList();

            FourSquareService fs = new FourSquareService();
            var venues = await fs.FetchVenuesAsync(position.Coordinate.Point.Position.Latitude, position.Coordinate.Point.Position.Longitude, listOfSelectedCategories.Count > 0 ? listOfSelectedCategories : null);

            var li = venues.Select(v => new HubType() { Id = v.Name, Name = v.Name, CategoryName = v.Categories.First(), DistanceInMiles = v.Distance, Rating = "::", Description = "??" }).ToList();

            return li;
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

        private void Grid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var grid = sender as Grid;
            var id = grid.Tag.ToString();

            // ToDo: What ever we want to do here
        }

        private void Image_Tapped_1(object sender, TappedRoutedEventArgs e)
        {
            BurgerToggle.IsChecked = false;
        }

        private void Image_Tapped_2(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(SettingsPage));
        }
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
