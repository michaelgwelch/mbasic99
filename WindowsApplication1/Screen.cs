/*******************************************************************************
    Copyright 2007 Michael Welch
    
    This file is part of MBasic99.

    MBasic99 is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation; either version 2 of the License, or
    (at your option) any later version.

    MBasic99 is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with MBasic99; if not, write to the Free Software
    Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
*******************************************************************************/



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
        private const char space = ' ';
        private const int pixelsPerCell = 8;

        // circular buffer.
        private CharacterBitmap characters;
        private char[][] cells;
        private int bottomRow;

        public Screen()
        {
            characters = new CharacterBitmap();
            Height = numRows * pixelsPerCell * yScaleFactor; // 24 rows * 8 pixels/row * scaling factor of 2
            Width = numCols * pixelsPerCell * xScaleFactor;  // 32 cols * 8 pixels/col * scaling factor of 2
            SetStyle(ControlStyles.UserPaint |
                  ControlStyles.OptimizedDoubleBuffer |
                  ControlStyles.AllPaintingInWmPaint, true);
            BackColor = TIColor.Cyan;

            // cells is an array of 24 rows.
            // each element is an array of chars.
            cells = new char[numRows][];

            for (int row = 0; row < numRows; row++)
            {
                cells[row] = new char[numCols];
            }
            Clear();
        }

        public void Clear()
        {
            for (int row = 0; row < numRows; row++)
                for (int col = 0; col < numCols; col++)
                    cells[row][col] = space;
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
                    Bitmap bm = characters[cells[row][col]];
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
                cells[bottomRow][i + firstPrintCol] = ch;
            }
            Invalidate();
        }

        public void HorizontalCharacterRepeat(int rowNum, int colNum, char ch)
        {
            HorizontalCharacterRepeat(rowNum, colNum, ch, 1);
        }

        /// <summary>
        /// 0 indexed
        /// </summary>
        /// <param name="rowNum"></param>
        /// <param name="colNum"></param>
        /// <param name="ch"></param>
        /// <param name="repeat"></param>
        public void HorizontalCharacterRepeat(int rowNum, int colNum, char ch, int repeat)
        {
            for (int i = 0; i < repeat; i++)
            {
                colNum += i;
                if (colNum > numCols)
                {
                    colNum = 0;
                    rowNum = (rowNum + 1) % numRows;
                }
                cells[rowNum][colNum] = ch;
            }
            Invalidate();
        }

        public void VerticalCharacterRepeat(int rowNum, int colNum, char ch)
        {
            VerticalCharacterRepeat(rowNum, colNum, ch, 1);
        }

        public void VerticalCharacterRepeat(int rowNum, int colNum, char ch, int repeat)
        {
            for (int i = 0; i < repeat; i++)
            {
                rowNum += i;
                if (rowNum > numRows)
                {
                    rowNum = 0;
                    colNum = (colNum + 1) % numCols;
                }
                cells[rowNum][colNum] = ch;
            }
            Invalidate();
        }

        public char GetCharacter(int rowNum, int colNum)
        {
            return cells[rowNum][colNum];
        }

        public void CharacterDefinition(char ch, string hexCodes)
        {
            characters.ModifyBitmap(ch, hexCodes);
        }

        private void ShiftRowsUp()
        {
            bottomRow = (bottomRow+1) % 24;
            for (int col = 0; col < numCols; col++)
            {
                cells[bottomRow][col] = space;
            }
        }
    }
}
