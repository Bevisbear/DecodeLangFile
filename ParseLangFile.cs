using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace DecodeLangFile
{
    public class ParseLangFile
    {
        protected List<List<EsoLangDto>> _data = new List<List<EsoLangDto>>();
        protected bool _hasHeaderRow;
        private uint _filesize;

        private const uint _textIdRecoredSize = 16;
        private uint _recoredCount;
        private uint _fileId = 2;


        /// <summary>
        /// Decode ESO lang file
        /// 
        /// lang file format: https://en.uesp.net/wiki/Online_Mod:Language_Data_File_Format
        /// 
        /// Those code was helped me.
        /// https://github.com/uesp/uesp-esoapps/blob/master/common/EsoLangFile.cpp
        /// </summary>
        /// <param name="path">Your lang file path</param>
        public void ReadLangFile(string path)
        {
            byte[] buffer = new byte[8];
            byte[] langIdBuffer = new byte[16];
            uint textBeginOffset;

            byte[] data = File.ReadAllBytes(path);

            _filesize = (uint)data.Length;

            Array.Copy(data, buffer, 8);
            Array.Reverse(buffer, 0, buffer.Length);  //Reverse bytes order, new readed on head.

            _fileId = (uint)BitConverter.ToInt32(buffer, 4);
            _recoredCount = (uint)BitConverter.ToInt32(buffer, 0);

            Console.WriteLine("field Id: {0}", _fileId);
            Console.WriteLine("count int: {0}", _recoredCount);

            textBeginOffset = _recoredCount * _textIdRecoredSize + 8;
            Console.WriteLine("textBeginOffset: {0}", textBeginOffset);

            if (data == null || data.Length <= 0)
            {
                throw new Exception("Error: Invaild data!");
            }

            if (_filesize < 8)
            {
                throw new Exception("Error: Invaild Lang file size!");
            }

            if (_filesize > int.MaxValue)
            {
                throw new Exception("Error: Lang file too big");
            }


            byte[] textUtf8Buffer = new byte[1];

            for (uint i = 0; i < _recoredCount; ++i)
            {
                uint offset = 8 + i * _textIdRecoredSize;
               
                //Console.WriteLine("Offset: {0}", offset);

                Array.Copy(data, offset, langIdBuffer, 0, langIdBuffer.Length);
                Array.Reverse(langIdBuffer, 0, langIdBuffer.Length);

                //uint langIdOffset = (uint)BitConverter.ToInt32(langIdBuffer, 0);
                

                EsoLangDto lang = new EsoLangDto
                {
                    Id = (uint)BitConverter.ToInt32(langIdBuffer, 12),
                    Uknown = (uint)BitConverter.ToInt32(langIdBuffer, 8),
                    Index = (uint)BitConverter.ToInt32(langIdBuffer, 4),
                    Offset = (uint)BitConverter.ToInt32(langIdBuffer, 0),
                    //Text = text,
                };

                uint textOffset = lang.Offset + textBeginOffset;

                if (textOffset < _filesize)
                {
                    string textbuffer = "";

                    for (int c = 0; c + textOffset < _filesize; ++c)
                    {
                        int ost = c + (int)textOffset;
                        Array.Copy(data, ost, textUtf8Buffer, 0, textUtf8Buffer.Length);

                        var hex = BitConverter.ToString(textUtf8Buffer);

                        if (hex == "00")
                        {
                            break;
                        }
                        else
                        {
                            textbuffer += hex;
                        }
                    }

                    byte[] stringByte = FromHex(textbuffer);
                    string text = Encoding.UTF8.GetString(stringByte);

                    lang.Text = text;

                    //Console.WriteLine("text: {0}", text);
                }

                Console.WriteLine("id: {0}, unknwon: {1}, index: {2}, offset: {3}, text: {4}", 
                    lang.Id, lang.Uknown, lang.Index, lang.Offset, lang.Text);
            }

        }

        public static byte[] FromHex(string hex)
        {
            hex = hex.Replace("-", "");
            byte[] raw = new byte[hex.Length / 2];
            for (int i = 0; i < raw.Length; i++)
            {
                raw[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }
            return raw;
        }
    }

    public class EsoLangDto
    {
        public uint Id { get; set; }
        public uint Uknown { get; set; }
        public uint Index { get; set; }
        public uint Offset { get; set; }
        public string Text { get; set; }
    }
}
