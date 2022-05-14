using System;
using System.IO;
using System.Drawing;
using static Sai2_Scatter_Brush_Helper.StaticSettingsS2SBH;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using KGySoft.Drawing.Imaging;
using KGySoft.Drawing;
using System.Security.Permissions;

namespace Sai2_Scatter_Brush_Helper
{
    class Program
    {
        static void Main(string[] args)
        {

            string[] fakeArgs = { @".\Resources\TestImage.png" };

            if (args.Length == 0)
            {
                Console.WriteLine("No files given. Drag bmp brush files into app to go");
                return;
            }

            StaticSettingsS2SBH.LoadSettings();

            foreach (var item in args)
            {
                //Get file name
                FileInfo imageFileInfo = new FileInfo(item);
                String imageFileBaseName = Path.GetFileNameWithoutExtension(item);
                String newBrushIniFileLocation = StaticSettingsS2SBH.Sai2ScatterBrushFolderLocation + @"\" + imageFileBaseName + ".ini";
                String newBrushBmpFileLocation = StaticSettingsS2SBH.Sai2ScatterBrushFolderLocation + @"\" + imageFileBaseName + ".bmp";

                //Check if ini version exists already
                //We don't want to overwrite inis, we DO want to overwrite the actual files
                if (!File.Exists(newBrushIniFileLocation))
                {
                    //If ini version doesn't exist, clone the base
                    File.Copy(StaticSettingsS2SBH.BaseINIFileLocation, newBrushIniFileLocation);
                }

                if (imageFileInfo.Extension.ToLower() == ".bmp")
                {
                    //Needs admin privs if set to TRUE
                    File.Copy(imageFileInfo.FullName, newBrushBmpFileLocation/*, true*/);
                }
                else
                {
                    //Convert image to bmp
                    Bitmap bmp = ConvertToBitmap(imageFileInfo.FullName);
                    try
                    {
                        //KGySoftConvert(bmp, newBrushBmpFileLocationCopyWorkaround);
                        //File.Copy(newBrushBmpFileLocationCopyWorkaround, newBrushBmpFileLocation, true);

                        KGySoftConvert(bmp, newBrushBmpFileLocation);
                        Console.WriteLine("Created brush: [" + newBrushBmpFileLocation + "]");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Couldn't write to directory. Check the folder permissions. If you use Avast you may need to manually give the app exception permissions.");
                    }
                }
            }
            Console.ReadKey();
        }

        public static Bitmap ConvertToBitmap(string fileName)
        {
            Bitmap bitmap;
            using (Stream bmpStream = System.IO.File.Open(fileName, System.IO.FileMode.Open))
            {
                Image image = Image.FromStream(bmpStream);

                bitmap = new Bitmap(image);
            }
            return bitmap;
        }
        //Conversions constantly fuck up the color channels, Photoshop doesn't. Just use photoshop? This seems too hairy to bother figuring out in depth
        //Kgysoft solution was simplest and had the same reasults as other attempts
        private static void KGySoftConvert(Bitmap original, string destination)
        {
            //https://docs.kgysoft.net/drawing/?topic=html/M_KGySoft_Drawing_ImageExtensions_ConvertPixelFormat.htm
            using (Bitmap converted8Bpp = original.ConvertPixelFormat(PixelFormat.Format8bppIndexed,
                 OptimizedPaletteQuantizer.MedianCut()))
            {
                converted8Bpp.SaveAsBmp(destination);
            }

            /*Color[] palette =
            {
                Color.Black, Color.Gray, Color.White
            };
            using (Bitmap converted8Bpp = original.ConvertPixelFormat(PixelFormat.Format8bppIndexed,
            PredefinedColorsQuantizer.FromCustomPalette(palette, Color.Silver)))
            {
                converted8Bpp.SaveAsBmp(destination);
            }*/

            // Using an optimized palette without dithering

            /*var targetFormat = PixelFormat.Format8bppIndexed; // feel free to try other formats as well
            using (Bitmap bmpSrc = original)
            using (Bitmap bmpDst = new Bitmap(original.Width, original.Height, targetFormat))
            {
                using (IReadableBitmapData dataSrc = bmpSrc.GetReadableBitmapData())
                using (IWritableBitmapData dataDst = bmpDst.GetWritableBitmapData())
                {
                    for (int y = 0; y < dataSrc.Height; y++)
                    {
                        for (int x = 0; x < dataSrc.Width; x++)
                        {
                            // Please note that bmpDst.SetPixel would not work for indexed formats
                            // and even when it can be used it would be much slower.
                            dataDst.SetPixel(x, y, dataSrc.GetPixel(x, y));
                        }
                    }
                }
                bmpDst.SaveAsBmp(destination); // or saveAsGif/SaveAsTiff to preserve the indexed format
            }*/
        }
    }
}
