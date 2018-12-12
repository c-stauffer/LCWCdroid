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
    }
}
