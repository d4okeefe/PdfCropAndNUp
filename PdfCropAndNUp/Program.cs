using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfCropAndNUp
{
    class Program
    {
        //static void CreateImposedFile(
        //    string origFileName,
        //    bool hasCover,
        //    PdfBindTypeEnum pdfBindType,
        //    string[] newFileNames = null)
        //{
        //    var imposed_cover = string.Empty;
        //    var imposed_brief = string.Empty;
        //    // make sure file exists
        //    if (!System.IO.File.Exists(origFileName))
        //    {
        //        throw new Exception();
        //    }
        //    if (newFileNames != null)
        //    {
        //        if (pdfBindType == PdfBindTypeEnum.SaddleStitch)
        //        {
        //            imposed_brief = newFileNames[0];
        //        }
        //        else
        //        {
        //            imposed_cover = newFileNames[0];
        //            imposed_brief = newFileNames[1];
        //        }
        //    }
        //    else
        //    {
        //        if (pdfBindType == PdfBindTypeEnum.SaddleStitch)
        //        {
        //            imposed_brief = origFileName.Replace(".pdf", "__new_brief.pdf");
        //        }
        //        else
        //        {

        //            imposed_cover = origFileName.Replace(".pdf", "__new_cover.pdf");
        //            imposed_brief = origFileName.Replace(".pdf", "__new_brief.pdf");
        //        }
        //    }


        //    using (var fs = new System.IO.FileStream(
        //        origFileName, System.IO.FileMode.Open, System.IO.FileAccess.Read))
        //    {
        //        var origStream = new System.IO.MemoryStream();
        //        fs.CopyTo(origStream);


        //        var textCoordStream = new TextCoordinateFinder(origStream, hasCover).NewMemoryStream;
        //        var centerOnBookletPageStream = new CenterCroppedPdfOnPage(textCoordStream, PdfPageSizeEnum.Booklet).NewMemoryStream;
        //        var imposedDocumentStream = new ImposeBookletPage(centerOnBookletPageStream, hasCover, pdfBindType);

        //        if (pdfBindType == PdfBindTypeEnum.SaddleStitch)
        //        {
        //            var imposedPerfectBindBrief = imposedDocumentStream.BriefMemoryStream;

        //            // create new file
        //            StaticUtils.SaveStreamToFile(imposed_brief, imposedPerfectBindBrief);
        //        }
        //        else
        //        {
        //            var imposedPerfectBindCover = imposedDocumentStream.CoverMemoryStream;
        //            var imposedPerfectBindBrief = imposedDocumentStream.BriefMemoryStream;

        //            // create new files
        //            StaticUtils.SaveStreamToFile(imposed_cover, imposedPerfectBindCover);
        //            StaticUtils.SaveStreamToFile(imposed_brief, imposedPerfectBindBrief);
        //        }
        //    }
        //}

        static void Main(string[] args)
        {
            //string selectedFile = @"C:\scratch\35633 Ayers (Jan 11, 2018, 8 59 32 am)\"
            ////+ "35633 Ayers br 01.pdf";
            ////+ "35633 pdf Ayers.pdf";
            //+ "test_with_cover.pdf";
            ////+ "test_no_cover.pdf";
            ////+"test_cover_only.pdf";

            //string selectedFile = @"C:\scratch\35661 Wein (Jan 15, 2018, 3 51 26 pm)\"
            //    + "35661 pdf Wein.pdf";
            //string selectedFile = @"C:\scratch\35578 McGrath (Jan 16, 2018, 10 59 24 am)\"
            //        //+ "35578 pdf McGrath.pdf";
            //        + "35578 McGrath cv 02.pdf";

            string selectedFile = @"C:\scratch\37094 Joffe (Oct 25, 2018, 9 35 28 am)\37094 Joffe 10-22-18 639 pm Appellants Initial Brief (filing).pdf";


            //PdfCookbook.CreatePrintReadyFile(
            //    selectedFile,
            //    true,
            //    PdfBindTypeEnum.SaddleStitch);


            PdfCookbook.CreateCenteredBookletSizePdf(
                selectedFile,
                true);



            //string _selectedPdfFile = @"C:\scratch\35633 Ayers (Jan 11, 2018, 8 59 32 am)\"
            ////+ "35633 Ayers br 01.pdf";
            ////+ "35633 pdf Ayers.pdf";
            //+ "test_with_cover.pdf";
            ////+ "test_no_cover.pdf";
            ////+"test_cover_only.pdf";

            //string _newFileName = "";
            //PdfBindType _bind_type = PdfBindType.PerfectBind;
            //bool _is_typeset = true;
            //bool _has_cover = true; // means to crop and place first page separately
            //int _cover_length = 48;
            //int _page_count = 32;

            ////var impose = new ImposeSinglePdf(
            ////    _selectedPdfFile,
            ////    _newFileName,
            ////    _bind_type,
            ////    _is_typeset,
            ////    _has_cover,
            ////    _cover_length,
            ////    _page_count);

            //using (var filestream = new System.IO.FileStream(
            //    _selectedPdfFile, System.IO.FileMode.Open, System.IO.FileAccess.Read))
            //{
            //    var orig_stream = new System.IO.MemoryStream();
            //    filestream.CopyTo(orig_stream);

            //    //var removeFirstPage = StaticUtils.RemoveFirstPage(orig_stream);
            //    //StaticUtils.SaveStreamToFile(_selectedPdfFile.Replace(".pdf", "__new.pdf"), removeFirstPage);

            //    var textCoordFinder = new TextCoordinateFinder(orig_stream, _has_cover);
            //    var textCoordStream = textCoordFinder.NewMemoryStream;
            //    var centerOnBooklet = new CenterCroppedPdfOnPage(textCoordStream, PdfPageSize.Booklet).NewMemoryStream;
            //    //var imposedOnB4 = new ImposeBookletPage(centerOnBooklet, _has_cover, TypeOfPdfPage.B5).BriefMemoryStream;

            //    //var centerOnLetterPage = new CenterCroppedPdfOnLetterSizePage(textCoordStream).NewMemoryStream;

            //    var imposedDocument = new ImposeBookletPage(centerOnBooklet, _has_cover, _bind_type);

            //    //var imposedSaddleStitch = imposedDocument.BriefMemoryStream;
            //    //StaticUtils.SaveStreamToFile(_selectedPdfFile.Replace(".pdf", "__new_brief.pdf"), imposedSaddleStitch);


            //    var imposedPerfectBindCover = imposedDocument.CoverMemoryStream;
            //    var imposedPerfectBindBrief = imposedDocument.BriefMemoryStream;
            //    StaticUtils.SaveStreamToFile(_selectedPdfFile.Replace(".pdf", "__new_cover.pdf"), imposedPerfectBindCover);
            //    StaticUtils.SaveStreamToFile(_selectedPdfFile.Replace(".pdf", "__new_brief.pdf"), imposedPerfectBindBrief);
            //}
        }
    }
}