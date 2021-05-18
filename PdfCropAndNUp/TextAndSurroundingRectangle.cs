using iTextSharp.text;

namespace PdfCropAndNUp
{
    internal class TextAndSurroundingRectangle
    {
        public iTextSharp.text.Rectangle Rectangle;
        public string Text;
        public TextAndSurroundingRectangle(iTextSharp.text.Rectangle rect, System.String text)
        {
            Rectangle = rect;
            Text = text;
        }
    }
}