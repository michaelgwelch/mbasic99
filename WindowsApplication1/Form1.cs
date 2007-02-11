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
    public partial class Form1 : Form
    {
        public delegate void PrintCallback(string message);

        public Form1()
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

        CharacterBitmap characters = new CharacterBitmap();
        private void PutChar(char c, int row, int col)
        {
            PutChar(characters[c], row, col);
        }
        private void PutChar(Image img, int row, int col)
        {
            using (Graphics g = CreateGraphics())
            {
                g.ScaleTransform(2f, 2f);
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.SmoothingMode = SmoothingMode.None;

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
            //bool a = this.GetStyle(ControlStyles.OptimizedDoubleBuffer);
            //bool b = this.GetStyle(ControlStyles.AllPaintingInWmPaint);
            //this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            //this.Width = 256 * 2;
            //this.Height = 30 * 8 * 2;
            //Bitmap aimg = FromString("003844447C444444");
            //Bitmap bimg = FromString("0078242438242478");
            //Bitmap cimg = FromString("0038444040404438");
            //Bitmap dimg = FromString("0078242424242478");
            //Bitmap eimg = FromString("007C40407840407C");
            //Bitmap fimg = FromString("007C404078404040");
            //Bitmap gimg = FromString("003C40405C444438");
            //Bitmap himg = FromString("004444447C444444");
            //Bitmap iimg = FromString("0038101010101038");
            //Bitmap jimg = FromString("0004040404044438");
            //Bitmap kimg = FromString("0044485060504844");
            //Bitmap limg = FromString("004040404040407C");
            //Bitmap mimg = FromString("00446C5454444444");
            //Bitmap nimg = FromString("00446464544C4C44");
            //Bitmap oimg = FromString("007C44444444447C");
            //Bitmap pimg = FromString("0078444478404040");
            //Bitmap qimg = FromString("0038444444544834");
            //Bitmap rimg = FromString("0078444478504844");
            //Bitmap simg = FromString("0038444038044438");
            //Bitmap timg = FromString("007C101010101010");
            //Bitmap uimg = FromString("0044444444444438");
            //Bitmap vimg = FromString("0044444428281010");
            //Bitmap wimg = FromString("0044444454545428");
            //Bitmap ximg = FromString("0044442810284444");
            //Bitmap yimg = FromString("0044442810101010");
            //Bitmap zimg = FromString("007C04081020407C");
            //Bitmap spaceimg = FromString("0000000000000000");

            //Bitmap[] bmaps = new Bitmap[] {
            //    aimg, bimg, cimg, dimg, eimg, fimg,
            //    gimg, himg, iimg, jimg, kimg, limg,
            //    mimg, nimg, oimg, pimg, qimg, rimg,
            //    simg, timg, uimg, vimg, wimg, ximg,
            //    yimg, zimg };
            //for (char c = 'A'; c < 'Z' + 1; c++) bitmaps[c] = bmaps[c-65];
            //bitmaps[' '] = spaceimg;
            //bitmaps['!'] = FromString("0010101010100010");
            //bitmaps['"'] = FromString("0028282800000000");
            //bitmaps['#'] = FromString("0028287C287C2828");
            //bitmaps['$'] = FromString("0038545038145438");
            //bitmaps['%'] = FromString("0060640810204C0C");
            //bitmaps['&'] = FromString("0020505020544834");
            //bitmaps['\''] = FromString("0008081000000000");
            //bitmaps['('] = FromString("0008102020201008");
            //bitmaps[')'] = FromString("0020100808081020");
            //bitmaps['*'] = FromString("000028107C102800");
            //bitmaps['+'] = FromString("000010107C101000");
            //bitmaps[','] = FromString("0000000000301020");
            //bitmaps['-'] = FromString("000000007C000000");
            //bitmaps['.'] = FromString("0000000000003030");
            //bitmaps['/'] = FromString("0000040810204000");

            //bitmaps['0'] = FromString("0038444444444438");
            //bitmaps['1'] = FromString("0010301010101038");
            //bitmaps['2'] = FromString("003844040810207C");
            //bitmaps['3'] = FromString("0038440418044438");
            //bitmaps['4'] = FromString("00081828487C0808");
            //bitmaps['5'] = FromString("007C407804044438");
            //bitmaps['6'] = FromString("0018204078444438");
            //bitmaps['7'] = FromString("007C040810202020");
            //bitmaps['8'] = FromString("0038444438444438");
            //bitmaps['9'] = FromString("003844443C040830");
            //bitmaps[':'] = FromString("0000303000303000");
            //bitmaps[';'] = FromString("0000303000301020");
            //bitmaps['<'] = FromString("0008102040201008");
            //bitmaps['='] = FromString("0000007C007C0000");
            //bitmaps['>'] = FromString("0020100804081020");
            //bitmaps['?'] = FromString("0038440408100010");
            //bitmaps['@'] = FromString("0038445C545C4038");

            //bitmaps['['] = FromString("0038202020202038");
            //bitmaps['\\'] = FromString("0000402010080400");
            //bitmaps[']'] = FromString("0038080808080838");
            //bitmaps['^'] = FromString("0000102844000000");
            //bitmaps['_'] = FromString("000000000000007C");
            //bitmaps[(char)96] = spaceimg;
            
            ////lowercase chars
            //bitmaps['a'] = FromString("00000038447C4444");
            //bitmaps['b'] = FromString("0000007824382478");
            //bitmaps['c'] = FromString("0000003C4040403C");
            //bitmaps['d'] = FromString("0000007824242478");
            //bitmaps['e'] = FromString("0000007C4078407C");
            //bitmaps['f'] = FromString("0000007C40784040");
            //bitmaps['g'] = FromString("0000003C405C4438");
            //bitmaps['h'] = FromString("00000044447C4444");
            //bitmaps['i'] = FromString("0000003810101038");
            //bitmaps['j'] = FromString("0000000808084830");
            //bitmaps['k'] = FromString("0000002428302824");
            //bitmaps['l'] = FromString("000000404040407C");
            //bitmaps['m'] = FromString("000000446C544444");
            //bitmaps['n'] = FromString("0000004464544C44");
            //bitmaps['o'] = FromString("0000007C4444447C");
            //bitmaps['p'] = FromString("0000007844784040");
            //bitmaps['q'] = FromString("0000003844544834");
            //bitmaps['r'] = FromString("0000007844784844");
            //bitmaps['s'] = FromString("0000003C40380478");
            //bitmaps['t'] = FromString("0000007C10101010");
            //bitmaps['u'] = FromString("0000004444444438");
            //bitmaps['v'] = FromString("0000004444282810");
            //bitmaps['w'] = FromString("0000004444545428");
            //bitmaps['x'] = FromString("0000004428102844");
            //bitmaps['y'] = FromString("0000004428101010");
            //bitmaps['z'] = FromString("0000007C0810207C");

            //bitmaps['{'] = FromString("0018202040202018");
            //bitmaps['|'] = FromString("0010101000101010");
            //bitmaps['}'] = FromString("0030080804080830");
            //bitmaps['~'] = FromString("0000205408000000");
            //bitmaps[(char)127] = spaceimg;

            screen1.Print("Hello, World!");
            screen1.Print("I'm a TI Basic Look alike");

            screen1.CharacterDefinition((char)128, "1898FF3D3C3CE404");
            screen1.CharacterDefinition((char)129, "1819FFBC3C3C2720");
            timer.Interval = 500;
            screen1.Color(13, TIColor.DarkRed, TIColor.White);
            timer.Tick += timer_Tick;
            timer.Start();

            screen1.BackColor = TIColor.DarkGreen;
        }

        bool image1 = false;
        void timer_Tick(object sender, EventArgs e)
        {
            if (image1) screen1.VerticalCharacterRepeat(12, 16, (char)128);
            else screen1.VerticalCharacterRepeat(12, 16, (char)129);
            image1 = !image1;


        }
        Timer timer = new Timer();
        int clickNum;
        private void button1_Click_1(object sender, EventArgs e)
        {
            /*
            using (Graphics g = CreateGraphics())
            {
                g.Clear(this.BackColor);
            }*/
            
            switch (clickNum)
            {
                case 0:
                    screen1.Print(" !\"#$%&'()*+,-./");
                    break;
                case 1:
                    screen1.Print("0123456789:;<=>?@");
                    break;
                case 2:
                    screen1.Print("ABCDEFGHIJKLMNOPQRSTUVWXYZ");
                    break;
                case 3:
                    screen1.Print("[\\]^_" + (char)96);
                    break;
                case 4:
                    screen1.Print("abcdefghijklmnopqrstuvwxyz");
                    break;
                case 5:
                    screen1.Print("{|}~" + (char)127);
                    break;
            }
            clickNum++;
        }


    }
}