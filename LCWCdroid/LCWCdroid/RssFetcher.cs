using System;
using System.Collections.Generic;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace LCWCdroid
{
    public enum IncidentTypes
    {
        Medical,
        Fire,
        Traffic,
        Unknown
    }

    public static class RssFetcher
    {
        private const string Url = "https://webcad.lcwc911.us/Pages/Public/LiveIncidentsFeed.aspx";

        public static List<Incident> FetchFeed()
        {
            try
            {
                var feed = SyndicationFeed.Load(XmlReader.Create(Url));

                var list = new List<Incident>();
                foreach (var item in feed.Items)
                {
                    var incident = new Incident(item.Title.Text, item.Summary.Text, item.PublishDate.LocalDateTime);
                    list.Add(incident);
                    if (incident.CopyTo.HasValue)
                        list.Add(new Incident(item.Title.Text, item.Summary.Text, item.PublishDate.LocalDateTime, incident.CopyTo.Value));
                }
                System.Diagnostics.Debug.WriteLine("Refresh complete.");
                return list;
            }
            catch (Exception ex)
            {
                return new List<Incident>();
            }
        }
    }
}
