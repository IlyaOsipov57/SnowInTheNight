using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SnowInTheNight
{
    static class GameSaveManager
    {
        static Queue<GameSaveAction> actionQueue = new Queue<GameSaveAction>();
        static GameSave gameSave;

        public static void Enqueue(GameSaveAction action)
        {
            if (Closing)
                return;
            actionQueue.Enqueue(action);
        }
        public static bool IsClosing()
        {
            return Closing;
        }
        public static void Close()
        {
            Closing = true;
        }
        static bool Closing = false;
        static void Join()
        {
            if (gameSave != null)
            {
                gameSave.JoinSubroutine();
            }
        }
        public static void TakeActions(ref MapState mapState, GameState gameState)
        {
            if(Closing)
            {
                if(actionQueue.Count == 0)
                {
                    Join();
                    GameForm.CloseForm();
                }
            }

            if (actionQueue.Count == 0)
                return;
                
            Join();

            var action = actionQueue.Dequeue();
            action.Do(ref gameSave, ref mapState, gameState);
        }

        #region Actions
        public interface GameSaveAction
        {
            void Do(ref GameSave gameSave, ref MapState mapState, GameState gameState);
        }
        public class LoadAction : GameSaveAction
        {
            public LoadAction(GameSave saveToLoad, GameSave[] openedGameSaves, int sessionIndex)
            {
                this.SaveToLoad = saveToLoad;
                this.OpenedGameSaves = openedGameSaves;
                this.SessionIndex = sessionIndex;
            }
            GameSave SaveToLoad;
            GameSave[] OpenedGameSaves;
            int SessionIndex;
            public void Do(ref GameSave gameSave, ref MapState mapState, GameState gameState)
            {
                gameSave = SaveToLoad;
                mapState = gameSave.Load(mapState, gameState, SessionIndex);
                Core.OnLoad();
                foreach (var openedGameSave in OpenedGameSaves)
                {
                    if (openedGameSave != SaveToLoad)
                        openedGameSave.Close();
                }
            }
        }
        public class CreateAction : GameSaveAction
        {
            public CreateAction(String filepath, GameSave[] openedGameSaves, int sessionIndex)
            {
                this.Filepath = filepath;
                this.OpenendGameSaves = openedGameSaves;
                this.SessionIndex = sessionIndex;
            }

            String Filepath;
            GameSave[] OpenendGameSaves;
            int SessionIndex;
            public void Do(ref GameSave gameSave, ref MapState mapState, GameState gameState)
            {
                gameSave = new GameSave(mapState, gameState, Filepath, SessionIndex);

                foreach (var openedGameSave in OpenendGameSaves)
                {
                    openedGameSave.Close();
                }
            }
        }
        public class ReloadAction : GameSaveAction
        {
            public void Do(ref GameSave gameSave, ref MapState mapState, GameState gameState)
            {
                mapState = gameSave.Reload(mapState);
                Core.OnLoad();
            }
        }
        public class TravelAction : GameSaveAction
        {
            public TravelAction(Location target)
            {
                this.Target = target;
            }
            Location Target;
            public void Do(ref GameSave gameSave, ref MapState mapState, GameState gameState)
            {
                gameSave.Travel(mapState, Target);
                mapState.PlayerHealth = Math.Max(mapState.PlayerHealth, 0);
                GameSaveManager.Enqueue(new SaveAction(Target));
            }
        }

        public class SaveAction : GameSaveAction
        {
            public SaveAction(Location saveLocation)
            {
                this.SaveLocation = saveLocation;
            }
            Location SaveLocation;
            public void Do(ref GameSave gameSave, ref MapState mapState, GameState gameState)
            {
                gameSave.Save(mapState, gameState, SaveLocation);
            }
        }

        public class AutosaveAction : GameSaveAction
        {
            public AutosaveAction() { }
            public void Do(ref GameSave gameSave, ref MapState mapState, GameState gameState)
            {
                gameSave.Autosave(mapState, gameState);
            }
        }

        public class ClearAction : GameSaveAction
        {
            public ClearAction() { }
            public void Do(ref GameSave gameSave, ref MapState mapState, GameState gameState)
            {
                gameSave.ClearAutosave();
            }
        }
        #endregion
            
        public static bool GetSavesMenu(out LoadMenuData loadMenu)
        {
            int i = 1;
            var freshGameSaveFilename = StaticNames.SaveFilepath + i.ToString();
            loadMenu = new LoadMenuData();
            if (!Directory.Exists(StaticNames.SavesDirectoryPath))
            {
                Enqueue(new CreateAction(freshGameSaveFilename, new GameSave[0], 0));
                return false;
            }

            var gameSavePaths = Directory.GetFiles(StaticNames.SavesDirectoryPath, StaticNames.SaveFilename + "*");
            while (gameSavePaths.Contains(freshGameSaveFilename))
            {
                i++;
                freshGameSaveFilename = StaticNames.SaveFilepath + i.ToString();
            }

            var gameSaves = gameSavePaths
                .Select((filepath) => Open(filepath))
                .Where((save) => save != null)
                .OrderByDescending(save => save.GetIndex())
                .ToArray();
            if (gameSaves.Length == 0)
            {
                Enqueue(new CreateAction(freshGameSaveFilename, new GameSave[0], 0));
                return false;
            }

            var sessionIndex = gameSaves[0].GetIndex() + 1;
            loadMenu.Items = gameSaves.Select((save) => new LoadMenuData.LoadMenuItem()
            {
                Image = save.GetGameThumbnail(),
                LocationId = save.GetLocationId(),
                Time = save.GetGameTime(),
                OnChosen = new Action(() =>
                {
                    Enqueue(new LoadAction(save, gameSaves, sessionIndex));
                    SoundWorks.PlayLoadSound();
                })
            }).ToArray();
            loadMenu.OnExit = new Action(() =>
            {
                Enqueue(new CreateAction(freshGameSaveFilename, gameSaves, sessionIndex));
            });
            loadMenu.CanExit = true;
            loadMenu.Hint = "X - загрузить\r\nZ - отмена";

            return true;
        }

        private static GameSave Open(String filepath)
        {
            var opener = new GameSave.GameOpener();
            return opener.Open(filepath);
        }

        public static LoadMenuData GetLoadMenu(MapState mapState)
        {
            var loadMenu = new LoadMenuData();

            var canTravel = mapState.Items.Any(i => i is ItemBell);
            var location = gameSave.GetLastLocation();

            loadMenu.Items = new LoadMenuData.LoadMenuItem[]{
                new LoadMenuData.LoadMenuItem()
                {
                    Image = gameSave.GetSaveThumbnail(),
                    LocationId = location.Id,
                    OnChosen = new Action(() =>
                    {
                        if (canTravel)
                            Enqueue(new TravelAction(location));
                        else
                            Enqueue(new ReloadAction());
                    })
                }};
            loadMenu.CanExit = false;
            loadMenu.Hint = canTravel ? "Больше идти нет сил.\r\nX - вернуться" : "Больше идти нет сил.\r\nX - загрузить";

            return loadMenu;
        }
        public static LoadMenuData GetTravelMenu()
        {
            var loadMenu = new LoadMenuData();

            var travelLocations = Location.SaveLoadOrder.Where(location => gameSave.GetOpenedLocations().ContainsKey(location.Id)).ToArray();

            loadMenu.Items = travelLocations.Select(location => new LoadMenuData.LoadMenuItem()
            {
                Image = gameSave.GetOpenedLocations()[location.Id].Thumbnail,
                LocationId = location.Id,
                OnChosen = new Action(() =>
                {
                    Enqueue(new TravelAction(location));
                    SoundWorks.PlayLoadSound();
                })
            }).ToArray();
            loadMenu.CanExit = true;
            loadMenu.Hint = "X - переместиться\r\nZ - отмена";

            if (travelLocations.Length == 0)
                loadMenu.Hint = "Нет сохранений\r\nZ - отмена";

            return loadMenu;
        }
        public static bool IsVisited (Location location)
        {
            if (gameSave == null)
                return false;
            else
            {
                return gameSave.GetOpenedLocations().ContainsKey(location.Id);
            }
        }
    }
}
