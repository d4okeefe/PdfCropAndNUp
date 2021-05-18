using System;
using System.Collections.Generic;
using System.Linq;

namespace PdfCropAndNUp
{
    internal class TextCoordinateFinder
    {
        private System.IO.MemoryStream orig_stream;
        public System.IO.MemoryStream NewMemoryStream { get; private set; }
        public float CoverHeight { get; set; }
        public float BriefPageHeight { get; set; }
        public TextCoordinateFinder(System.IO.MemoryStream _orig_stream, bool hasCover = false)
        {
            orig_stream = _orig_stream;
            if (!hasCover)
            {
                NewMemoryStream = setNewCropBox_AllPagesEqual(orig_stream);
            }
            else
            {
                NewMemoryStream = setNewCropBox_CropCoverSeparately(orig_stream);
            }
        }

        private System.IO.MemoryStream setNewCropBox_CropCoverSeparately(System.IO.MemoryStream orig_stream)
        {
            try
            {
                System.IO.MemoryStream new_stream = null;
                using (new_stream = new System.IO.MemoryStream())
                using (var reader = new iTextSharp.text.pdf.PdfReader(orig_stream.ToArray()))
                {

                    // get extreme coords for each page: extreme coords & page number
                    var furthest_text_coords_each_page = getFurthestTextCoords_NoWhiteSpace(reader);

                    if (reader.NumberOfPages == 1)
                    {
                        var cover_margins = furthest_text_coords_each_page
                             .Where(x => x.PageNumber == 1).FirstOrDefault();
                        // set coords, first page
                        var cv_pg_dict = reader.GetPageN(1);
                        var new_crop_box_page_1 = new iTextSharp.text.pdf.PdfRectangle
                            (cover_margins.Left,
                            cover_margins.Bottom,
                            cover_margins.Right,
                            cover_margins.Top);
                        cv_pg_dict.Put(iTextSharp.text.pdf.PdfName.CROPBOX, new_crop_box_page_1);
                        cv_pg_dict.Put(iTextSharp.text.pdf.PdfName.MEDIABOX, new_crop_box_page_1);
                        var stamper = new iTextSharp.text.pdf.PdfStamper(reader, new_stream);
                        stamper.Close();
                        return new_stream;
                    }
                    else if (reader.NumberOfPages > 1)
                    {
                        // if cover text coords are less than extreme for rest
                        // then use extreme for rest for cover

                        // get coords, first page
                        var cover_margins = furthest_text_coords_each_page
                             .Where(x => x.PageNumber == 1).FirstOrDefault();
                        CoverHeight = cover_margins.Top - cover_margins.Bottom;

                        // get coords, page 2 to end
                        var brief_margins = new FourCoordinates
                        {
                            Left = furthest_text_coords_each_page
                                .Where(x => x.PageNumber != 1).Select(x => x.Left).Min(),
                            Right = furthest_text_coords_each_page
                                .Where(x => x.PageNumber != 1).Select(x => x.Right).Max(),
                            Bottom = furthest_text_coords_each_page
                                .Where(x => x.PageNumber != 1).Select(x => x.Bottom).Min(),
                            Top = furthest_text_coords_each_page
                                .Where(x => x.PageNumber != 1).Select(x => x.Top).Max()
                        };
                        BriefPageHeight = brief_margins.Top - brief_margins.Bottom;

                        var difference = new FourCoordinates
                        {
                            Left = brief_margins.Left - cover_margins.Left,
                            Bottom = brief_margins.Bottom - cover_margins.Bottom,
                            Right = cover_margins.Right - brief_margins.Right,
                            Top = cover_margins.Top - brief_margins.Top
                        };
                        var cover_enlarged = difference.Left >= 0 || difference.Bottom >= 0
                            || difference.Right >= 0 || difference.Top >= 0;

                        if (!cover_enlarged) 
                        {
                            // use maxes for cover for all pages
                            for (var i = 1; i <= reader.NumberOfPages; i++)
                            {
                                var pageDict2toEnd = reader.GetPageN(i);
                                var new_crop_box_1_to_end = new iTextSharp.text.pdf.PdfRectangle
                                    (brief_margins.Left,
                                    brief_margins.Bottom,
                                    brief_margins.Right,
                                    brief_margins.Top);

                                pageDict2toEnd.Put(iTextSharp.text.pdf.PdfName.CROPBOX, new_crop_box_1_to_end);
                                pageDict2toEnd.Put(iTextSharp.text.pdf.PdfName.MEDIABOX, new_crop_box_1_to_end);
                            }
                        }
                        else
                        {
                            // set cover crop separately
                            var pageDictPage1 = reader.GetPageN(1);
                            var new_crop_box_page_1 = new iTextSharp.text.pdf.PdfRectangle
                                (cover_margins.Left,
                                cover_margins.Bottom,
                                cover_margins.Right,
                                cover_margins.Top);
                            pageDictPage1.Put(iTextSharp.text.pdf.PdfName.CROPBOX, new_crop_box_page_1);
                            pageDictPage1.Put(iTextSharp.text.pdf.PdfName.MEDIABOX, new_crop_box_page_1);

                            for (var i = 2; i <= reader.NumberOfPages; i++)
                            {
                                var pageDict2toEnd = reader.GetPageN(i);
                                var new_crop_box_2_to_end = new iTextSharp.text.pdf.PdfRectangle
                                    (brief_margins.Left,
                                    brief_margins.Bottom,
                                    brief_margins.Right,
                                    brief_margins.Top);

                                pageDict2toEnd.Put(iTextSharp.text.pdf.PdfName.CROPBOX, new_crop_box_2_to_end);
                                pageDict2toEnd.Put(iTextSharp.text.pdf.PdfName.MEDIABOX, new_crop_box_2_to_end);
                            }
                        }
                        var stamper = new iTextSharp.text.pdf.PdfStamper(reader, new_stream);
                        stamper.Close();
                    }
                    return new_stream;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return null;
            }
        }

        private System.IO.MemoryStream setNewCropBox_AllPagesEqual(System.IO.MemoryStream orig_stream)
        {
            try
            {
                System.IO.MemoryStream new_stream = null;
                using (new_stream = new System.IO.MemoryStream())
                using (var reader = new iTextSharp.text.pdf.PdfReader(orig_stream.ToArray()))
                {
                    // get extreme coords for each page: extreme coords & page number
                    var furthest_text_coords_each_page = getFurthestTextCoords_NoWhiteSpace(reader);

                    var furthest_text_coords_entire_doc = new FourCoordinates();
                    furthest_text_coords_entire_doc.Left = furthest_text_coords_each_page.Select(x => x.Left).Min();
                    furthest_text_coords_entire_doc.Right = furthest_text_coords_each_page.Select(x => x.Right).Max();
                    furthest_text_coords_entire_doc.Bottom = furthest_text_coords_each_page.Select(x => x.Bottom).Min();
                    furthest_text_coords_entire_doc.Top = furthest_text_coords_each_page.Select(x => x.Top).Max();

                    // first try, simply use extreme coords
                    for (var i = 1; i <= reader.NumberOfPages; i++)
                    {
                        var pageDict = reader.GetPageN(i);
                        var new_crop_box = new iTextSharp.text.pdf.PdfRectangle
                            (
                            furthest_text_coords_entire_doc.Left,
                            furthest_text_coords_entire_doc.Bottom,
                            furthest_text_coords_entire_doc.Right,
                            furthest_text_coords_entire_doc.Top
                            );

                        pageDict.Put(iTextSharp.text.pdf.PdfName.CROPBOX, new_crop_box);
                        pageDict.Put(iTextSharp.text.pdf.PdfName.MEDIABOX, new_crop_box);
                    }
                    var stamper = new iTextSharp.text.pdf.PdfStamper(reader, new_stream);
                    stamper.Close();
                }
                return new_stream;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return null;
            }
        }

        private List<FourCoordinates> getFurthestTextCoords_NoWhiteSpace(
            iTextSharp.text.pdf.PdfReader reader)
        {
            try
            {
                var list_of_doc_crop_boxes = new List<FourCoordinates>();

                // get coordinates of each page (i.e., the extreme coordinates)
                for (int i = 1; i <= reader.NumberOfPages; i++)
                {
                    // check that all pages are same size ???
                    var page_size = reader.GetPageSize(i);

                    var text_coords = new FourCoordinates();
                    text_coords.PageNumber = i;

                    // get all text from page
                    var strat = new PdfTextCoordinatesStrategy();
                    var allTextFromPage =
                        iTextSharp.text.pdf.parser.PdfTextExtractor
                        .GetTextFromPage(reader, i, strat);

                    // skip pages that contain no text
                    if (string.IsNullOrWhiteSpace(allTextFromPage)) continue;

                    // get extreme coordinates, skipping "TextAndSurroundingRectangle.Text" that is whitespace
                    text_coords.Left = strat.myPoints.Where(x => !string.IsNullOrWhiteSpace(x.Text))
                        .Select(x => x.Rectangle.Left).Min();
                    text_coords.Right = strat.myPoints.Where(x => !string.IsNullOrWhiteSpace(x.Text))
                        .Select(x => x.Rectangle.Right).Max();
                    text_coords.Bottom = strat.myPoints.Where(x => !string.IsNullOrWhiteSpace(x.Text))
                        .Select(x => x.Rectangle.Bottom).Min();
                    text_coords.Top = strat.myPoints.Where(x => !string.IsNullOrWhiteSpace(x.Text))
                        .Select(x => x.Rectangle.Top).Max();

                    // add points identified to list_of_doc_crop_boxes
                    list_of_doc_crop_boxes.Add(text_coords);
                }
                return list_of_doc_crop_boxes;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return null;
            }
        }
    }
}