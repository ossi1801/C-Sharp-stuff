//Object recognition with Emgu.CV using GPU
using Emgu.CV;
using Emgu.CV.Util;
using Emgu.CV.Structure;
using Emgu.CV.Dnn;
using Emgu.CV.CvEnum;
using Emgu.CV.Cuda;
using System.Drawing;
using System.Runtime.InteropServices;

//Download Weights and config for YOLO in below link
//------
//https://pjreddie.com/darknet/yolo/
//------------
//https://www.emgu.com/wiki/files/4.5.1/document/html/219dd707-ca8f-374d-2ed4-f0e8dc612ace.htm
//https://www.emgu.com/wiki/index.php/Working_with_Images#Creating_image_from_Bitmap
//Requirements:
//  NuGet\Install-Package System.Drawing.Common -Version 7.0.0
//  Emgu.CV  - Version : 4.4.0.4099
//  Emgu.CV.Bitmap  - Version : 4.4.0.4099
//  Emgu.CV.runtime.windows.cuda  - Version : 4.4.0.4099, NOTE DO NOT HAVE 2 RUNTIMES (CPU & GPU) INSTALLED AT THE SAME TIME PROGRAM WILL CRASH OR DEFAULT TO CPU!
internal class Program
{

    private static void Main(string[] args)
    {

        if (CudaInvoke.HasCuda)
        {
            Console.WriteLine("CUDA ON");
        }
        else
            throw new Exception("CUDA IS NOT INSTALLED PROPERLY CHECK DRIVERS AND PACKAGES");

        var net = DnnInvoke.ReadNetFromDarknet(
            "\yolov3.cfg",
            "\yolov3.weights"
            );
        var classLabels = File.ReadAllLines("\coco.names");

        net.SetPreferableBackend(Emgu.CV.Dnn.Backend.Cuda);
        net.SetPreferableTarget(Emgu.CV.Dnn.Target.Cuda);

        List<Tuple<int, int, int>> ColorList = new List<Tuple<int, int, int>>();
        ColorList.Add(new Tuple<int, int, int>(0, 255, 0)); //Green
        ColorList.Add(new Tuple<int, int, int>(255, 0, 0)); //Red
        ColorList.Add(new Tuple<int, int, int>(0, 0, 255)); //Blue
        ColorList.Add(new Tuple<int, int, int>(255, 255, 255)); //White


        Console.WriteLine("Capturing...");
        Mat frame = new Mat();
        VectorOfMat output = new VectorOfMat();

        VectorOfRect boxes = new VectorOfRect();
        VectorOfFloat scores = new VectorOfFloat();
        VectorOfInt indices = new VectorOfInt();


        while (true)
        {
            var bmp = (Bitmap)ScreenCapture.CaptureDesktop(); //Cap screen
            Image<Bgr, byte> image = bmp.ToImage<Bgr, byte>().Resize(416, 416, Inter.Area);
            frame = image.Mat; //Grab mat from image

            boxes = new VectorOfRect();
            scores = new VectorOfFloat();
            indices = new VectorOfInt();

            image = frame.ToImage<Bgr, byte>(); // Cast resize "frame" back to image

            Mat input = DnnInvoke.BlobFromImage(image, 1 / 255.0, swapRB: true);

            net.SetInput(input);
            net.Forward(output, net.UnconnectedOutLayersNames);

            for (int i = 0; i < output.Size; i++)
            {
                Mat mat = output[i];
                float[,] data = (float[,])mat.GetData();
                for (int j = 0; j < data.GetLength(0); j++)
                {
                    List<float> row = Enumerable.Range(0, data.GetLength(1)).Select(x => data[j, x]).ToList(); //Get rows
                    List<float> rowScore = new List<float>(row); //Copy
                    rowScore.RemoveRange(0, 5); //Remove useless
                    var classId = rowScore.IndexOf(rowScore.Max());
                    var confidence = rowScore[classId];

                    if (confidence > 0.8f)
                    {
                        var centerX = (int)(row[0] * frame.Width);
                        var centerY = (int)(row[1] * frame.Height);
                        var boxWidth = (int)(row[2] * frame.Width);
                        var boxHeight = (int)(row[3] * frame.Height);

                        var x = (int)(centerX - boxWidth / 2);
                        var y = (int)(centerY - boxHeight / 2);

                        boxes.Push(new Rectangle[] { new Rectangle(x, y, boxWidth, boxHeight) });
                        indices.Push(new int[] { classId });
                        scores.Push(new float[] { confidence });

                    }
                }

            }

            var bestIndex = DnnInvoke.NMSBoxes(boxes.ToArray(), scores.ToArray(), .8f, .8f);
            var frameOut = frame.ToImage<Bgr, byte>();
            for (int i = 0; i < bestIndex.Length; i++)
            {
                int index = bestIndex[i];
                var box = boxes[index];
                Tuple<int, int, int> color = new Tuple<int, int, int>(255, 255, 255);
                if (i < 3)
                {
                    color = ColorList[i];
                }
                CvInvoke.Rectangle(frameOut, box, new MCvScalar(color.Item1, color.Item2, color.Item3), 2);
                CvInvoke.PutText(frameOut, classLabels[indices[index]], new Point(box.X, box.Y + 20), //Offset 20 px text so not on border
                    FontFace.HersheyTriplex, 0.5, new MCvScalar(color.Item1, color.Item2, color.Item3), 1);
                Console.WriteLine(classLabels[indices[index]] + i); // Label

            }
            CvInvoke.Resize(frameOut, frameOut, new System.Drawing.Size(0, 0), 1.777, 1);
            CvInvoke.Imshow("output", frameOut);
            if (CvInvoke.WaitKey(1000) == 27)
                break;

        }

        Console.WriteLine("End!");

    }

    public class ScreenCapture
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetDesktopWindow();

        [StructLayout(LayoutKind.Sequential)]
        private struct Rect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetWindowRect(IntPtr hWnd, ref Rect rect);

        public static System.Drawing.Image CaptureDesktop()
        {
            return CaptureWindow(GetDesktopWindow());
        }

        public static Bitmap CaptureActiveWindow()
        {
            return CaptureWindow(GetForegroundWindow());
        }

        public static Bitmap CaptureWindow(IntPtr handle)
        {
            var rect = new Rect();
            GetWindowRect(handle, ref rect);
            var bounds = new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);
            var result = new Bitmap(bounds.Width, bounds.Height);

            using (var graphics = Graphics.FromImage(result))
            {
                graphics.CopyFromScreen(new Point(bounds.Left, bounds.Top), Point.Empty, bounds.Size);
            }

            return result;
        }
    }

}
