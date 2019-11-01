using System;
using System.Text;
using System.IO;

namespace dataproto
{
    class Program
    {
        static void Main(string[] args)
        {
            string filename;
            Console.WriteLine("Starting data generation program");
            Console.WriteLine("How many devices to you want to generate data for? (1 - 20");
            int numdevices;
            if (!Int32.TryParse(Console.ReadLine(), out numdevices))
            {
                Console.WriteLine("That takes a special kind of stupid!");
                return;
            };
            Console.WriteLine("Enter the serial number prefix you want use");
            var prefix = Console.ReadLine();
            
            var rand = new Random();
            var basepath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            basepath = Path.Combine(Path.Combine(basepath, "temp"), "dataproto");
            DirectoryInfo di = new DirectoryInfo(basepath);
            if (!di.Exists)
            {
                di.Create();
                Console.WriteLine($"Creating basepath {basepath}");
            }

            var accountId = rand.Next();
            var sn = "";
            for (int idx = 0; idx < 24; idx++)
            {
                sn = $"{prefix}{idx.ToString("000")}";
                var snstr = sn.ToString();
                Console.WriteLine($"Sensor serial number: {snstr}");
                
                var sb = new StringBuilder();
                
                var timestamp = new DateTime(2017, 3, 15, 10, 17, 22);
                var min = (double)rand.Next(-30, 100);
                var max = Math.Round(min + 20 * rand.NextDouble(), 2);
                Console.WriteLine($"Sensor Serial Number: {snstr} Min: {min}  Max: {max}");
                sb.Append("[");
                var first = true;
                DateTime newtimestamp = DateTime.MinValue;
                while (timestamp < DateTime.Now)
                {
                    var data = Math.Round(min + (max - min) * rand.NextDouble(), 2);
                    var timestr = timestamp.ToString("yyyy-MM-dd HH:mm:ss");
                    var record = $"{{\"timestamp\": \"{timestr}\", \"data\": {data} }}";
                    Console.WriteLine(record);
                    if (!first)
                    {
                        sb.Append(",");
                    }
                    else
                    {
                        first = false;
                    }
                    sb.Append(record);

                    newtimestamp = timestamp.AddMinutes(10).AddSeconds( (int)(5 * rand.NextDouble() 
                                * ((rand.Next() % 2 == 0) ? 1 : -1)) );
                    if ((newtimestamp.Month != timestamp.Month) || newtimestamp >= DateTime.Now)
                    {
                        sb.Append("]");
                        filename = Path.Combine(basepath, $"{accountId}_{snstr}_{timestamp.Year}_{timestamp.Month:00}_data.json");
                        File.WriteAllText(filename, sb.ToString());
                        sb.Clear();
                        sb.Append("[");
                        first = true;
                    }
                    timestamp = newtimestamp;
                }                
            }
        }
    }
}
