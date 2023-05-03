using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SnowInTheNight;

namespace SnowMapEditor
{
    class MapData
    {
        public MapData()
        {
            var storage = Storage.Deserialize(SnowInTheNight.Hook.HookUpMapData());
            storage.Load(this);
            //var s = SnowWalkingTest.Properties.Resources.MapData;
        }
        public List<Editable> Data = new List<Editable>();

        public String Save()
        {
            var storage = new Storage(this);
            return storage.Serialize();
        }
    }
}
