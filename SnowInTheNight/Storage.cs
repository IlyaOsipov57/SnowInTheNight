using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace SnowInTheNight
{
    [DataContract]
    class StoragePolyline
    {
        [DataMember]
        public String T;
        [DataMember]
        public String C;
        [DataMember]
        public RealPoint[] P;
        public void Load(MapState mapState)
        {
            if (P.Length == 1)
                P = new RealPoint[] { P[0], P[0] };
            var spl = C.Split(':');
            var seed = 0;
            if (!int.TryParse(spl[0], out seed))
                seed = 0;
            if (T == "Fence")
            {
                mapState.Decorations.Add(new Fence(P) { Seed = seed});
                mapState.Colliders.AddRange(ColliderWorks.GeneratePolyline(P));
            }
            if (T == "Wall")
            {
                mapState.Decorations.Add(new Wall(P) { Seed = seed });
                mapState.Colliders.AddRange(ColliderWorks.GeneratePolyline(P));
            }
            if (T == "Trail")
            {
                if (spl.Length > 1)
                {
                    if (spl[1].ToUpper() == "BEGGAR")
                    {
                        mapState.SetBeggarPath(P, seed);
                        return;
                    }

                    if (spl[1].ToUpper() == "DARKDOCTOR")
                    {
                        mapState.SetDarkDoctorPath(P, seed);
                        return;
                    }
                }
                mapState.Steps.AddRange(StepsGenerator.Generate(P,seed, 0.3));
            }
            if (T == "Road")
            {
                if (C.ToUpper() == "PAVED")
                {
                    mapState.PavedRoad.AddRange(ColliderWorks.GeneratePolyline(50, P));
                }
                if (spl.Length > 1 && spl[1].ToUpper() == "PAVED")
                {
                    mapState.PavedRoad.AddRange(ColliderWorks.GeneratePolyline(seed, P));
                }
                else if (C.ToUpper() == "FOLLOWAREA")
                {
                    mapState.DarkAnticolliders.AddRange(ColliderWorks.GeneratePolyline(P));
                }
                else
                {
                    mapState.Anticolliders.AddRange(ColliderWorks.GeneratePolyline(seed, P));
                }
            }
        }
    }

    [DataContract]
    class StorageSingular
    {
        [DataMember]
        public String T;
        [DataMember]
        public RealPoint P;
        public void Load(MapState mapState)
        {
            var seed = 0;
            var tt = T.Split(':');
            var t1 = tt[0].Trim();
            var t2 = tt.Length > 1 ? tt[1] : "0";
            if (!int.TryParse(t2, out seed))
                seed = 0;
            switch (t1.ToUpper())
            {
                case "FIREPLACE":
                    mapState.AddFirePlace(P, seed);
                    break;
                case "MAID":
                    mapState.AddMaid(P);
                    break;
                case "HOME":
                    mapState.AddDoctorsHouse(P);
                    break;
                case "SLED":
                    mapState.AddSled(P);
                    break;
                case "BELL":
                    mapState.AddBell(P);
                    break;
                case "CHURCH":
                    mapState.Addchurch(P);
                    break;
                case "GRAVE":
                    mapState.AddGrave(P,seed);
                    break;
                case "HOUSE1":
                    mapState.AddHouse1(P, seed);
                    break;
                case "HOUSE2":
                    mapState.AddHouse2(P, seed);
                    break;
                case "HOUSE3":
                    mapState.AddHouse3(P, seed);
                    break;
                case "HOUSE4":
                    mapState.AddHouse4(P, seed);
                    break;
                case "RATHOUSE":
                    mapState.AddRatHouse(P);
                    break;
                case "BEGGAR":
                    if (t2.ToUpper() == "DEAD")
                    {
                        mapState.AddDeadBeggar(P);
                    }
                    else
                    {
                        mapState.AddBeggar(P);
                    }
                    break;
                case "TREE":
                    mapState.AddTree(P, seed);
                    break;
                case "COALHOUSE":
                    mapState.AddCoalHouse(P, seed);
                    break;
                case "BUCKET":
                    mapState.AddLostBucket(P);
                    break;
                case "ANOMALY":
                    mapState.AddAnomaly(P);
                    break;
                case "ELDER":
                    mapState.AddElder(P);
                    break;
                case "STUMP":
                    mapState.AddStump(P, seed);
                    break;
                case "POST":
                    mapState.AddPostSign(P, seed);
                    break;
                case "CART":
                    mapState.AddCart(P);
                    break;
                case "DOGHOUSE":
                    mapState.AddDogHouse(P);
                    break;
                case "STABLE":
                    mapState.AddStable(P);
                    break;
                case "CRYPT":
                    mapState.AddCrypt(P);
                    break;
                case "DOOR":
                    mapState.AddCastleDoor(P);
                    break;
                case "GRAVESTONE":
                    mapState.AddGraveStone(P,seed);
                    break;
                case "WELL":
                    mapState.AddWell(P);
                    break;
                case "MADMAN":
                    mapState.AddMadman(P);
                    break;
                case "GHOST":
                    mapState.AddGhost(P);
                    break;
                case "MILL":
                    mapState.AddMill(P);
                    break;
                case "GUARD":
                    mapState.AddGuard(P);
                    break;
                case "NOMAD":
                    mapState.AddNomad(P);
                    break;
                case "MAPTEXT":
                    mapState.AddMapText(P, seed);
                    break;
            }
        }
    }
    [DataContract]
    class Storage
    {
        [DataMember]
        public StoragePolyline[] P;
        [DataMember]
        public StorageSingular[] S;

        public void Load(MapState mapState)
        {
            foreach (var p in P)
            {
                p.Load(mapState);
            }
            foreach(var s in S)
            {
                s.Load(mapState);
            }
        }

        public String Serialize()
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(Storage));
            using (MemoryStream stream = new MemoryStream())
            {
                serializer.WriteObject(stream, this);
                stream.Position = 0;
                using (StreamReader sr = new StreamReader(stream))
                {
                    string json = sr.ReadToEnd();
                    return json;
                }
            }
        }
        public static Storage Deserialize(String json)
        {
            try
            {
                var bytes = Encoding.UTF8.GetBytes(json);
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(Storage));
                using (MemoryStream stream = new MemoryStream(bytes))
                {
                    stream.Position = 0;
                    return (Storage)serializer.ReadObject(stream);
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
