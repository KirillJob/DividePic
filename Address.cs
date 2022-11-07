namespace PicDivide
{
    internal static class HeaderAddresses
    {
        internal static readonly int Signature       = 0;
        internal static readonly int HeaderLength    = 4;
        internal static readonly int DataOffset      = 8;
        internal static readonly int Reserved        = 12;
        internal static readonly int ImageWidth      = 16;
        internal static readonly int ImageHeight     = 20;
        internal static readonly int BitPerPixel     = 24;
        internal static readonly int ImageLineLength = 28;
    }
}