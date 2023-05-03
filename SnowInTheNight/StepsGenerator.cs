using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SnowInTheNight
{
    class StepsGenerator
    {
        private StepsGenerator() { }
        public static MapState.Step[] Generate(RealPoint[] polyline, int seed, double sloppyness)
        {
            if (polyline.Length < 1)
                return new MapState.Step[0];
            
            var leftDir = (polyline[1] - polyline[0]).Rotate90().Normalize() * MapState.LegLength;
            var gen = new StepsGenerator()
            {
                steps = new List<MapState.Step>(),
                LeftLeg = polyline[0] + leftDir,
                RightLeg = polyline[0] - leftDir,
                R = new Random(seed),
                sloppyness = Math.Min(1,Math.Max(0,sloppyness))
            };

            for (int i = 1; i < polyline.Length; i++)
            {
                gen.MoveTo(polyline[i]);
            }

            return gen.steps.ToArray();
        }
        List<MapState.Step> steps;
        RealPoint RightLeg;
        RealPoint LeftLeg;
        public enum Leg { Left, Right };
        Leg CurrentLeg = Leg.Right;
        double sloppyness;
        Random R;
        RealPoint position
        {
            get
            {
                return (RightLeg + LeftLeg) / 2;
            }
        }
        void MoveTo(RealPoint to)
        {
            var delta = to - position;
            while (delta.SquaredLength > 100)
            {
                var direction = delta.Normalize();

                StartNewStep(direction);

                direction = FixMisdirection(direction);

                RealPoint fixedLeg = CurrentLeg == Leg.Left ? RightLeg : LeftLeg;
                RealPoint movingLeg = CurrentLeg == Leg.Left ? LeftLeg : RightLeg;

                var stepLength = 8 + 16 * R.NextDouble();
                var coolStep = R.NextDouble() < sloppyness;

                movingLeg = GetNewLegPosition(coolStep, stepLength, direction, fixedLeg, movingLeg);

                if (CurrentLeg == Leg.Left)
                {
                    LeftLeg = movingLeg;
                }
                else // CurrentLeg == Leg.Right
                {
                    RightLeg = movingLeg;
                }
                delta = to - position;
            }
        }
        private RealPoint FixMisdirection(RealPoint direction)
        {
            var delta = RightLeg - LeftLeg;
            var perp = delta.Rotate90();
            if (perp * direction < 0)
            {
                direction = perp / perp.Length;
            }
            return direction;
        }
        private void StartNewStep(RealPoint direction)
        {
            var delta = RightLeg - LeftLeg;
            var newLeg = delta * direction > 0 ? Leg.Left : Leg.Right;

            steps.Add(new MapState.Step()
            {
                Position = newLeg == Leg.Left ? LeftLeg : RightLeg,
                Direction = delta.Rotate90()
            });

            CurrentLeg = newLeg;
        }
        private RealPoint GetNewLegPosition(bool stepIsCool, double stepLength, RealPoint direction, RealPoint fixedLeg, RealPoint movingLeg)
        {
            var delta = movingLeg - fixedLeg;
            var codirection = direction.Rotate90();
            var x = delta * codirection;
            var y = delta * direction;

            y += stepLength;

            if (!stepIsCool)
            {
                y = Math.Min(MapState.LegLength, y);
            }
            else
            {
                y = Math.Min(2*MapState.LegLength, y);
            }
            y = Math.Max(-2 * MapState.LegLength, y);
            var signx = x >= 0 ? 1 : -1;
            x = signx * Math.Sqrt(4 * MapState.LegLength * MapState.LegLength - y * y);

            movingLeg = fixedLeg + direction * y + codirection * x;
            return movingLeg;
        }
    }
    [Serializable]
    class ContinuousStepsGenerator : Collider
    {
        CircleCollider decoratedCollider;
        public ContinuousStepsGenerator(RealPoint position, RealPoint direction, int seed, double sloppyness)
        {
            var leftDir = direction.Rotate90().Normalize() * MapState.LegLength;
            this.LeftLeg = position + leftDir;
            this.RightLeg = position - leftDir;
            this.R = new Random(seed);
            this.sloppyness = Math.Min(1, Math.Max(0, sloppyness));
            this.Target = position;
            this.decoratedCollider = new CircleCollider(position, MapState.LegLength);
        }


        public RealPoint Collide(RealPoint point)
        {
            decoratedCollider.Position = this.position;
            return decoratedCollider.Collide(point);
        }
        public RealPoint AntiCollide(RealPoint point) { return RealPoint.Zero; }
        public void DrawAntiCollider(System.Drawing.Graphics g) { }


        public void SetTarget (RealPoint target)
        {
            this.Target = target;
        }
        public void Update (double deltaTime, MapState mapState, GameState gameState)
        {
            for (int i = 0; i < SpeedPercentage / 100 + 1; i++)
            {
                if (needNewInput)
                {
                    var delta = Target - position;
                    if (delta.SquaredLength > MapState.LegLength * MapState.LegLength)
                    {
                        var direction = delta.Normalize();

                        StartNewStep(direction, mapState);
                        var stepLength = 8 + 16 * R.NextDouble();
                        currentStepIsCool = R.NextDouble() < sloppyness;
                        currentStepLengthLeft = stepLength;
                        needNewInput = false;
                    }
                }
                if (!needNewInput)
                {
                    ContinueCurrentMove(deltaTime);
                    Collide(deltaTime, mapState);
                }
            }
            UpdateCold(deltaTime, mapState);
        }

        private void UpdateCold(double deltaTime, MapState mapState)
        {
            {
                var directionBack = ColliderWorks.AntiCollide(mapState.Anticolliders, position);
                if (directionBack == RealPoint.Infinity)
                    directionBack = RealPoint.Zero;
                isCold = (directionBack.Length > 100);
                if (isCold)
                {
                    if (!Core.MapMode)
                    {
                        timeInSnow += deltaTime;
                        if (timeInSnow >= 5)
                        {
                            if (health >= 0)
                            {
                                health -= 0.1 * deltaTime;
                            }
                        }
                    }
                }
                else
                {
                    timeInSnow = 0.0;
                    health += 0.1 * deltaTime;
                    health = Math.Min(1, health);
                }
            }
            {
                var directionBack = ColliderWorks.AntiCollide(mapState.PavedRoad, position);
                if (directionBack == RealPoint.Infinity)
                    directionBack = RealPoint.Zero;
                isOnPavedRoad = (directionBack.Length == 0);
            }
        }

        private void Collide(double deltaTime, MapState mapState)
        {
            if (!Uncollidable)
            {
                var shift = ColliderWorks.Collide(mapState.Colliders, position);
                position += shift;
                LeaveNoTrace = shift != RealPoint.Zero;
            }
        }

        private void ContinueCurrentMove(double deltaTime)
        {
            var speed = isCold ? 100 : isOnPavedRoad ? 300 : 200;
            speed = speed * SpeedPercentage / 100 / (SpeedPercentage / 100 + 1);
            
            var delta = Target - position;
            var direction = delta.Normalize();

            direction = FixMisdirection(direction);

            RealPoint fixedLeg = CurrentLeg == Leg.Left ? RightLeg : LeftLeg;
            RealPoint movingLeg = CurrentLeg == Leg.Left ? LeftLeg : RightLeg;

            speed = FixOverspeed(speed, direction, fixedLeg, movingLeg);

            movingLeg = GetNewLegPosition(deltaTime, speed, direction, fixedLeg, movingLeg);

            if (CurrentLeg == Leg.Left)
            {
                LeftLeg = movingLeg;
            }
            else // CurrentLeg == Leg.Right
            {
                RightLeg = movingLeg;
            }
        }

        private RealPoint GetNewLegPosition(double deltaTime, int speed, RealPoint direction, RealPoint fixedLeg, RealPoint movingLeg)
        {
            var delta = movingLeg - fixedLeg;
            var codirection = direction.Rotate90();
            var x = delta * codirection;
            var y = delta * direction;


            var yDelta = deltaTime * speed;
            if(currentStepLengthLeft<yDelta)
            {
                yDelta = currentStepLengthLeft;
                needNewInput = true;
            }
            currentStepLengthLeft -= yDelta;

            y += yDelta;
            {
                if (y > MapState.LegLength)
                {
                    needNewInput = true;
                }
                y = Math.Min(MapState.LegLength, y);
            }
            y = Math.Max(-2 * MapState.LegLength, y);
            var signx = x >= 0 ? 1 : -1;
            x = signx * Math.Sqrt(4 * MapState.LegLength * MapState.LegLength - y * y);

            movingLeg = fixedLeg + direction * y + codirection * x;
            return movingLeg;
        }

        private int FixOverspeed(int speed, RealPoint direction, RealPoint fixedLeg, RealPoint movingLeg)
        {
            var delta = movingLeg - fixedLeg;

            {
                speed /= 2;
            }
            if (delta * direction < -MapState.LegLength)
            {
                speed /= 2;
            }
            return speed;
        }

        private RealPoint FixMisdirection(RealPoint direction)
        {
            var delta = RightLeg - LeftLeg;
            var perp = delta.Rotate90();
            if (perp * direction < 0)
            {
                direction = perp / perp.Length;
            }
            return direction;
        }

        private void StartNewStep(RealPoint direction, MapState mapState)
        {
            if(Uncollidable || (mapState.PlayerPosition - position).Length < 100)
            {
                SoundWorks.PlaySomeFootstep();
            }

            var delta = RightLeg - LeftLeg;
            IsLeftStep = delta * direction > 0;
            var newLeg = IsLeftStep ? Leg.Left : Leg.Right;
            if(!LeaveNoTrace)
                CreateStep(mapState, delta, newLeg);
            CurrentLeg = newLeg;
        }
        private void CreateStep(MapState mapState, RealPoint d, Leg newLeg)
        {
            var position = newLeg == Leg.Left ? LeftLeg : RightLeg;

            if (Uncollidable)
            {
                MapState.IntroSteps.Add(new MapState.Step()
                {
                    Position = position,
                    Direction = d.Rotate90()
                });
                return;
            }

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
        }

        public int SpeedPercentage = 100;
        double currentStepLengthLeft;
        bool currentStepIsCool;
        public bool needNewInput = true;
        bool isCold;
        bool isOnPavedRoad;
        double timeInSnow = 0;
        double health = 1;

        RealPoint Target;
        public RealPoint RightLeg;
        public RealPoint LeftLeg;
        public bool IsLeftStep;
        public enum Leg { Left, Right };
        Leg CurrentLeg = Leg.Right;
        double sloppyness;
        Random R;
        public bool LockedOnInteraction = false;
        public bool LooselyFollower = false;
        public bool Uncollidable = false;
        public bool LeaveNoTrace = false;
        public RealPoint position
        {
            get
            {
                return (RightLeg + LeftLeg) / 2;
            }
            private set
            {
                var delta = value - position;
                RightLeg += delta;
                LeftLeg += delta;
            }
        }

        internal Drawer.PlayerDrawer GetDoctorDrawer()
        {
            return new Drawer.PlayerDrawer(LeftLeg, RightLeg)
            {
                Health = this.health,
                IsLeftStep = IsLeftStep
            };
        }
    }
}
