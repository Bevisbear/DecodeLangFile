using System;

namespace DecodeLangFile
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var parser = new ParseLangFile();
            parser.ReadLangFile(@"D:\eso_zh\import\update30\en.lang");
            //parser.ReadLangFile(@"C:\Users\Bevis\Documents\Elder Scrolls Online\live\AddOns\gamedata\lang\zh.lang");
        }
    }
}
