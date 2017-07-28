using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.IO;

namespace GMS.Utils
{

    public class ImageUtil
    {
        public static void ResizeImage(string imagePath, string destFileName, string folderPath, int width, int height, string logoPath)
        {
            try
            {
                Bitmap image1 = null;
                // check if File exists
                if (!File.Exists(imagePath))
                    throw new Exception("File cannot be found. Path:" + imagePath);

                try
                {
                    // Retrieve the image.
                    image1 = new Bitmap(imagePath);
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                int origImageWidth = image1.Width;
                int origImageHeight = image1.Height;

                System.Drawing.Imaging.ImageFormat imgFormat = null;

                destFileName = destFileName.ToLower();
                FileInfo fileInfo = new FileInfo(imagePath);

                if (destFileName.EndsWith(".png"))
                {
                    imgFormat = System.Drawing.Imaging.ImageFormat.Png;
                }
                else if (destFileName.EndsWith(".bmp"))
                {
                    imgFormat = System.Drawing.Imaging.ImageFormat.Jpeg;
                    destFileName = destFileName.Replace(@".bmp", @".jpg");
                }
                else if (destFileName.EndsWith(".tiff"))
                {
                    imgFormat = System.Drawing.Imaging.ImageFormat.Jpeg;
                    destFileName = destFileName.Replace(@".tiff", @".jpg");
                }
                else if (destFileName.EndsWith(".jpg") || destFileName.EndsWith(".jpeg"))
                {
                    imgFormat = System.Drawing.Imaging.ImageFormat.Jpeg;
                }
                else if (destFileName.EndsWith(".gif"))
                {
                    imgFormat = System.Drawing.Imaging.ImageFormat.Gif;
                }
                else
                    throw new Exception("Unsupported image file type. Only bmp,png,jpg,tiff,gif supported. FileName:" + destFileName);

                int imageWidth = origImageWidth;
                int imageHeight = origImageHeight;
                string destPath = folderPath;

                Size newSize = new Size(imageWidth, imageHeight);

                Image newImage = resizeQuality(image1, height, width, logoPath);



                newImage.Save(string.Format(@"{0}", destPath + "\\" + destFileName), System.Drawing.Imaging.ImageFormat.Jpeg);//Format Jpeg -> png yapıldı kalite için!!

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static Image resizeQuality(Image imgPhoto, int Yukseklik, int width, string logoPath)
        {
            int sourceWidth = imgPhoto.Width;
            int sourceHeight = imgPhoto.Height;
            int destWidth = Yukseklik;
            int destHeight = sourceHeight * Yukseklik / imgPhoto.Width; //resmin bozulmaması için en boy ayarını veriyoruz.                    
            Bitmap bmPhoto = new Bitmap(destWidth, destHeight, PixelFormat.Format24bppRgb); //pixel formatı değiştirdim
            bmPhoto.SetResolution(imgPhoto.HorizontalResolution, imgPhoto.VerticalResolution);
            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic; // resmin kalitesini ayarlıyoruz. Burada InterpolationMode özelliklerini bulabilirsiniz.
            grPhoto.FillRectangle(Brushes.White, 0, 0, destWidth, destHeight);
            grPhoto.DrawImage(imgPhoto, new Rectangle(0, 0, destWidth, destHeight), new Rectangle(0, 0, sourceWidth, sourceHeight), GraphicsUnit.Pixel);
            // Watermark
            if (!string.IsNullOrEmpty(logoPath))
            {
                Bitmap logo = new Bitmap(logoPath);
                grPhoto.DrawImage(logo, 5, 5, (width / 5), (Yukseklik / 10));
            }
            // Watermark End
            grPhoto.Dispose();
            return bmPhoto;
        }
    }
}