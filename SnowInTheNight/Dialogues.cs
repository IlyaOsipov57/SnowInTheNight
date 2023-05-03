using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SnowInTheNight
{
    interface Dialogue
    {
        String CurrentLine
        {
            get;
        }
        void Update(double deltaTime, Interaction owner, MapState mapState, GameState gameState);
        void Interact(Interaction owner, MapState mapState, GameState gameState);
        void Spawn(Interaction owner, MapState mapState, GameState gameState);
        void Despawn(Interaction owner, MapState mapState, GameState gameState);
    }
    [Serializable]
    partial class Maid : Dialogue
    {
        private String[] Lines
        {
            get
            {
                return new String[]{
                    "Никто в деревне не согласился одолжить вам лошадь.",
                    "Простите.#####\r\nЯ стучалась в каждый дом.",
                    "Но никто и слышать не хотел, что вас в десяти милях отсюда ждёт тяжелобольной.",
                    "Неужели вы пойдёте пешком?",
                    "Прошу вас, хотя бы возьмите фонарь.",
                    "Берегите себя."
                };
            }
        }
        private int currentLine = 0;
        public String CurrentLine
        {
            get
            {
                if(Lines.Length > currentLine && currentLine >= 0)
                    return Lines[currentLine];
                return "";
            }
        }
        public void Update(double deltaTime, Interaction owner, MapState mapState, GameState gameState)
        {

        }
        public void Interact(Interaction owner, MapState mapState, GameState gameState)
        {
            currentLine++;
            if(currentLine == 4)
            {
                this.frame = 2;
            }
            if(currentLine == 5)
            {
                this.frame = 4;
                mapState.AddItem(new ItemLantern(), gameState);
                gameState.IsInInventoryMode = true;
                var character = owner as Character;
                if (character == null)
                    return;
                character.BubbleIsInteractive = false;
            }
        }
        public void Spawn(Interaction owner, MapState mapState, GameState gameState)
        {
            if(currentLine == 4)
            {
                this.frame = 2;
            }
        }
        public void Despawn(Interaction owner, MapState mapState, GameState gameState)
        {
            var character = owner as Character;
            if (character == null)
                return;
            if (!character.BubbleIsInteractive)
            {
                frame = 5;
                character.Dead = true;
                mapState.PlayerHasLearnedHowToExitItemMenu = true;
            }
            else
            {
                frame = 0;
            }
        }
    }
    partial class Beggar : Dialogue
    {
        private String[] Lines
        {
            get
            {
                return new String[]{
                    "Кто-то идёт...#### Не мне одному не спится этой ночью.",
                    "Скажи, путник, нет ли у тебя немного угля? Если и ты мне не поможешь, то я обречен.",
                    "Представь, я ходил к угольщику, но его жена прогнала меня.",
                    "Всего лишь маленький совочек угля, чего ей это стоило. Но нет, эта злюка мне отказала.",
                    "Если я теперь умру от холода, это она меня убила, так и знай.",
                    ""
                };
            }
        }
        private String BucketLine
        {
            get
            {
                return "Зачем тебе моё ведёрко? В нём даже угольной пыли не осталось.";
            }
        }
        private bool PlayerHasBucket = false;
        private bool IsItTimeForBucketLine = false;
        private int currentLine = 0;
        public String CurrentLine
        {
            get
            {
                if (IsItTimeForBucketLine)
                    return BucketLine;
                if (Lines.Length > currentLine && currentLine >= 0)
                    return Lines[currentLine];
                return "";
            }
        }
        public void Update(double deltaTime, Interaction owner, MapState mapState, GameState gameState)
        {
            var character = owner as Character;
            if (character == null)
                return;
            if (dead)
                character.Dead = true;
            if (!PlayerHasBucket)
            {
                if (mapState.Items.FirstOrDefault(i => i is ItemBucket) != null)
                {
                    PlayerHasBucket = true;
                    IsItTimeForBucketLine = true;
                    if(character.SpeechTextBubble.SymbolsVisible >= character.SpeechTextBubble.Text.Length || currentLine == 0)
                    {
                        currentLine++;
                    }
                    character.BubbleIsInteractive = true;
                    character.SymbolsVisible = 0;
                }
            }
            if (character.isVisible)
            {
                frame = 0;
                if (!IsItTimeForBucketLine)
                {
                    if (currentLine == 2)
                    {
                        frame = 2;
                    }
                    if (currentLine == 4)
                    {
                        if (character.SpeechTextBubble.SymbolsVisible >= 32)
                        {
                            frame = 2;
                        }
                    }
                }
            }
        }
        public void Interact(Interaction owner, MapState mapState, GameState gameState)
        {
            if (IsItTimeForBucketLine)
            {
                IsItTimeForBucketLine = false;
            }
            else
            {
                currentLine++;
            }
            if (currentLine >= 5)
            {
                var character = owner as Character;
                if (character == null)
                    return;
                character.BubbleIsInteractive = false;
            }
        }
        public void Spawn(Interaction owner, MapState mapState, GameState gameState)
        {
            this.frame = 0;
            if (currentLine == 2)
            {
                this.frame = 2;
            }
        }
        public void Despawn(Interaction owner, MapState mapState, GameState gameState)
        {
            frame = 0;
            IsItTimeForBucketLine = false;
        }
    }
    partial class NastyWoman : Dialogue
    {
        public String[] Lines = null;
        private String[][] LLines
        {
            get
            {
                return new String[][]{
                    new String[]{
                        "Кто здесь?",
                        "Доктор?### Ах, до чего же неловко получилось. Простите, я была уверена, что это снова тот попрошайка.",
                        "Для вас, доктор, у нас всегда найдётся уголь. Вот, возьмите, полное ведёрко.",
                        ""
                    },
                    new String[]{
                        "",
                        "Доктор, вы вернулись?",
                        "Для вас у нас всегда найдётся уголь. Вот, возьмите, полное ведёрко.",
                        ""
                    },
                    new String[]{
                        "",
                        "Доктор, вы вернулись?",
                        "Всё истратили? Да на вас угля не напасёшься.\r\nЛадно, держите, но вы в долгу.",
                        ""
                    }
                };
            }
        }
        public int currentLineSet = 0;
        private int currentLine = 0;
        internal void UpdateToVersion1()
        {
            var lines = Lines;
            Lines = null;
            if (lines[0] != "")
            {
                currentLineSet = 0;
                return;
            }
            if (lines[2].StartsWith("Для"))
            {
                currentLineSet = 1;
                return;
            }
            currentLineSet = 2;
            return;
        }
        public String CurrentLine
        {
            get
            {
                if (LLines.Length > currentLineSet && currentLineSet >= 0)
                    if (LLines[currentLineSet].Length > currentLine && currentLine >= 0)
                        return LLines[currentLineSet][currentLine];
                return "";
            }
        }
        public void Update(double deltaTime, Interaction owner, MapState mapState, GameState gameState)
        {
            if(currentLine == 2)
            {
                var character = owner as Character;
                if (character == null)
                    return;
                if (character.SpeechTextBubble.SymbolsVisible >= 45)
                {
                    if (needToPlaySound)
                    {
                        SoundWorks.PlayCoalThrowing();
                        needToPlaySound = false;
                    }
                }
            }
        }
        private int fucksGiven = 0;
        public void Interact(Interaction owner, MapState mapState, GameState gameState)
        {
            currentLine++;
            if(currentLine == 1)
            {
                frame = 0;
            }
            else
            {
                var sign = mapState.PlayerPosition.X > Position.X ? 0 : -1;
                frame = 2 + sign;
            }
            if(currentLine == 2)
            {
                var b = (ItemBucket)mapState.Items.FirstOrDefault(i => i is ItemBucket);
                if (b == null || b.Uses == 2)
                {
                    currentLine++;
                }
                else
                {
                    fucksGiven++;
                    if(fucksGiven >= 2)
                    {
                        currentLineSet = 2;
                    }
                    b.Uses = 2;
                    needToPlaySound = true;
                }
            }
            if(currentLine == 3)
            {
                var character = owner as Character;
                if (character == null)
                    return;
                character.BubbleIsInteractive = false;
            }
        }
        private bool needToPlaySound;
        public void Spawn(Interaction owner, MapState mapState, GameState gameState)
        {
        }
        public void Despawn(Interaction owner, MapState mapState, GameState gameState)
        {
            if(currentLine > 0)
            {
                currentLineSet = 1;
                currentLine = 1;
                frame = 0;
                var character = owner as Character;
                if (character == null)
                    return;
                character.BubbleIsInteractive = true;
            }
        }
    }

    partial class Elder : Dialogue
    {
        private String[] Lines
        {
            get
            {
                return new String[]{
                    "Собрался в соседнюю деревню?",
                    "Вы, молодые, думаете, что можете всё что угодно.",
                    "Но жизнь очень коротка! Ты не представляешь, как она сжалась в моих воспоминаниях.",
                    "Настолько мало можно успеть, что мне трудно понять, как вам, молодым, не страшно.",
                    "",
                    "Ну что, теперь понимаешь, о чём я?",
                    "Даже обычной, вполне благополучной жизни далеко не всегда хватит, чтобы добраться до соседней деревни.",
                    "Вот и посуди сам, стоит ли пытаться?",
                    "",
                    "Кажется, ты так и не понял суть моих слов."
                };
            }
        }
        private int currentLine = 0;
        public String CurrentLine
        {
            get
            {
                if (Lines.Length > currentLine && currentLine >= 0)
                    return Lines[currentLine];
                return "";
            }
        }
        public void Update(double deltaTime, Interaction owner, MapState mapState, GameState gameState)
        {
            if (!talks)
            {
                frame = 0;
                return;
            }
            frame = 1;
            var character = owner as Character;
            if (character == null)
                return;
            if(currentLine == 1 && character.SpeechTextBubble.SymbolsVisible <= 12)
            {
                frame = 2;
            }
            if (currentLine == 2 && character.SpeechTextBubble.SymbolsVisible <= 22)
            {
                frame = 2;
            }
            if (currentLine == 3 && character.SpeechTextBubble.SymbolsVisible >= 62 && character.SpeechTextBubble.SymbolsVisible <= 70)
            {
                frame = 2;
            }
            if(currentLine == 6 && character.SpeechTextBubble.SymbolsVisible >= 50)
            {
                frame = 0;
            }
        }
        public void Interact(Interaction owner, MapState mapState, GameState gameState)
        {
            currentLine++;
            if (currentLine == 10)
                currentLine = 8;
            if (currentLine == 4 || currentLine == 8)
            {
                var character = owner as Character;
                if (character == null)
                    return;
                character.BubbleIsInteractive = false;
            }
        }
        bool talks;
        int oldWraps = 0;
        public void Spawn(Interaction owner, MapState mapState, GameState gameState)
        {
            talks = true;
            if (currentLine == 4 && mapState.Wraps != oldWraps)
            {
                var character = owner as Character;
                if (character == null)
                    return;
                character.BubbleIsInteractive = true;
                currentLine++;
            }
            if (currentLine == 8 && mapState.Wraps != oldWraps)
            {
                var character = owner as Character;
                if (character == null)
                    return;
                character.BubbleIsInteractive = true;
                currentLine++;
            }
            oldWraps = mapState.Wraps;
        }
        public void Despawn(Interaction owner, MapState mapState, GameState gameState)
        {
            talks = false;
        }
    }
    [Serializable]
    partial class Madman : Dialogue
    {
        private String[] Lines
        {
            get
            {
                return new String[]{
                    "Герцог Фридрих, это снова вы? Костьми лягу, но не пущу.\r\nВозвращайтесь обратно в свой склеп!",
                    "Ты же мертвец, что ты делаешь по эту сторону забора?\r\nА ну возвращайся обратно!",
                    "Ночью вход на кладбище закрыт, возвращайся утром!\r\nА лучше и утром не возвращайся!"
                };
            }
        }
        private int currentLine = 0;
        public String CurrentLine
        {
            get
            {
                if (Lines.Length > currentLine && currentLine >= 0)
                    return Lines[currentLine];
                return "";
            }
        }
        public void Update(double deltaTime, Interaction owner, MapState mapState, GameState gameState)
        {
        }
        public void Interact(Interaction owner, MapState mapState, GameState gameState)
        {
        }
        public void Spawn(Interaction owner, MapState mapState, GameState gameState)
        {
            if(mapState.PlayerPosition.Y > Position.Y)
            {
                mapState.HasMetMadman = true;
                currentLine = 0;

                var character = owner as Character;
                if (character == null)
                    return;
                character.BubbleIsInteractive = false;
                character.SpeechTextBubble.Position = this.Position + new RealPoint(0, -50);
                character.SpeechTextBubble.Alignment = new RealPoint(0.5, 1);
            }
            else
            {
                if (mapState.HasMetMadman)
                    currentLine = 1;
                else
                    currentLine = 2;

                var character = owner as Character;
                if (character == null)
                    return;
                character.BubbleIsInteractive = false;
                character.SpeechTextBubble.Position = this.Position + new RealPoint(0,5);
                character.SpeechTextBubble.Alignment = new RealPoint(0.5, 0);
            }
            frame = 1;
        }
        public void Despawn(Interaction owner, MapState mapState, GameState gameState)
        {
            frame = 0;
        }
    }

    [Serializable]
    partial class Ghost : Dialogue
    {
        private String[][] LLines
        {
            get
            {
                return new String[][]
                {
                    new String[]{
                        "Хотели попасть в замок, а дорога привела на кладбище?",
                        "Вы не первый, да и не последний. Все когда-нибудь допускают подобную ошибку.",
                        "Замок и правда в той стороне, да только старый сторож вас не пропустит.",
                        "Он очень трепетно относится к своим обязанностям.",
                        ""
                    },
                    new String[]{
                        "Я так и знал, что он вас тоже примет за восставшего из мёртвых.",
                        "До сих пор не возьму в толк, он правда в это верит или придуривается?",
                        "Во всяком случае, князя он убедил в важности своей службы настолько, что тот вручил ему орден и планирует усилить охрану кладбища.",
                        "Утром его сменит другой караульный. Придётся ждать здесь.",
                        "Или буря стихнет раньше. Тогда можно будет обойти кладбище.",
                        ""
                    }
                };
            }
        }
        private int currentLineSet = 0;
        private int currentLine = 0;
        public String CurrentLine
        {
            get
            {
                if (LLines.Length > currentLineSet && currentLineSet >= 0)
                    if (LLines[currentLineSet].Length > currentLine && currentLine >= 0)
                        return LLines[currentLineSet][currentLine];
                return "";
            }
        }
        public void Update(double deltaTime, Interaction owner, MapState mapState, GameState gameState)
        {
        }
        public void Interact(Interaction owner, MapState mapState, GameState gameState)
        {
            currentLine++;

            if (currentLine == 4 && currentLineSet == 0)
            {
                var character = owner as Character;
                if (character == null)
                    return;
                character.BubbleIsInteractive = false;
            }

            if (currentLineSet == 1 && currentLine == 5)
            {
                var character = owner as Character;
                if (character == null)
                    return;
                character.BubbleIsInteractive = false;
            }

            frame = 0;
        }
        public void Spawn(Interaction owner, MapState mapState, GameState gameState)
        {
            isInDialogue = true;
            if (currentLine == 0)
                frame = 1;
            else
                frame = 0;
            if (mapState.HasMetMadman)
            {
                if (currentLineSet == 0)
                {
                    currentLineSet = 1;
                    currentLine = 0;
                    frame = 1;

                    var character = owner as Character;
                    if (character == null)
                        return;
                    character.BubbleIsInteractive = true;
                }
            }
        }
        public void Despawn(Interaction owner, MapState mapState, GameState gameState)
        {
            frame = 1;
            frameTimer = 40;
            isInDialogue = false;
        }
    }

    [Serializable]
    class DarkDoctorDialogue : Dialogue
    {
        private String[] Lines
        {
            get
            {
                return new String[]{
                    "Вот это да! Уж кого не ожидал здесь встретить, так это своего коллегу.####\r\nВы ведь, судя по одежде, тоже врач?",
                    "Не поверите, что со мной приключилось.",
                    "Лошадь издохла от мороза, а тут, как назло, надо ехать на вызов, но дорога не близкая, а метель не утихает.",
                    "Может быть, чем идти пешком, лучше было бы остаться дома. Но тогда и должности лишишься, и в опалу попадёшь...#### да и совесть замучает, в конце-то концов.",
                    ""
                };
            }
        }
        private int currentLine = 0;
        public String CurrentLine
        {
            get
            {
                if (Lines.Length > currentLine && currentLine >= 0)
                    return Lines[currentLine];
                return "";
            }
        }
        public void Update(double deltaTime, Interaction owner, MapState mapState, GameState gameState)
        {
        }
        public void Interact(Interaction owner, MapState mapState, GameState gameState)
        {
            currentLine++;
            if (currentLine == 4)
            {
                var character = owner as Character;
                if (character == null)
                    return;
                character.BubbleIsInteractive = false;
            }
        }
        public void Spawn(Interaction owner, MapState mapState, GameState gameState)
        {
        }
        public void Despawn(Interaction owner, MapState mapState, GameState gameState)
        {
        }
    }

    [Serializable]
    partial class Guard : Dialogue
    {
        private String[] Lines
        {
            get
            {
                return new String[]{
                    "Обычно у нас и один доктор - редкость. А тут - сразу два!",
                    "Стоять! Мне приказано вас арестовать.",
                    "Но его-то за что? Вы, видимо, перепутали его со мной, мы похоже одеты и...",
                    "Ты думаешь, я не могу отличить одного врача от другого? Хочешь, чтобы я и тебя задержал?",
                    "Нет. Меня тоже не за что задерживать. Никто из нас не стучал в ворота.",
                    "Так мне и не нужен тот, кто стучал в ворота. Приказом требуется арестовать именно сообщника.",
                    "А про тебя пока что никаких приказов не поступало. Но будь готов, как только - так сразу.",
                    "Задержанный, проследуйте в канцелярию, назначенный вам адвокат хочет обсудить детали вашего дела.",
                    "Ну а ты что стоишь? Тебя там больной ждёт, умирает. Проходи, не задерживайся.",
                    ""
                };
            }
        }
        private int currentLine = 0;
        public String CurrentLine
        {
            get
            {
                if (Lines.Length > currentLine && currentLine >= 0)
                    return Lines[currentLine];
                return "";
            }
        }
        bool started;
        public void Update(double deltaTime, Interaction owner, MapState mapState, GameState gameState)
        {
            if (gameState.EndingPhase == 0)
                return;

            var c = owner as Character;
            if (c == null)
                return;

            if(!started)
            {
                started = true;
                c.ActionInteractor = new CircleInteractor(Position, 99999);
                c.BubbleIsInteractive = true;
                c.Interact(mapState, gameState);
            }

            if(currentLine == 2 || currentLine == 4)
            {
                c.SpeechTextBubble.Position = mapState.DarkDoctor.position + new RealPoint(20, -50);
                c.SpeechTextBubble.MaxWidth = 200;
            }
            else
            {
                c.SpeechTextBubble.Position = this.Position + new RealPoint(0, -62);
                c.SpeechTextBubble.MaxWidth = 280;
                if(currentLine == 8)
                    c.SpeechTextBubble.MaxWidth = 200;
            }

            if (gameState.EndingPhase == 3)
            {
                c.BubbleIsInteractive = true;
                c.Interact(mapState, gameState);
                c.BubbleIsInteractive = false;
                gameState.EndingPhase = 4;
            }
            if (gameState.EndingPhase == 5)
            {
                currentLine = 9;
            }

            frame = 0;
            if(currentLine == 1)
            {
                frame = 2;
            }
            if(currentLine == 3 && c.SpeechTextBubble.SymbolsVisible > 75)
            {
                frame = 1;
            }
            if (currentLine == 5 && c.SpeechTextBubble.SymbolsVisible > 75)
            {
                frame = 2;
            }
            if (currentLine == 6)
            {
                frame = 1;
            }
            if (currentLine == 7 && c.SpeechTextBubble.SymbolsVisible > 10)
            {
                frame = 2;
            }
            if (currentLine == 8 && c.SpeechTextBubble.SymbolsVisible < 20)
            {
                frame = 1;
            }
        }
        public void Interact(Interaction owner, MapState mapState, GameState gameState)
        {
            currentLine++;
            if(currentLine == 7)
            {
                gameState.EndingPhase = 2;
            }
            if(currentLine >= 7)
            {
                var character = owner as Character;
                if (character == null)
                    return;
                character.BubbleIsInteractive = false;
            }
        }
        public void Spawn(Interaction owner, MapState mapState, GameState gameState)
        {
            var c = owner as Character;
            if (c == null)
                return;
            c.BubbleIsInteractive = false;
        }
        public void Despawn(Interaction owner, MapState mapState, GameState gameState)
        {
        }
    }


    [Serializable]
    partial class Nomad : Dialogue
    {
        bool timeToGetFurious;
        private String[] Lines
        {
            get
            {
                return new String[]{
                    "Не трожь мою сумку!",
                    "Ты что творишь, ворюга!\r\nА ну положи на место!",
                    "Я тебя запомнил, бесстыжий!"
                };
            }
        }
        private int currentLine = 0;
        public String CurrentLine
        {
            get
            {
                if (Lines.Length > currentLine && currentLine >= 0)
                    return Lines[currentLine];
                return "";
            }
        }
        public void Update(double deltaTime, Interaction owner, MapState mapState, GameState gameState)
        {
            if(timeToGetFurious)
            {
                var c = owner as Character;
                if (c == null)
                    return;
                frame = 1;
                timeToGetFurious = false;
                c.BubbleIsInteractive = true;
                c.Interact(mapState, gameState);
                c.BubbleIsInteractive = false;
            }
            if (currentLine == 1)
            {
                if ((mapState.PlayerPosition - this.Position).Length > 90)
                {
                    var c = owner as Character;
                    if (c == null)
                        return;
                    c.BubbleIsInteractive = true;
                    c.Interact(mapState, gameState);
                    c.BubbleIsInteractive = false;
                }
            }
        }
        public void Interact(Interaction owner, MapState mapState, GameState gameState)
        {
            currentLine++;
        }
        public void Spawn(Interaction owner, MapState mapState, GameState gameState)
        {
            if (currentLine > 0)
            {
                frame = 1;
            }
        }
        public void Despawn(Interaction owner, MapState mapState, GameState gameState)
        {
            frame = 0;
        }
    }
}
