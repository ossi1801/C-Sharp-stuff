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
}
