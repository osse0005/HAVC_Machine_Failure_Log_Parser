using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;

namespace csvParser
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            //Console.WriteLine("Hello World!");

            var parser = new Parser();

            string deviceID = "dev1";
            StreamReader reader = new StreamReader("machine1DataLog.csv");
            parser.ParseEvents(deviceID, reader);

			int eventCount = parser.GetEventCount("Testing");
            Console.WriteLine("Event Count =" +eventCount);
		}
    }

    public class Parser : IEventCounter
    {
        int failureEventCount = 0;

        List<KeyValuePair<string,int>> list = new List<KeyValuePair<string, int>>();

        public int GetEventCount(string deviceID)
        {
            // Console.WriteLine("GetEventCount Hit");
            // return 0;
            string dateFormat = "yyyy-MM-dd H:mm:ss";
            CultureInfo provider = CultureInfo.InvariantCulture;

            bool eventStart = false;
            bool eventTrigger = false;
            DateTime eventStartTime = new DateTime();

			foreach (KeyValuePair<string, int> pair in list)
			{
                Console.WriteLine("KeyTimeToConvert:"+ pair.Key+"Result:");
                DateTime logTime = DateTime.ParseExact(pair.Key, dateFormat, provider);

                if(pair.Value == 3 && eventStart != true){
                    eventStart = true;
                    eventStartTime = logTime;
                }
                //Fail case trigger condition met
                if(pair.Value == 2 && eventStart == true && eventTrigger != true){
                    TimeSpan timeSinceEventStart = logTime - eventStartTime;
                    var fiveMinutes = new TimeSpan(0,5,0);
                    //
                    if (timeSinceEventStart >= fiveMinutes){
                        eventTrigger = true;
                    }
                }
                //Machine failure conditions complete
                if (pair.Value == 0 && eventStart == true && eventTrigger == true){
                    failureEventCount++;
                }
                else if (pair.Value == 0 && eventStart == true && eventTrigger == false){
                    eventStart = false;
                    eventTrigger = false;
                    eventStartTime = new DateTime();
                    failureEventCount = 0;
                }
			}


            return failureEventCount;
        }

        public void ParseEvents(string deviceID, StreamReader eventLog)
        {
            Console.WriteLine("ParseEventHit");
            string currentLine;
            while((currentLine = eventLog.ReadLine()) != null){
                Console.WriteLine(currentLine);

                string[] record = currentLine.Split((char)9);//ASCII equiv '\t'
                string time = record[0];
                int state;
                if ( Int32.TryParse(record[1],out state)){}
                list.Add(new KeyValuePair<string, int>(time,state));
            }

			// Console.WriteLine("Content from list");
			//Validate data load
			/*  foreach (KeyValuePair<string,int> pair in list)
			  {
				  Console.WriteLine("Time:"+ pair.Key);
				  Console.WriteLine("Value:" + pair.Value);
			  }  */

			string dateFormat = "yyyy-MM-dd H:mm:ss";
			CultureInfo provider = CultureInfo.InvariantCulture;

            DateTime time1 = DateTime.ParseExact("2011-03-07 12:00:00", dateFormat, provider);
            DateTime time2 = DateTime.ParseExact("2011-03-07 12:03:27", dateFormat, provider);

            TimeSpan difference = time2 - time1;
            Console.WriteLine("TimeDif:" + difference.Minutes);

			var fiveMinutes = new TimeSpan(0, 5, 0);
			Console.WriteLine("FiveMinutes:" + fiveMinutes);
        }
    }
}
