using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Net;

namespace Timestamp_Test
{
    class Program
    {

        //
        //Make sure you have an available video file
        //
        const string VIDEOFILEPATH = "Act of Aggression.webm";
        //
        //Change this ^
        //

        static void WriteTimeStamps()
        {
            StreamWriter file = new StreamWriter("chat.txt");
            string TimeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            file.WriteLine(TimeStamp + " Sup");
            file.WriteLine(DateTime.Now.AddSeconds(4).ToString("yyyy-MM-dd HH:mm:ss.fff") + " Heya, this is a test chat messaged.");
            file.WriteLine(DateTime.Now.AddSeconds(8).ToString("yyyy-MM-dd HH:mm:ss.fff") + " Another test message;");
            file.WriteLine(DateTime.Now.AddSeconds(12).ToString("yyyy-MM-dd HH:mm:ss.fff") + " And another one!");
            file.Close();
            file = new StreamWriter("StreamStart.txt");
            file.WriteLine(TimeStamp);
            file.Close();
        }
        static void Main(string[] args)
        {
            WriteTimeStamps(); //Comment this out after running once
            StreamReader file = new StreamReader("StreamStart.txt");
            DateTime streamstart = Convert.ToDateTime(file.ReadLine());
            file.Close();
            file = new StreamReader("chat.txt");
            DateTime startofprogram = DateTime.Now;
            System.Diagnostics.Process.Start(@VIDEOFILEPATH);

            bool seekbackwards = false, endoffile = false;
            TimeSpan lastposition = new TimeSpan(0);
            string Line = " ", Date;
            int index, dateindex;
            DateTime chatstamp;
            while (true)
            {
                if (!file.EndOfStream)
                {
                    Line = file.ReadLine();
                }
                else
                    endoffile = true;
                index = Line.IndexOf(' ');
                dateindex = Line.IndexOf(' ', index + 1);
                Date = Line.Substring(0, dateindex);
                chatstamp = Convert.ToDateTime(Date);
                while (true)
                {
                    //lol
                    WebRequest req = HttpWebRequest.Create("http://localhost:13579/variables.html");
                    req.Method = "GET";
                    string source;
                    using (StreamReader reader = new StreamReader(req.GetResponse().GetResponseStream()))
                    {
                        source = reader.ReadToEnd();
                    }
                    string lookfor = "<p id=\"position\">";
                    int lookforindex = source.IndexOf(lookfor) + lookfor.Length;
                    int endlookforindex = source.IndexOf('<', lookforindex);
                    string stringposition = source.Substring(lookforindex, endlookforindex - lookforindex);
                    TimeSpan currentposition = new TimeSpan(0, 0, 0, 0, Convert.ToInt32(stringposition));

                    if (currentposition > lastposition && seekbackwards)
                    {
                        Console.Clear();

                        file.BaseStream.Position = 0;
                        file.DiscardBufferedData();
                        seekbackwards = false;
                        endoffile = false;
                        break;
                    }

                    if (currentposition > chatstamp - streamstart && !endoffile)
                    {
                        Console.Write(Date + " ");
                        Console.WriteLine(Line.Substring(dateindex + 1));
                        break;
                    }
                    else if (currentposition < lastposition)
                    {
                        seekbackwards = true;
                    }
                    else
                    {
                        Thread.Sleep(20);
                    }
                    lastposition = currentposition;
                }
                
            }
            Console.ReadKey();
        }
    }
}
