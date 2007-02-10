using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace WindowsApplication1
{
    class TIColor
    {
        public readonly Color Color;
        public readonly int TIColorCode;
        private TIColor(Color color, int tiColorCode)
        {
            Color = color;
            TIColorCode = tiColorCode;
        }

        public static implicit operator Color(TIColor tiColor)
        {
            return tiColor.Color;
        }

        public static readonly TIColor Transparent = new TIColor(Color.Transparent, 1);
        public static readonly TIColor Black = new TIColor(Color.FromArgb(0, 0, 0), 2);
        public static readonly TIColor MediumGreen = new TIColor(Color.FromArgb(72, 156, 8), 3);
        public static readonly TIColor LightGreen = new TIColor(Color.FromArgb(112, 191, 136), 4);
        public static readonly TIColor DarkBlue = new TIColor(Color.FromArgb(40, 60, 138), 5);
        public static readonly TIColor LightBlue = new TIColor(Color.FromArgb(80, 108, 207), 6);
        public static readonly TIColor DarkRed = new TIColor(Color.FromArgb(208, 72, 0), 7);
        public static readonly TIColor Cyan = new TIColor(Color.FromArgb(0, 204, 255), 8);
        public static readonly TIColor MediumRed = new TIColor(Color.FromArgb(208, 88, 40), 9);
        public static readonly TIColor LightRed = new TIColor(Color.FromArgb(255, 160, 64), 10);
        public static readonly TIColor DarkYellow = new TIColor(Color.FromArgb(252, 240, 80), 11);
        public static readonly TIColor LightYellow = new TIColor(Color.FromArgb(255, 255, 128), 12);
        public static readonly TIColor DarkGreen = new TIColor(Color.FromArgb(0, 128, 0), 13);
        public static readonly TIColor Magenta = new TIColor(Color.FromArgb(205, 88, 205), 14);
        public static readonly TIColor Gray = new TIColor(Color.FromArgb(224, 224, 224), 15);
        public static readonly TIColor White = new TIColor(Color.FromArgb(255, 255, 255), 16);

    }
}
