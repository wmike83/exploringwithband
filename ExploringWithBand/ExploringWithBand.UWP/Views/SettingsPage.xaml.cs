using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace ExploringWithBand.UWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        private Dictionary<string, List<string>> categories = new Dictionary<string, List<string>>();

        public SettingsPage()
        {
            this.InitializeComponent();

            categories.Add("Food", new List<string>() { "4d4b7105d754a06374d81259" });
            categories.Add("Drinks", new List<string>() { "4d4b7105d754a06376d81259" });
            categories.Add("Coffee", new List<string>() { "4bf58dd8d48988d16d941735", "4bf58dd8d48988d1e0931735" });
            categories.Add("Shops", new List<string>() { "4d4b7105d754a06378d81259" });
            categories.Add("Arts", new List<string>() { "4d4b7104d754a06370d81259" });
            categories.Add("Outdoors", new List<string>() { "4d4b7105d754a06377d81259" });
            categories.Add("Sights", new List<string>() { "4bf58dd8d48988d12d941735" });

            Loaded += SettingsPage_Loaded;
        }

        private void SettingsPage_Loaded(object sender, RoutedEventArgs e)
        {
            var selected = (App.Current as App).SelectedCategories;

            lstInterests.ItemsSource = categories.Select(c => new { Name = c.Key, IsSelected = selected.ContainsKey(c.Key) }).ToList();
        }

        private void Image_Tapped_1(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        private void Image_Tapped_2(object sender, TappedRoutedEventArgs e)
        {
            BurgerToggle.IsChecked = false;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var checkBox = sender as CheckBox;
            string tag = checkBox.Tag.ToString();

            var selected = (App.Current as App).SelectedCategories;

            if (!selected.ContainsKey(tag))
            {
                selected.Add(tag, categories[tag]);
            }
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            var checkBox = sender as CheckBox;
            string tag = checkBox.Tag.ToString();

            var selected = (App.Current as App).SelectedCategories;

            if (selected.ContainsKey(tag))
            {
                selected.Remove(tag);
            }
        }
    }
}
