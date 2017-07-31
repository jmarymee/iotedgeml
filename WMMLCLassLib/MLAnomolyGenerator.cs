using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMMLCLassLib
{
    public class MLAnomolyGenerator
    {
        DateTime startDate = DateTime.Now.AddDays(-365);
        //double currentNormal = 15.5;
        double currentHigh = 30.2;
        double currentLow = 11.1;

        public List<AnomolyData> GenerateDeviceData(int DaysSincelastCheck, int machineID, int cycles)
        {
            List<AnomolyData> dList = new List<AnomolyData>();
            DateTime currentDate = startDate;

            for (int loop = 1; loop < cycles; loop++)
            {
                currentDate = currentDate.AddHours(1);
                int serviceDaysToAdd = Convert.ToInt32((currentDate - startDate).TotalDays);

                dList.Add(new AnomolyData() {
                    daysSinceLastCheck = DaysSincelastCheck+serviceDaysToAdd,
                    currentPull = Utilities.RandomNormal(currentLow, currentHigh, 1.0),
                    isNormal = true,
                    machineID = machineID,
                    soundLevel = Utilities.RandomNormal(1, 10, 1),
                    timeStamp = currentDate });
            }

            return dList;
        }
        
    }

    public class AnomolyData
    {
        //used to timestamp a freezer telemetry reading
        public DateTime timeStamp { get; set; }

        //MachineID
        public int machineID { get; set; }

        //How much current is the machine pulling?
        public double currentPull { get; set; }

        //Sound Level
        public int soundLevel { get; set; }

        //Days since last service
        public int daysSinceLastCheck { get; set; }

        //IsNormal?
        public bool isNormal { get; set; }
    }

    public static class Utilities
    {
        public static int RandomHighOfMedian(int low, int high)
        {
            Random r = new Random();
            //find avergae
            var average = (low + high) / 2;
            var highRandom = r.Next(average, high);
            return highRandom;
        }

        public static int RandomNormal(int low, int high, int deviation)
        {
            int average = (low + high) / 2;
            int lowNorm = average - deviation;
            int highNorm = average + deviation;

            Random r = new Random();
            int rNormal = r.Next(lowNorm, highNorm);
            //Console.WriteLine(rNormal);
            return rNormal;
        }

        public static Double RandomHighOfMedian(Double low, Double high)
        {
            Random r = new Random();
            //find average
            var decValLow = low - Math.Truncate(low);
            var intValLow = Convert.ToInt32(low - decValLow);

            var decValHigh = high - Math.Truncate(high);
            var intValHigh = Convert.ToInt32(high - decValHigh);

            int average = (intValLow + intValHigh) / 2;

            var highRandomDecimal = r.NextDouble();

            var randomInt = r.Next(average, intValHigh);

            Double finalRandom = randomInt + highRandomDecimal;
            return finalRandom;
        }

        public static Double RandomNormal(double low, double high, double deviation)
        {
            Random r = new Random();
            //find average
            var decValLow = low - Math.Truncate(low);
            var intValLow = Convert.ToInt32(low - decValLow);

            var decValHigh = high - Math.Truncate(high);
            var intValHigh = Convert.ToInt32(high - decValHigh);

            int average = (intValLow + intValHigh) / 2;

            var RandomDecimal = r.NextDouble();

            Double finalRandom = average + RandomDecimal;

            //Debug
            Console.WriteLine(finalRandom);

            return finalRandom;
        }
    }
}
