internal class Program
{
    private static void Main(string[] args)
    {
         
      string path = "C:\\Users\\Pictures\\";
      Image<Bgr, byte> img = new Bitmap((path + "\\bgr.png")).ToImage<Bgr, byte>();
      Image<Bgr, byte> template = new Bitmap((path + "\\test.png")).ToImage<Bgr, byte>();
      findPlaceOnMap(img,template);
    }
    public static void findPlaceOnMap(Image<Bgr, byte> img, Image<Bgr, byte> template)
    {
       
        var result = new Mat();
        CvInvoke.MatchTemplate(img, template, result, TemplateMatchingType.Ccorr); //CcoeffNormed
    
        double minVal = Double.MaxValue;
        double maxVal = Double.MinValue;

        var minLoc = new Point();
        var maxLoc = new Point();
        var mask = new Mat();
        CvInvoke.MinMaxLoc(result, ref minVal, ref maxVal, ref minLoc, ref maxLoc, mask);


        var maxPoint = maxLoc;
        var color1 = new MCvScalar(0, 255, 0);
        var point1 = new Point(maxPoint.X + template.Cols, maxPoint.Y + template.Rows);
        CvInvoke.Rectangle(img, new Rectangle(point1, ((Size)maxLoc)), color1);
        CvInvoke.Resize(img, img, new Size(0, 0), 0.5, 0.5);
        CvInvoke.Imshow("Find on bgr", img);
    }
    public static void findBestPlaceOnMap(Image<Bgr, byte> img, List<Image<Bgr, byte>> templates)
    {
        List<double> minVals = new List<double>();
        List<double> maxVals = new List<double>();
        List<Point> minLocs = new List<Point>();
        List<Point> maxLocs = new List<Point>();    
        for (int i = 0; i < templates.Count; i++)
        {
            var result = new Mat();
            CvInvoke.MatchTemplate(img, templates[i], result, TemplateMatchingType.SqdiffNormed); //When using SqdiffNormed use lowest values [minVal,minLoc,Min()]
            double minVal = Double.MaxValue;
            double maxVal = Double.MinValue;
            var minLoc = new Point();
            var maxLoc = new Point();
            var mask = new Mat();
            CudaInvoke.MinMaxLoc(result, ref minVal, ref maxVal, ref minLoc, ref maxLoc, mask);
            minVals.Add(minVal);
            maxVals.Add(maxVal);
            minLocs.Add(minLoc);
            maxLocs.Add(maxLoc);
            //Console.WriteLine(i+"/360 "+":" +maxVal);
        }
        int index  = minVals.IndexOf(minVals.Min());
        var maxPoint = minLocs[index];
        var color1 = new MCvScalar(0, 255, 0);
        var point1 = new Point(maxPoint.X + templates[index].Cols, maxPoint.Y + templates[index].Rows);
        CvInvoke.Rectangle(img, new Rectangle(point1, ((Size)minLocs[index])), color1);
        CvInvoke.Imshow("Find tmp", img);
    }
}
