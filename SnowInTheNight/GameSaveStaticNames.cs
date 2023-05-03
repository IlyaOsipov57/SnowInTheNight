using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SnowInTheNight
{
    static class StaticNames
    {
        public static String autosaveFilename = "autosavedata";
        public static String autosaveThumbnailFilename = "en_route";
        public static String saveFilename = "savedata";
        public static String metaFilename = "meta";

        public static String SavesDirectoryPath = "Saves";
        public static String SaveFilename = "Save";
        public static String SaveFilepath
        {
            get
            {
                return Path.Combine(SavesDirectoryPath, SaveFilename);
            }
        }
    }
}
