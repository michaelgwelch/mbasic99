using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Globalization;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace WindowsApplication1
{
    public partial class Screen : Form
    {
        public delegate void PrintCallback(string message);

        public Screen()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Bitmap bmp = new Bitmap(8, 8);
            
            //bmp.SetPixel(1, 1, Color.Red);
            //bmp.SetPixel(2, 2, Color.Red);
            //bmp.SetPixel(3, 3, Color.Red);
            //bmp.SetPixel(4, 4, Color.Red);

            
            
            //Graphics g = CreateGraphics();
            //g.DrawImage(bmp, 50, 100);
            
            //if (i == 0)
            //{
            //    g.DrawLine(new Pen(Color.Red), new Point(0, 0), new Point(100, 100));
            //}
            //else
            //{
            //    g.DrawLine(new Pen(Color.Black), new Point(0, 0), new Point(200, 200));
            //}
            //i++;


            //g.DrawImage(dimg, new Point(82, 200));


            //PutChar(aimg, 1, 1);
            //PutChar(bimg, 1, 2);
            //PutChar(cimg, 1, 3);
            //PutChar(dimg, 1, 4);
            //PutChar(eimg, 1, 5);
            //PutChar(fimg, 1, 6);
            //PutChar(gimg, 1, 7);
            //PutChar(himg, 1, 8);
            //PutChar(iimg, 1, 9);
            //PutChar(jimg, 1, 10);
            //PutChar(kimg, 1, 11);
            //PutChar(limg, 1, 12);
            //PutChar(mimg, 1, 13);
            //PutChar(nimg, 1, 14);
            //PutChar(oimg, 1, 15);
            //PutChar(pimg, 1, 16);
            //PutChar(qimg, 1, 17);
            //PutChar(rimg, 1, 18);
            //PutChar(simg, 1, 19);
            //PutChar(timg, 1, 20);
            //PutChar(uimg, 1, 21);
            //PutChar(vimg, 1, 22);
            //PutChar(wimg, 1, 23);
            //PutChar(ximg, 1, 24);
            //PutChar(yimg, 1, 25);
            //PutChar(zimg, 1, 26);

            //Bitmap aaimg = new Bitmap(@"C:\Documents and Settings\Michael Welch\Desktop\ti chars\A.PNG");
            //Bitmap bbimg = new Bitmap(@"C:\Documents and Settings\Michael Welch\Desktop\ti chars\B.PNG");
            //aaimg.MakeTransparent();
            //bbimg.MakeTransparent();
            //PutChar(aaimg, 2, 1);
            //PutChar(bbimg, 2, 2);
        }

        private void PutChar(char c, int row, int col)
        {
            PutChar(bitmaps[c], row, col);
        }
        private void PutChar(Image img, int row, int col)
        {
            using (Graphics g = CreateGraphics())
            {
                g.SmoothingMode = SmoothingMode.None;
                g.ScaleTransform(2f, 2f);

                g.DrawImage(img, new Point((col - 1) * 8, (row - 1) * 8));
            }
        }

        public void Print(string msg)
        {
            for (int i = 0; i < msg.Length; i++ )
            {
                char c = msg[i];
                int col = i + 3;
                PutChar(c, 24, col);
            }
        }

        private static Bitmap FromString(string hexCode)
        {
            Bitmap m = new Bitmap(8, 8);
            m.MakeTransparent();

            string padding = new string('0', 16 - hexCode.Length);
            hexCode = hexCode + padding;

            for (int j = 0; j < 8; j++)
            {
                String hexNumber = hexCode.Substring(j * 2, 2);
                byte b = Byte.Parse(hexNumber, NumberStyles.AllowHexSpecifier | NumberStyles.HexNumber);
                BitArray ba = new BitArray(new byte[] { b });
                for (int i = 0; i < 8; i++)
                {
                    if (ba[7-i]) m.SetPixel(i, j, Color.Black);
                }
            }
            return m;
        }

        private Dictionary<char, Bitmap> bitmaps = new Dictionary<char, Bitmap>();

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Width = 256 * 2;
            this.Height = 30 * 8 * 2;
            Bitmap aimg = FromString("003844447C444444");
            Bitmap bimg = FromString("0078242438242478");
            Bitmap cimg = FromString("0038444040404438");
            Bitmap dimg = FromString("0078242424242478");
            Bitmap eimg = FromString("007C40407840407C");
            Bitmap fimg = FromString("007C404078404040");
            Bitmap gimg = FromString("003C40405C444438");
            Bitmap himg = FromString("004444447C444444");
            Bitmap iimg = FromString("0038101010101038");
            Bitmap jimg = FromString("0004040404044438");
            Bitmap kimg = FromString("0044485060504844");
            Bitmap limg = FromString("004040404040407C");
            Bitmap mimg = FromString("00446C5454444444");
            Bitmap nimg = FromString("00446464544C4C44");
            Bitmap oimg = FromString("007C44444444447C");
            Bitmap pimg = FromString("0078444478404040");
            Bitmap qimg = FromString("0038444444544834");
            Bitmap rimg = FromString("0078444478504844");
            Bitmap simg = FromString("0038444038044438");
            Bitmap timg = FromString("007C101010101010");
            Bitmap uimg = FromString("0044444444444438");
            Bitmap vimg = FromString("0044444428281010");
            Bitmap wimg = FromString("0044444454545428");
            Bitmap ximg = FromString("0044442810284444");
            Bitmap yimg = FromString("0044442810101010");
            Bitmap zimg = FromString("007C04081020407C");
            Bitmap spaceimg = FromString("0000000000000000");

            Bitmap[] bmaps = new Bitmap[] {
                aimg, bimg, cimg, dimg, eimg, fimg,
                gimg, himg, iimg, jimg, kimg, limg,
                mimg, nimg, oimg, pimg, qimg, rimg,
                simg, timg, uimg, vimg, wimg, ximg,
                wimg, zimg };
            for (char c = 'A'; c < 'Z' + 1; c++) bitmaps[c] = bmaps[c-65];
            bitmaps[' '] = spaceimg;
            

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            
            Print("HELLO THERE");
        }


    }
}