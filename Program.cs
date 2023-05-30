using System.Drawing;
using OpenCvSharp;

namespace ASCII
{
  public class Program
  {
    public static void Main(string[] args)
    {
      string? path = null;
      if (args.Length != 0) path = args[0];
      
      try { new ASCII().ServeVideo(path); }
      catch (Exception e) { Console.WriteLine($"{e}\n\nUSAGE: dotnet run [ <path/to/image> ]"); }
    }
  }

  public class ASCII
  { 
    public int Width = Console.WindowWidth;
    public int Height = Console.WindowHeight;
    
    public string Density = "       .:-i|=+%O#@";
    public bool InverseDensity = true;

    public void ServeVideo(string? imagePath)
    { 
      Console.Clear();
      Console.ForegroundColor = ConsoleColor.Blue;

      int len = Density.Length-1;
      int b1 = 0;
      int b2 = len;
      if (InverseDensity)
      {
        b1 = len;
        b2 = 0;
      }

      VideoCapture capture;
      if (string.IsNullOrEmpty(imagePath))
        capture = new VideoCapture(0);
      else 
        capture = new VideoCapture(imagePath);

      Mat image = new Mat();
      while (true)
      { // capture image from video into buffer
        capture.Read(image);
        if (image.Empty()) break;

        // convert image to bitmap and resize
        var bmp = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(image);
        var scaledBmp = new Bitmap(Width, Height);
        using (Graphics g = Graphics.FromImage(scaledBmp))
        {
          g.DrawImage(bmp, 0, 0, Width, Height);
        }

        // loop through pixels in bitmap starting from top left corner
        for (int y = 0; y < Height; y++)
        {
          for (int x = 0; x < Width; x++)
          { // get average brightness of pixel
            var clr = scaledBmp.GetPixel(x, y);
            float avg = (clr.R+clr.G+clr.B)/3;

            // remap average (0-255) to ascii density (0-len or len-0)
            int index = (int) Math.Floor(map(avg, 0, 255, b1, b2));
            char c = Density[index];

            // update character on screen
            Console.SetCursorPosition(x, y); 
            Console.Write(c);
          }
        }
      }
    }

    // Re-maps a number from one range to another.
    // https://forum.unity.com/threads/re-map-a-number-from-one-range-to-another.119437/
    static float map(float s, float a1, float a2, float b1, float b2)
    {
      return b1 + (s-a1)*(b2-b1)/(a2-a1);
    }
  }
}
