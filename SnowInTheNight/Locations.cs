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

        private String name;

        public String Name
        {
            get
            {
                return TextWorks.GetText(name);
            }
        }

        public String ThumbnailFilename
        {
            get;
            private set;
        }

        private Location(int id, String name, String thumbnailFilename)
        {
            this.Id = id;
            this.name = name;
            this.ThumbnailFilename = thumbnailFilename;
        }

        public bool IsVisited()
        {
            return GameSaveManager.IsVisited(this);
        }

        public static Location Autosave = new Location(-1, "В пути", "autosave");
        public static Location DoctorsHome = new Location(0, "Дом доктора", "doctors_home");
        public static Location OldChurch = new Location(1, "Старая церковь", "old_church");
        public static Location Crossroads = new Location(2, "Распутье", "crossroads");
        public static Location ShepherdsHut = new Location(3, "Хижина пастуха", "shepherds_hut");
        public static Location Mill = new Location(4, "Мельница", "windmill");
        public static Location Graveyard = new Location(5, "Кладбище", "graveyard");
        public static Location Grove = new Location(6, "Ельник", "grove");
        public static Location Castle = new Location(7, "Замок", "castle");

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
