using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Timers;
using System.Web;
using System.Xml;

namespace MusicFeedPrototype
{
    class Program
    {
        public static HttpClient httpClient = new HttpClient();
        static void Main(string[] args)
        {
            Timer myTimer = new Timer(2000);
            myTimer.Elapsed += new ElapsedEventHandler(GetCurrentSongInfo);
            myTimer.AutoReset = true;
            myTimer.Enabled = true;
            Console.ReadLine();
        }
        static void UpdateFile(string fileContents)
        {
            File.WriteAllText(@"C:/Streaming/CurrentSong.txt", fileContents);
        }
        static void GetCurrentSongInfo(object source, ElapsedEventArgs e)
        {
            //Console.WriteLine("Hello World!");
            string statusURL = "http://127.0.0.1:8080/requests/status.xml";
            var authByteArray = Encoding.ASCII.GetBytes(":dummy");
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(authByteArray));
            var responseObj = httpClient.GetAsync(statusURL).Result;
            var responseContent = responseObj.Content.ReadAsStringAsync().Result;

            XmlDocument responseXml = new XmlDocument();
            responseXml.LoadXml(responseContent);

            XmlNodeList nodeList = responseXml.GetElementsByTagName("info");

            var trackTitle = "";
            var trackArtist = "";

            foreach (XmlElement node in nodeList)
            {
                if (node.GetAttribute("name") == "title")
                {
                    trackTitle = node.InnerText;
                }
                if (node.GetAttribute("name") == "artist")
                {
                    trackArtist = node.InnerText;
                }
            }
            string fullTrackName = "   Artist: " + HttpUtility.HtmlDecode(trackArtist) + "   Song: " + HttpUtility.HtmlDecode(trackTitle);
            Console.WriteLine(fullTrackName);
            UpdateFile(fullTrackName);
        }
    }
}
