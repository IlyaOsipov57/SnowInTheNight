using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SnowInTheNight
{
    interface Script
    {
        void Update(double deltaTime, MapState mapState, GameState gameState);
    }
    [Serializable]
    class TalkBehindTheCurtains : Script
    {
        RealPoint initializerPosition;
        RealPoint firstShoutPosition;
        RealPoint secondShoutPosition;
        public TalkBehindTheCurtains(RealPoint position)
        {
            initializerPosition = position + new RealPoint(140, -10);
            firstShoutPosition = position + new RealPoint(45, -120);
            secondShoutPosition = position + new RealPoint(15, -80);
        }
        double timer = -1;
        bool failedWalking = false;
        public void Update(double deltaTime, MapState mapState, GameState gameState)
        {
            if (timer < 0)
            {
                if ((mapState.PlayerPosition - initializerPosition).SquaredLength < 70 * 70)
                {
                    timer = 0;

                    //var r = 70;
                    //mapState.Decorations.Add(new Circle(initializerPosition, r));
                    //mapState.Decorations.Add(new Circle(lolKekPosition, r*2));
                }
            }
            if (timer >= 0)
            {
                var nextTimer = timer + deltaTime;
                var time = 0;

                time = 1;
                if (timer < time && time <= nextTimer)
                {
                    var s = new Shout(firstShoutPosition, 999999, 999999,
"Мама, папа! Посмотрите, это же наш доктор!") { ExtraSeconds = 1.6 };//43 -> 2.15
                    s.ShoutTextBubble.MaxWidth = 200;
                    mapState.DynamicDecorations.Add(s);
                    nextTimer = time;
                }

                time = 5;
                if (timer < time && time <= nextTimer)
                {
                    if (gameState.LastFails.Count > 40)
                    {
                        failedWalking = true;
                    }

                    var line = "Почему он гуляет в такую погоду?#### Он сумасшедший?";
                    var s = new Shout(firstShoutPosition, 999999, 999999,line);//54 -> 2.7
                    s.ShoutTextBubble.MaxWidth = 220;
                    mapState.DynamicDecorations.Add(s);
                    nextTimer = time;
                }

                time = 10;
                if (timer < time && time <= nextTimer)
                {
                    var s = new Shout(secondShoutPosition, 999999, 999999,
"А ну отойди от окна!\r\nИ не смей обижать доктора.") { ExtraSeconds = 1.4 };//48 -> 2.4
                    s.ShoutTextBubble.MaxWidth = 250;
                    mapState.DynamicDecorations.Add(s);
                    nextTimer = time;
                }

                time = 14;
                if (timer < time && time <= nextTimer)
                {
                    var s = new Shout(secondShoutPosition, 999999, 999999,
"Князь лично его назначил. Прогневаешь князя - будет беда.") { ExtraSeconds = 0.9 };//58 -> 2.9
                    s.ShoutTextBubble.MaxWidth = 250;
                    mapState.DynamicDecorations.Add(s);
                    nextTimer = time;
                }
                
                if (failedWalking)
                {
                    time = 18;
                    if (timer < time && time <= nextTimer)
                    {
                        var s = new Shout(secondShoutPosition, 999999, 999999,
"Тяжело, наверно, по снегу ходить. Но ничего, скоро научится.");//61 -> 3.1
                        s.ShoutTextBubble.MaxWidth = 250;
                        mapState.DynamicDecorations.Add(s);
                        nextTimer = time;
                    }
                }

                timer = nextTimer;

            }
        }
    }
    partial class KnockingOnDoor : Script
    {
        RealPoint husbandShoutPosition;
        RealPoint wifeShoutPosition;

        double knockTimer = 0;
        double timer = 0;
        public bool DialogueStarted;
        public bool Knocking;
        public bool NoMoreKnoking;
        public void Update(double deltaTime, MapState mapState, GameState gameState)
        {
            if (Knocking && !NoMoreKnoking)
            {
                DialogueStarted = true;
                knockTimer += deltaTime;
                if (knockTimer > 1)
                {
                    Knocking = false;
                    knockTimer = 0;
                    decoratedInteraction.Respawn();
                }
            }
            if (DialogueStarted)
            {
                var nextTimer = timer + deltaTime;
                var time = 0;

                time = 1;
                if (timer < time && time <= nextTimer)
                {
                    var s = new Shout(husbandShoutPosition, 999999, 999999,
"Я не ослышался? Как будто покупатель?") { ExtraSeconds = 1 };//38
                    s.ShoutTextBubble.MaxWidth = 200;
                    mapState.DynamicDecorations.Add(s);
                    nextTimer = time;
                }

                time = 4;
                if (timer < time && time <= nextTimer)
                {
                    var s = new Shout(wifeShoutPosition, 999999, 999999,
"Я не слышу ровно ничего. Только ветки по окнам стучат."); //55
                    s.ShoutTextBubble.MaxWidth = 200;
                    mapState.DynamicDecorations.Add(s);
                    nextTimer = time;
                }

                time = 8;
                if (timer < time && time <= nextTimer)
                {
                    nextTimer = time;
                    DialogueStarted = false;
                }

                time = 9;
                if (timer < time && time <= nextTimer)
                {
                    var s = new Shout(husbandShoutPosition, 999999, 999999,
"Нет, жена, кто-то там есть, кто-то есть.") { ExtraSeconds = 0.85 };//41
                    s.ShoutTextBubble.MaxWidth = 200;
                    mapState.DynamicDecorations.Add(s);
                    nextTimer = time;
                }

                time = 12;
                if (timer < time && time <= nextTimer)
                {
                    var s = new Shout(wifeShoutPosition, 999999, 999999,
"Да что с тобой?\r\nНа улице ночь, все наши покупатели давно спят.");//63
                    s.ShoutTextBubble.MaxWidth = 200;
                    mapState.DynamicDecorations.Add(s);
                    nextTimer = time;
                }

                time = 17;
                if (timer < time && time <= nextTimer)
                {
                    nextTimer = time;
                    DialogueStarted = false;
                }

                time = 18;
                if (timer < time && time <= nextTimer)
                {
                    var s = new Shout(husbandShoutPosition, 999999, 999999,
"Сейчас выйду!") { ExtraSeconds = 1.2 };//14
                    s.ShoutTextBubble.MaxWidth = 200;
                    mapState.DynamicDecorations.Add(s);
                    nextTimer = time;
                }

                time = 20;
                if (timer < time && time <= nextTimer)
                {
                    var s = new Shout(wifeShoutPosition, 999999, 999999,
"Не смей ходить.#### Я сама пойду вместо тебя.") { ExtraSeconds = 1.55 };//47
                    s.ShoutTextBubble.MaxWidth = 200;
                    mapState.DynamicDecorations.Add(s);
                    nextTimer = time;
                }

                time = 24;
                if (timer < time && time <= nextTimer)
                {
                    var s = new Shout(husbandShoutPosition, 999999, 999999,
"Только не забудь, перечисли все сорта, какие у нас есть.");//57
                    s.ShoutTextBubble.MaxWidth = 210;
                    mapState.DynamicDecorations.Add(s);
                    nextTimer = time;
                }
                time = 26;
                if (timer < time && time <= nextTimer)
                {
                    NoMoreKnoking = true;
                    decoratedInteraction.Disabled = true;
                }
                time = 28;
                if (timer < time && time <= nextTimer)
                {
                    nextTimer = time;
                    mapState.ShowNastyWoman();

                }

                timer = nextTimer;
            }
        }
    }
    [Serializable]
    class DarkDoctorScript : Script
    {
        public RealPoint[] Trail;
        private int index = 0;
        private bool started = false;
        private bool finished = false;
        public void Update(double deltaTime, MapState mapState, GameState gameState)
        {
            if (finished)
                return;
            if ((mapState.PlayerPosition - mapState.DarkDoctor.position).Length < 200)
            {
                started = true;
            }
            if (!started)
                return;
            if ((mapState.DarkDoctor.position - Trail[index]).Length < 10)
            {
                if (index >= Trail.Length - 1)
                {
                    finished = true;
                    mapState.Scripts.Add(new DarkDoctorAtFirePlaceScript(Trail[index]));
                    return;
                }
                index++;
                mapState.DarkDoctor.SetTarget(Trail[index]);
            }
        }
    }
    [Serializable]
    class DarkDoctorAtFirePlaceScript : Script
    {
        private int index = 0;
        private bool started = false;
        private bool finished = false;
        Character DoctorCharacter;
        Shout DoctorShout1;
        bool isFollowing = false;
        public DarkDoctorAtFirePlaceScript(RealPoint position)
        {
            DoctorCharacter = new Character(position, 100, 150, new DarkDoctorDialogue());
            DoctorCharacter.SpeechTextBubble.Position += new RealPoint(0, -50);
            DoctorShout1 = new Shout(position + new RealPoint(0, -50), 999999, 999999,
                "Вам тоже в эту сторону?#### Я составлю вам компанию.################");
            DoctorShout1.ShoutTextBubble.Alignment = new RealPoint(0.5, 1);
            DoctorShout1.ShoutTextBubble.MaxWidth = 210;
        }
        public void Update(double deltaTime, MapState mapState, GameState gameState)
        {
            if (finished)
                return;
            if (!started && (mapState.PlayerPosition - mapState.DarkDoctor.position).Length < 150)
            {
                started = true;
                mapState.Interactions.Add(DoctorCharacter);
            }
            if (!started)
                return;

            DoctorShout1.ShoutTextBubble.Position = mapState.DarkDoctor.position + new RealPoint(0, -50);

            if (!isFollowing)
            {
                var delta = mapState.PlayerPosition - mapState.DarkDoctor.position;
                if (delta.Length > 200 && delta.Y < -40)
                {
                    DoctorCharacter.Dead = true;
                    mapState.DynamicDecorations.Add(DoctorShout1);
                    isFollowing = true;
                }
            }

            if (isFollowing && !mapState.DarkDoctor.LockedOnInteraction)
            {
                var distance = mapState.DarkDoctor.LooselyFollower ? 100 : 30;

                var delta = mapState.PlayerPosition - mapState.DarkDoctor.position;
                var target = delta.Normalize() * Math.Max(0, delta.Length - distance);

                target += mapState.DarkDoctor.position;

                {
                    var fix = ColliderWorks.AntiCollide(mapState.DarkAnticolliders, target);
                    fix = fix.Normalize() * Math.Max(0, fix.Length - 50);
                    target += fix;
                }

                var circleCollider = new CircleCollider(new RealPoint(1640, -5180), 250);
                var circleAnticollider = new CircleCollider(new RealPoint(1640, -5170), 150);
                if (phase == 2)
                {
                    if (target.X < 1780)
                    {
                        var fix = circleAnticollider.AntiCollide(target);
                        target += fix;
                    }
                }
                else
                {
                    if (circleCollider.Collide(mapState.PlayerPosition) != RealPoint.Zero)
                    {
                        phase = 1;
                    }
                    if (circleCollider.Collide(mapState.DarkDoctor.position) != RealPoint.Zero)
                    {
                        phase = 2;
                    }
                }

                delta = target - mapState.DarkDoctor.position;

                mapState.DarkDoctor.SpeedPercentage = GetSpeedPercentage(mapState, delta.Length, target, mapState.PlayerPosition);
                
                if (mapState.DarkDoctor.LooselyFollower)
                {
                    mapState.DarkDoctor.SpeedPercentage /= 2;
                }

                mapState.DarkDoctor.SetTarget(target);
            }
        }

        private int GetSpeedPercentage(MapState mapState, double length, RealPoint target, RealPoint playerPosition)
        {
            if (length < 10)
                return 0;
            if (length < 50)
                return 50;
            if (length < 100)
                return 100;
            if (length < 350)
                return 150;
            if (phase == 2)
                return 0;
            if (target.X > 1780)
                return 0;
            if (playerPosition.Y > -4700)
                return 0;
            return 150;
        }
        private int phase;
    }
    [Serializable]
    class DarkDoctorAtCastleDoorScript : Script
    {
        bool finished;
        bool started;
        bool broken;
        RealPoint DoorPosition;
        Shout DoctorShout1;
        Shout DoctorShout2;
        double timer = 0;
        public DarkDoctorAtCastleDoorScript(RealPoint doorPosition)
        {
            this.DoorPosition = doorPosition;
            DoctorShout1 = new Shout(RealPoint.Zero, 999999, 999999,
                "Смотрите, вот и вход в замок.\r\nВы когда-нибудь были внутри?\r\n####Может быть постучать в ворота?############");
            DoctorShout1.ShoutTextBubble.MaxWidth = 250;
            DoctorShout1.ShoutTextBubble.Alignment = new RealPoint(0.5, 1);
            DoctorShout2 = new Shout(RealPoint.Zero, 999999, 999999,
                "Впрочем, наверное, не стоит. Пойдёмте через предместья, так даже быстрее получится.");
            DoctorShout2.ShoutTextBubble.MaxWidth = 210;
            DoctorShout2.ShoutTextBubble.Alignment = new RealPoint(0.5, 1);
        }
        public void Update(double deltaTime, MapState mapState, GameState gameState)
        {
            if (finished)
                return;
            if (!started && (mapState.DarkDoctor.position - (DoorPosition + new RealPoint(75, 100))).Length < 125)
            {
                started = true;
            }
            if (!started)
                return;
            DoctorShout1.ShoutTextBubble.Position = mapState.DarkDoctor.position + new RealPoint(0, -50);
            DoctorShout2.ShoutTextBubble.Position = mapState.DarkDoctor.position + new RealPoint(0, -50);

            if (broken)
            {
                var nextTimer = timer + deltaTime;
                var time = 0;

                time = 9;
                if (timer < time && time <= nextTimer)
                {
                    mapState.DynamicDecorations.Add(DoctorShout2);
                    nextTimer = time;
                }

                time = 16;
                if (timer < time && time <= nextTimer)
                {
                    finished = true;
                }
                timer = nextTimer;

                return;
            }

            {
                var nextTimer = timer + deltaTime;
                var time = 0;

                time = 1;
                if (timer < time && time <= nextTimer)
                {
                    mapState.DarkDoctor.LockedOnInteraction = true;
                    mapState.DarkDoctor.SetTarget(DoorPosition + new RealPoint(0, 44));
                    mapState.DarkDoctor.SpeedPercentage = 70;
                    mapState.DynamicDecorations.Add(DoctorShout1);
                    nextTimer = time;
                }


                if (nextTimer > 2 && (mapState.PlayerPosition - mapState.DarkDoctor.position).Length > 300)
                {
                    mapState.DarkDoctor.LockedOnInteraction = false;
                    var doorInteraction = new StaticInteraction(DoorPosition + new RealPoint(0, -20), 100, "На дверях табличка: «не стучать».");
                    mapState.Interactions.Add(doorInteraction);
                    broken = true;
                    timer = nextTimer;
                    return;
                }


                time = 9;
                if (timer < time && time <= nextTimer)
                {
                    mapState.DynamicDecorations.Add(DoctorShout2);
                    nextTimer = time;
                }

                time = 13;
                if (timer < time && time <= nextTimer)
                {
                    mapState.DarkDoctor.SetTarget(DoorPosition + new RealPoint(0, 100));
                    mapState.DarkDoctor.SpeedPercentage = 50;
                    nextTimer = time;
                    mapState.VillageDialogueState = 1;
                }

                time = 15;
                if (timer < time && time <= nextTimer)
                {
                    mapState.DarkDoctor.LockedOnInteraction = false;
                    var doorInteraction = new StaticInteraction(DoorPosition + new RealPoint(0, -20), 100, "На дверях табличка: «не стучать».");
                    mapState.Interactions.Add(doorInteraction);
                    nextTimer = time;
                    finished = true;
                }

                timer = nextTimer;
            }
        }
    }

    [Serializable]
    class DarkDoctorAtFirstHouseScript : Script
    {
        bool finished;
        bool started;
        RealPoint HousePosition;
        public DarkDoctorAtFirstHouseScript(RealPoint housePosition)
        {
            this.HousePosition = housePosition;
        }
        public void Update(double deltaTime, MapState mapState, GameState gameState)
        {
            if (finished)
                return;
            if (!started && mapState.VillageDialogueState == 1 && (mapState.DarkDoctor.position - (HousePosition + new RealPoint(0, 200))).Length < 150)
            {
                started = true;
            }
            if (!started)
                return;

            var HouseShout = new Shout(HousePosition + new RealPoint(0, -20), 999999, 999999,
                "Смотрите! Идут, как ни в чём не бывало!");
            HouseShout.ShoutTextBubble.MaxWidth = 210;
            HouseShout.ShoutTextBubble.Alignment = new RealPoint(0.5, 1);
            mapState.DynamicDecorations.Add(HouseShout);
            finished = true;
        }
    }

    [Serializable]
    class DarkDoctorAtSecondHouseScript : Script
    {
        bool finished;
        bool started;
        RealPoint HousePosition;
        Shout HouseShout1;
        Shout DoctorShout;
        Shout HouseShout2;
        double timer = 0;
        public DarkDoctorAtSecondHouseScript(RealPoint housePosition)
        {
            this.HousePosition = housePosition + new RealPoint(-60, -20);
            HouseShout1 = new Shout(HousePosition, 999999, 999999,
                "Как же в голову такое могло прийти, в ворота стучать?!");
            HouseShout1.ShoutTextBubble.MaxWidth = 210;
            HouseShout1.ShoutTextBubble.Alignment = new RealPoint(0.5, 1);

            DoctorShout = new Shout(RealPoint.Zero, 999999, 999999,
                "Я и не стучал, я только собирался.");
            DoctorShout.ShoutTextBubble.MaxWidth = 250;
            DoctorShout.ShoutTextBubble.Alignment = new RealPoint(0.5, 1);

            HouseShout2 = new Shout(HousePosition, 999999, 999999,
                "Боже мой, да он и не скрывает!");
            HouseShout2.ShoutTextBubble.MaxWidth = 250;
            HouseShout2.ShoutTextBubble.Alignment = new RealPoint(0.5, 1);
        }
        public void Update(double deltaTime, MapState mapState, GameState gameState)
        {
            if (finished)
                return;
            if (!started && (mapState.DarkDoctor.position - (HousePosition + new RealPoint(-100, 0))).Length < 150)
            {
                started = true;
            }
            if (!started)
                return;

            DoctorShout.ShoutTextBubble.Position = mapState.DarkDoctor.position + new RealPoint(0, -50);

            {
                var nextTimer = timer + deltaTime;
                var time = 0.0;


                time = 1;
                if (timer < time && time <= nextTimer)
                {
                    mapState.VillageDialogueState = 2;
                    mapState.DynamicDecorations.Add(HouseShout1);
                    nextTimer = time;
                }

                time = 5.5;
                if (timer < time && time <= nextTimer)
                {
                    mapState.DarkDoctor.LockedOnInteraction = true;
                    mapState.DarkDoctor.SetTarget(mapState.DarkDoctor.position);
                    mapState.DynamicDecorations.Add(DoctorShout);
                    nextTimer = time;
                }

                if (nextTimer > 6 && (mapState.PlayerPosition - mapState.DarkDoctor.position).Length > 300)
                {
                    mapState.DarkDoctor.LockedOnInteraction = false;
                }
                
                time = 7.5;
                if (timer < time && time <= nextTimer)
                {
                    mapState.DarkDoctor.LockedOnInteraction = false;
                    nextTimer = time;
                }

                time = 8.5;
                if (timer < time && time <= nextTimer)
                {
                    if (mapState.VillageDialogueState == 2)
                    {
                        mapState.DynamicDecorations.Add(HouseShout2);
                        mapState.VillageDialogueState = 3;
                    }
                    nextTimer = time;
                }

                time = 14;
                if (timer < time && time <= nextTimer)
                {
                    nextTimer = time;
                    finished = true;
                }

                timer = nextTimer;
            }
        }
    }
    [Serializable]
    class DarkDoctorAtThirdHouseScript : Script
    {
        bool finished;
        bool started;
        RealPoint HousePosition;
        Shout HouseShout1;
        Shout HouseShout1b;
        Shout DoctorShout;
        Shout HouseShout2;
        double timer = 0;
        public DarkDoctorAtThirdHouseScript(RealPoint housePosition)
        {
            this.HousePosition = housePosition + new RealPoint(0, -20);
            HouseShout1 = new Shout(HousePosition, 999999, 999999,
                "Стража долго разбираться не будет!");
            HouseShout1.ShoutTextBubble.MaxWidth = 210;
            HouseShout1.ShoutTextBubble.Alignment = new RealPoint(0.5, 1);

            HouseShout1b = new Shout(HousePosition, 999999, 999999,
                "Боже мой, да он и не скрывает!");
            HouseShout1b.ShoutTextBubble.MaxWidth = 250;
            HouseShout1b.ShoutTextBubble.Alignment = new RealPoint(0.5, 1);

            DoctorShout = new Shout(RealPoint.Zero, 999999, 999999,
                "Но я ни в чём не виноват!");
            DoctorShout.ShoutTextBubble.MaxWidth = 250;
            DoctorShout.ShoutTextBubble.Alignment = new RealPoint(0.5, 1);

            HouseShout2 = new Shout(HousePosition, 999999, 999999,
                "Смотрите, врёт и не краснеет!");
            HouseShout2.ShoutTextBubble.MaxWidth = 250;
            HouseShout2.ShoutTextBubble.Alignment = new RealPoint(0.5, 1);
        }
        public void Update(double deltaTime, MapState mapState, GameState gameState)
        {
            if (finished)
                return;
            if (!started && (mapState.PlayerPosition - (HousePosition + new RealPoint(0, 100))).Length < 150)
            {
                started = true;
            }
            if (!started)
                return;

            DoctorShout.ShoutTextBubble.Position = mapState.DarkDoctor.position + new RealPoint(0, -50);

            {
                var nextTimer = timer + deltaTime;
                var time = 0.0;


                time = 1;
                if (timer < time && time <= nextTimer)
                {
                    if (mapState.VillageDialogueState == 2)
                    {
                        mapState.DynamicDecorations.Add(HouseShout1b);
                    }
                    else
                    {
                        mapState.DynamicDecorations.Add(HouseShout1);
                    }
                    mapState.VillageDialogueState = 4;
                    nextTimer = time;
                }

                time = 4.5;
                if (timer < time && time <= nextTimer)
                {
                    mapState.DarkDoctor.LockedOnInteraction = true;
                    mapState.DarkDoctor.SetTarget(mapState.DarkDoctor.position);
                    mapState.DynamicDecorations.Add(DoctorShout);
                    nextTimer = time;
                }

                if (nextTimer > 5 && (mapState.PlayerPosition - mapState.DarkDoctor.position).Length > 300)
                {
                    mapState.DarkDoctor.LockedOnInteraction = false;
                }

                time = 6;
                if (timer < time && time <= nextTimer)
                {
                    mapState.DarkDoctor.LockedOnInteraction = false;
                    nextTimer = time;
                }

                time = 7;
                if (timer < time && time <= nextTimer)
                {
                    if (mapState.VillageDialogueState == 4)
                    {
                        mapState.DynamicDecorations.Add(HouseShout2);
                        mapState.VillageDialogueState = 5;
                    }
                    nextTimer = time;
                }

                time = 14;
                if (timer < time && time <= nextTimer)
                {
                    nextTimer = time;
                    finished = true;
                }

                timer = nextTimer;
            }
        }
    }

    [Serializable]
    class DarkDoctorAtFourthHouseScript : Script
    {
        bool doneFirst;
        double timeout = 0.0;
        bool finished;
        bool started;
        RealPoint HousePosition;
        Shout HouseShout1;
        Shout HouseShout1b;
        public DarkDoctorAtFourthHouseScript(RealPoint housePosition)
        {
            this.HousePosition = housePosition + new RealPoint(0, -20);

            HouseShout1b = new Shout(HousePosition, 999999, 999999,
                "Смотрите, врёт и не краснеет!");
            HouseShout1b.ShoutTextBubble.MaxWidth = 250;
            HouseShout1b.ShoutTextBubble.Alignment = new RealPoint(0.5, 1);

            HouseShout1 = new Shout(HousePosition, 999999, 999999,
                "Это ведь нам всем теперь достанется.#### Что же теперь будет?");
            HouseShout1.ShoutTextBubble.MaxWidth = 250;
            HouseShout1.ShoutTextBubble.Alignment = new RealPoint(0.5, 1);
        }
        public void Update(double deltaTime, MapState mapState, GameState gameState)
        {
            if (finished)
                return;
            if (!doneFirst)
            {
                if (!started && (mapState.PlayerPosition - (HousePosition + new RealPoint(50, -100))).Length < 150)
                {
                    started = true;
                }
                if (!started)
                    return;

                if (mapState.VillageDialogueState == 4)
                {
                    mapState.DynamicDecorations.Add(HouseShout1b);
                    timeout = 4;
                }
                mapState.VillageDialogueState = 6;
                started = false;
                doneFirst = true;
            }
            else
            {
                if (timeout > 0)
                {
                    timeout -= deltaTime;
                    return;
                }

                if (!started && (mapState.PlayerPosition - (HousePosition + new RealPoint(100, 0))).Length < 150)
                {
                    started = true;
                }
                if (!started)
                    return;

                mapState.DynamicDecorations.Add(HouseShout1);
                finished = true;
            }
        }
    }

    [Serializable]
    class RatHouseScript : Script
    {
        bool finished;
        bool doneFirst;
        bool started;
        RealPoint HousePosition;
        double timer = 0;
        public RatHouseScript(RealPoint housePosition)
        {
            this.HousePosition = housePosition;
        }
        public void Update(double deltaTime, MapState mapState, GameState gameState)
        {
            if (finished)
                return;

            var darkDelta = mapState.DarkDoctor.position - (HousePosition + new RealPoint(0,200));
            if (!mapState.DarkDoctor.LooselyFollower && darkDelta.X > -300 && darkDelta.Y > 0)
            {
                mapState.DarkDoctor.LooselyFollower = true;
            }
            if (mapState.DarkDoctor.LooselyFollower && !(darkDelta.X > -320 && darkDelta.Y > -20))
            {
                mapState.DarkDoctor.LooselyFollower = false;
            }
            //mapState.DarkDoctor.LooselyFollower = darkDelta.Length < 300 && darkDelta.Y > 0;
            var delta = mapState.PlayerPosition - HousePosition;

            var darkPosition1 = mapState.GuardPosition + new RealPoint(-70, 60);
            if (!doneFirst && delta.Length < 400 && delta.X > 0 && delta.Y > 0)
            {
                var bell = mapState.Items.FirstOrDefault(i => i is ItemBell) as ItemBell;
                if (bell != null)
                {
                    bell.Unusable = true;
                }
                gameState.EndingPhase = 1;
                mapState.DarkDoctor.LockedOnInteraction = true;
                mapState.DarkDoctor.SetTarget(darkPosition1);
                doneFirst = true;
            }

            if (!started && (mapState.DarkDoctor.position - darkPosition1).Length < MapState.LegLength)
            {
                var darkPosition2 = mapState.GuardPosition + new RealPoint(-50, 50);
                mapState.DarkDoctor.SetTarget(darkPosition2);
            }

            if (!doneFirst)
                return;

            if(!started && delta.Y <150 - MapState.LegLength)
            {
                gameState.EndingPhase = 3;
                started = true;
            }

            if (!started)
                return;

            if (gameState.EndingPhase != 4)
                return;

            {
                var nextTimer = timer + deltaTime;
                var time = 0.0;


                time = 1;
                if (timer < time && time <= nextTimer)
                {
                    mapState.DarkDoctor.SetTarget(mapState.DarkDoctor.position + new RealPoint(1000, 0));
                    mapState.DarkDoctor.SpeedPercentage = 50;
                    nextTimer = time;
                }


                time = 2;
                if (timer < time && time <= nextTimer)
                {
                    mapState.DarkDoctor.SpeedPercentage = 80;
                    nextTimer = time;
                }


                time = 4;
                if (timer < time && time <= nextTimer)
                {
                    mapState.DarkDoctor.SetTarget(mapState.DarkDoctor.position + new RealPoint(5, -20));
                    nextTimer = time;
                }


                time = 5;
                if (timer < time && time <= nextTimer)
                {
                    mapState.DarkDoctor.SetTarget(mapState.DarkDoctor.position + new RealPoint(40, 20));
                    nextTimer = time;
                }

                time = 5.8;
                if (timer < time && time <= nextTimer)
                {
                    gameState.EndingPhase = 5;
                    mapState.DarkDoctor.SetTarget(mapState.DarkDoctor.position + new RealPoint(1000, 0));
                    nextTimer = time;
                }
                timer = nextTimer;
            }
        }
    }
}
