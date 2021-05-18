using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfCropAndNUp
{
    public static class PdfCookbook
    {
        public static System.IO.FileInfo CreateCenteredBookletSizePdf(
            string origFileName,
            bool hasCover,
            string newFileName = "")
        {
            if(string.IsNullOrWhiteSpace(newFileName))
            {
                newFileName = origFileName.Replace(".pdf", "_new.pdf");
            }
            using(var fs = new System.IO.FileStream(
                origFileName, System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                var ms = new System.IO.MemoryStream();
                fs.CopyTo(ms);
                var text_coord_stream = new TextCoordinateFinder(ms, hasCover);
                var cover_height = text_coord_stream.CoverHeight;
                var text_coords_pdf = text_coord_stream.NewMemoryStream;

                var center_pdf = new CenterCroppedPdfOnPage(text_coords_pdf, PdfPageSizeEnum.Booklet).NewMemoryStream;
                StaticUtils.SaveStreamToFile(newFileName, center_pdf);
            }
            if(System.IO.File.Exists(newFileName)) return new System.IO.FileInfo(newFileName);
            else return null;
        }
        public static List<System.IO.FileInfo> CreatePrintReadyFile(
            string origFileName,
            bool hasCover,
            PdfBindTypeEnum pdfBindType,
            string[] newFileNames = null,
            bool isCameraReadyCenteredPdf = false,
            bool isCameraReadyOffsetPdf = true)
        {
            var imposed_cover = string.Empty;
            var imposed_brief = string.Empty;
            // make sure file exists
            if(!System.IO.File.Exists(origFileName))
            {
                throw new Exception();
            }
            if(newFileNames != null)
            {
                if(pdfBindType == PdfBindTypeEnum.SaddleStitch)
                {
                    imposed_brief = newFileNames[0];
                }
                else
                {
                    imposed_cover = newFileNames[0];
                    imposed_brief = newFileNames[1];
                }
            }
            else
            {
                if(pdfBindType == PdfBindTypeEnum.SaddleStitch)
                {
                    imposed_brief = origFileName.Replace(".pdf", "__new_brief.pdf");
                }
                else
                {

                    imposed_cover = origFileName.Replace(".pdf", "__new_cover.pdf");
                    imposed_brief = origFileName.Replace(".pdf", "__new_brief.pdf");
                }
            }

            using(var fs = new System.IO.FileStream(
                origFileName, System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                // set variables
                var origStream = new System.IO.MemoryStream();
                fs.CopyTo(origStream);
                System.IO.MemoryStream textCoordStream = null;
                System.IO.MemoryStream centerOnBookletPageStream = null;
                ImposeBookletPage imposedDocumentStream = null;

                // set up page by user option
                if(isCameraReadyOffsetPdf)
                {
                    textCoordStream = new TextCoordinateFinder(origStream, hasCover).NewMemoryStream;
                    centerOnBookletPageStream = new CenterCroppedPdfOnPage(textCoordStream, PdfPageSizeEnum.Booklet).NewMemoryStream;
                    imposedDocumentStream = new ImposeBookletPage(centerOnBookletPageStream, hasCover, pdfBindType);
                }
                if(isCameraReadyCenteredPdf)
                {

                    centerOnBookletPageStream = new CenterCroppedPdfOnPage(origStream, PdfPageSizeEnum.Booklet).NewMemoryStream;
                    imposedDocumentStream = new ImposeBookletPage(centerOnBookletPageStream, hasCover, pdfBindType);
                }

                // impose SS or PB
                if(pdfBindType == PdfBindTypeEnum.SaddleStitch)
                {
                    var imposedPerfectBindBrief = imposedDocumentStream.BriefMemoryStream;

                    // create new file
                    StaticUtils.SaveStreamToFile(imposed_brief, imposedPerfectBindBrief);
                }
                else
                {
                    if(null != imposedDocumentStream.CoverMemoryStream)
                    {
                        var imposedPerfectBindCover = imposedDocumentStream.CoverMemoryStream;
                        StaticUtils.SaveStreamToFile(imposed_cover, imposedPerfectBindCover);
                    }
                    if(null != imposedDocumentStream.BriefMemoryStream)
                    {
                        var imposedPerfectBindBrief = imposedDocumentStream.BriefMemoryStream;
                        StaticUtils.SaveStreamToFile(imposed_brief, imposedPerfectBindBrief);
                    }
                }
            }
            var list = new List<System.IO.FileInfo>();
            if(System.IO.File.Exists(imposed_cover)) list.Add(new System.IO.FileInfo(imposed_cover));
            if(System.IO.File.Exists(imposed_brief)) list.Add(new System.IO.FileInfo(imposed_brief));

            if(list.Count() > 0) return list;
            else return null;
        }
    }
}