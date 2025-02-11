using Microsoft.Diagnostics.Tracing.Parsers;
using Microsoft.Diagnostics.Tracing.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;


namespace RDP_DVC_Watcher
{
    public class ETWListener
    {
        public static void TestListen()
        {
          

            using (var KS = new TraceEventSession("DVCWatcher"))
            {
                KS.EnableProvider("8375996d-5801-4fe9-b0ae-f5c428758960");

                Console.CancelKeyPress += (s, e) => KS.Dispose();

                KS.Source.Dynamic.All += Dynamic_All;
                Console.WriteLine("Started listening");
                Thread x = new Thread(PrintEverySoOften);
                x.Start();

                KS.Source.Process();
                
                Console.WriteLine("stopped");
            }
        }

        public static void PrintEverySoOften()
        {
            int top = Console.CursorTop;
            while (true)
            {
                Console.SetCursorPosition(0, top);
                Console.WriteLine("Doing the loop: " + DateTime.Now.ToString());


                foreach (var item in DVChannelToTraffic)
                {
                    Console.WriteLine("Channel: " + item.Key + " == " + item.Value);
                }
                Thread.Sleep(1 * 1000);
            }
        }

        public static Dictionary<string, int> DVChannelToTraffic = new Dictionary<string, int>();

        private static void Dynamic_All(Microsoft.Diagnostics.Tracing.TraceEvent obj)
        {


            string line = obj.ToString();
            string toLowerLine = line.ToLower();
            if (toLowerLine.Contains("serverbase"))
            {

                if (toLowerLine.Contains("firing write flush"))
                {
    
                    string channelIDString = " Name=\"";
                    string sizeString = "Size=\"";
                    string channel = line.Substring(line.IndexOf(channelIDString) + channelIDString.Length);
                    channel = channel.Substring(0, channel.IndexOf("\""));
                    string sizeDVC = line.Substring(line.IndexOf(sizeString) + sizeString.Length);
                    sizeDVC = sizeDVC.Substring(0, sizeDVC.IndexOf("\""));
                    sizeDVC =sizeDVC.Replace(",", "");
                    int channelTrafficSize = Int32.Parse(sizeDVC);
                    if (DVChannelToTraffic.ContainsKey(channel))
                    {

                        DVChannelToTraffic[channel] += channelTrafficSize;
                    }
                    else
                    {
                        DVChannelToTraffic.Add(channel, channelTrafficSize);
                    }
                }
                
            }

          
        }
    }
}
