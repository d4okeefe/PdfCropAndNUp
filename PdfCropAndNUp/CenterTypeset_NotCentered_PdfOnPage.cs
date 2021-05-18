//using Acrobat;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace PdfCropAndNUp
//{
//    class CenterTypeset_NotCentered_PdfOnPage
//    {
//        private CockleFilePdf selectedPdfFile;
//        private bool hasCover;
//        private int? coverLen;
//        private ViewModels.SearchViewModelFolder.CenteredCoverType courtBriefSizeDoc;
//        private string newFileName;

//        public CockleFilePdf NewFileCreated { get; set; }

//        public CenterTypeset_NotCentered_PdfOnPage(CockleFilePdf _selectedFile,
//            int? _cv_len,
//            ViewModels.SearchViewModelFolder.CenteredCoverType _courtBriefSizeDoc =
//            ViewModels.SearchViewModelFolder.CenteredCoverType.Letter)
//        {
//            if (!Models.Utilities.AcrobatJS.AreAcrobatJavascriptsInPlace()) { throw new Exception(); }

//            selectedPdfFile = _selectedFile;
//            hasCover = null == _cv_len ? false : true;
//            coverLen = _cv_len;
//            courtBriefSizeDoc = _courtBriefSizeDoc;

//            // develop new file name
//            var directory = System.IO.Path.GetDirectoryName(selectedPdfFile.FullName);
//            var ticket_atty = selectedPdfFile.TicketPlusAttorney;
//            if (string.IsNullOrEmpty(ticket_atty))
//            {

//            }
//            else
//            {
//                var i = 0;
//                var _newfilename = string.Empty;
//                do
//                {
//                    if (i == 0)
//                    {
//                        _newfilename = ticket_atty
//                            + (courtBriefSizeDoc == ViewModels.SearchViewModelFolder.CenteredCoverType.Letter
//                            ? " centered on letter_page" : " centered on booklet_page")
//                            + ".pdf";
//                    }
//                    else
//                    {
//                        _newfilename = ticket_atty +
//                            (courtBriefSizeDoc == ViewModels.SearchViewModelFolder.CenteredCoverType.Letter
//                            ? " centered on letter_page" : " centered on booklet_page")
//                            + "_" + i + ".pdf";
//                    }
//                    i++;
//                } while (System.IO.File.Exists(_newfilename)); // fix

//                newFileName = System.IO.Path.Combine(directory, _newfilename);
//            }

//            NewFileCreated = new CockleFilePdf(newFileName, selectedPdfFile, Utilities.SourceFileTypeEnum.UnrecognizedCentered);

//            if (courtBriefSizeDoc == ViewModels.SearchViewModelFolder.CenteredCoverType.Letter)
//            {
//                // fix later

//                centerOnLetterPaper();
//            }
//            else
//            {
//                centerOnBriefPaper();
//            }
//        }
//        private void centerOnBriefPaper()
//        {
//            try
//            {
//                var filename = selectedPdfFile.FullName;
//                var new_filename = selectedPdfFile.FullName.Replace(".pdf", "_centered.pdf");
//                var ms = PdfCropAndNUp.StaticUtils.ConvertFileToStream(filename);
//                if (null == ms) throw new Exception();
//                var centered_stream = new PdfCropAndNUp.CenterCroppedPdfOnPage(ms).NewMemoryStream;
//                if (null == centered_stream) throw new Exception();
//                var new_file = PdfCropAndNUp.StaticUtils.SaveStreamToFile(new_filename, centered_stream);
//                NewFileCreated = new CockleFilePdf(new_filename, Utilities.SourceFileTypeEnum.UnrecognizedCentered);
//            }
//            catch (Exception ex)
//            {
//                System.Diagnostics.Debug.WriteLine(ex.Message);
//                NewFileCreated = null;
//                throw new Exception("Error in attempt to center Pdf file.");
//            }
//            finally
//            {

//            }
//        }
//        private void centerOnBriefPaper_Old()
//        {
//            CAcroApp app = new AcroApp();
//            CAcroPDDoc doc = new AcroPDDoc();
//            try
//            {
//                var opened = doc.Open(selectedPdfFile.FullName);
//                if (!opened) { throw new Exception("Unable to open file."); }

//                object js_object = doc.GetJSObject();


//                Type js_type = js_object.GetType();
//                object[] js_param = { };
//                string script_name = string.Empty;
//                if (hasCover)
//                {
//                    switch (coverLen)
//                    {
//                        case 48:
//                        default:
//                            script_name = Utilities.AcrobatJS.Javascripts
//                                [Utilities.LocalJavascripts.center_booklet_48pica];
//                            break;
//                        case 49:
//                            script_name = Utilities.AcrobatJS.Javascripts
//                                [Utilities.LocalJavascripts.center_booklet_49pica];
//                            break;
//                        case 50:
//                            script_name = Utilities.AcrobatJS.Javascripts
//                                [Utilities.LocalJavascripts.center_booklet_50pica];
//                            break;
//                        case 51:
//                            script_name = Utilities.AcrobatJS.Javascripts
//                                [Utilities.LocalJavascripts.center_booklet_51pica];
//                            break;
//                    }
//                }
//                else
//                {
//                    script_name = Utilities.AcrobatJS.Javascripts
//                        [Utilities.LocalJavascripts.center_booklet_no_cover];
//                }

//                js_type.InvokeMember(script_name,
//                    BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance,
//                    null, js_object, js_param);


//                // save document
//                var test = doc.Save(1, NewFileCreated.FullName); // doc.Save(1, NewFileCreated.FullName); would overwrite original
//            }
//            catch (Exception ex)
//            {
//                System.Diagnostics.Debug.WriteLine(ex.Message);
//                NewFileCreated = null;
//                throw new Exception("Error in attempt to center Pdf file.");
//            }
//            finally
//            {
//                doc.Close();
//                app.CloseAllDocs();
//                app.Exit();

//                doc = null;
//                app = null;

//                GC.Collect();
//            }
//        }

//        private void centerOnLetterPaper()
//        {
//            CAcroApp app = new AcroApp();
//            CAcroPDDoc doc = new AcroPDDoc();
//            try
//            {
//                var opened = doc.Open(selectedPdfFile.FullName);
//                if (!opened) { throw new Exception("Unable to open file."); }

//                object js_object = doc.GetJSObject();


//                Type js_type = js_object.GetType();
//                object[] js_param = { };
//                string script_name = string.Empty;
//                if (hasCover)
//                {
//                    switch (coverLen)
//                    {
//                        case 48:
//                        default:
//                            script_name = Utilities.AcrobatJS.Javascripts
//                                [Utilities.LocalJavascripts.center_letter_48pica];
//                            break;
//                        case 49:
//                            script_name = Utilities.AcrobatJS.Javascripts
//                                [Utilities.LocalJavascripts.center_letter_49pica];
//                            break;
//                        case 50:
//                            script_name = Utilities.AcrobatJS.Javascripts
//                                [Utilities.LocalJavascripts.center_letter_50pica];
//                            break;
//                        case 51:
//                            script_name = Utilities.AcrobatJS.Javascripts
//                                [Utilities.LocalJavascripts.center_letter_51pica];
//                            break;
//                    }
//                }
//                else
//                {
//                    script_name = Utilities.AcrobatJS.Javascripts
//                        [Utilities.LocalJavascripts.center_letter_no_cover];
//                }

//                js_type.InvokeMember(script_name,
//                    BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance,
//                    null, js_object, js_param);

//                // save document
//                doc.Save(1, NewFileCreated.FullName);
//            }
//            catch (Exception ex)
//            {
//                System.Diagnostics.Debug.WriteLine(ex.Message);
//                NewFileCreated = null;
//                throw new Exception("Error in attempt to center Pdf file.");
//            }
//            finally
//            {
//                doc.Close();
//                app.CloseAllDocs();
//                app.Exit();

//                doc = null;
//                app = null;

//                GC.Collect();
//            }
//        }
//    }
//}
