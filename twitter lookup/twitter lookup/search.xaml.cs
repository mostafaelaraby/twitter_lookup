using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Xml.Linq;
using System.IO;
using System.ServiceModel.Syndication;
using System.Xml;
using Microsoft.Phone.Tasks;



namespace twitter_lookup
{
    public partial class search : PhoneApplicationPage
    {
        public search()
        {
            InitializeComponent();
            SupportedOrientations = SupportedPageOrientation.PortraitOrLandscape;
        }
        void twitter_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null)
                MessageBox.Show("This is not a valid twitter user my friend!");
            else
            {
                this.State["feed"] = e.Result;

                UpdateFeedList(e.Result);
            }
        }
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            WebClient twitter = new WebClient();
            twitter.DownloadStringCompleted += new DownloadStringCompletedEventHandler(twitter_DownloadStringCompleted);
            twitter.DownloadStringAsync(new Uri("http://search.twitter.com/search.atom?q=" + HttpUtility.UrlEncode(search_text.Text)));
        }
        private void UpdateFeedList(string feedXML)
        {
            // Load the feed into a SyndicationFeed instance.
            StringReader stringReader = new StringReader(feedXML);
            XmlReader xmlReader = XmlReader.Create(stringReader);
            SyndicationFeed feed = SyndicationFeed.Load(xmlReader);
            // In Windows Phone OS 7.1, WebClient events are raised on the same type of thread they were called upon. 
            // For example, if WebClient was run on a background thread, the event would be raised on the background thread. 
            // While WebClient can raise an event on the UI thread if called from the UI thread, a best practice is to always 
            // use the Dispatcher to update the UI. This keeps the UI thread free from heavy processing.
            IEnumerable<SyndicationItem> x = feed.Items;
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                // Bind the list of SyndicationItems to our ListBox.
            
                search_results.ItemsSource = from item in feed.Items
                                             select new RSSFeed
                                             {
                                                 Title = item.Title.Text,
                                                 NavURL = item.Links[1].Uri.AbsoluteUri,
                                                 date = item.PublishDate.DateTime
                                             };
            });
            
        }

        private void hyperlinkButton1_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }
        private void ApplicationBarIconButton_Click_1(object sender, EventArgs e)
        {
            RoutedEventArgs x = null;
            button1_Click(sender, x);
        }
    }
}