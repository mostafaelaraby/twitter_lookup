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
using Coding4Fun.Phone.Controls;

namespace twitter_lookup
{
    
    public partial class MainPage : PhoneApplicationPage
    {
        int number_tweeets=-1;
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            SupportedOrientations = SupportedPageOrientation.PortraitOrLandscape;
        }
        void twitter_DownloadStringCompleted(object sender,DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null)
                MessageBox.Show("This is not a valid twitter user my friend!");
            else
            {
                XElement xmltweets = XElement.Parse(e.Result);
                results.ItemsSource = from tweet in xmltweets.Descendants("status")
                                      select new TwitterItem
                                      {
                                          ImageSource = tweet.Element("user").Element("profile_image_url").Value,
                                          Message = tweet.Element("text").Value,
                                          UserName = tweet.Element("user").Element("screen_name").Value
                                      };
            }
        }
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            WebClient twitter = new WebClient();
            twitter.DownloadStringCompleted += new DownloadStringCompletedEventHandler(twitter_DownloadStringCompleted);
            if(number_tweeets==-1)
            twitter.DownloadStringAsync(new Uri("http://api.twitter.com/1/statuses/user_timeline.xml?screen_name=" + username.Text + "&include_entities=true&include_rts=true&count=50"));
            else
                twitter.DownloadStringAsync(new Uri("http://api.twitter.com/1/statuses/user_timeline.xml?screen_name=" + username.Text + "&include_entities=true&include_rts=true&count="+number_tweeets));
        }

        private void hyperlinkButton1_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/search.xaml", UriKind.Relative));
        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            var input = new InputPrompt();
           
    input.Title = "# of Tweets";
    input.Message = "Specify Number of tweets to be shown \n for user look up ;)";
    input.InputScope = new InputScope { Names = { new InputScopeName() { NameValue = InputScopeNameValue.TelephoneNumber } } };
    input.Show();
    input.Completed += number_tweets;
        }
        private void number_tweets(object sender, PopUpEventArgs<string, PopUpResult> e)
        {
            string result = e.Result;
            try
            {
                if (Int32.Parse(result) <= 0)
                {
                    MessageBox.Show("Please enter a positive number :)");
                    ApplicationBarIconButton_Click(this, e);
                }
                else
                    number_tweeets = Int32.Parse(result);
            }
            catch (FormatException)
            {
                MessageBox.Show("Please enter a valid number");
                ApplicationBarIconButton_Click(this,e);
            }
        }

        private void ApplicationBarIconButton_Click_1(object sender, EventArgs e)
        {
            RoutedEventArgs x=null;
            button1_Click(sender, x);
        }

    }
}