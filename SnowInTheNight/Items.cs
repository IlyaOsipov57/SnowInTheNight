using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace SnowInTheNight
{
    static class Items
    {
    }

    interface Item
    {
        Image Image
        {
            get;
        }
        String Description
        {
            get;
        }
        void Action(MapState mapState, GameState gameState);

        int Index { get; }
    }
    [Serializable]
    class ItemBag : Item
    {
        public Image Image
        {
            get
            {
                return SnowInTheNight.Properties.Resources.bag;
            }
        }
        public String Description
        {
            get
            {
                return "Дорожная сумка. В неё сложены все остальные предметы. (Z - закрыть инвентарь)";
            }
        }
        public void Action(MapState mapState, GameState gameState)
        {
            gameState.IsInInventoryMode = false;
        }
        public int Index
        {
            get
            {
                return 0;
            }
        }
    }
    [Serializable]
    class ItemMap : Item
    {
        public Image Image
        {
            get
            {
                return SnowInTheNight.Properties.Resources.map;
            }
        }
        public String Description
        {
            get
            {
                return "Карта. На ней зарисованы те места, где я уже побывал. (X - посмотреть)";
            }
        }
        public void Action(MapState mapState, GameState gameState)
        {
            Core.MapMode = !Core.MapMode;
        }

        public int Index
        {
            get
            {
                return 2;
            }
        }
    }
    [Serializable]
    class ItemLantern : Item
    {
        public Image Image
        {
            get
            {
                return SnowInTheNight.Properties.Resources.lantern;
            }
        }
        public String Description
        {
            get
            {
                if(Core.IsBigModeStable())
                {
                    if (Core.BigMode)
                        return "Фонарь горит. С ним можно разглядеть, что происходит вдали. (X - потушить)";
                    else
                        return "Фонарь потушен. Без него дальше вытянутой руки не видно. (X - зажечь)";
                }

                if (Core.BigMode)
                    return "Фонарь потушен. Без него дальше вытянутой руки не видно.";
                else
                    return "Фонарь горит. С ним можно разглядеть, что происходит вдали.";
            }
        }
        public void Action(MapState mapState, GameState gameState)
        {
            if (mapState.PlayerHasLearnedHowToExitItemMenu)
                gameState.IsInInventoryMode = false;

            if (!Core.IsBigModeStable())
                return;
            if(Core.BigMode)
            {
                Core.BigMode = false;
                SoundWorks.PlayLanternOff();
            }
            else
            {
                Core.BigMode = true;
                SoundWorks.PlayLanternOn();
            }
        }

        public int Index
        {
            get
            {
                return 1;
            }
        }
    }
    [Serializable]
    class ItemKey : Item
    {
        public Image Image
        {
            get
            {
                return SnowInTheNight.Properties.Resources.key;
            }
        }
        public String Description
        {
            get
            {
                return "Ключ от дома. Надеюсь, в моё отсутствие там ничего не случится. (Х - выйти из игры)";
            }
        }
        public void Action(MapState mapState, GameState gameState)
        {
            //GameForm.CloseForm();
            gameState.IsInInventoryMode = false;
            Core.ExitGameMenu.Open();
        }

        public int Index
        {
            get
            {
                return 5;
            }
        }
    }
    [Serializable]
    class ItemBell : Item
    {
        public bool Unusable = false;
        public Image Image
        {
            get
            {
                return SnowInTheNight.Properties.Resources.bell;
            }
        }
        public String Description
        {
            get
            {
                //return "Колокольчик. Его звон навевает воспоминания. (Х - вернуться к началу)";
                if (Unusable)
                {
                    return "Колокольчик. Его звон навевает воспоминания. (Недоступно)";
                }
                else
                {
                    return "Колокольчик. Его звон навевает воспоминания. (Х - вернуться к костру)";
                }
            }
        }
        public void Action(MapState mapState, GameState gameState)
        {
            if (!Unusable)
            {
                gameState.IsInInventoryMode = false;
                Core.LoadGameMenu.Open(GameSaveManager.GetTravelMenu());
            }
        }

        public int Index
        {
            get
            {
                return 4;
            }
        }
    }
    [Serializable]
    class ItemBucket : Item
    {
        public int Uses = -1;
        public Image Image
        {
            get
            {
                if(Uses <= 0)
                    return SnowInTheNight.Properties.Resources.emptyBucket;
                if (Uses == 1)
                    return SnowInTheNight.Properties.Resources.usedBucket;
                return SnowInTheNight.Properties.Resources.fullBucket;
            }
        }
        public String Description
        {
            get
            {
                if(Uses == -1)
                    return "Пустое ведро. Возможно, угольщик одолжит мне немного угля.";
                if(Uses == 0)
                    return "Пустое ведро. У меня больше не сталось угля.";
                if (Uses == 1)
                    return "Ведро с углём. Уголь заканчивается, но ещё на один раз должно хватить.";
                return "Ведро, полное угля. Можно развести костёр и немного согреться.";
            }
        }
        public void Action (MapState mapState, GameState gameState)
        {

        }

        public int Index
        {
            get
            {
                return 3;
            }
        }
    }
}
