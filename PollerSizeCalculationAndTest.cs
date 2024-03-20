using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
internal class Program
{
    // In a poll rate of once in 30ms in 60 seconds you do 2000 polls
    // And in 10 minutes (2000 * 10) = 20 000 polls
    // with the chance of 33% of hitting the same value
    // Avg of 4,1GB/10min file size as text  -> as 7zip packed 135MB/10min
    // And with binary 6.5GB~ unzipped
    // The reduced amount is ~30 times smaller, meaning at optimal 
    // reduced package size would be 135*6*24 = 19440MB => ~20GB for one day....

    // You could probably add a method to create file every second ~30th loop cycle
    // And then add them to a Zip record to reduce the massive size.
    // Then you could retrive the zipped record as needed by their datetime second name attribute??? 
    // Using the c# *ZipArchive.CreateEntry()* to create an entry &
    // *ZipArchiveEntry tile = tiles.GetEntry(relativeTilePath);* to read specific entry or area of ntrys


    private static void Main(string[] args)
    {
        //Console.WriteLine("Hello, World!");
        testerClass testerClass = new testerClass();
        testerClass.populatewithgarbage();
    }

    public class SQL_Data_Holder
    {
        public string tag { get; set; }
        public string value { get; set; }
        public SQL_Data_Holder(string tag, string _value)
        {
            this.tag = tag;
            this.value = _value;
        }

    }
    public class testerClass
    {
        private List<SQL_Data_Holder> originalData = new List<SQL_Data_Holder>();
        public static string folder = "binary_data";
        public static string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, folder);
        public static string filePath = path + "\\binary_data.bin";

        public void populatewithgarbage()
        {
            Random random = new Random();
            Directory.CreateDirectory(path);
            Console.WriteLine(path);
            for (int i = 0; i < 7000; i++)
            {
                string x = RandomString(16);
                originalData.Add(new SQL_Data_Holder(x, random.Next(1, 6).ToString()));
                //Console.WriteLine(sQL_Data_Holders[i].value);
            }
            Console.WriteLine(originalData.Count);
            //Thread.Sleep(3000);
            int k = 0;
            int pollAmount = 20000;
            while (k < pollAmount)
            {
                List<SQL_Data_Holder> changedData = new List<SQL_Data_Holder>();
                for (int i = 0; i < originalData.Count; i++)
                {
                    var newValue = random.Next(1, 3).ToString();
                    if (originalData[i].value != newValue)
                    {
                        originalData[i].value = newValue;
                        changedData.Add(originalData[i]);
                        //Console.WriteLine(sQL_Data_Holders[i].value);
                    }
                }
                if (k == 0)
                    saveData(originalData);
                else
                    saveData(changedData);

                //Thread.Sleep(1);
                if (k % 100 == 0)
                    Console.WriteLine(((double)k / pollAmount) * 100 + "%");
                k++;
            }
        }
        public static string RandomString(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public void saveData(List<SQL_Data_Holder> data)
        {
            //Binary test
            // using (BinaryWriter writer = new BinaryWriter(File.Open(filePath, FileMode.Append)))
            // {
            //     BitConverter.DoubleToInt64Bits(double.MaxValue);
            //     int sCount = 0;
            //     for (int i = 0; i < data.Count; i++)
            //     {
            //         sCount += System.Text.ASCIIEncoding.ASCII.GetByteCount(data[i].tag);
            //         sCount += System.Text.ASCIIEncoding.ASCII.GetByteCount(data[i].value);
            //         sCount += System.Text.ASCIIEncoding.ASCII.GetByteCount(DateTime.UtcNow.ToString());
            //     }
            //     int totalByteSizeOfArray = sCount + data.Count() * 64;
            //     Console.WriteLine(totalByteSizeOfArray);
            //     byte[] a = new byte[totalByteSizeOfArray];
            //     writer.Write(a);
            //     writer.Flush();
            // }
            string[] dataArr = data.Select(x => "" + x.tag + "|" + x.value + "|" + double.MaxValue + "|" + DateTime.UtcNow).ToArray();
            File.AppendAllLines(filePath, dataArr);


        }

       
       // public void zipping()
       // {
       //     ZipArchive.CreateEntry();
       //     System.IO.Compression.ZipFile.CreateFromDirectory(filePath, Path.Combine(path, "" + DateTime.UtcNow));
       //     using (ZipArchive tiles = ZipFile.OpenRead(containerPath))
       //     {
       //         ZipArchiveEntry tile = tiles.GetEntry(relativeTilePath);
       //         Image tileImage = Image.FromStream(tile.Open());
       //     }
       // }
    }

}
