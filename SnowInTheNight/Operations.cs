using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SnowInTheNight
{
    interface Operation
    {
        void DoIt(Interaction owner, MapState mapState, GameState gameState);
    }
    [Serializable]
    class GameEnd : Operation
    {
        public void DoIt(Interaction owner, MapState mapState, GameState gameState)
        {
            gameState.GameEnds = true;
        }
    }
    partial class Nomad : Operation
    {
        public void DoIt(Interaction owner, MapState mapState, GameState gameState)
        {
            timeToGetFurious = true;
        }
    }
}
