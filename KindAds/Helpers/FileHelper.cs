using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Web;

namespace KindAds.Helpers
{
  public static class FileHelper
  {
    public static string GetFileExtension(string sfilename)
    {
      return sfilename.Substring(sfilename.IndexOf("."), (sfilename.Length - sfilename.IndexOf(".")));
    }
       
       

    public static byte[] ResizeImageSite(byte[] OriginalImage, int width, int height)
    {
      Stream OriginalStream = new MemoryStream (OriginalImage);
      Stream ResultStream= new MemoryStream();

      System.Drawing.Image imagex = System.Drawing.Image.FromStream(OriginalStream);
      int alto = imagex.Height;
      int ancho = imagex.Width;
      Decimal factor = 0.3003M;
      if (ancho > width || alto > height)
      {
        if (ancho > alto)
        {
          factor = (Decimal)width / ancho;
        }
        else
        {
          factor = (Decimal)height / alto;
        }
      }
      else { factor = 1; }

      System.Drawing.Image image__1 = null;
      using (image__1 = System.Drawing.Image.FromStream(OriginalStream))
      {
        int newWidth = Convert.ToInt32(image__1.Width * factor);
        int newHeight = Convert.ToInt32(image__1.Height * factor);
        Bitmap thumbnailBitmap = null;
        Graphics thumbnailGraph = null;
          using (thumbnailBitmap = new Bitmap(newWidth, newHeight))
          {
            using (thumbnailGraph = Graphics.FromImage(thumbnailBitmap))
            {
              thumbnailGraph.CompositingQuality = CompositingQuality.HighQuality;
              thumbnailGraph.SmoothingMode = SmoothingMode.HighQuality;
              thumbnailGraph.InterpolationMode = InterpolationMode.HighQualityBicubic;
              Rectangle imageRectangle = new Rectangle(0, 0, newWidth, newHeight);
              thumbnailGraph.DrawImage(image__1, imageRectangle);
              thumbnailBitmap.Save(ResultStream, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
          }
        
      }

      return ToByteArray(ResultStream);
    }

    public static Byte[] ToByteArray(this Stream stream)
    {
      stream.Position = 0;
      byte[] buffer = new byte[stream.Length];
      for (int totalBytesCopied = 0; totalBytesCopied < stream.Length;)
        totalBytesCopied += stream.Read(buffer, totalBytesCopied, Convert.ToInt32(stream.Length) - totalBytesCopied);
      return buffer;
    }


  }
}
