using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfCropAndNUp
{
    public static class StaticUtils
    {
        public static int GetCoverLength_FirstPageTypesetPdf(System.IO.MemoryStream orig_stream)
        {
            try
            {
                //if(!System.IO.File.Exists(filename)) throw new Exception();
                using(var reader = new iTextSharp.text.pdf.PdfReader(orig_stream.ToArray()))
                {
                    var text_coords = new FourCoordinates();
                    text_coords.PageNumber = 1;

                    // get all text from page
                    var strat = new PdfTextCoordinatesStrategy();
                    var allTextFromPage =
                        iTextSharp.text.pdf.parser.PdfTextExtractor
                        .GetTextFromPage(reader, 1, strat);

                    // skip pages that contain no text
                    if(string.IsNullOrWhiteSpace(allTextFromPage)) throw new Exception("No text on page!");

                    // get extreme coordinates, skipping "TextAndSurroundingRectangle.Text" that is whitespace
                    text_coords.Left = strat.myPoints.Where(x => !string.IsNullOrWhiteSpace(x.Text))
                        .Select(x => x.Rectangle.Left).Min();
                    text_coords.Right = strat.myPoints.Where(x => !string.IsNullOrWhiteSpace(x.Text))
                        .Select(x => x.Rectangle.Right).Max();
                    text_coords.Bottom = strat.myPoints.Where(x => !string.IsNullOrWhiteSpace(x.Text))
                        .Select(x => x.Rectangle.Bottom).Min();
                    text_coords.Top = strat.myPoints.Where(x => !string.IsNullOrWhiteSpace(x.Text))
                        .Select(x => x.Rectangle.Top).Max();

                    // most common measures: 575f==48pi: 587f==49pi: 599f==50pi: 611f==51pi
                    // may run into problems with pro se, no cockle line covers
                    // each 
                    if(text_coords.Height == 0f || text_coords.Height < 569f || text_coords.Height > 616f)
                        throw new Exception("Problem finding text on page");
                    if(text_coords.Height <= 580) return 48; // range 569-580
                    else if(text_coords.Height <= 593) return 49; // range 581-593
                    else if(text_coords.Height <= 605) return 50; // range 594-605
                    else /*if (text_coords.Height <= 616)*/ return 51; // range 606-616
                }
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.Write(ex);
                throw new Exception("Problem collecting cover length");
            }
        }
        public static int GetCoverLength_FirstPageTypesetPdf(string filename)
        {
            try
            {
                if (!System.IO.File.Exists(filename)) throw new Exception();
                using (var reader = new iTextSharp.text.pdf.PdfReader(filename))
                {
                    var text_coords = new FourCoordinates();
                    text_coords.PageNumber = 1;

                    // get all text from page
                    var strat = new PdfTextCoordinatesStrategy();
                    var allTextFromPage =
                        iTextSharp.text.pdf.parser.PdfTextExtractor
                        .GetTextFromPage(reader, 1, strat);

                    // skip pages that contain no text
                    if (string.IsNullOrWhiteSpace(allTextFromPage)) throw new Exception("No text on page!");

                    // get extreme coordinates, skipping "TextAndSurroundingRectangle.Text" that is whitespace
                    text_coords.Left = strat.myPoints.Where(x => !string.IsNullOrWhiteSpace(x.Text))
                        .Select(x => x.Rectangle.Left).Min();
                    text_coords.Right = strat.myPoints.Where(x => !string.IsNullOrWhiteSpace(x.Text))
                        .Select(x => x.Rectangle.Right).Max();
                    text_coords.Bottom = strat.myPoints.Where(x => !string.IsNullOrWhiteSpace(x.Text))
                        .Select(x => x.Rectangle.Bottom).Min();
                    text_coords.Top = strat.myPoints.Where(x => !string.IsNullOrWhiteSpace(x.Text))
                        .Select(x => x.Rectangle.Top).Max();

                    // most common measures: 575f==48pi: 587f==49pi: 599f==50pi: 611f==51pi
                    // may run into problems with pro se, no cockle line covers
                    // each 
                    if (text_coords.Height == 0f || text_coords.Height < 569f || text_coords.Height > 616f)
                        throw new Exception("Problem finding text on page");
                    if (text_coords.Height <= 580) return 48; // range 569-580
                    else if (text_coords.Height <= 593) return 49; // range 581-593
                    else if (text_coords.Height <= 605) return 50; // range 594-605
                    else /*if (text_coords.Height <= 616)*/ return 51; // range 606-616
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex);
                throw new Exception("Problem collecting cover length");
            }
        }
        public static System.IO.MemoryStream SelectPages(
            System.IO.MemoryStream orig_stream, string page_range)
        {
            try
            {
                using (var new_stream = new System.IO.MemoryStream())
                using (var reader = new iTextSharp.text.pdf.PdfReader(orig_stream.ToArray()))
                {
                    reader.SelectPages(page_range);
                    var stamper = new iTextSharp.text.pdf.PdfStamper(reader, new_stream);
                    stamper.Close();
                    return new_stream;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        public static System.IO.MemoryStream RemoveFirstPage(System.IO.MemoryStream orig_stream)
        {
            try
            {
                using (var new_stream = new System.IO.MemoryStream())
                using (var reader = new iTextSharp.text.pdf.PdfReader(orig_stream.ToArray()))
                {
                    var pages = "2-" + reader.NumberOfPages;
                    reader.SelectPages(pages);
                    var stamper = new iTextSharp.text.pdf.PdfStamper(reader, new_stream);
                    stamper.Close();
                    return new_stream;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public static bool SaveStreamToFile(
            string new_file_name,
            System.IO.MemoryStream stream_to_convert)
        {
            try
            {
                using (var fs = new System.IO.FileStream(
                    new_file_name, System.IO.FileMode.Create, System.IO.FileAccess.Write))
                {
                    fs.Write(stream_to_convert.ToArray(), 0, stream_to_convert.ToArray().Length);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

            if (System.IO.File.Exists(new_file_name))
            {
                return true;
            }
            return false;
        }
        public static System.IO.MemoryStream AddBlankPagesUntilMod4Equals0(System.IO.MemoryStream orig_stream)
        {
            try
            {
                using (var new_stream = new System.IO.MemoryStream())
                using (var reader = new iTextSharp.text.pdf.PdfReader(orig_stream.ToArray()))
                using (var stamper = new iTextSharp.text.pdf.PdfStamper(reader, new_stream))
                {
                    while (reader.NumberOfPages % 4 != 0)
                    {
                        stamper.InsertPage(reader.NumberOfPages + 1, reader.GetPageSizeWithRotation(1));
                    }
                    return new_stream;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        private static System.IO.MemoryStream AddBlankPagesToEndOfFile(
            System.IO.MemoryStream stream, int numberPagesToAdd = 1)
        {
            try
            {
                using (var new_stream = new System.IO.MemoryStream())
                using (var reader = new iTextSharp.text.pdf.PdfReader(stream.ToArray()))
                using (var stamper = new iTextSharp.text.pdf.PdfStamper(reader, new_stream))
                {
                    for (int i = 0; i < numberPagesToAdd; i++)
                    {
                        stamper.InsertPage(reader.NumberOfPages + 1, reader.GetPageSizeWithRotation(1));
                    }
                    return new_stream;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        public static System.IO.MemoryStream ConvertFileToStream(string filename)
        {
            try
            {
                if (!System.IO.File.Exists(filename)) throw new Exception();
                using (var fs = new System.IO.FileStream(
                    filename, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                {
                    var ms = new System.IO.MemoryStream();
                    fs.CopyTo(ms);
                    return ms;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
