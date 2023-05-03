using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace SnowInTheNight
{
    interface Interaction
    {
        double GetInteractiveDistance(RealPoint point, RealPoint direction);
        void Interact(MapState mapState, GameState gameState);
        void Update(bool isActive, double deltaTime, MapState mapState, GameState gameState);

        void Draw(Graphics g);
    }
    [Serializable]
    class Character : Interaction
    {
        public Interactor ActionInteractor;
        public Interactor SpawnInteractor;
        public Interactor DespawnInteractor;

        public TextBubble SpeechTextBubble;
        public Dialogue CharacterDialogue;

        public bool isVisible;
        private bool isActive;
        private bool isPrinting;
        public bool BubbleIsInteractive = true;
        public bool Dead;
        public double SymbolsVisible = 0;

        public Character(RealPoint position, double actionRadius, double visibilityRadius, Dialogue dialogue)
        {
            ActionInteractor = new DirectedInteractor(position, actionRadius);
            SpawnInteractor = new CircleInteractor(position, visibilityRadius);
            DespawnInteractor = new CircleInteractor(position, visibilityRadius + MapState.LegLength*2);
            SpeechTextBubble = new TextBubble(position, "")
            {
                Alignment = new RealPoint(0.5, 1)
            };
            CharacterDialogue = dialogue;
        }

        public double GetInteractiveDistance(RealPoint point, RealPoint direction)
        {
            if (Dead)
                return double.MaxValue;
            return ActionInteractor.GetInteractiveDistance(point, direction);
        }
        public void Interact(MapState mapState, GameState gameState)
        {
            if(isPrinting)
            {
                SymbolsVisible = 999999;
                return;
            }
            if (BubbleIsInteractive)
            {
                SymbolsVisible = 0;
                CharacterDialogue.Interact(this, mapState, gameState);
            }
        }
        public void Update(bool isActive, double deltaTime, MapState mapState, GameState gameState)
        {
            this.isActive = isActive;
            if (!isVisible)
            {
                if (SpawnInteractor.GetInteractiveDistance(mapState.PlayerPosition, mapState.PlayerDirection) < double.MaxValue)
                {
                    isVisible = true;
                    SymbolsVisible = 0;
                    CharacterDialogue.Spawn(this, mapState, gameState);
                }
            }
            if (isVisible)
            {
                if (DespawnInteractor.GetInteractiveDistance(mapState.PlayerPosition, mapState.PlayerDirection) == double.MaxValue)
                {
                    isVisible = false;
                    CharacterDialogue.Despawn(this, mapState, gameState);
                }
            }
            if(isVisible)
            {
                SpeechTextBubble.Text = CharacterDialogue.CurrentLine;
                if(SymbolsVisible < 999999)
                    SymbolsVisible += deltaTime * TextBubble.printingSpeed;
                SpeechTextBubble.SymbolsVisible = (int)SymbolsVisible;
                isPrinting = SpeechTextBubble.Text.Length > SymbolsVisible;
            }
            CharacterDialogue.Update(deltaTime, this, mapState, gameState);
        }
        public void Draw(Graphics g)
        {
            if (Dead)
                return;
            if(!isVisible)
                return;
            if(isActive)
            {
                if (BubbleIsInteractive)
                {
                    if(String.IsNullOrEmpty(SpeechTextBubble.Text))
                        SpeechTextBubble.DrawMarker(g);
                    else
                        SpeechTextBubble.DrawInteractiveTextBubble(g);
                }
                else
                    SpeechTextBubble.DrawActiveTextBubble(g);
            }
            else
            {
                if (!String.IsNullOrEmpty(SpeechTextBubble.Text))
                    SpeechTextBubble.DrawTextBubble(g);
            }
        }
    }

    [Serializable]
    class StaticInteraction : Interaction
    {
        public Interactor VisibilityInteractor;
        public Interactor DespawnInteractor;

        public TextBubble ReadingTextBubble;

        public String SecondText;

        public bool isInteracting;
        bool isActive;

        public Operation OnInteraction;
        public bool DisableOnUse = false;
        public bool Disabled = false;
        public StaticInteraction(RealPoint position, double radius, RealPoint targetDirection, String text)
        {
            VisibilityInteractor = new TargetedInteractor(position, radius, targetDirection);
            DespawnInteractor = new TargetedInteractor(position, radius + MapState.LegLength * 2, targetDirection)
            {
                Threshold = 0.25
            };
            ReadingTextBubble = new TextBubble(position, text)
            {
                SymbolsVisible = int.MaxValue
            };
        }
        public StaticInteraction (RealPoint position, double radius, String text)
        {
            VisibilityInteractor = new DirectedInteractor(position, radius);
            DespawnInteractor = new DirectedInteractor(position, radius + MapState.LegLength * 2)
            {
                Threshold = 0.25
            };
            ReadingTextBubble = new TextBubble(position, text)
            {
                SymbolsVisible = int.MaxValue
            };
        }

        public double GetInteractiveDistance(RealPoint point, RealPoint direction)
        {
            if (Disabled)
                return double.MaxValue;
            return VisibilityInteractor.GetInteractiveDistance(point, direction);
        }
        public void Interact(MapState mapState, GameState gameState)
        {
            if (Disabled)
                return;
            isInteracting = true;
            if (OnInteraction != null)
                OnInteraction.DoIt(this, mapState, gameState);
        }
        public void Update(bool isActive, double deltaTime, MapState mapState, GameState gameState)
        {
            this.isActive = isActive;
            if (isInteracting)
            {
                if (DespawnInteractor.GetInteractiveDistance(mapState.PlayerPosition, mapState.PlayerDirection) == double.MaxValue)
                {
                    if (DisableOnUse)
                    {
                        Disabled = true;
                    }
                    isInteracting = false;
                }
            }
        }
        public void Respawn()
        {
            Disabled = false;
            isInteracting = false;
        }
        public void Draw(Graphics g)
        {
            if (Disabled)
                return;
            if(isInteracting)
            {
                if (isActive)
                    ReadingTextBubble.DrawActiveTextBubble(g);
                else
                    ReadingTextBubble.DrawTextBubble(g);
            }
            else
            {
                if (isActive)
                {
                    ReadingTextBubble.DrawMarker(g);
                }
            }
        }
    }

    [Serializable]
    partial class PostSign : Interaction
    {
        private StaticInteraction decoratedInteraction;
        private double counter = 0.0;
        String[] Lines
        {
            get
            {
                return new String[]
                {
                    "<- Дорога к замку   ",
                    "   Дорога к замку ->",
                    "< Дорога к замку  ",
                    "  Дорога к замку >"
                };
            }
        }
        public double GetInteractiveDistance(RealPoint point, RealPoint direction)
        {
            return decoratedInteraction.GetInteractiveDistance(point, direction);
        }
        public void Interact(MapState mapState, GameState gameState)
        {
            decoratedInteraction.Interact(mapState, gameState);
        }
        public void Update(bool isActive, double deltaTime, MapState mapState, GameState gameState)
        {
            if (decoratedInteraction.isInteracting)
            {
                counter -= deltaTime;
                if (counter <= 0.0)
                {
                    counter = 0.4 + R.NextDouble();
                    decoratedInteraction.ReadingTextBubble.Text = Lines[flipped? 1 : 0];
                }
            }
            decoratedInteraction.Update(isActive, deltaTime, mapState, gameState);
        }

        public void Draw(Graphics g)
        {
            decoratedInteraction.Draw(g);
        }
    }

    partial class FirePlace : Interaction
    {
        public Interactor VisibilityInteractor;
        public Interactor DespawnInteractor;
        public Interactor SoundInteractor;

        public TextBubble ReadingTextBubble;

        bool isInteracting;
        bool isActive;

        bool dead;
        bool bubbleIsInteractive;
        public int Id;

        public FirePlace(RealPoint position, int id)
        {
            this.Id = id;
            this.Position = position;
            var radius = 100;

            if (id == 1)
                this.isFueled = true;
            if (id == 4)
                radius = 50;
            if (id == 5)
                radius = 80;

            VisibilityInteractor = new DirectedInteractor(position, radius);
            DespawnInteractor = new DirectedInteractor(position, radius + MapState.LegLength * 2)
            {
                Threshold = 0.25
            };
            SoundInteractor = new CircleInteractor(position, 50);
            ReadingTextBubble = new TextBubble(position + new RealPoint(0,-10), "")
            {
                SymbolsVisible = int.MaxValue
            };
        }
        public void OnLoad()
        {
            isInteracting = false;
            bubbleIsInteractive = false;
        }
        public double GetInteractiveDistance(RealPoint point, RealPoint direction)
        {
            if (dead)
                return double.MaxValue;
            return VisibilityInteractor.GetInteractiveDistance(point, direction);
        }
        public void Interact(MapState mapState, GameState gameState)
        {
            if(isFueled)
            {
                if (!isInteracting)
                {
                    bubbleIsInteractive = false;
                    isInteracting = true;
                    this.isVisited = true;
                    ReadingTextBubble.Text = "Игра сохранена.";
                    GameSaveManager.Enqueue(new GameSaveManager.SaveAction(Location.FromId(Id)));
                    return;
                }
                if(bubbleIsInteractive)
                {
                    bubbleIsInteractive = false;
                    isInteracting = true;
                    ReadingTextBubble.Text = "Игра сохранена.";
                    return;
                }
                return;
            }
            isInteracting = true;
            var bucket = (ItemBucket)(mapState.Items.FirstOrDefault(i => i is ItemBucket));
            if (bucket != null)
            {
                if (bucket.Uses > 0)
                {
                    bucket.Uses--;
                    this.isFueled = true;
                    this.isVisited = true;
                    ReadingTextBubble.Text = "Теперь здесь можно согреться.";
                    bubbleIsInteractive = true;
                    GameSaveManager.Enqueue(new GameSaveManager.SaveAction(Location.FromId(Id)));
                    return;
                }
                else if(bucket.Uses == 0)
                {
                    ReadingTextBubble.Text = "У меня больше нет угля.";
                    return;
                }
            }
            ReadingTextBubble.Text = "Здесь можно развести костёр, но у меня нет угля.";
        }
        public void Update(bool isActive, double deltaTime, MapState mapState, GameState gameState)
        {
            var bucket = (ItemBucket)(mapState.Items.FirstOrDefault(i => i is ItemBucket));
            if (bucket != null && bucket.Uses > 0)
            {
                dead = false;
            }
            if (isFueled)
            {
                dead = false;
            }
            this.isActive = isActive;

            if (isFueled && SoundInteractor.GetInteractiveDistance(mapState.PlayerPosition, mapState.PlayerDirection) < double.MaxValue)
            {
                SoundWorks.NearFire = true;
            }

            if (isInteracting)
            {
                if (DespawnInteractor.GetInteractiveDistance(mapState.PlayerPosition, mapState.PlayerDirection) == double.MaxValue)
                {
                    bubbleIsInteractive = false;
                    if (bucket == null || bucket.Uses <= 0)
                    {
                        dead = true;
                    }
                    isInteracting = false;
                }
            }
        }
        public void Draw(Graphics g)
        {
            if (isInteracting)
            {
                if (isActive)
                {
                    if (bubbleIsInteractive)
                        ReadingTextBubble.DrawInteractiveTextBubble(g);
                    else
                        ReadingTextBubble.DrawActiveTextBubble(g);
                }
                else
                    ReadingTextBubble.DrawTextBubble(g);
            }
            else
            {
                if (isActive)
                {
                    ReadingTextBubble.DrawMarker(g);
                }
            }
        }
    }

    partial class BellGate : Interaction
    {
        private StaticInteraction decoratedInteraction;
        public BellGate(RealPoint position)
        {
            this.Position = position;
            decoratedInteraction = new StaticInteraction(position, 100, "") {  DisableOnUse = true };
            decoratedInteraction.ReadingTextBubble = new TextBubble(position + new RealPoint(0, -20), "Так вот откуда был этот звон.")
            {
                SymbolsVisible = int.MaxValue
            };
        }

        public double GetInteractiveDistance(RealPoint point, RealPoint direction)
        {
            return decoratedInteraction.GetInteractiveDistance(point, direction);
        }
        public void Interact(MapState mapState, GameState gameState)
        {
            if (this.frame == 0)
            {
                decoratedInteraction.Interact(mapState, gameState);
                this.frame = 1;
                mapState.AddItem(new ItemBell(), gameState);
                //gameState.IsInInventoryMode = true;
                //gameState.SelectedItem = mapState.Items.Count - 1;
            }
        }
        private double bellTimer = 0;
        private Random R = new Random();
        public void Update(bool isActive, double deltaTime, MapState mapState, GameState gameState)
        {
            decoratedInteraction.Update(isActive, deltaTime, mapState, gameState);
            if(frame == 0)
            {
                if(bellTimer >= 0)
                {
                    bellTimer -= deltaTime;
                }
                else
                {
                    bellTimer = R.NextDouble() * 4 + 1;

                    var delta = mapState.PlayerPosition - this.Position;
                    var distance = delta.Length;
                    var intencity = (int)Math.Floor((800 - distance) / 100);
                    SoundWorks.PlayBellSound(intencity);
                }
            }
        }

        public void Draw(Graphics g)
        {
            decoratedInteraction.Draw(g);
        }

    }
    partial class LostBucket : Interaction
    {
        private StaticInteraction decoratedInteraction;
        public LostBucket(RealPoint position)
        {
            this.Position = position;
            decoratedInteraction = new StaticInteraction(position + new RealPoint(0, -20), 100, "") {  DisableOnUse = true};

            DecoratedCollider = new CircleCollider(position, 5);
        }

        public double GetInteractiveDistance(RealPoint point, RealPoint direction)
        {
            if (Found)
                return double.MaxValue;
            return decoratedInteraction.GetInteractiveDistance(point, direction);
        }
        public void Interact(MapState mapState, GameState gameState)
        {
            decoratedInteraction.Interact(mapState, gameState);
            if (!Found)
            {
                Found = true;
                mapState.AddItem(new ItemBucket(), gameState);
            }
        }
        public void Update(bool isActive, double deltaTime, MapState mapState, GameState gameState)
        {
            decoratedInteraction.Update(isActive, deltaTime, mapState, gameState);
        }

        public void Draw(Graphics g)
        {
            decoratedInteraction.Draw(g);
        }

    }
    [Serializable]
    partial class KnockingOnDoor : Interaction
    {
        private StaticInteraction decoratedInteraction;
        public RealPoint Position;
        public KnockingOnDoor(RealPoint position)
        {
            this.Position = position;
            decoratedInteraction = new StaticInteraction(position + new RealPoint(56, -40), 100, "Дом угольщика");
            decoratedInteraction.DisableOnUse = true;
            husbandShoutPosition = position + new RealPoint(0, -80);
            wifeShoutPosition = position + new RealPoint(60, -100);
        }

        public double GetInteractiveDistance(RealPoint point, RealPoint direction)
        {
            if (Knocking)
                return double.MaxValue;
            return decoratedInteraction.GetInteractiveDistance(point, direction);
        }
        public void Interact(MapState mapState, GameState gameState)
        {
            if (Knocking)
                return;
            if (NoMoreKnoking)
                return;
            if (mapState.Items.FirstOrDefault(i => i is ItemBucket) != null)
            {
                decoratedInteraction.ReadingTextBubble.Text = "";
                Knocking = true;
                SoundWorks.PlaySomeKnocking();
            }
            decoratedInteraction.Interact(mapState, gameState);
        }
        public void Update(bool isActive, double deltaTime, MapState mapState, GameState gameState)
        {
            if(decoratedInteraction.DisableOnUse)
            {
                if (mapState.Items.FirstOrDefault(i => i is ItemBucket) != null)
                {
                    decoratedInteraction.DisableOnUse = false;
                    decoratedInteraction.Disabled = false;
                }
            }
            decoratedInteraction.Update(isActive, deltaTime, mapState, gameState);
        }

        public void Draw(Graphics g)
        {
            decoratedInteraction.Draw(g);
        }
    }
    partial class DeadBeggar : Interaction
    {
        private StaticInteraction decoratedInteraction;

        public double GetInteractiveDistance(RealPoint point, RealPoint direction)
        {
            if (!Exists)
                return double.MaxValue;
            if (Found)
                return double.MaxValue;
            return decoratedInteraction.GetInteractiveDistance(point, direction);
        }
        public void Interact(MapState mapState, GameState gameState)
        {
            if (Exists)
            {
                decoratedInteraction.ReadingTextBubble.Position += new RealPoint(0, -50);
                decoratedInteraction.Interact(mapState, gameState);
                if (!Found)
                {
                    Found = true;
                    mapState.AddItem(new ItemMap(), gameState);
                }
            }
        }
        public void Update(bool isActive, double deltaTime, MapState mapState, GameState gameState)
        {
            decoratedInteraction.Update(isActive, deltaTime, mapState, gameState);
        }

        public void Draw(Graphics g)
        {
            decoratedInteraction.Draw(g);
        }

    }
    /*class InteractionObsolete
    {
        public bool Visible = false;
        public String MarkerText = "Х";
        public CircleInteractor Interactor;
        public RPoint MarkerPosition;
        bool IsPrinting;
        double SymbolsVisible;
        public delegate void InteractionAction(GameState gameState, MapState mapState, InteractionObsolete self);
        public InteractionAction ActionOnInteractionStarted;
        public InteractionAction ActionOnStopBeingVisible;
        public InteractionAction ActionOnStartBeingVisible;
        public void Draw (Graphics g)
        {
            if (!Visible && !finishing)
                return;
            if (String.IsNullOrWhiteSpace(MarkerText))
                return;
            var bubble = new TextBubble(MarkerPosition, MarkerText);
            bubble.SymbolsVisible = (int)SymbolsVisible;
            IsPrinting = MarkerText.Length > SymbolsVisible;
            if ((MarkerText.Length < SymbolsVisible - extraSeconds*printingSpeed) && finishing)
                return;
            bubble.DrawInteractiveTextBubble(g);
        }
        public static double printingSpeed = 20;
        public void Update (double deltaTime)
        {
            if(!Visible)
            {
                SymbolsVisible = 999999;
                return;
            }
            if(Visible || finishing)
                SymbolsVisible += deltaTime*printingSpeed;
        }

        public void StartInteraction(GameState gameState, MapState mapState)
        {
            if(IsPrinting)
            {
                EndPrinting();
                return;
            }
            if (finishing)
                return;
            if (ActionOnInteractionStarted != null)
                ActionOnInteractionStarted(gameState, mapState, this);
        }
        public void RestartCaret ()
        {
            IsPrinting = true;
            SymbolsVisible = 0;
        }
        public void EndPrinting()
        {
            SymbolsVisible = MarkerText.Length;
            IsPrinting = false;
        }
        private bool finishing = false;
        private double extraSeconds = 0;
        public void CarefullyFinish(double extraSeconds)
        {
            if (finishing)
                return;
            finishing = true;
            this.extraSeconds = extraSeconds;
        }
        public void OnStartBeingVisible(GameState gameState, MapState mapState)
        {
            if (finishing)
                return;
            Interactor.Threshold = 0;
            RestartCaret();
            if (ActionOnStartBeingVisible != null)
                ActionOnStartBeingVisible(gameState, mapState, this);
        }
        public void OnStopBeingVisible(GameState gameState, MapState mapState)
        {
            if (finishing)
                return;
            Interactor.Threshold = 0.5;
            if (ActionOnStopBeingVisible != null)
                ActionOnStopBeingVisible(gameState, mapState, this);
        }
    }*/
}
