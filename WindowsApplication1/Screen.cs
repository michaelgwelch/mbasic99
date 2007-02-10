using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace WindowsApplication1
{
    class Screen : Control
    {
        private const int xScaleFactor = 2;
        private const int yScaleFactor = 2;
        private const int numCols = 32;
        private const int numRows = 24;
        private const int firstPrintCol = 2;

        // circular buffer.
        private CharacterBitmap characters;
        private Bitmap[][] cells;
        private int bottomRow;

        public Screen()
        {
            characters = new CharacterBitmap();
            Height = 24 * 8 * yScaleFactor; // 24 rows * 8 pixels/row * scaling factor of 2
            Width = 32 * 8 * xScaleFactor;  // 32 cols * 8 pixels/col * scaling factor of 2
            SetStyle(ControlStyles.UserPaint |
                  ControlStyles.OptimizedDoubleBuffer |
                  ControlStyles.AllPaintingInWmPaint, true);
            BackColor = TIColor.Cyan;

            // cells is an array of 24 rows.
            // each element is an array of bitmaps.
            cells = new Bitmap[numRows][];

            for (int row = 0; row < numRows; row++)
            {
                cells[row] = new Bitmap[numCols];
                for (int col = 0; col < numCols; col++)
                {
                    cells[row][col] = characters.Space;
                }
            }

        }

        public void Color(int characterSet, TIColor foreColor, TIColor backColor)
        {

            characters.Color(characterSet, foreColor, backColor);
            Invalidate();
        }

        protected override void OnBackColorChanged(EventArgs e)
        {
            base.OnBackColorChanged(e);
            Invalidate();
        }

        protected override void OnForeColorChanged(EventArgs e)
        {
            base.OnForeColorChanged(e);
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
            Rectangle badArea = pe.ClipRectangle;
            Graphics g = pe.Graphics;

            g.Clear(BackColor);
            g.ScaleTransform(xScaleFactor, yScaleFactor);
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            g.SmoothingMode = SmoothingMode.None;

            for (int row = 0; row < numRows; row++)
            {
                int displayRow = CalculateDisplayRow(row);
                for (int col = 0; col < numCols; col++)
                {
                    Bitmap bm = cells[row][col];
                    if (bm == null) continue;
                    g.DrawImage(bm, CalculateDisplayPosition(displayRow, col));
                }
            }
        }

        private static Point CalculateDisplayPosition(int row, int col)
        {
            return new Point(col * 8, row * 8);
        }

        private int CalculateDisplayRow(int cellRow)
        {
            return (numRows + cellRow - bottomRow - 1) % numRows;
        }

        public void Print(string value)
        {
            ShiftRowsUp();
            for (int i = 0; i < value.Length; i++)
            {
                char ch = value[i];
                cells[bottomRow][i+firstPrintCol] = characters[ch];
            }
            Invalidate();
        }

        public void HChar(int rowNum, int colNum, char ch)
        {
            HChar(rowNum, colNum, ch, 1);
        }

        /// <summary>
        /// 0 indexed
        /// </summary>
        /// <param name="rowNum"></param>
        /// <param name="colNum"></param>
        /// <param name="ch"></param>
        /// <param name="repeat"></param>
        public void HChar(int rowNum, int colNum, char ch, int repeat)
        {
            for (int i = 0; i < repeat; i++)
            {
                colNum += i;
                if (colNum > numCols)
                {
                    colNum = 0;
                    rowNum = (rowNum + 1) % numRows;
                }
                cells[rowNum][colNum] = characters[ch];
            }
            Invalidate();
        }

        public void VChar(int rowNum, int colNum, char ch)
        {
            VChar(rowNum, colNum, ch, 1);
        }

        public void VChar(int rowNum, int colNum, char ch, int repeat)
        {
            for (int i = 0; i < repeat; i++)
            {
                rowNum += i;
                if (rowNum > numRows)
                {
                    rowNum = 0;
                    colNum = (colNum + 1) % numCols;
                }
                cells[rowNum][colNum] = characters[ch];
            }
            Invalidate();
        }

        public void Char(char ch, string hexCodes)
        {
            characters.ModifyBitmap(ch, hexCodes);
        }

        private void ShiftRowsUp()
        {
            bottomRow = (bottomRow+1) % 24;
            for (int col = 0; col < numCols; col++)
            {
                cells[bottomRow][col] = characters.Space;
            }
        }
    }
}
