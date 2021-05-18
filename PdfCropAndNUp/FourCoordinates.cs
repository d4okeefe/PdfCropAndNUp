namespace PdfCropAndNUp
{
    internal class FourCoordinates
    {
        public float Bottom { get; set; }
        public float Left { get; set; }
        public float Top { get; set; }
        public float Right { get; set; }
        public float Height
        {
            get { return Top - Bottom; }
        }
        public float Width
        {
            get { return Right - Left; }
        }
        public int PageNumber { get; set; }

        public FourCoordinates() { }
        public FourCoordinates(float b, float l, float t, float r)
        {
            Bottom = b;
            Left = l;
            Top = t;
            Right = r;
        }
    }
}