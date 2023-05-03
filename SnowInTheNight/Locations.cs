using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SnowInTheNight
{
    class Location
    {
        public int Id
        {
            get;
            private set;
        }

        public String Name
        {
            get;
            private set;
        }

        public String ThumbnailFilename
        {
            get;
            private set;
        }

        private Location(int id, String name, String thumbnailFilename)
        {
            this.Id = id;
            this.Name = name;
            this.ThumbnailFilename = thumbnailFilename;
        }

        public bool IsVisited()
        {
            return GameSaveManager.IsVisited(this);
        }

        public static Location Autosave = new Location(-1, TextWorks.GetText("В пути"), "autosave");
        public static Location DoctorsHome = new Location(0, TextWorks.GetText("Дом доктора"), "doctors_home");
        public static Location OldChurch = new Location(1, TextWorks.GetText("Старая церковь"), "old_church");
        public static Location Crossroads = new Location(2, TextWorks.GetText("Распутье"), "crossroads");
        public static Location ShepherdsHut = new Location(3, TextWorks.GetText("Хижина пастуха"), "shepherds_hut");
        public static Location Mill = new Location(4, TextWorks.GetText("Мельница"), "windmill");
        public static Location Graveyard = new Location(5, TextWorks.GetText("Кладбище"), "graveyard");
        public static Location Grove = new Location(6, TextWorks.GetText("Ельник"), "grove");
        public static Location Castle = new Location(7, TextWorks.GetText("Замок"), "castle");

        public static Location[] SaveLoadOrder = new Location[] {
            Castle,
            Grove,
            Graveyard,
            Mill,
            ShepherdsHut,
            Crossroads,
            OldChurch,
        };

        private static Dictionary<int, Location> LocationsDictionary;

        static Location()
        {
            var allLocations = new Location[]{
                DoctorsHome,
                OldChurch,
                Crossroads,
                ShepherdsHut,
                Mill,
                Graveyard,
                Grove,
                Castle
            };
            LocationsDictionary = allLocations.ToDictionary(l => l.Id);
        }

        public static explicit operator int(Location location)
        {
            return location.Id;
        }

        public static explicit operator Location(int id)
        {
            return FromId(id);
        }

        public static Location FromId(int id)
        {
            if (!LocationsDictionary.ContainsKey(id))
                return Autosave;
            return LocationsDictionary[id];
        }
    }
}
