using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfCropAndNUp
{
    public class ReorderSaddleStitchPages
    {
        System.IO.MemoryStream orig_stream;
        public System.IO.MemoryStream NewMemoryStream { get; set; }
        public ReorderSaddleStitchPages(System.IO.MemoryStream _orig_stream)
        {
            orig_stream = _orig_stream;
            try
            {
                // test that pdf is divisible by 4
                System.IO.MemoryStream temp_stream = null;
                using (var reader = new iTextSharp.text.pdf.PdfReader(orig_stream.ToArray()))
                {
                    if (reader.NumberOfPages % 4 != 0)
                    {
                        temp_stream = StaticUtils.AddBlankPagesUntilMod4Equals0(orig_stream);
                    }
                }
                if (temp_stream != null)
                {
                    orig_stream = temp_stream;
                }

                using (var new_stream = new System.IO.MemoryStream())
                using (var reader = new iTextSharp.text.pdf.PdfReader(orig_stream.ToArray()))
                {
                    var order = new SaddleStitchPageOrder(reader.NumberOfPages);
                    reader.SelectPages(order.PageOrder);
                    var doc = new iTextSharp.text.Document(reader.GetPageSizeWithRotation(1));
                    var pdfcopy_provider = new iTextSharp.text.pdf.PdfCopy(doc, new_stream);
                    doc.Open();
                    for (int i = 1; i <= reader.NumberOfPages; i++)
                    {
                        var importedPage = pdfcopy_provider.GetImportedPage(reader, i);
                        pdfcopy_provider.AddPage(importedPage);
                    }
                    doc.Close();
                    NewMemoryStream = new_stream;
                }
            }

            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }
    }
}
