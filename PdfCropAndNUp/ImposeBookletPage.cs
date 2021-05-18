using System;
using System.Collections.Generic;
using System.Linq;

namespace PdfCropAndNUp
{
    internal class ImposeBookletPage
    {
        public bool HasCover { get; set; }
        public System.IO.MemoryStream BriefMemoryStream { get; set; }
        public System.IO.MemoryStream CoverMemoryStream { get; set; }
        public int NumberOfPages { get; private set; }
        public PdfBindTypeEnum PdfBindType { get; set; }
        public float CoverWidth
        {
            get
            {
                return 1031.76f;
            }
        }
        public float CoverHeight
        {
            get
            {
                return 728.64f;
            }
        }
        public float BriefWidth
        {
            get
            {
                return PdfBindType == PdfBindTypeEnum.SaddleStitch ? 1031.76f : 515.88f;
            }
        }
        public float BriefHeight
        {
            get
            {
                return 728.64f;
            }
        }
        public bool CoverOnly
        {
            get
            {
                return null != orig_stream && HasCover
                    && new iTextSharp.text.pdf.PdfReader(
                        orig_stream.ToArray()).NumberOfPages == 1;
            }
        }
        private System.IO.MemoryStream orig_stream;
        private float SS_B4_OUTER_MARGIN_COVER
        {
            get
            {
                return CoverWidth / 2 + 5.75f;
            }
        }
        private float SS_B4_BOTTOM_MARGIN_COVER = 27.5f;
        private float SS_B4_BOTTOM_MARGIN = 33.5f;
        private float SS_B4_OUTER_MARGIN = 75f;

        private float PB_B4_OUTER_MARGIN_COVER
        {
            get
            {
                return CoverWidth / 2 + 25.75f;
            }
        }
        private float PB_B4_BOTTOM_MARGIN_COVER = 15.5f;
        private float PB_B5_OUTER_MARGIN_ODD_PAGE = 3f;
        private float PB_B5_BOTTOM_MARGIN_ODD_PAGE = 17.5f;
        private float PB_B5_OUTER_MARGIN_EVEN_PAGE = 78f;
        private float PB_B5_BOTTOM_MARGIN_EVEN_PAGE = 17.5f;

        // warn that blank pages must be inserted by user !!!
        public ImposeBookletPage(
            System.IO.MemoryStream _orig_stream,
            bool _has_cover = false,
            PdfBindTypeEnum _pdfBindType = PdfBindTypeEnum.SaddleStitch,
            int numberOfPages = 0)
        {
            try
            {
                orig_stream = _orig_stream;
                HasCover = _has_cover;
                NumberOfPages = numberOfPages;
                if (NumberOfPages == 0)
                {
                    NumberOfPages = new iTextSharp.text.pdf.PdfReader(orig_stream.ToArray()).NumberOfPages;
                }
                PdfBindType = _pdfBindType;
                if (PdfBindType != PdfBindTypeEnum.SaddleStitch
                    && PdfBindType != PdfBindTypeEnum.PerfectBind)
                {
                    throw new Exception();
                }



                if (PdfBindType == PdfBindTypeEnum.SaddleStitch)
                {
                    BriefMemoryStream = imposeSaddleStitch();
                }
                else // B5
                {
                    if (HasCover)
                    {
                        if (CoverOnly)
                        {
                            CoverMemoryStream = imposePerfectBindCover();
                        }
                        else
                        {
                            CoverMemoryStream = imposePerfectBindCover();
                            BriefMemoryStream = imposePerfectBindBrief();
                        }
                    }
                    if (!CoverOnly)
                    {
                        BriefMemoryStream = imposePerfectBindBrief();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }

        private System.IO.MemoryStream imposePerfectBindCover() // MEASUREMENTS NOT CORRECT HERE !!!
        {
            System.IO.MemoryStream orig_cover_stream = null;
            System.IO.MemoryStream cover_stream = null;
            try
            {
                if (CoverOnly) orig_cover_stream = orig_stream;
                else orig_cover_stream = StaticUtils.SelectPages(orig_stream, "1");


                using (cover_stream = new System.IO.MemoryStream())
                using (var reader = new iTextSharp.text.pdf.PdfReader(
                    orig_cover_stream.ToArray()))
                {
                    var docB4 = new iTextSharp.text.Document(
                        new iTextSharp.text.Rectangle(CoverWidth, CoverHeight));
                    var writer = iTextSharp.text.pdf.PdfWriter.GetInstance(docB4, cover_stream);
                    docB4.Open();
                    docB4.Add(new iTextSharp.text.Chunk());
                    var tRight = writer.GetImportedPage(reader, 1);
                    var contentbyte = writer.DirectContent;
                    // need to account for differences in book size !!! page height should be ok
                    contentbyte.AddTemplate(tRight, PB_B4_OUTER_MARGIN_COVER, PB_B4_BOTTOM_MARGIN_COVER, true);
                    docB4.Close();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return null;
            }
            return cover_stream;
        }

        private System.IO.MemoryStream imposePerfectBindBrief() // MEASUREMENTS GOOD
        {
            System.IO.MemoryStream orig_brief_stream = null;
            System.IO.MemoryStream brief_stream = null;

            try
            {
                // extract first page, if necessary
                if (HasCover) orig_brief_stream = StaticUtils.RemoveFirstPage(orig_stream);
                else orig_brief_stream = orig_stream;

                // impose brief on B5 pages
                using (brief_stream = new System.IO.MemoryStream())
                using (var reader = new iTextSharp.text.pdf.PdfReader(
                    orig_brief_stream.ToArray()))
                {
                    var docB5 = new iTextSharp.text.Document(
                        new iTextSharp.text.Rectangle(BriefWidth, BriefHeight));
                    var writer = iTextSharp.text.pdf.PdfWriter.GetInstance(docB5, brief_stream);
                    docB5.Open();
                    for (int i = 1; i <= reader.NumberOfPages; ++i)
                    {
                        if (i != 1) docB5.NewPage();
                        docB5.Add(new iTextSharp.text.Chunk());
                        var importedpage = writer.GetImportedPage(reader, i);
                        var contentbyte = writer.DirectContent;
                        if (i % 2 == 1) // odd pages
                        {
                            contentbyte.AddTemplate(
                                importedpage,
                                PB_B5_OUTER_MARGIN_ODD_PAGE/*outer margin*/,
                                PB_B5_BOTTOM_MARGIN_ODD_PAGE, /*lower margin*/
                                true);
                        }
                        else
                        {
                            contentbyte.AddTemplate(
                                importedpage,
                                PB_B5_OUTER_MARGIN_EVEN_PAGE/*outer margin*/,
                                PB_B5_BOTTOM_MARGIN_EVEN_PAGE, /*lower margin*/
                                true);
                        }
                    }
                    docB5.Close();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return null;
            }
            return brief_stream;
        }
        private System.IO.MemoryStream imposeSaddleStitch() // MEASUREMENTS GOOD !!!
        {

            // split into two streams, rejoin later
            System.IO.MemoryStream cover_stream = null;
            System.IO.MemoryStream brief_stream = null;
            System.IO.MemoryStream combined_stream = null;

            try
            {
                // 3 situations:
                // OPTION 1: COVER ONLY
                if (CoverOnly)
                {
                    using (cover_stream = new System.IO.MemoryStream())
                    using (var reader = new iTextSharp.text.pdf.PdfReader(orig_stream.ToArray()))
                    {
                        var docB4 = new iTextSharp.text.Document(
                            new iTextSharp.text.Rectangle(CoverWidth, CoverHeight));
                        var writer = iTextSharp.text.pdf.PdfWriter.GetInstance(docB4, cover_stream);
                        docB4.Open();
                        docB4.Add(new iTextSharp.text.Chunk());
                        var tRight = writer.GetImportedPage(reader, 1);
                        var contentbyte = writer.DirectContent;
                        // need to account for differences in book size !!! page height should be ok
                        contentbyte.AddTemplate(tRight, SS_B4_OUTER_MARGIN_COVER,
                            //SS_B4_BOTTOM_MARGIN, true);
                            SS_B4_BOTTOM_MARGIN_COVER, true);
                        docB4.Close();
                    }
                    return cover_stream;
                }
                else
                {
                    // OPTION 2: BRIEF ONLY
                    if (!HasCover)
                    {
                        // impose brief
                        var reorderedStream = new ReorderSaddleStitchPages(orig_stream).NewMemoryStream;

                        using (brief_stream = new System.IO.MemoryStream())
                        using (var reader = new iTextSharp.text.pdf.PdfReader(reorderedStream.ToArray()))
                        {
                            var docB4 = new iTextSharp.text.Document(
                            new iTextSharp.text.Rectangle(BriefWidth, BriefHeight));
                            var writer = iTextSharp.text.pdf.PdfWriter.GetInstance(docB4, brief_stream);
                            docB4.Open();
                            for (int i = 1; i <= reader.NumberOfPages; i += 2)
                            {
                                if (i != 1) docB4.NewPage();
                                docB4.Add(new iTextSharp.text.Chunk());
                                var tLeft = writer.GetImportedPage(reader, i);
                                var tRight = writer.GetImportedPage(reader, i + 1);
                                var contentbyte = writer.DirectContent;
                                contentbyte.AddTemplate(tLeft, SS_B4_OUTER_MARGIN, SS_B4_BOTTOM_MARGIN, true);
                                contentbyte.AddTemplate(tRight, SS_B4_OUTER_MARGIN_COVER, SS_B4_BOTTOM_MARGIN, true);
                            }
                            docB4.Close();
                        }
                        return brief_stream;
                    }
                    // OPTION 3: COVER AND BRIEF
                    else
                    {
                        // first, impose brief
                        var removeFirstPage = StaticUtils.RemoveFirstPage(orig_stream);
                        var reorderedStream = new ReorderSaddleStitchPages(removeFirstPage).NewMemoryStream;

                        using (brief_stream = new System.IO.MemoryStream())
                        using (var reader = new iTextSharp.text.pdf.PdfReader(reorderedStream.ToArray()))
                        {
                            var docB4 = new iTextSharp.text.Document(
                            new iTextSharp.text.Rectangle(BriefWidth, BriefHeight));
                            var writer = iTextSharp.text.pdf.PdfWriter.GetInstance(docB4, brief_stream);
                            docB4.Open();
                            for (int i = 1; i <= reader.NumberOfPages; i += 2)
                            {
                                if (i != 1) docB4.NewPage();
                                docB4.Add(new iTextSharp.text.Chunk());
                                var tLeft = writer.GetImportedPage(reader, i);
                                var tRight = writer.GetImportedPage(reader, i + 1);
                                var contentbyte = writer.DirectContent;
                                contentbyte.AddTemplate(tLeft, SS_B4_OUTER_MARGIN, SS_B4_BOTTOM_MARGIN, true);
                                contentbyte.AddTemplate(tRight, SS_B4_OUTER_MARGIN_COVER, SS_B4_BOTTOM_MARGIN, true);
                            }
                            docB4.Close();
                        }

                        // second, impose cover
                        var extractFirstPage = StaticUtils.SelectPages(orig_stream, "1");
                        using (cover_stream = new System.IO.MemoryStream())
                        using (var reader = new iTextSharp.text.pdf.PdfReader(extractFirstPage.ToArray()))
                        {
                            var docB4 = new iTextSharp.text.Document(
                            new iTextSharp.text.Rectangle(CoverWidth, CoverHeight));
                            var writer = iTextSharp.text.pdf.PdfWriter.GetInstance(docB4, cover_stream);
                            docB4.Open();
                            docB4.Add(new iTextSharp.text.Chunk());
                            var tRight = writer.GetImportedPage(reader, 1);
                            var contentbyte = writer.DirectContent;
                            // need to account for differences in book size !!! page height should be ok
                            contentbyte.AddTemplate(tRight, SS_B4_OUTER_MARGIN_COVER,
                                //SS_B4_BOTTOM_MARGIN, true);
                                SS_B4_BOTTOM_MARGIN_COVER, true);
                            docB4.Close();
                        }

                        // combine cover and brief
                        using (combined_stream = new System.IO.MemoryStream())
                        using (var cover_reader = new iTextSharp.text.pdf.PdfReader(cover_stream.ToArray()))
                        using (var brief_reader = new iTextSharp.text.pdf.PdfReader(brief_stream.ToArray()))
                        {
                            var comb_doc = new iTextSharp.text.Document(cover_reader.GetPageSizeWithRotation(1));
                            var pdfcopy_provider = new iTextSharp.text.pdf.PdfCopy(comb_doc, combined_stream);
                            comb_doc.Open();
                            // add cover
                            var importedCv = pdfcopy_provider.GetImportedPage(cover_reader, 1);
                            pdfcopy_provider.AddPage(importedCv);
                            // add blank page after cover
                            pdfcopy_provider.AddPage(cover_reader.GetPageSizeWithRotation(1), rotation: 1);
                            // add brief pages
                            for (int i = 1; i <= brief_reader.NumberOfPages; i++)
                            {
                                var importedBr = pdfcopy_provider.GetImportedPage(brief_reader, i);
                                pdfcopy_provider.AddPage(importedBr);
                            }
                            comb_doc.Close();
                            return combined_stream;
                        }
                    }
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