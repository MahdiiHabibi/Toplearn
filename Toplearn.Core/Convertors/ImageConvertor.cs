using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;

namespace Toplearn.Core.Convertors
{
    public class ImageConvertor
    {
        public void Image_resize(string input_Image_Path, string output_Image_Path, int new_Width)
        {
	        Bitmap imgToResize = new Bitmap(Image.FromFile(input_Image_Path));

			// Get the image current width
			int sourceWidth = imgToResize.Width;
			// Get the image current height
			int sourceHeight = imgToResize.Height;
			float nPercent = 0;
			float nPercentW = 0;
			float nPercentH = 0;
			// Calculate width and height with new desired size
			nPercentW = ((float)new_Width / (float)sourceWidth);
			nPercentH = ((float)new_Width / (float)sourceHeight);
			nPercent = Math.Min(nPercentW, nPercentH);
			// New Width and Height
			int destWidth = (int)(sourceWidth * nPercent);
			int destHeight = (int)(sourceHeight * nPercent);
			Bitmap b = new Bitmap(destWidth, destHeight);
			Graphics g = Graphics.FromImage((System.Drawing.Image)b);
			g.InterpolationMode = InterpolationMode.HighQualityBicubic;
			// Draw image with new width and height
			g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
			g.Dispose();
			

			// Save 
			var image = b as Image;
			image.Save(output_Image_Path);
			b.Dispose();
        }
    }
}
