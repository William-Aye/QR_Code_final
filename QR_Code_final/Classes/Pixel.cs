using System;


namespace QR_Code_final
{
    class Pixel
    {
        private byte r, g, b;

        public Pixel(byte b, byte g, byte r)
        {
            this .r = r;
            this .g = g;
            this .b = b;
        }

        public byte R { get { return r; } set { r = value; } }
        public byte G { get { return g; } set { g = value; } }
        public byte B { get { return b; } set { b = value; } }
    }
}
