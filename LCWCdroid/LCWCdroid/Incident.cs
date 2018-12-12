using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace LCWCdroid
{
    /*
        The RSS doesn't classify into Fire/Medical/Traffic
        The description is semi-colon delimited into
                <Township>;<Intersection>;<Units Assigned>
        and it looks like Units Assigned is optional (see the second example below).
        I don't think the guid is necessary for the app to store.
        The date is GMT so it should be converted to local time for display.

        Example RSS entries:

        <item>
        <title>MEDICAL EMERGENCY</title>
        <link>http://www.lcwc911.us/lcwc/lcwc/publiccad.asp</link>
        <description>
        MANHEIM TOWNSHIP; LITITZ PIKE & VILLAGE DR; MEDIC 82-2;
        </description>
        <pubDate>Fri, 30 Nov 2018 13:21:59 GMT</pubDate>
        <guid isPermaLink="false">2c60ec10-6258-4388-9d59-83995cae6dea</guid>
        </item>

        <item>
        <title>VEHICLE ACCIDENT-NO INJURIES</title>
        <link>http://www.lcwc911.us/lcwc/lcwc/publiccad.asp</link>
        <description>UPPER LEACOCK TOWNSHIP; QUARRY RD / STORMSTOWN RD; </description>
        <pubDate>Fri, 30 Nov 2018 12:10:28 GMT</pubDate>
        <guid isPermaLink="false">2ca08e60-9f16-4e18-a135-0c3786b25a88</guid>
        </item>
 */

    public class Incident : INotifyPropertyChanged
    {
        #region Incident Type Determination Hints
        private static readonly IList<string> TrafficUnitHints = new ReadOnlyCollection<string>(new List<string> { "squad" });
        private static readonly IList<string> FireUnitHints = new ReadOnlyCollection<string>(new List<string> { "engine", "ladder", "traffic", "duty chief" });
        private static readonly IList<string> MedicalUnitHints = new ReadOnlyCollection<string>(new List<string> { "ambulance", "medic", "ems", "intermediate" });
        private static readonly IList<string> FireTitleHints = new ReadOnlyCollection<string>(new List<string> { "fire" });
        #endregion

        private static readonly System.Globalization.TextInfo _textInfo = new System.Globalization.CultureInfo("en-US").TextInfo;

        private string _description;

        private string _title;
        private string _location;
        private string _intersection;
        private string _units;
        private DateTime _incidentDate;
        private IncidentTypes _incidentType;

        // Concerning the ToLowerInvariant call inside ToTitleCase -- ToTitleCase treats words in all upper-case as acronyms, so it wasn't working!

        public string Title { get => _textInfo.ToTitleCase(_title.ToLowerInvariant()); private set => SetProperty(ref _title, value, nameof(Title)); }

        public string Location { get => _textInfo.ToTitleCase(_location.ToLowerInvariant()); private set => SetProperty(ref _location, value, nameof(Location)); }

        public string Intersection { get => _textInfo.ToTitleCase(_intersection.ToLowerInvariant()); private set => SetProperty(ref _intersection, value, nameof(Intersection)); }

        public string Units { get => _textInfo.ToTitleCase(_units.ToLowerInvariant()); private set => SetProperty(ref _units, value, nameof(Units)); }

        public DateTime IncidentDate { get => _incidentDate; private set => SetProperty(ref _incidentDate, value, nameof(IncidentDate)); }

        public IncidentTypes IncidentType { get => _incidentType; private set => SetProperty(ref _incidentType, value, nameof(IncidentType)); }

        internal IncidentTypes? CopyTo { get; set; }

        public Incident(string title, string description, DateTime incidentDate, IncidentTypes incidentType = IncidentTypes.Unknown)
        {
            _description = description;
            Title = title;
            Location = ParseLocation();
            Intersection = ParseIntersection();
            Units = ParseUnits();
            IncidentDate = incidentDate;
            IncidentType = incidentType == IncidentTypes.Unknown ? DetermineIncidentType() : incidentType;
        }
        private string ParseLocation()
        {
            return CleanString(_description.Split(';')[0]);
        }

        private string ParseIntersection()
        {
            try
            {
                return CleanString(_description.Split(';')[1]);
            }
            catch (IndexOutOfRangeException)
            {
                return string.Empty;
            }
        }

        private string ParseUnits()
        {
            try
            {
                return CleanString(_description.Split(';')[2]);
            }
            catch (IndexOutOfRangeException)
            {
                return string.Empty;
            }
        }

        private string CleanString(string str)
        {
            return str.Trim(' ').Replace("<br>", Environment.NewLine).Trim(' ');
        }

        private IncidentTypes DetermineIncidentType()
        {
            var title = Title.ToLowerInvariant();
            var units = Units.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.ToLowerInvariant().Trim()).ToList();

            if (TrafficUnitHints.Any(h => units.Any(u => u.Contains(h))))
            {
                // a lot of times an incident (i.e. vehicle fire) will have a squad and engine/traffic and show up
                // under both groups on the webpage.
                if (FireTitleHints.Any(h => title.Contains(h)))
                    CopyTo = IncidentTypes.Fire;
                return IncidentTypes.Traffic;
            }
            if (FireUnitHints.Any(h => units.Any(u => u.Contains(h))))
                return IncidentTypes.Fire;
            if (MedicalUnitHints.Any(h => units.Any(u => u.Contains(h))))
                return IncidentTypes.Medical;
            if (FireTitleHints.Any(h => title.Contains(h)))
                return IncidentTypes.Fire;
            return IncidentTypes.Traffic;
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
