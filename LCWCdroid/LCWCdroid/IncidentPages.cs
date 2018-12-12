using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace LCWCdroid
{
    public class IncidentPages : INotifyPropertyChanged
    {
        private ObservableCollection<IncidentPage> _pages;
        private bool _isRefreshing = false;

        public ObservableCollection<IncidentPage> Pages { get => _pages; set => SetProperty(ref _pages, value, nameof(Pages)); }

        public bool IsRefreshing { get => _isRefreshing; set => SetProperty(ref _isRefreshing, value, nameof(IsRefreshing)); }

        public ICommand RefreshCommand => new Command(async () =>
                                                        {
                                                            IsRefreshing = true;

                                                            await FetchAsync();

                                                            IsRefreshing = false;
                                                        });

        public async Task FetchAsync()
        {
            if (Pages == null)
                Pages = new ObservableCollection<IncidentPage>();
            else
                Pages.Clear();
            var all = await Task.Run(() => RssFetcher.FetchFeed());
            Pages.Add(new IncidentPage { PageTitle = "Fire Incidents", IncidentList = new ObservableCollection<Incident>(all.Where(i => i.IncidentType == IncidentTypes.Fire).ToList()) });
            Pages.Add(new IncidentPage { PageTitle = "Medical Incidents", IncidentList = new ObservableCollection<Incident>(all.Where(i => i.IncidentType == IncidentTypes.Medical).ToList()) });
            Pages.Add(new IncidentPage { PageTitle = "Traffic Incidents", IncidentList = new ObservableCollection<Incident>(all.Where(i => i.IncidentType == IncidentTypes.Traffic).ToList()) });
            OnPropertyChanged(nameof(Pages));
        }

        public void Fetch()
        {
            if (Pages == null)
                Pages = new ObservableCollection<IncidentPage>();
            else
                Pages.Clear();
            var all = RssFetcher.FetchFeed();
            Pages.Add(new IncidentPage { PageTitle = "Fire Incidents", IncidentList = new ObservableCollection<Incident>(all.Where(i => i.IncidentType == IncidentTypes.Fire).ToList()) });
            Pages.Add(new IncidentPage { PageTitle = "Medical Incidents", IncidentList = new ObservableCollection<Incident>(all.Where(i => i.IncidentType == IncidentTypes.Medical).ToList()) });
            Pages.Add(new IncidentPage { PageTitle = "Traffic Incidents", IncidentList = new ObservableCollection<Incident>(all.Where(i => i.IncidentType == IncidentTypes.Traffic).ToList()) });
        }

        #region INotifyPropertyChanged
        protected bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName]string propertyName = "",
            Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
