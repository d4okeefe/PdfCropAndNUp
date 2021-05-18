using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfCropAndNUp
{
    public class CenterCroppedPdfOnPage
    {
        public System.IO.MemoryStream NewMemoryStream { get; private set; }
        private PdfPageSizeEnum typeOfPdfPage;
        public float PageWidth
        {
            get
            {
                return typeOfPdfPage == PdfPageSizeEnum.Booklet ? 441f : 612f;
            }
        }
        public float PageHeight
        {
            get
            {
                return typeOfPdfPage == PdfPageSizeEnum.Booklet ? 666f : 792f;
            }
        }
        public CenterCroppedPdfOnPage(
            System.IO.MemoryStream orig_stream,
            PdfPageSizeEnum _typeOfPdfPage = PdfPageSizeEnum.Booklet)
        {
            try
            {
                typeOfPdfPage = _typeOfPdfPage;
                if (typeOfPdfPage != PdfPageSizeEnum.Booklet && typeOfPdfPage != PdfPageSizeEnum.Letter)
                { throw new Exception(); }

                using (var new_stream = new System.IO.MemoryStream())
                using (var reader = new iTextSharp.text.pdf.PdfReader(orig_stream.ToArray()))
                {
                    for (int i = 1; i <= reader.NumberOfPages; i++)
                    {
                        var box_size = reader.GetCropBox(i);
                        var height = box_size.Height;
                        var width = box_size.Width;

                        var left = box_size.Left;
                        var right = box_size.Right;
                        var top = box_size.Top;
                        var bottom = box_size.Bottom;

                        var new_left = left + ((width - PageWidth) / 2);
                        var new_bottom = bottom + ((height - PageHeight) / 2);
                        var new_right = right - ((width - PageWidth) / 2);
                        var new_top = top - ((height - PageHeight) / 2);

                        var rect = new iTextSharp.text.pdf.PdfRectangle(
                            new_left, new_bottom, new_right, new_top);

                        var pageDict = reader.GetPageN(i);
                        pageDict.Put(iTextSharp.text.pdf.PdfName.CROPBOX, rect);
                        pageDict.Put(iTextSharp.text.pdf.PdfName.MEDIABOX, rect);
                    }
                    var stamper = new iTextSharp.text.pdf.PdfStamper(reader, new_stream);
                    stamper.Close();
                    NewMemoryStream = new_stream;
                }
            }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine(ex); }
        }
    }
}
