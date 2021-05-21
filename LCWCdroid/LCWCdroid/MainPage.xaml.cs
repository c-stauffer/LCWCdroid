using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LCWCdroid
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : CarouselPage
    {
        public MainPage()
        {
            //try {
            var pages = new IncidentPages();
            pages.Fetch();
            BindingContext = pages;//.Pages;
                                   //System.Diagnostics.Debug.WriteLine(Children.Count);
                                   //}
                                   //catch (Exception ex)
                                   //{
                                   //    System.Diagnostics.Debug.WriteLine(ex.Message);
                                   //}
            InitializeComponent();
        }

        private async void OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            // Pepega mode
            int pageIndex;
            if (sender == listView0)
                pageIndex = 0;
            else if (sender == listView1)
                pageIndex = 1;
            else
                pageIndex = 2;

            var placemark = new Xamarin.Essentials.Placemark
            {
                CountryName = "United States",
                AdminArea = "PA",
                Thoroughfare = ((IncidentPages)BindingContext).Pages[pageIndex].IncidentList[e.ItemIndex].Intersection,
                Locality = ((IncidentPages)BindingContext).Pages[pageIndex].IncidentList[e.ItemIndex].Location
            };

            await Xamarin.Essentials.Map.OpenAsync(placemark);
        }
    }
}
