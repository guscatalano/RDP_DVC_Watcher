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
                KS.EnableProvider("ca341b3c-b9d2-4d0f-9bd3-d88183596db9");
                KS.EnableProvider("8a633d91-8b07-4aae-9a00-d07e2afd29d6");
                KS.EnableProvider("f115ddaf-e07e-4b15-9721-427134b41eba");
                KS.EnableProvider("d953b8d8-7ea7-44b1-9ef5-c34af653329d");


                KS.EnableProvider("DAA6CAF5-6678-43f8-A6FE-B40EE096E06E");
                KS.EnableProvider("ea605ac7-d9de-434a-8271-682fee6d59ca");
                KS.EnableProvider("43471865-f3ee-5dcf-bf8b-193fcbbe0f37");
                KS.EnableProvider("080656c3-c24f-4660-8f5a-ce83656b0e7c");


                KS.EnableProvider("a8f457b8-a2b8-56cc-f3f5-3c00430937bb");
                KS.EnableProvider("7756e5a6-21b2-4c40-855e-88cf2b13c7cb");
                KS.EnableProvider("76de1e7b-74d5-575e-1f81-4ffe6a42777b");
                KS.EnableProvider("8a633d91-8b07-4aae-9a00-d07e2afd29d6");

                KS.EnableProvider("48ef6c18-022b-4394-bee5-7b822b42ae4c");
                KS.EnableProvider("335934aa-6dd9-486c-88a5-f8d6a7d2baef");
                KS.EnableProvider("ca341b3c-b9d2-4d0f-9bd3-d88183596db9");
                KS.EnableProvider("eb6594d8-6fad-53f7-350e-f4e4c531f68c");
                //KS.EnableProvider("8a633d91-8b07-4aae-9a00-d07e2afd29d6");


                Console.CancelKeyPress += delegate (object s, ConsoleCancelEventArgs e) { KS.Dispose(); };
                
                
                

                KS.Source.Dynamic.All += Dynamic_All;
                Console.WriteLine("Started");
                Thread x = new Thread(PrintEverySoOften);
                x.Start();

                KS.Source.Process();
                x.Abort();
                Console.WriteLine("stopped?");
            }
        }

        public static void PrintEverySoOften()
        {
            while (true)
            {
                Console.WriteLine("Doing the loop");
                foreach (var item in ChannelToTraffic)
                {
                    Console.WriteLine("Channel: " + item.Key + " == " + item.Value);
                }

                foreach (var item in DVChannelToTraffic)
                {
                    Console.WriteLine("Channel: " + item.Key + " == " + item.Value);
                }
                Thread.Sleep(5 * 1000);
            }
        }

        public static Dictionary<string, int> ChannelToTraffic = new Dictionary<string, int>();
        public static Dictionary<string, int> DVChannelToTraffic = new Dictionary<string, int>();

        private static void Dynamic_All(Microsoft.Diagnostics.Tracing.TraceEvent obj)
        {
            if (obj.ToString().ToLower().Contains("inputcore"))
            {
                return;
            }
            if (obj.ToString().ToLower().Contains("shell"))
            {
                return;
            }
            if (obj.ToString().ToLower().Contains("asynchronous"))
            {
                return;
            }

            if (obj.ToString().ToLower().Contains("network"))
            {
                return;
            }

            if (obj.ToString().ToLower().Contains("windows-schannel"))
            {
                return;
            }

            if (obj.ToString().ToLower().Contains("serviceusageaudit"))
            {
                return;
            }

            if (obj.ToString().ToLower().Contains("audio.client"))
            {
                return;
            }
            if (obj.ToString().ToLower().Contains("stat"))
            {
                Console.WriteLine(obj.ToString());
            }

                //Microsoft.Windows.Audio.Client
                //ServiceUsageAudit
                if (obj.ToString().ToLower().Contains("serverbase"))
            {
                //maybe WriteChannelData
                if (obj.ToString().Contains("VCMgr") || obj.ToString().Contains("DVC incoming data") || obj.ToString().ToLower().Contains("firing write flush"))
                {
                    
                    string line = obj.ToString();
                    if (line.Contains("DVC incoming data"))
                    {
                        string channelIDString = "ChannelId=\"";
                        string sizeString = "Size=\"";
                        string channel = line.Substring(line.IndexOf(channelIDString)+ channelIDString.Length);
                        channel = channel.Substring(0, channel.IndexOf("\""));
                        string sizeDVC = line.Substring(line.IndexOf(sizeString) + sizeString.Length);
                        sizeDVC = sizeDVC.Substring(0, sizeDVC.IndexOf("\""));
                        int channelTrafficSize = Int32.Parse(sizeDVC);
                        if (ChannelToTraffic.ContainsKey(channel))
                        {

                            ChannelToTraffic[channel] += channelTrafficSize;
                        }
                        else
                        {
                            ChannelToTraffic.Add(channel, channelTrafficSize);
                        }
                    }

                    if (line.ToLower().Contains("firing write flush"))
                    {
                        //Console.WriteLine(line);
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

            //VCMgr
            //DVC incoming data
        }
    }
}
