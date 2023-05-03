using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;

namespace SnowMapEditor
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
        public void Load(MapData mapData)
        {
            if (T == "Fence")
            {
                mapData.Data.Add(new Fence(P.ToList()) { Comment = C});
            }
            if (T == "Trail")
            {
                mapData.Data.Add(new Trail(P.ToList()) { Comment = C });
            }
            if (T == "Road")
            {
                mapData.Data.Add(new Road(P.ToList()) { Comment = C });
            }
            if(T == "Wall")
            {
                mapData.Data.Add(new Wall(P.ToList()) { Comment = C });
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
        public void Load(MapData mapData)
        {
            mapData.Data.Add(new Singular(new List<RealPoint>() { P }) { Comment = T });
        }
    }
    [DataContract]
    class Storage
    {
        [DataMember]
        public StoragePolyline[] P;
        [DataMember]
        public StorageSingular[] S;

        public Storage(MapData mapData)
        {
            P = mapData.Data.Where(d => !(d is Singular)).Select(d => new StoragePolyline() { P = d.Polyline.ToArray(), T = d.ObjectTypeName, C = d.Comment}).ToArray();
            S = mapData.Data.Where(d => d is Singular).Select(d => new StorageSingular() { P = d.Polyline[0], T = d.Comment }).ToArray();
        }
        public void Load(MapData mapData)
        {
            foreach(var p in P)
            {
                p.Load(mapData);
            }
            foreach (var s in S)
            {
                s.Load(mapData);
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
