using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SnowInTheNight
{
    [Serializable]
    class MapState
    {
        public int Version = 0;
        public static int currentVersion = 0;

        public static double LegLength = 8;
        public static double StepLength = 3;
        public RealPoint LeftLeg = (RealPoint)IntPoint.Left * LegLength;
        public RealPoint RightLeg = (RealPoint)IntPoint.Right * LegLength;
        public List<Step> Steps = new List<Step>();

        [NonSerialized]
        public List<Collider> Colliders = new List<Collider>();
        [NonSerialized]
        public List<DrawableCollider> Anticolliders = new List<DrawableCollider>();
        [NonSerialized]
        public List<DrawableCollider> PavedRoad = new List<DrawableCollider>();
        [NonSerialized]
        public List<Collider> DarkAnticolliders = new List<Collider>();
        [NonSerialized]
        public List<Decoration> Decorations = new List<Decoration>();

        public List<Collider> DynamicColliders = new List<Collider>();
        public List<Decoration> DynamicDecorations = new List<Decoration>();
        public List<Interaction> Interactions = new List<Interaction>();
        public List<Item> Items = new List<Item>();
        public List<Script> Scripts = new List<Script>();
        public HashSet<IntPoint> Visited = new HashSet<IntPoint>();
        public double PlayerHealth = 1;
        private NastyWoman TheNastyWoman;
        private RealPoint[] BeggarPath;
        private int BeggarPathSeed;
        public RealPoint Anomaly;
        public int Wraps = 0;
        public ContinuousStepsGenerator DarkDoctor;
        public static ContinuousStepsGenerator IntroDoctor = new ContinuousStepsGenerator(new RealPoint(0, 20), new RealPoint(1, 0), 10, 0.8) { SpeedPercentage = 30, Uncollidable = true};
        public static List<Step> IntroSteps = StepsGenerator.Generate(new RealPoint[]{new RealPoint(-150,250), new RealPoint(0,20)}, 10, 0.3).ToList();
        public bool HasMetMadman = false;
        public int VillageDialogueState = 0;
        public bool LanternIsOn_SaveValue;
        public double Time_SaveValue;
        public RealPoint GuardPosition;
        public bool PlayerHasLearnedHowToExitItemMenu = false;

        public MapState()
        {
            Storage.Deserialize(SnowInTheNight.Properties.Resources.MapData).Load(this);
            
            Anticolliders = ColliderWorks.Reorder(Anticolliders).ToList();
            Decorations = ColliderWorks.Reorder(Decorations).ToList();
            Steps = Steps
                .Where(s => ColliderWorks.AntiCollide(Anticolliders, s.Position).SquaredLength < 10000)
                .Where(s => ColliderWorks.AntiCollide(PavedRoad, s.Position).SquaredLength > 0)
                .ToList();
            
            Items.Add(new ItemBag());
            Items.Add(new ItemKey());
        }

        public void AddItem(Item item, GameState gameState)
        {
            Items.Add(item);
            ReorderItems();
            gameState.SelectedItem = Items.IndexOf(item);
        }
        public void ReorderItems()
        {
            Items = Items.OrderBy(i => i.Index).ToList();
        }

        internal void AddTree(RealPoint position, int seed)
        {
            Decorations.Add(new Tree(position, seed));
            Colliders.Add(new CircleCollider(position, 10));
        }

        internal void AddBeggar(RealPoint point)
        {
            var beggar = new Beggar(point);
            DynamicDecorations.Add(beggar);
            DynamicColliders.Add(beggar);
            var beggarInteraction = new Character(point, 70, 170, beggar);
            Interactions.Add(beggarInteraction);
            beggarInteraction.SpeechTextBubble.Position = point + new RealPoint(0, -40);
        }

        internal void AddBell(RealPoint point)
        {
            var bellGate = new BellGate(point);
            DynamicDecorations.Add(bellGate);
            Interactions.Add(bellGate);
            Colliders.AddRange(ColliderWorks.GeneratePolyline(bellGate.GetBorder()));
            //Anticolliders.Add(new CircleCollider(point, 100));
        }

        internal void Addchurch(RealPoint position)
        {
            var church = new ChurchHouse(position.X, position.Y);
            Decorations.Add(church);
            Colliders.AddRange(ColliderWorks.GeneratePolygon(church.GetBorder()));

            var door = new StaticInteraction(position - new RealPoint(27, 40), 100, new RealPoint(1, -1), "Эта церковь заброшена.\r\nНо недавно тут кто-то был.");
            door.DisableOnUse = true;
            Interactions.Add(door);
        }
        internal void AddRatHouse(RealPoint position)
        {
            var rathouse = new RatHouse(position.X, position.Y);
            DynamicDecorations.Add(rathouse);
            Colliders.AddRange(ColliderWorks.GeneratePolygon(rathouse.GetBorder()));

            Scripts.Add(new RatHouseScript(position));

            var door = new StaticInteraction(position - new RealPoint(0, 30), 75, new RealPoint(0, -1), "");
            door.DisableOnUse = true;
            door.OnInteraction = new GameEnd();
            Interactions.Add(door);
        }
        internal void AddFirePlace(RealPoint position, int id)
        {
            var firePlace = new FirePlace(position, id);
            DynamicDecorations.Add(firePlace);
            Interactions.Add(firePlace);
            Colliders.Add(new CircleCollider(position + new RealPoint(0,5), 15));

            var mapText = new MapText(position, firePlace.Id + 10);
            Decorations.Add(mapText);
        }
        internal void AddDoctorsHouse(RealPoint position)
        {
            var house = new DoctorsHouse(position.X, position.Y);
            Decorations.Add(house);
            Colliders.AddRange(ColliderWorks.GeneratePolygon(house.GetBorder()));

            var door = new StaticInteraction(position - new RealPoint(52, 40), 100, new RealPoint(1, -1), "Я уже запер дверь.\r\nВозвращаться - плохая примета.");
            door.DisableOnUse = true;
            Interactions.Add(door);
        }
        internal void AddSled(RealPoint position)
        {
            var sled = new SledDecoration(position);
            Decorations.Add(sled);
            Colliders.AddRange(ColliderWorks.GeneratePolygon(sled.GetBorder()));
        }
        internal void AddMaid(RealPoint position)
        {
            var maid = new Maid(position);
            var maidInteraction = new Character(position, 70, 100, maid);
            maidInteraction.SpeechTextBubble.Position = position + new RealPoint(0, -50);
            Interactions.Add(maidInteraction);
            DynamicDecorations.Add(maid);
            DynamicColliders.Add(maid);
        }

        [Serializable]
        public class Step
        {
            public Step()
            {

            }
            public Step(double positionX, double positionY, double directionX, double directionY)
            {
                Position = new RealPoint(positionX, positionY);
                Direction = new RealPoint(directionX, directionY);
            }
            public RealPoint Position;
            public RealPoint Direction;
        }
        internal RealPoint PlayerPosition
        {
            get
            {
                return (RightLeg + LeftLeg) / 2;
            }
            set
            {
                var delta = value - PlayerPosition;
                LeftLeg += delta;
                RightLeg += delta;
            }
        }
        internal RealPoint PlayerDirection
        {
            get
            {
                return (RightLeg - LeftLeg).Rotate90();
            }
            set
            {
                var pos = PlayerPosition;
                var dir = value.Normalize();
                LeftLeg = pos + MapState.LegLength * dir.Rotate90();
                RightLeg = pos - MapState.LegLength * dir.Rotate90();
            }
        }

        internal void AddGrave(RealPoint P, int seed)
        {
            var grave = new GraveWood(P, seed);
            Decorations.Add(grave);
            Colliders.Add(new CircleCollider(P, 5));
        }

        internal void AddGraveStone(RealPoint P, int seed)
        {
            var grave = new GraveStone(P, seed);
            Decorations.Add(grave);
            Colliders.Add(new CircleCollider(P, 5));
        }

        internal void AddHouse1(RealPoint P, int seed)
        {
            var h = new SmallHouse(P.X, P.Y, seed);
            Decorations.Add(h);
            Colliders.AddRange(ColliderWorks.GeneratePolygon(h.GetBorder()));
            if(seed == 101)
                DynamicDecorations.Add(new Shout(P + new RealPoint(30, -50), 100, 200, "Что, опять пришли?##### Нет у нас лошади."));
            if(seed == 102)
            {
                Scripts.Add(new DarkDoctorAtFirstHouseScript(P));
            }
            if(seed == 103)
            {
                Scripts.Add(new DarkDoctorAtSecondHouseScript(P));
            }
            if(seed == 104)
            {
                Scripts.Add(new DarkDoctorAtThirdHouseScript(P));
            }
            if (seed == 105)
            {
                Scripts.Add(new DarkDoctorAtFourthHouseScript(P));
            }
        }

        internal void AddHouse2(RealPoint P, int seed)
        {
            var h = new BigHouse(P.X, P.Y, seed);
            Decorations.Add(h);
            Colliders.AddRange(ColliderWorks.GeneratePolygon(h.GetBorder()));
            if(seed == 101)
                Scripts.Add(new TalkBehindTheCurtains(P));
        }

        internal void AddHouse3(RealPoint P, int seed)
        {
            var h = new LongHouse(P.X, P.Y, seed);
            Decorations.Add(h);
            Colliders.AddRange(ColliderWorks.GeneratePolygon(h.GetBorder()));
        }
        internal void AddHouse4(RealPoint P, int seed)
        {
            var h = new KeepersHouse(P.X, P.Y, seed);
            Decorations.Add(h);
            Colliders.AddRange(ColliderWorks.GeneratePolygon(h.GetBorder()));
        }

        internal void AddCoalHouse(RealPoint position, int seed)
        {
            var h = new CoalHouse(position.X, position.Y, seed);
            Decorations.Add(h);
            Colliders.AddRange(ColliderWorks.GeneratePolygon(h.GetBorder()));

            var door = new KnockingOnDoor(position);
            Interactions.Add(door);
            Scripts.Add(door);

            TheNastyWoman = new NastyWoman(position + new RealPoint(64, 1));
        }
        internal void AddLostBucket(RealPoint P)
        {
            var bucket = new LostBucket(P);
            DynamicDecorations.Add(bucket);
            Interactions.Add(bucket);
            DynamicColliders.Add(bucket);
        }

        internal void ShowNastyWoman()
        {
            DynamicDecorations.Add(TheNastyWoman);
            Interactions.Add(new Character(TheNastyWoman.Position - new RealPoint(0, 50), 100, 160, TheNastyWoman));
        }
        internal void SetBeggarPath(RealPoint[] path, int seed)
        {
            this.BeggarPath = path;
            this.BeggarPathSeed = seed;
        }
        public void AddBeggarPath()
        {
            var steps = StepsGenerator.Generate(BeggarPath, BeggarPathSeed, 0.3);
            Steps.AddRange(steps.Where(s => ColliderWorks.AntiCollide(Anticolliders, s.Position).SquaredLength < 10000).ToList());
        }

        internal void AddDeadBeggar(RealPoint point)
        {
            var deadBeggar = new DeadBeggar(point);
            DynamicDecorations.Add(deadBeggar);
            Interactions.Add(deadBeggar);
        }

        internal void AddAnomaly(RealPoint P)
        {
            this.Anomaly = P;
        }

        internal void AddElder(RealPoint P)
        {
            var elder = new Elder(P);
            DynamicDecorations.Add(elder);
            var elderCharacter = new Character(P, 100, 200, elder);
            elderCharacter.SpeechTextBubble.Position += new RealPoint(0, -50);
            Interactions.Add(elderCharacter);
            Colliders.Add(new CircleCollider(P, 10));
        }

        internal void AddStump(RealPoint P, int seed)
        {
            var stump = new Stump(P, seed);
            Decorations.Add(stump);
            Colliders.Add(new CircleCollider(P, 10));
        }

        internal void AddPostSign(RealPoint P, int textIndex)
        {
            var postSign = new PostSign(P);
            DynamicDecorations.Add(postSign);
            Interactions.Add(postSign);
            DynamicColliders.Add(new CircleCollider(P, 3));
        }

        internal void AddCart(RealPoint P)
        {
            var cart = new Cart(P);
            Decorations.Add(cart);
            Colliders.AddRange(ColliderWorks.GeneratePolygon(cart.GetBorder()));
        }
        internal void AddDogHouse(RealPoint P)
        {
            var dogHouse = new DogHouse(P);
            Decorations.Add(dogHouse);
            Colliders.AddRange(ColliderWorks.GeneratePolygon(dogHouse.GetBorder()));
        }

        internal void AddStable(RealPoint P)
        {
            var stable = new Stable(P);
            Decorations.Add(stable);
        }

        internal void AddCrypt(RealPoint P)
        {
            var crypt = new Tomb(P);
            Decorations.Add(crypt);
            Colliders.AddRange(ColliderWorks.GeneratePolygon(crypt.GetBorder()));
        }

        internal void AddMill(RealPoint P)
        {
            var mill = new Mill(P);
            Decorations.Add(mill);
            Colliders.AddRange(ColliderWorks.GeneratePolygon(mill.GetBorder()));

            var door = new StaticInteraction(P, 100, "Старая мельница, разрушенная ветром.");
            door.DespawnInteractor = new CircleInteractor(P, 100);
            door.ReadingTextBubble.Position = P + new RealPoint(2, -30);
            door.DisableOnUse = true;
            Interactions.Add(door);
        }

        internal void AddCastleDoor(RealPoint P)
        {
            var door = new CastleDoor(P);
            Decorations.Add(door);
            var doorScript = new DarkDoctorAtCastleDoorScript(P);
            Scripts.Add(doorScript);
        }

        internal void AddWell(RealPoint P)
        {
            var well = new Well(P);
            Decorations.Add(well);
            Colliders.Add(new CircleCollider(P, 10));
        }

        internal void SetDarkDoctorPath(RealPoint[] P, int seed)
        {
            DarkDoctor = new ContinuousStepsGenerator(P[0], P[1] - P[0], seed, 0);
            DynamicColliders.Add(DarkDoctor);
            var script = new DarkDoctorScript() { Trail = P };
            Scripts.Add(script);
        }

        internal void AddMadman (RealPoint P)
        {
            var madman = new Madman(P);
            DynamicDecorations.Add(madman);
            DynamicColliders.Add(madman);
            var madmanCharacter = new Character(P, 80, 120, madman);
            madmanCharacter.BubbleIsInteractive = false;
            Interactions.Add(madmanCharacter);
        }

        internal void AddGhost(RealPoint P)
        {
            var ghost = new Ghost(P);
            DynamicDecorations.Add(ghost);
            DynamicColliders.Add(ghost);
            var ghostCharacter = new Character(P, 100, 100, ghost);
            ghostCharacter.SpeechTextBubble.Position += new RealPoint(0, -62);
            Interactions.Add(ghostCharacter);
        }

        internal void AddGuard(RealPoint P)
        {
            var guard = new Guard(P);
            DynamicDecorations.Add(guard);
            DynamicColliders.Add(guard);
            var guardCharacter = new Character(P, 0, 300, guard);
            guardCharacter.SpeechTextBubble.Position += new RealPoint(0, -62);
            Interactions.Add(guardCharacter);
            GuardPosition = P;
        }

        internal void AddNomad(RealPoint P)
        {
            var nomad = new Nomad(P);
            DynamicDecorations.Add(nomad);
            DynamicColliders.Add(nomad);

            var bagInteractor = new StaticInteraction(P + new RealPoint(15,-25), 60, "Сумка пуста.");
            bagInteractor.OnInteraction = nomad;
            bagInteractor.DisableOnUse = true;
            Interactions.Add(bagInteractor);

            var nomadCharacter = new Character(P, 0, 120, nomad);
            nomadCharacter.SpeechTextBubble.Position = P + new RealPoint(-25, -50);
            nomadCharacter.BubbleIsInteractive = false;
            Interactions.Add(nomadCharacter);
        }

        internal void AddMapText(RealPoint P, int textIndex)
        {
            var mapText = new MapText(P, textIndex);
            Decorations.Add(mapText);
        }
    }
}
