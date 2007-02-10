using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Globalization;
using System.Collections;

namespace WindowsApplication1
{
    class CharacterBitmap
    {
        private Dictionary<char, Bitmap> bitmaps = new Dictionary<char, Bitmap>();
        private Dictionary<int, TIColor> foreColors = new Dictionary<int, TIColor>();
        private Dictionary<int, TIColor> backColors = new Dictionary<int, TIColor>();
        private Dictionary<char, string> codes = new Dictionary<char, string>();
        public CharacterBitmap()
        {
            for (int i = 1; i <= 16; i++)
            {
                foreColors[i] = TIColor.Black;
                backColors[i] = TIColor.Transparent;
            }

            for (char ch = char.MinValue; ch < 160; ch++) bitmaps[ch] = new Bitmap(8, 8);
            // non defined chars
            for (char ch = (char)128; ch < 160; ch++) ModifyBitmap(ch, "");
             
            // defined chars 32-127
            ModifyBitmap(' ',"");
            ModifyBitmap('!',"0010101010100010");
            ModifyBitmap('"',"0028282800000000");
            ModifyBitmap('#',"0028287C287C2828");
            ModifyBitmap('$',"0038545038145438");
            ModifyBitmap('%',"0060640810204C0C");
            ModifyBitmap('&',"0020505020544834");
            ModifyBitmap('\'', "0008081000000000");
            ModifyBitmap('(',"0008102020201008");
            ModifyBitmap(')',"0020100808081020");
            ModifyBitmap('*',"000028107C102800");
            ModifyBitmap('+',"000010107C101000");
            ModifyBitmap(',',"0000000000301020");
            ModifyBitmap('-',"000000007C000000");
            ModifyBitmap('.',"0000000000003030");
            ModifyBitmap('/',"0000040810204000");

            ModifyBitmap('0',"0038444444444438");
            ModifyBitmap('1',"0010301010101038");
            ModifyBitmap('2',"003844040810207C");
            ModifyBitmap('3',"0038440418044438");
            ModifyBitmap('4',"00081828487C0808");
            ModifyBitmap('5',"007C407804044438");
            ModifyBitmap('6',"0018204078444438");
            ModifyBitmap('7',"007C040810202020");
            ModifyBitmap('8',"0038444438444438");
            ModifyBitmap('9',"003844443C040830");
            ModifyBitmap(':',"0000303000303000");
            ModifyBitmap(';',"0000303000301020");
            ModifyBitmap('<',"0008102040201008");
            ModifyBitmap('=',"0000007C007C0000");
            ModifyBitmap('>',"0020100804081020");
            ModifyBitmap('?',"0038440408100010");
            ModifyBitmap('@',"0038445C545C4038");

            // upper case chars
            ModifyBitmap('A',"003844447C444444");
            ModifyBitmap('B',"0078242438242478");
            ModifyBitmap('C',"0038444040404438");
            ModifyBitmap('D',"0078242424242478");
            ModifyBitmap('E',"007C40407840407C");
            ModifyBitmap('F',"007C404078404040");
            ModifyBitmap('G',"003C40405C444438");
            ModifyBitmap('H',"004444447C444444");
            ModifyBitmap('I',"0038101010101038");
            ModifyBitmap('J',"0004040404044438");
            ModifyBitmap('K',"0044485060504844");
            ModifyBitmap('L',"004040404040407C");
            ModifyBitmap('M',"00446C5454444444");
            ModifyBitmap('N',"00446464544C4C44");
            ModifyBitmap('O',"007C44444444447C");
            ModifyBitmap('P',"0078444478404040");
            ModifyBitmap('Q',"0038444444544834");
            ModifyBitmap('R',"0078444478504844");
            ModifyBitmap('S',"0038444038044438");
            ModifyBitmap('T',"007C101010101010");
            ModifyBitmap('U',"0044444444444438");
            ModifyBitmap('V',"0044444428281010");
            ModifyBitmap('W',"0044444454545428");
            ModifyBitmap('X',"0044442810284444");
            ModifyBitmap('Y',"0044442810101010");
            ModifyBitmap('Z',"007C04081020407C");


            ModifyBitmap('[',"0038202020202038");
            ModifyBitmap('\\', "0000402010080400");
            ModifyBitmap(']',"0038080808080838");
            ModifyBitmap('^',"0000102844000000");
            ModifyBitmap('_',"000000000000007C");
            ModifyBitmap((char)96, "");

            //lowercase chars
            ModifyBitmap('a',"00000038447C4444");
            ModifyBitmap('b',"0000007824382478");
            ModifyBitmap('c',"0000003C4040403C");
            ModifyBitmap('d',"0000007824242478");
            ModifyBitmap('e',"0000007C4078407C");
            ModifyBitmap('f',"0000007C40784040");
            ModifyBitmap('g',"0000003C405C4438");
            ModifyBitmap('h',"00000044447C4444");
            ModifyBitmap('i',"0000003810101038");
            ModifyBitmap('j',"0000000808084830");
            ModifyBitmap('k',"0000002428302824");
            ModifyBitmap('l',"000000404040407C");
            ModifyBitmap('m',"000000446C544444");
            ModifyBitmap('n',"0000004464544C44");
            ModifyBitmap('o',"0000007C4444447C");
            ModifyBitmap('p',"0000007844784040");
            ModifyBitmap('q',"0000003844544834");
            ModifyBitmap('r',"0000007844784844");
            ModifyBitmap('s',"0000003C40380478");
            ModifyBitmap('t',"0000007C10101010");
            ModifyBitmap('u',"0000004444444438");
            ModifyBitmap('v',"0000004444282810");
            ModifyBitmap('w',"0000004444545428");
            ModifyBitmap('x',"0000004428102844");
            ModifyBitmap('y',"0000004428101010");
            ModifyBitmap('z',"0000007C0810207C");

            ModifyBitmap('{',"0018202040202018");
            ModifyBitmap('|',"0010101000101010");
            ModifyBitmap('}',"0030080804080830");
            ModifyBitmap('~',"0000205408000000");
            ModifyBitmap((char)127, "");
        }

        public Bitmap this[char c]
        {
            get { return Get(c); }
        }

        public Bitmap Get(char c)
        {
            if (c < 32 || c > 159) throw new ArgumentOutOfRangeException("c");
            return bitmaps[c];
        }

        public void Color(int charSet, TIColor foreColor, TIColor backColor)
        {
            foreColors[charSet] = foreColor;
            backColors[charSet] = backColor;
            for (int i = 0; i < 8; i++)
            {
                char ch = CalculateCharacter(charSet, i);
                ModifyBitmap(ch, codes[ch]);
            }
        }
        private static char CalculateCharacter(int charSet, int offset)
        {
            return (char)(32 + offset + (charSet - 1) * 8);
        }

        private static int GetCharacterSet(char c)
        {
            return ((c - 32) / 8) + 1;
        }

        public void ModifyBitmap(char c, string hexCode)
        {
            int charSet = GetCharacterSet(c);
            TIColor foreColor = foreColors[charSet];
            TIColor backColor = backColors[charSet];
            Bitmap m = bitmaps[c];
            string padding = new string('0', 16 - hexCode.Length);
            hexCode = hexCode + padding;
            codes[c] = hexCode;
            for (int j = 0; j < 8; j++)
            {
                String hexNumber = hexCode.Substring(j * 2, 2);
                byte b = Byte.Parse(hexNumber, NumberStyles.AllowHexSpecifier | NumberStyles.HexNumber);
                BitArray ba = new BitArray(new byte[] { b });
                for (int i = 0; i < 8; i++)
                {
                    if (ba[7 - i]) m.SetPixel(i, j, foreColor);
                    else m.SetPixel(i, j, backColor);
                }
            }
        }

        public Bitmap Space
        {
            get
            {
                return Get(' ');
            }
        }
    }
}
