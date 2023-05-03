using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnowInTheNight
{
    class GameState
    {
        public RealPoint CameraPosition = RealPoint.Zero;
        public List<Snowflake> Snowflakes = new List<Snowflake>();
        public bool IsLeftStep;
        public bool IsStormy = false;
        private IntPoint MapCameraOffset = IntPoint.Zero;

        private static DateTime gameStart = DateTime.Now;
        private static double saveGameSeconds = 0;
        public int SecondsFromStart = 0;
        public int FPS = 0;
        public enum Leg { Left, Right };
        Leg CurrentLeg = Leg.Right;
        bool needNewInput = false;
        static double minStepTime = 0.15;
        static double minStepTime2 = 0.3;
        double lastStepTimer = minStepTime;
        public bool isOnPavedRoad = false;
#if DEBUG
        public static bool ElfMode = false;
        public static bool ShamanMode = false;
#endif
        public static double StartScreenPhase = 0;
        public static bool InStartScreen
        {
            get
            {
                return StartScreenPhase < 2;
            }
        }
        public static bool ShowInputs = false;
        public double GameEndTimer = 0;
        public int EndingPhase;
        public bool GameEnds;
        public List<DateTime> LastFails = new List<DateTime>();
        public void Update(MapState mapState, double deltaTime)
        {
            if(Meta.InputController.InputsButton.Read())
            {
                ShowInputs = !ShowInputs;
            }
            if(Meta.InputController.LanguageButton.Read())
            {
                TextWorks.Language = (TextWorks.Language == TextWorks.Lang.English) ? TextWorks.Lang.Russian : TextWorks.Lang.English;
            }
            if(InStartScreen)
            {
                if(Meta.InputController.InteractionButton.Read())
                {
                    StartScreenPhase++;
                }

                if (StartScreenPhase > 0)
                {
                    StartScreenPhase += 0.07 * deltaTime;
                }
                CameraPosition = new RealPoint(0, 0);
                UpdateSnowflakes(mapState, deltaTime);
                UpdateTimerAndFPS(deltaTime);

                if (MapState.IntroDoctor != null)
                {
                    MapState.IntroDoctor.SetTarget(new RealPoint(100, -300));
                    MapState.IntroDoctor.Update(deltaTime, mapState, this);
                }
                return;
            }

            if (GameEnds)
            {
                GameEndTimer += deltaTime;
                if(GameEndTimer > 15)
                {
                    GameForm.CloseForm();
                }
            }

            var isInLoadMenu = Core.LoadGameMenu.IsOpen();
            var isInExitMenu = Core.ExitGameMenu.IsOpen();

            if(mapState.PlayerHealth <= -3 && !isInLoadMenu)
            {
                Core.ExitGameMenu.Close();
                IsInInventoryMode = false;
                isInExitMenu = false;
                isInLoadMenu = true;
                GameSaveManager.Enqueue(new GameSaveManager.ClearAction());
                Core.LoadGameMenu.Open(GameSaveManager.GetLoadMenu(mapState));
            }

            UpdateTimerAndFPS(deltaTime);
            RunScripts(deltaTime, mapState);
            if (!isInLoadMenu && !isInExitMenu && !GameEnds)
            {
                UpdateInventory(mapState);
            }
            else
            {
                if(isInLoadMenu)
                    Core.LoadGameMenu.Update(deltaTime, mapState, this);
                if(isInExitMenu)
                    Core.ExitGameMenu.Update(deltaTime);
            }
            if (!Core.MapMode && !IsInInventoryMode && !isInLoadMenu && !isInExitMenu && !GameEnds)
            {
                if (EndingPhase != 1)
                {
                    UpdatePlayer(mapState, deltaTime);
                }
                else
                {
                    if (mapState.GuardPosition != null)
                    {
                        RealPoint direction;
                        bool inputIsNew;
                        Meta.InputController.Direction.Read(out direction, out inputIsNew);
                        if (inputIsNew)
                        {
                            var pos = mapState.PlayerPosition;
                            var legDir = (mapState.GuardPosition - pos).Normalize().Rotate90() * MapState.LegLength;
                            mapState.LeftLeg = pos + legDir;
                            mapState.RightLeg = pos - legDir;
                        }
                    }
                }
                CollidePlayer(mapState, deltaTime);
            }

            if(!Core.MapMode)
            {
                if (mapState.DarkDoctor != null)
                {
                    mapState.DarkDoctor.Update(deltaTime, mapState, this);
                }
            }


            UpdateInteractions(mapState, deltaTime);
            if (!Core.MapMode && !IsInInventoryMode && !isInLoadMenu && !isInExitMenu)
            {
                TryInteract(mapState, deltaTime);
            }

            if (!GameEnds)
            {
                UpdateMap(mapState, deltaTime);
            }
            UpdateDecorations(deltaTime, mapState);
            //if (Core.MapMode)
            {
                CameraPosition = mapState.PlayerPosition + (RealPoint)MapCameraOffset;
            }
            /*else
            {
                CameraPosition = (mapState.PlayerPosition - CameraPosition) * 0.1 + CameraPosition;
            }*/
            UpdateSnowflakes(mapState, deltaTime);
            UpdateTrainer(mapState);
        }

        private void UpdateTrainer(MapState mapState)
        {
            if (EndingPhase > 0)
            {
                isOnPavedRoad = false;
                return;
            }

            var directionBack = ColliderWorks.AntiCollide(mapState.PavedRoad, mapState.PlayerPosition);
            if (directionBack == RealPoint.Infinity)
                directionBack = RealPoint.Zero;
            isOnPavedRoad = (directionBack.Length == 0);
        }

        private static string GetStepsInitializingString(MapState mapState)
        {
            String s;
            s = "";
            foreach (var step in mapState.Steps)
            {
                s += "Steps.Add(new Step() { Position = new RPoint(" + (int)step.Position.X + "," + (int)step.Position.Y + "), Direction = new RPoint(" + (int)step.Direction.X + "," + (int)step.Direction.Y + ") });\r\n";
            }
            return s;
        }

        private void RunScripts(double deltaTime, MapState mapState)
        {
            foreach(var script in mapState.Scripts.ToArray())
            {
                script.Update(deltaTime, mapState, this);
            }
        }

        private bool mapTextUnveiled = false;
        private void UpdateMap(MapState mapState, double deltaTime)
        {
            if (!mapTextUnveiled)
            {
                var mapTexts = mapState.Decorations.Where(d => d is MapText).Select(d => d as MapText);
                var mapText1 = mapTexts.FirstOrDefault(t => t.TextIndex == 1);
                if (mapText1 != null)
                {
                    var center = ((mapText1.Position + new RealPoint(200, 100)) / 40).Round();
                    Unveil(mapState, center);
                    mapTextUnveiled = true;
                }
            }
            {
                var center = (mapState.PlayerPosition / 40).Round();
                Unveil(mapState, center);
            }
            if (Core.MapMode)
            {
                var shift = Meta.InputController.MapAxis.Read();
                MapCameraOffset += shift * 10;
            }
            else
            {
                MapCameraOffset = IntPoint.Zero;
            }
        }

        private static void Unveil(MapState mapState, IntPoint center)
        {
            for (int i = -5; i <= 5; i++)
            {
                for (int j = -5; j <= 5; j++)
                {
                    if (i * i * j * j < 400)
                    {
                        var cell = center + new IntPoint(i, j);
                        if (!mapState.Visited.Contains(cell))
                            mapState.Visited.Add(cell);
                    }
                }
            }
        }
        public bool IsInInventoryMode = false;
        public int SelectedItem = 0;
        private void UpdateInventory(MapState mapState)
        {
            var modeChangeRequested = Meta.InputController.InventoryButton.Read();
            if(Core.MapMode)
            {
                var actionRequested = Meta.InputController.InteractionButton.Read();
                if (actionRequested || modeChangeRequested)
                {
                    Core.MapMode = false;
                    IsInInventoryMode = false;
                }
                return;
            }
            if(modeChangeRequested)
            {
                IsInInventoryMode = !IsInInventoryMode;
            }
            var itemChange = Meta.InputController.ItemAxis.Read();
            if(IsInInventoryMode)
            {
                SelectedItem += itemChange;
                SelectedItem = (SelectedItem + mapState.Items.Count) % mapState.Items.Count;
                var actionRequested = Meta.InputController.InteractionButton.Read();
                if(actionRequested)
                {
                    mapState.Items[SelectedItem].Action(mapState, this);
                }
            }
        }

        private void UpdateDecorations(double deltaTime, MapState mapState)
        {
            foreach (var decoration in mapState.DynamicDecorations.ToArray())
            {
                decoration.Update(deltaTime, mapState, this);
            }
        }

        public Interaction ActiveInteraction = null;
        private void UpdateInteractions(MapState mapState, double deltaTime)
        {
            if (Core.MapMode)
                return;
            ActiveInteraction = ColliderWorks.FindBestInteraction(mapState.Interactions, mapState.PlayerPosition, mapState.PlayerDirection);


            foreach (var interaction in mapState.Interactions.ToArray())
            {
                interaction.Update(interaction == ActiveInteraction, deltaTime, mapState, this);
            }
        }
        private void TryInteract(MapState mapState, double deltaTime)
        {
            if (Core.MapMode)
                return;
            var actionRequested = Meta.InputController.InteractionButton.Read();
            
            if (ActiveInteraction != null && actionRequested)
            {
                ActiveInteraction.Interact(mapState, this);
            }
            /*{
                interaction.Update(deltaTime);
                var wasVisible = interaction.Visible;
                interaction.Visible = true;// ColliderWorks.CalculateInteractiveDistance(interaction, mapState.PlayerPosition, mapState.PlayerDirection);
                if(wasVisible && !interaction.Visible)
                {
                    interaction.OnStopBeingVisible(this, mapState);
                }
                if (!wasVisible && interaction.Visible)
                {
                    interaction.OnStartBeingVisible(this, mapState);
                }
                if(interaction.Visible && actionRequested)
                {
                    interaction.StartInteraction(this, mapState);
                }
            }*/
        }

        private void CollidePlayer(MapState mapState, double deltaTime)
        {
            mapState.PlayerPosition += ColliderWorks.Collide(mapState.Colliders.Concat(mapState.DynamicColliders), mapState.PlayerPosition);
        }
        
        double fpsTimer = 0;
        private void UpdateTimerAndFPS(double deltaTime)
        {
            fpsTimer -= deltaTime;
            if (fpsTimer <= 0)
            {
                fpsTimer = 1;
                if (deltaTime == 0)
                {
                    FPS = -1;
                }
                else
                {
                    FPS = (int)(1.0 / deltaTime);
                }
            }
            if(InStartScreen)
            {
                gameStart = DateTime.Now;
            }
            if (!GameEnds)
            {
                SecondsFromStart = (int)(TimeFromStart);
            }
        }

        public static double TimeFromStart
        {
            get
            {
                return (DateTime.Now - gameStart).TotalSeconds + saveGameSeconds;
            }
            set
            {
                gameStart = DateTime.Now;
                saveGameSeconds = value;
            }
        }

        Random R = new Random();
        RealPoint WindDirection = RealPoint.Zero;
        RealPoint stormDirection = RealPoint.Zero;
        public double timeInSnow = 0;
        public bool lastMomentSaveUsed = false;
        private void UpdateSnowflakes(MapState mapState, double deltaTime)
        {
            WindDirection += new RealPoint(R.NextDouble() * 200 - 100, R.NextDouble() * 200 - 100) * deltaTime;
            var maxspeed = 50;
            if (WindDirection.X > maxspeed || WindDirection.Y > maxspeed || WindDirection.X < -maxspeed || WindDirection.Y < -maxspeed)
            {
                WindDirection = RealPoint.Zero;
            }

            var directionBack = ColliderWorks.AntiCollide(mapState.Anticolliders, mapState.PlayerPosition);
            if (directionBack == RealPoint.Infinity)
                directionBack = RealPoint.Zero;
            if (EndingPhase > 2)
            {
                directionBack = new RealPoint(0, 200);
            }

            IsStormy = (directionBack.Length > 100);
            SoundWorks.HighWindOn = IsStormy;
            if(IsStormy)
            {
                if (!Core.MapMode)
                {
                    timeInSnow += deltaTime;
                    if (EndingPhase == 0)
                    {
                        if (timeInSnow >= 5)
                        {
                            if (mapState.PlayerHealth >= 0)
                            {
                                mapState.PlayerHealth -= 0.1 * deltaTime;
                            }
                            else
                            {
                                if (directionBack.Length < 110 && !lastMomentSaveUsed)
                                {
                                    mapState.PlayerHealth += 0.1;
                                    lastMomentSaveUsed = true;
                                }
                                else
                                    mapState.PlayerHealth -= 0.5 * deltaTime;
                            }
                        }
                    }
                }
            }
            else
            {
                lastMomentSaveUsed = false;
                timeInSnow = 0.0;
                mapState.PlayerHealth += 0.1 * deltaTime;
                mapState.PlayerHealth = Math.Min(1, mapState.PlayerHealth);
            }

            
            if(timeInSnow > 4)
            {
                if (Core.IsBigModeStable() && Core.BigMode)
                {
                    Core.BigMode = false;
                    SoundWorks.PlayLanternOffByWind();
                }
            }

            var newStromDirection = directionBack * (IsStormy? 2 : 0);
            if (stormDirection != RealPoint.Zero && newStromDirection != RealPoint.Zero)
            {
                int i = 0;
                while (newStromDirection * stormDirection < 0.7 && i <100)
                {
                    i++;
                    newStromDirection = ((newStromDirection + stormDirection) / 2).Normalize() * newStromDirection.Length;
                }
            }
            for(int i =0 ; i< Core.Ticks; i++)
            {
                stormDirection += (newStromDirection - stormDirection) * 0.02;
            }
            

            foreach (var snowflake in Snowflakes)
            {
                if (!IsStormy)
                {
                    snowflake.Position += new RealPoint(0, maxspeed * (R.NextDouble() + 0.5)) * deltaTime;
                }
                snowflake.Position += WindDirection * deltaTime;
                snowflake.Position += stormDirection * deltaTime;
            }

            var spawnRadius = GameForm.FitWidth * 3 / 4;

            foreach(var s in Snowflakes)
            {
                var delta = s.Position - CameraPosition;
                if (Math.Abs(delta.X) > spawnRadius)
                {
                    var sx = (delta.X + spawnRadius*7) % (spawnRadius * 2) - spawnRadius;
                    var sy = -spawnRadius + R.NextDouble() * spawnRadius * 2;
                    s.Position = CameraPosition + new RealPoint(sx, sy);
                }
                if(Math.Abs(delta.Y) > spawnRadius)
                {
                    var sx = -spawnRadius + R.NextDouble() * spawnRadius * 2;
                    var sy = (delta.Y + spawnRadius*7) % (spawnRadius * 2) - spawnRadius;
                    s.Position = CameraPosition + new RealPoint(sx, sy);
                }
            }

            var wantedCount = IsStormy ? 400 : 200;

            var maxSpawnDespawn = 0;
            
            for(int i =0; i<Core.Ticks; i++)
            {
                maxSpawnDespawn += (int)Math.Ceiling((wantedCount - Snowflakes.Count - maxSpawnDespawn) * 0.01);
            }

            maxSpawnDespawn = Math.Abs(maxSpawnDespawn);
            
            if(Snowflakes.Count == 0)
            {
                maxSpawnDespawn = 10000;
            }

            while (Snowflakes.Count < wantedCount && maxSpawnDespawn > 0)
            {
                maxSpawnDespawn--;
                var sx = -spawnRadius + R.NextDouble() * spawnRadius*2;
                var sy = -spawnRadius + R.NextDouble() * spawnRadius*2;
                Snowflakes.Add(new Snowflake() { Position = CameraPosition + new RealPoint(sx, sy)});
            }

            while (Snowflakes.Count > wantedCount && maxSpawnDespawn > 0)
            {
                maxSpawnDespawn--;
                var i = R.Next(0, Snowflakes.Count);
                Snowflakes.RemoveAt(i);
            }
        }
        private RealPoint oldDirection = RealPoint.Zero;
        private void UpdatePlayer(MapState mapState, double deltaTime)
        {
            var speed = IsStormy? 100 : isOnPavedRoad? 300 : 200;
            
            RealPoint direction;
            bool inputIsNew;
            lastStepTimer += deltaTime;
            if (isOnPavedRoad)
            {
                direction = (RealPoint)Meta.InputController.MapAxis.Read();
                if (direction != RealPoint.Zero)
                    direction = direction.Normalize();
                inputIsNew = direction != oldDirection;
                if (inputIsNew && lastStepTimer < minStepTime2)
                    oldDirection = direction;
                else
                    direction = oldDirection;
            }
            else
            {
                if (lastStepTimer < minStepTime && Meta.InputController.Direction.InputHasChanged(oldDirection))
                {
                    LastFails.Add(DateTime.Now);
                    direction = RealPoint.Zero;
                    //direction = oldDirection;
                    inputIsNew = false;
                    // New input breaks the step
                    // And character keeps moving
                }
                else
                {
                    Meta.InputController.Direction.Read(out direction, out inputIsNew);

                    oldDirection = direction;
                }
            }

            LastFails = LastFails.Where(f => (DateTime.Now - f).TotalSeconds < 10).ToList();

            if (EndingPhase >= 2)
            {
                direction = new RealPoint(0, direction.Y);
                if (direction != new RealPoint(0, -1))
                {
                    direction = RealPoint.Zero;
                }
            }
            
#if DEBUG
            if(ElfMode)
            {
                mapState.LeftLeg += direction * speed* deltaTime;
                mapState.RightLeg += direction * speed * deltaTime;
            }
#endif

            if (inputIsNew)
                lastStepTimer = 0;

            if (inputIsNew || (needNewInput && isOnPavedRoad))
            {
                StartNewStep(mapState, direction);
            }

            if (mapState.PlayerHealth <= 0)
                return;

            if (direction.SquaredLength > 0.5) // direction != RPoint.Zero
            {
                direction = FixMisdirection(mapState, direction);

                RealPoint fixedLeg = CurrentLeg == Leg.Left ? mapState.RightLeg : mapState.LeftLeg;
                RealPoint movingLeg = CurrentLeg == Leg.Left ? mapState.LeftLeg : mapState.RightLeg;

                speed = FixOverspeed(speed, direction, fixedLeg, movingLeg);

                GetNewLegPosition(deltaTime, speed, direction, ref fixedLeg, ref movingLeg);

                if (CurrentLeg == Leg.Left)
                {
                    mapState.LeftLeg = movingLeg;
                    mapState.RightLeg = fixedLeg;
                }
                else // CurrentLeg == Leg.Right
                {
                    mapState.RightLeg = movingLeg;
                    mapState.LeftLeg = fixedLeg;
                }
            }

            UpdatePlayerPositionAnomaly(mapState);
        }
        private static double stepTimer = 0;
        private void GetNewLegPosition(double deltaTime, int speed, RealPoint direction, ref RealPoint fixedLeg, ref RealPoint movingLeg)
        {
            var delta = movingLeg - fixedLeg;
            var codirection = direction.Rotate90();
            var x = delta * codirection;
            var y = delta * direction;

            var limit = Math.Max(MapState.LegLength, y);

            y += deltaTime * speed;

            if(isOnPavedRoad)
            {
                if (!IsStormy)
                {
                    limit = 1.5 * MapState.LegLength;
                }

                if (y > limit)
                    needNewInput = true;
            }

            y = Math.Min(limit, y);
            y = Math.Max(-2 * MapState.LegLength, y);
            var signx = x >= 0 ? 1 : -1;
            var dx = signx * Math.Sqrt(4 * MapState.LegLength * MapState.LegLength - y * y) - x;

            if (!isOnPavedRoad)
            {
                movingLeg = fixedLeg + direction * y + codirection * (x + dx);
            }
            else
            {
                movingLeg = fixedLeg + direction * y + codirection * (x + dx / 2);
                fixedLeg = fixedLeg - codirection * dx / 2;
            }
        }

        private static int FixOverspeed(int speed, RealPoint direction, RealPoint fixedLeg, RealPoint movingLeg)
        {
            var delta = movingLeg - fixedLeg;
            //if (delta * direction <= 0)
            {
                speed /= 2;
            }
            if (delta * direction < -MapState.LegLength)
            {
                speed /= 2;
            }
            return speed;
        }

        private static RealPoint FixMisdirection(MapState mapState, RealPoint direction)
        {
            var delta = mapState.RightLeg - mapState.LeftLeg;
            var perp = delta.Rotate90();
            if (perp * direction < 0)
            {
                direction = perp / perp.Length;
            }
            return direction;
        }

        private void StartNewStep(MapState mapState, RealPoint direction)
        {
            if (mapState.PlayerHealth <= 0)
            {
                mapState.PlayerHealth -= 1;
                return;
            }
            stepTimer = 0;
            needNewInput = false;
            SoundWorks.PlaySomeFootstep();
            var delta = mapState.RightLeg - mapState.LeftLeg;
            IsLeftStep = delta * direction > 0;
            var newLeg = IsLeftStep ? Leg.Left : Leg.Right;
            CreateStep(mapState, delta, newLeg);
            CurrentLeg = newLeg;
        }

        private void CreateStep(MapState mapState, RealPoint d, Leg newLeg)
        {
#if DEBUG
            if (ElfMode)
                return;
#endif
            var position = newLeg == Leg.Left ? mapState.LeftLeg : mapState.RightLeg;

            {
                var directionBack = ColliderWorks.AntiCollide(mapState.Anticolliders, position);
                if (directionBack == RealPoint.Infinity)
                    directionBack = RealPoint.Zero;
                var isStormy = (directionBack.Length > 100);

                if (isStormy)
                    return;
            }
            {
                var directionBack = ColliderWorks.AntiCollide(mapState.PavedRoad, position);
                if (directionBack == RealPoint.Infinity)
                    directionBack = RealPoint.Zero;
                var isPaved = (directionBack.Length == 0);

                if (isPaved)
                    return;
            }
            mapState.Steps.Add(new MapState.Step()
            {
                Position = position,
                Direction = d.Rotate90()
            });
            CreateAnomalyStep(mapState, d, position);
        }
        private static RealPoint AnomalyShift = new RealPoint(-200,800);
        private static void CreateAnomalyStep(MapState mapState, RealPoint d, RealPoint position)
        {
            var delta = position - mapState.Anomaly;
            if (delta.X < 400 && delta.X > -400)
            {
                if (delta.Y < 500 && delta.Y > 0)
                {
                    mapState.Steps.Add(new MapState.Step()
                    {
                        Position = position - AnomalyShift,
                        Direction = d.Rotate90()
                    });
                }
                if (delta.Y < 0 && delta.Y > -500)
                {
                    mapState.Steps.Add(new MapState.Step()
                    {
                        Position = position + AnomalyShift,
                        Direction = d.Rotate90()
                    });
                }
            }
        }

        private void UpdatePlayerPositionAnomaly(MapState mapState)
        {
            var delta = mapState.PlayerPosition - mapState.Anomaly;
            if (delta.X < 400 && delta.X > -400)
            {
                if (delta.Y < 500 && delta.Y > 400)
                {
                    var shift = -AnomalyShift;
                    mapState.PlayerPosition += shift;
                    foreach(var snowflake in Snowflakes)
                    {
                        snowflake.Position += shift;
                    }
                    mapState.Wraps += 1;
                }
                if (delta.Y < -400 && delta.Y > -500)
                {
                    var shift = AnomalyShift;
                    mapState.PlayerPosition += shift;
                    foreach (var snowflake in Snowflakes)
                    {
                        snowflake.Position += shift;
                    }
                    mapState.Wraps -= 1;
                }
            }
        }
    }
    class Snowflake
    {
        public RealPoint Position;
    }
}
