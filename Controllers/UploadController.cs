using System.IO;
using System.Web.Mvc;
using System.Web;
using MdDLsampleCS;
using System;
using ImageFileUpload.Ocr;
using System.Drawing;
using System.Drawing.Imaging;

namespace ImageFileUpload.Controllers
{
    public class UploadController : Controller
    {
        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                var fileName = Path.GetFileName(file.FileName);
                var path = @"c:\temp\test\image\" + fileName;
                file.SaveAs(path);
                byte[] pngBytes = System.IO.File.ReadAllBytes(path);


                byte[] dibBytes = ConvertPngToDib(pngBytes);
                System.IO.File.WriteAllBytes(@"c:\temp\test\image\output.bmp", dibBytes);


                MdDLmainCS ocr = new MdDLmainCS();
                ocr.analizeByMemory(dibBytes);
                // ocr.Main(ocrParams);
            }

            return RedirectToAction("../");
        }

        public static byte[] ConvertPngToDib(byte[] pngBytes)
        {
            // PNGをImageオブジェクトに変換
            using (MemoryStream memoryStream = new MemoryStream(pngBytes))
            {
                using (Image pngImage = Image.FromStream(memoryStream))
                {
                    // DIB形式のImageオブジェクトを作成
                    Bitmap dibImage = new Bitmap(pngImage.Width, pngImage.Height, PixelFormat.Format24bppRgb);
                    using (Graphics graphics = Graphics.FromImage(dibImage))
                    {
                        // DIBにPNGを描画
                        graphics.DrawImage(pngImage, 0, 0);
                    }

                    // DIBをbyte配列に変換
                    using (MemoryStream dibMemoryStream = new MemoryStream())
                    {
                        dibImage.Save(dibMemoryStream, ImageFormat.Bmp);
                        return dibMemoryStream.ToArray();
                    }
                }
            }
        }
        


        public ActionResult Base64(string b64img)
        {
                var fileName = "b64.png";
                var path = @"c:\temp\test\image\" + fileName;
                 
                string base64String = b64img.Replace("data:image/png;base64,", "");
                byte[] imageBytes = Convert.FromBase64String(base64String);
                System.IO.File.WriteAllBytes(path, imageBytes);
                
                MdDLmainCS ocr = new MdDLmainCS();
                //string[] ocrParams = new string[] { path };
                ocr.analizeByImage(path);

            return RedirectToAction("../");
        }


        public ActionResult BitMap(string b64img)
        {

            string base64String = b64img.Replace("data:image/png;base64,", "");
            byte[] imageBytes = Convert.FromBase64String(base64String);
            var fileName = "b64.png";
            var path = @"c:\temp\test\image\" + fileName;
            System.IO.File.WriteAllBytes(path, imageBytes);

            byte[] dibBytes = ConvertPngToDib(imageBytes);

            MdDLmainCS ocr = new MdDLmainCS();
            ocr.analizeByMemory(dibBytes);


            return RedirectToAction("../");
        }

    }
}