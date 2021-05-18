using System.Collections.Generic;

namespace PdfCropAndNUp
{
    internal class PdfTextCoordinatesStrategy : iTextSharp.text.pdf.parser.LocationTextExtractionStrategy
    {
        //Hold each coordinate
        public List<TextAndSurroundingRectangle> myPoints = new List<TextAndSurroundingRectangle>();

        //Automatically called for each chunk of text in the PDF
        public override void RenderText(iTextSharp.text.pdf.parser.TextRenderInfo renderInfo)
        {
            base.RenderText(renderInfo);

            //Get the bounding box for the chunk of text
            var bottomLeft = renderInfo.GetDescentLine().GetStartPoint();
            var topRight = renderInfo.GetAscentLine().GetEndPoint();

            //Create a rectangle from it
            var rect = new iTextSharp.text.Rectangle(
                bottomLeft[iTextSharp.text.pdf.parser.Vector.I1], bottomLeft[iTextSharp.text.pdf.parser.Vector.I2],
                topRight[iTextSharp.text.pdf.parser.Vector.I1], topRight[iTextSharp.text.pdf.parser.Vector.I2]);

            //Add this to our main collection
            this.myPoints.Add(new TextAndSurroundingRectangle(rect, renderInfo.GetText()));
        }
    }
}