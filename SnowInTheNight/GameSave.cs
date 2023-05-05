using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;

namespace SnowInTheNight
{
    [Serializable]
    partial class GameSave
    {
        [NonSerialized]
        String FilePath;
        [NonSerialized]
        MemoryStream SaveStream;
        [NonSerialized]
        Image Thumbnail;
        [NonSerialized]
        Thread Subroutine;

        bool HasAutosave = false;
        int LastLocationId = 0;
        double GameTime = 0;
        int Index = 0;

        Dictionary<int, OpenedLocation> OpenedLocations = new Dictionary<int, OpenedLocation>();

        public GameSave(MapState mapState, GameState gameState, String filePath, int sessionIndex)
        {
            this.Index = sessionIndex;
            this.FilePath = filePath;
            this.LastLocationId = Location.DoctorsHome.Id;
            FixZeroLocation(mapState, gameState);
        }

        public void JoinSubroutine()
        {
            if (Subroutine != null)
                Subroutine.Join();
        }
        public Image GetGameThumbnail()
        {
            return Thumbnail;
        }
        public int GetLocationId()
        {
            if (HasAutosave)
                return Location.Autosave.Id;
            return LastLocationId;
        }
        public String GetGameTime()
        {
            return Core.TimeToString((int)GameTime);
        }
        public int GetIndex()
        {
            return Index;
        }
        public Image GetSaveThumbnail()
        {
            return OpenedLocations[LastLocationId].Thumbnail;
        }
        public Location GetLastLocation()
        {
            return Location.FromId(LastLocationId);
        }
        public Dictionary<int, OpenedLocation> GetOpenedLocations ()
        {
            return OpenedLocations;
        }
        private bool ZipExists()
        {
            return File.Exists(FilePath);
        }

        private void CreateZip()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(FilePath));
            new ZipOutputStream(File.Create(FilePath)).Close();
        }

        private ZipFile GetZip()
        {
            return new ZipFile(FilePath);
        }

        public void Autosave(MapState mapState, GameState gameState)
        {
            this.HasAutosave = true;
            this.GameTime = GameState.TimeFromStart;

            var thumbnail = CreateThumbnail(mapState, gameState);
            var autosaveStreamSource = new ZipStreamSource(mapState);

            Subroutine = new Thread(new ThreadStart(() => DoAutosave(thumbnail, autosaveStreamSource)));
            Subroutine.Start();
        }

        private void DoAutosave(Bitmap thumbnail, ZipStreamSource saveStreamSource)
        {
            var metaStreamSource = new ZipStreamSource(this);
            var thumbnailStreamSource = new ZipStreamSource(thumbnail);
            if (!ZipExists())
                CreateZip();
            using (var zipFile = GetZip())
            {
                zipFile.BeginUpdate();
                zipFile.Add(metaStreamSource, StaticNames.metaFilename);
                zipFile.Add(saveStreamSource, StaticNames.autosaveFilename);
                zipFile.Add(thumbnailStreamSource, StaticNames.autosaveThumbnailFilename);
                zipFile.CommitUpdate();
            }
        }

        public void ClearAutosave()
        {
            this.HasAutosave = false;
            this.GameTime = GameState.TimeFromStart;

            if (!ZipExists())
                return;

            Subroutine = new Thread(new ThreadStart(() => DoClearAutosave()));
            Subroutine.Start();
        }

        private void DoClearAutosave()
        {
            if (LastLocationId == 0)
            {
                if(File.Exists(FilePath))
                    File.Delete(FilePath);
                return;
            }

            var metaStreamSource = new ZipStreamSource(this);
            using (var zipFile = GetZip())
            {
                zipFile.BeginUpdate();
                zipFile.Add(metaStreamSource, StaticNames.metaFilename);
                if (zipFile.GetEntry(StaticNames.autosaveThumbnailFilename) != null)
                    zipFile.Delete(StaticNames.autosaveThumbnailFilename);
                if (zipFile.GetEntry(StaticNames.autosaveFilename) != null)
                    zipFile.Delete(StaticNames.autosaveFilename);
                zipFile.CommitUpdate();
            }
        }

        private void FixZeroLocation(MapState mapState, GameState gameState)
        {
            var location = Location.DoctorsHome;
            if (LastLocationId != location.Id)
                return;

            Bitmap thumbnail;
            SaveThumbnailToGameSave(mapState, gameState, location, out thumbnail);

            BinaryFormatter formatter = new BinaryFormatter();
            if (SaveStream != null)
                SaveStream.Close();
            SaveStream = new MemoryStream();
            formatter.Serialize(SaveStream, mapState);
        }

        public void Save(MapState mapState, GameState gameState, Location location)
        {
            this.HasAutosave = false;
            this.GameTime = GameState.TimeFromStart;

            Bitmap thumbnail;
            SaveThumbnailToGameSave(mapState, gameState, location, out thumbnail);

            var saveStreamSource = new ZipStreamSource(mapState);
            if (SaveStream != null)
                SaveStream.Close();
            SaveStream = saveStreamSource.GetCopy();

            Subroutine = new Thread(new ThreadStart(() => DoSave(location.ThumbnailFilename, thumbnail, saveStreamSource)));
            Subroutine.Start();
        }

        private void SaveThumbnailToGameSave(MapState mapState, GameState gameState, Location location, out Bitmap thumbnail)
        {
            this.LastLocationId = location.Id;
            if (!OpenedLocations.ContainsKey(LastLocationId))
            {
                OpenedLocations.Add(LastLocationId, new OpenedLocation());
            }
            var openedLocation = OpenedLocations[LastLocationId];
            openedLocation.Position = mapState.PlayerPosition;
            openedLocation.Direction = mapState.PlayerDirection;
            openedLocation.ThumbnailFilename = location.ThumbnailFilename;

            thumbnail = CreateThumbnail(mapState, gameState);

            if (openedLocation.Thumbnail != null)
                openedLocation.Thumbnail.Dispose();
            openedLocation.Thumbnail = thumbnail;
        }

        private void DoSave(String thumbnailFilename, Bitmap thumbnail, ZipStreamSource saveStreamSource)
        {
            var metaStreamSource = new ZipStreamSource(this);
            var thumbnailStreamSource = new ZipStreamSource(thumbnail);
            if (!ZipExists())
                CreateZip();
            using (var zipFile = GetZip())
            {
                zipFile.BeginUpdate();
                zipFile.Add(metaStreamSource, StaticNames.metaFilename);
                zipFile.Add(thumbnailStreamSource, thumbnailFilename);
                zipFile.Add(saveStreamSource, StaticNames.saveFilename);
                if (zipFile.GetEntry(StaticNames.autosaveThumbnailFilename) != null)
                    zipFile.Delete(StaticNames.autosaveThumbnailFilename);
                if (zipFile.GetEntry(StaticNames.autosaveFilename) != null)
                    zipFile.Delete(StaticNames.autosaveFilename);
                zipFile.CommitUpdate();
            }
        }

        private Bitmap CreateThumbnail(MapState mapState, GameState gameState)
        {
            var size = new IntPoint(144, 48);
            var thumbnail = new Bitmap(size.X, size.Y);
            var g = Graphics.FromImage(thumbnail);
            var cameraPosition = mapState.PlayerPosition;
            var pixelCameraPosition = cameraPosition.Round();

            g.TranslateTransform(size.X / 2, size.Y / 2);
            g.ScaleTransform(0.5f, 0.5f);
            g.TranslateTransform(-pixelCameraPosition.X, -pixelCameraPosition.Y);

            Drawer.Redraw(g, null, gameState, mapState, size, true);

            return thumbnail;
        }

        public MapState Load(MapState mapState, GameState gameState, int sessionIndex)
        {
            this.Index = sessionIndex;
            MapState loadedMapState;
            if (HasAutosave)
            {
                loadedMapState = LoadFromAutoSaveData();
                Thumbnail.Dispose();
                FixZeroLocation(mapState, gameState);
            }
            else
            {
                loadedMapState = LoadFromSaveData();
            }
            Thumbnail = null;

            Subroutine = new Thread(new ThreadStart(() => DoLoad()));
            Subroutine.Start();

            GameState.TimeFromStart = this.GameTime;

            FixMapState(mapState, loadedMapState);

            return loadedMapState;
        }

        private static void FixMapState(MapState fullMapState, MapState incompleteMapState)
        {
            incompleteMapState.PavedRoad = fullMapState.PavedRoad;
            incompleteMapState.Anticolliders = fullMapState.Anticolliders;
            incompleteMapState.DarkAnticolliders = fullMapState.DarkAnticolliders;
            incompleteMapState.Colliders = fullMapState.Colliders;
            incompleteMapState.Decorations = fullMapState.Decorations;
        }

        private void DoLoad()
        {
            using (var zipFile = GetZip())
            {
                foreach (var location in Location.SaveLoadOrder)
                {
                    if (!OpenedLocations.ContainsKey(location.Id))
                        continue;
                    var openedLocation = OpenedLocations[location.Id];
                    if (openedLocation.Thumbnail == null)
                    {
                        var entry = zipFile.GetEntry(openedLocation.ThumbnailFilename);
                        var stream = zipFile.GetInputStream(entry);
                        var bitmap = new Bitmap(stream);
                        openedLocation.Thumbnail = bitmap;
                    }
                }
            }
        }

        public MapState Reload(MapState mapState)
        {
            this.HasAutosave = false;
            this.GameTime = GameState.TimeFromStart;

            MapState loadedMapState;
            if (SaveStream == null)
            {
                loadedMapState = LoadFromSaveData();
            }
            else
            {
                var formatter = new BinaryFormatter();
                SaveStream.Seek(0, SeekOrigin.Begin);
                loadedMapState = (MapState)formatter.Deserialize(SaveStream);
            }

            Subroutine = new Thread(new ThreadStart(() => DoClearAutosave()));
            Subroutine.Start();

            FixMapState(mapState, loadedMapState);

            return loadedMapState;
        }

        private MapState LoadFromAutoSaveData()
        {
            MapState mapState = LoadFromFile(StaticNames.autosaveFilename);
            // update mapstate

            return mapState;
        }

        private MapState LoadFromSaveData()
        {
            MapState mapState = LoadFromFile(StaticNames.saveFilename);
            // update mapstate

            var formatter = new BinaryFormatter();
            if (SaveStream != null)
                SaveStream.Close();
            SaveStream = new MemoryStream();
            formatter.Serialize(SaveStream, mapState);

            return mapState;
        }

        private MapState LoadFromFile(string filename)
        {
            MapState mapState;
            using (var zipFile = GetZip())
            {
                var entry = zipFile.GetEntry(filename);
                var stream = zipFile.GetInputStream(entry);
                var formatter = new BinaryFormatter();
                mapState = (MapState)formatter.Deserialize(stream);
                return mapState;
            }
        }

        public bool Travel(MapState mapState, Location location)
        {
            if (!OpenedLocations.ContainsKey(location.Id))
                return false;

            var targetLocation = OpenedLocations[location.Id];

            mapState.PlayerPosition = targetLocation.Position;
            mapState.PlayerDirection = targetLocation.Position;
            return true;
        }

        public class GameOpener
        {
            public GameSave Open(String filePath)
            {
                try
                {
                    var zipFile = new ZipFile(filePath);

                    GameSave gameSave;
                    {
                        var entry = zipFile.GetEntry(StaticNames.metaFilename);
                        var stream = zipFile.GetInputStream(entry);
                        var formatter = new BinaryFormatter();
                        gameSave = (GameSave)formatter.Deserialize(stream);
                    }

                    gameSave.FilePath = filePath;

                    if (gameSave.HasAutosave)
                    {
                        var entry = zipFile.GetEntry(StaticNames.autosaveThumbnailFilename);
                        var stream = zipFile.GetInputStream(entry);
                        var bitmap = new Bitmap(stream);
                        gameSave.Thumbnail = bitmap;
                    }
                    else
                    {
                        var lastLocation = gameSave.OpenedLocations[gameSave.LastLocationId];
                        var entry = zipFile.GetEntry(lastLocation.ThumbnailFilename);
                        var stream = zipFile.GetInputStream(entry);
                        var bitmap = new Bitmap(stream);
                        lastLocation.Thumbnail = bitmap;
                        gameSave.Thumbnail = bitmap;
                    }

                    zipFile.Close();

                    return gameSave;
                }
                catch (Exception e)
                {
                    Program.ErrorLog(e);
                    return null;
                }
            }
        }

        public void Close()
        {
            if (SaveStream != null)
                SaveStream.Close();
            if (Thumbnail != null)
                Thumbnail.Dispose();
        }

        [Serializable]
        public class OpenedLocation
        {
            [NonSerialized]
            public Image Thumbnail;
            public String ThumbnailFilename;
            public RealPoint Position;
            public RealPoint Direction;
        }

        public class ZipStreamSource : IStaticDataSource
        {
            public MemoryStream DataStream
            {
                get;
                private set;
            }
            public Stream GetSource()
            {
                return DataStream;
            }

            public MemoryStream GetCopy()
            {
                var result = new MemoryStream();
                DataStream.CopyTo(result);
                DataStream.Seek(0, SeekOrigin.Begin);
                return result;
            }

            public ZipStreamSource(Bitmap b)
            {
                DataStream = new MemoryStream();
                b.Save(DataStream, System.Drawing.Imaging.ImageFormat.Png);
                DataStream.Seek(0, SeekOrigin.Begin);
            }
            public ZipStreamSource(MapState mapState)
            {
                BinaryFormatter formatter = new BinaryFormatter();
                DataStream = new MemoryStream();
                formatter.Serialize(DataStream, mapState);
                DataStream.Seek(0, SeekOrigin.Begin);
            }
            public ZipStreamSource(GameSave gameSave)
            {
                BinaryFormatter formatter = new BinaryFormatter();
                DataStream = new MemoryStream();
                formatter.Serialize(DataStream, gameSave);
                DataStream.Seek(0, SeekOrigin.Begin);
            }
        }
    }
}
