using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SnowInTheNight
{
    static class TextWorks
    {
        static TextWorks()
        {
            Init();
        }

        public static int GetTextIndex (String text)
        {
            text = text.Replace("\r\n", "\n");
            if(!TextToIndex.ContainsKey(text))
            {
                AddText(new TextItem(text));
            }
            return TextToIndex[text];
        }

        public static String GetText (int index)
        {
            if(0 <= index && index < IndexToText.Count)
            {
                return IndexToText[index].GetText();
            }
            return String.Empty;
        }

        public static String GetText(String text)
        {
            return GetText(GetTextIndex(text));
        }

        public enum Lang {Russian, English}
        public static Lang Language = Lang.English;

        public class TextItem
        {
            public String Key;
            public String Rus;
            public String Eng;
            public String GetText()
            {
                switch(Language)
                {
                    case Lang.English:
                        return Eng;
                    case Lang.Russian:
                        return Rus;
                    default:
                        return Key;
                }
            }

            public TextItem() { }

            public TextItem(String key)
            {
                Key = key;
                Rus = key;
                Eng = "???";
            }
        }

        static Dictionary<String, int> TextToIndex = new Dictionary<String,int>();
        static List<TextItem> IndexToText = new List<TextItem>();

        public static void AddText(TextItem item)
        {
            TextToIndex.Add(item.Key, IndexToText.Count);
            IndexToText.Add(item);
        }

        public static void Save ()
        {
            var lines = new List<String>();
            foreach(var item in IndexToText)
            {
                lines.Add(item.Key + separator);
                lines.Add(item.Eng + separator);
            }

            File.WriteAllLines("Translations.txt", lines);
        }

        private static char separator = '|';

        public static void Init ()
        {
            var Texts = GetAllTexts();
            var s = 2;
            var l = Texts.Length - s + 1;
            for (int i = 0; i < l; i += s)
            {
                AddText(new TextItem()
                {
                    Key = Texts[i].TrimStart('\r', '\n'),
                    Rus = Texts[i].TrimStart('\r', '\n'),
                    Eng = Texts[i + 1].TrimStart('\r', '\n'),
                });
            }
        }

        private static String[] GetAllTexts ()
        {
            return Properties.Resources.TranslationData.Split('|');
        }
    }
}
