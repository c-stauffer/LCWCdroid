using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace LCWCdroid
{
    public class IncidentPage : INotifyPropertyChanged
    {
        private string _pageTitle;
        private ObservableCollection<Incident> _incidentList;

        public string PageTitle { get => _pageTitle; set => SetProperty(ref _pageTitle, value, nameof(PageTitle)); }
        public ObservableCollection<Incident> IncidentList { get => _incidentList; set => SetProperty(ref _incidentList, value, nameof(IncidentList)); }


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
