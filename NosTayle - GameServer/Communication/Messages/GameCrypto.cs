using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.Communication.Messages
{
    class GameCrypto
    {
        public static string DecryptSessionPacket(byte[] bytes)
        {
            string encrypted_string = "";

            for (int i = 1; i < bytes.Length; i++)
            {
                if (bytes[i] == 0xE) { return encrypted_string; }

                int firstbyte = (int)(bytes[i] - 0xF);
                int secondbyte = firstbyte;
                secondbyte &= 0xF0;
                firstbyte = (int)(firstbyte - secondbyte);
                secondbyte >>= 0x4;

                switch (secondbyte)
                {
                    case 0:
                        encrypted_string += ' ';
                        break;

                    case 1:
                        encrypted_string += ' ';
                        break;

                    case 2:
                        encrypted_string += '-';
                        break;

                    case 3:
                        encrypted_string += '.';
                        break;

                    default:
                        secondbyte += 0x2C;
                        encrypted_string += (char)secondbyte;
                        break;
                }

                switch (firstbyte)
                {
                    case 0:
                        encrypted_string += ' ';
                        break;

                    case 1:
                        encrypted_string += ' ';
                        break;

                    case 2:
                        encrypted_string += '-';
                        break;

                    case 3:
                        encrypted_string += '.';
                        break;

                    default:
                        firstbyte += 0x2C;
                        encrypted_string += (char)firstbyte;
                        break;
                }
            }

            return encrypted_string;
        }

        public static string DecryptGame2(string str)
        {
            List<byte> receiveData = new List<byte>();
            char[] table = { ' ', '-', '.', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'n' };
            int count = 0;
            for (count = 0; count < str.Length; count++)
            {
                if ((int)str[count] <= 0x7A)
                {
                    int len = (int)str[count];

                    for (int i = 0; i < len; i++)
                    {
                        count++;

                        try
                        {
                            receiveData.Add(unchecked((byte)(str[count] ^ 0xFF)));
                        }
                        catch
                        {
                            receiveData.Add(255);
                        }
                    }
                }
                else
                {
                    int len = (int)str[count];
                    len &= (int)0x7F;

                    for (int i = 0; i < (int)len; i++)
                    {
                        count++;
                        int highbyte = 0;
                        try
                        {
                            highbyte = (int)str[count];
                        }
                        catch
                        {
                            highbyte = 0;
                        }
                        highbyte &= 0xF0;
                        highbyte >>= 0x4;

                        int lowbyte = 0;
                        try
                        {
                            lowbyte = (int)str[count];
                        }
                        catch
                        {
                            lowbyte = 0;
                        }
                        lowbyte &= 0x0F;

                        if (highbyte != 0x0 && highbyte != 0xF)
                        {
                            receiveData.Add(unchecked((byte)table[highbyte - 1]));
                            i++;
                        }

                        if (lowbyte != 0x0 && lowbyte != 0xF)
                        {
                            receiveData.Add(unchecked((byte)table[lowbyte - 1]));
                        }
                    }
                }
            }
            return Encoding.GetEncoding(1258).GetString(receiveData.ToArray());
        }

        public static string DecryptGame(int session_id, byte[] str, int length)
        {
            string encrypted_string = "";
            int session_key = session_id & 0xFF;
            byte session_number = unchecked((byte)(session_id >> 6));
            session_number &= unchecked((byte)0xFF);
            session_number &= unchecked((byte)0x80000003);

            switch (session_number)
            {
                case 0:
                    for (int i = 0; i < length; i++)
                    {
                        byte firstbyte = unchecked((byte)(session_key + 0x40));
                        byte highbyte = unchecked((byte)(str[i] - firstbyte));
                        encrypted_string += (char)highbyte;
                    }
                    break;

                case 1:
                    for (int i = 0; i < length; i++)
                    {
                        byte firstbyte = unchecked((byte)(session_key + 0x40));
                        byte highbyte = unchecked((byte)(str[i] + firstbyte));
                        encrypted_string += (char)highbyte;
                    }
                    break;

                case 2:
                    for (int i = 0; i < length; i++)
                    {
                        byte firstbyte = unchecked((byte)(session_key + 0x40));
                        byte highbyte = unchecked((byte)(str[i] - firstbyte ^ 0xC3));
                        encrypted_string += (char)highbyte;
                    }
                    break;

                case 3:
                    for (int i = 0; i < length; i++)
                    {
                        byte firstbyte = unchecked((byte)(session_key + 0x40));
                        byte highbyte = unchecked((byte)(str[i] + firstbyte ^ 0xC3));
                        encrypted_string += (char)highbyte;
                    }
                    break;

                default:
                    encrypted_string += (char)0xF;
                    break;
            }


            string[] temp = encrypted_string.Split((char)0xFF);
            string save = "";

            for (int i = 0; i < temp.Length; i++)
            {
                save += DecryptGame2(temp[i]);
                if (i < temp.Length - 2)
                    save += (char)0xFF;
            }

            return save;
        }

        public static byte[] Encrypt(string str)
        {
            byte[] bytesBase = Encoding.GetEncoding(1258).GetBytes(str);

            int length = str.Length;
            int secondlength = (length / 122);
            int zaehler = 0;
            List<byte> bytes = new List<byte>();

            for (int i = 0; i < length; i++)
            {
                if (i == (122 * zaehler))
                {
                    if (secondlength == 0)
                        bytes.Add((byte)(Math.Abs((((length / 122) * 122) - length))));
                    else
                    {
                        bytes.Add((byte)0x7A);
                        secondlength--;
                        zaehler++;
                    }
                }

                bytes.Add((byte)(bytesBase[i] ^ 0xFF));
            }


            bytes.Add((byte)0xFF);
            return bytes.ToArray();
        }

        public static string GetRealString(string str)
        {
            byte[] bytes = Encoding.Default.GetBytes(str);
            return Encoding.GetEncoding(1258).GetString(bytes);
        }
    }
}
