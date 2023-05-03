using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SnowInTheNight
{
    class UAxisForgetful
    {
        Keys? last = null;
        bool isNew = false;

        Dictionary<Keys, RealPoint> validKeys;
        List<Keys> keysDown;
        public UAxisForgetful(Keys left, Keys right, Keys up, Keys down)
        {
            this.validKeys = new Dictionary<Keys, RealPoint>{
                {left, (RealPoint)IntPoint.Left},
                {right, (RealPoint)IntPoint.Right},
                {up, (RealPoint)IntPoint.Up},
                {down, (RealPoint)IntPoint.Down}
            };
            this.keysDown = new List<Keys>();
        }
        public void Read(out RealPoint direction, out bool inputIsNew)
        {
            lock (Meta.Controller.InputLock)
            {
                direction =  RealPoint.Zero;
                if (last.HasValue)
                {
                    Meta.InputController.Accept(last.Value);
                    direction = validKeys[last.Value];
                }
                inputIsNew = isNew;
                isNew = false;
            }
        }
        public bool InputHasChanged(RealPoint oldDirection)
        {
            lock (Meta.Controller.InputLock)
            {
                var direction = last.HasValue ? validKeys[last.Value] : RealPoint.Zero;
                if (direction != oldDirection)
                    return true;
                return isNew;
            }
        }
        public void KeyDown(Keys key)
        {
            if (validKeys.Keys.Contains(key))
            {
                if (keysDown.Contains(key))
                    return;
                last = key;
                isNew = true;
                keysDown.Add(key);
            }
        }
        public void KeyUp(Keys key)
        {
            if (keysDown.Contains(key))
                keysDown.Remove(key);
            if (last == key)
            {
                last = null;
                isNew = false;
            }
        }
        public void Update(double deltaTime)
        {
        }
    }
    class UItemAxis
    {
        List<Keys> queue = new List<Keys>();
        Keys left;
        Keys right;
        private double ignore = 0;
        public UItemAxis (Keys left, Keys right)
        {
            this.left = left;
            this.right = right;
        }
        public int Read()
        {
            lock (Meta.Controller.InputLock)
            {
                var result = (Keys?)null;

                if (queue.Count > 0)
                    result = queue.Last();

                if (!result.HasValue)
                    return 0;

                Meta.InputController.Accept(result.Value);

                if (ignore < 0)
                    ignore = 0;
                if (ignore > 0)
                    return 0;

                ignore = 0.2;
                if (result.Value == left)
                    return -1;
                if (result.Value == right)
                    return 1;
                return 0;
            }
        }
        public void KeyDown(Keys key)
        {
            if (key != left && key != right)
                return;
            queue.Add(key);
        }
        public void KeyUp(Keys key)
        {
            if (key != left && key != right)
                return;
            queue.RemoveAll(q => q == key);
        }
        public void Reset()
        {
            queue.Clear();
        }

        public void Update(double deltaTime)
        {
            ignore -= deltaTime;
        }
    }
    class UMapAxis
    {
        UAxis horizontal;
        UAxis vertical;
        public UMapAxis(Keys left, Keys right, Keys up, Keys down)
        {
            horizontal = new UAxis(left, right);
            vertical = new UAxis(up, down);
        }
        public IntPoint Read()
        {
            return new IntPoint(horizontal.Read(), vertical.Read());
        }
        public void KeyDown(Keys key)
        {
            horizontal.KeyDown(key);
            vertical.KeyDown(key);
        }
        public void KeyUp(Keys key)
        {
            horizontal.KeyUp(key);
            vertical.KeyUp(key);
        }
        public void Reset()
        {
            horizontal.Reset();
            vertical.Reset();
        }
        public void Update(double deltaTime)
        {
            horizontal.Update(deltaTime);
            vertical.Update(deltaTime);
        }
    }
    class UAxis
    {
        Keys? last = null;
        List<Keys> queue = new List<Keys>();
        Keys left;
        Keys right;
        public UAxis(Keys minus, Keys plus)
        {
            this.left = minus;
            this.right = plus;
        }
        public int Read()
        {

            lock (Meta.Controller.InputLock)
            {
                var result = last;

                if (queue.Count == 0)
                    last = null;
                else
                    last = queue.Last();

                if (!result.HasValue)
                    return 0;

                Meta.InputController.Accept(result.Value);

                if (result.Value == left)
                    return -1;
                if (result.Value == right)
                    return 1;
                return 0;
            }
        }
        public void KeyDown(Keys key)
        {
            if (key != left && key != right)
                return;
            last = key;
            queue.Add(key);
        }
        public void KeyUp(Keys key)
        {
            if (key != left && key != right)
                return;
            queue.RemoveAll(q => q == key);
        }
        public void Reset()
        {
            last = null;
            queue.Clear();
        }

        internal void Update(double deltaTime)
        {
        }
    }

    class UClick
    {
        private bool _pressed = false;
        private bool _wasRead = false;
        private Keys key;
        public UClick (Keys key)
        {
            this.key = key;
        }
        public bool Read()
        {

            lock (Meta.Controller.InputLock)
            {
                var result = _pressed && !_wasRead;
                if (result)
                {
                    _wasRead = true;
                    Meta.InputController.Accept(key);
                }
                return result;
            }
        }
        public void KeyDown(Keys key)
        {
            if (key != this.key)
                return;
            _pressed = true;
        }
        public void KeyUp(Keys key)
        {
            if (key != this.key)
                return;
            _wasRead = false;
            _pressed = false;
        }
        public void Update (double deltaTime)
        {

        }
        public void Reset()
        {
            _wasRead = false;
            _pressed = false;
        }
    }

    class UButtonView
    {
        private bool _pressed = false;
        private bool _accepted = false;
        private bool _wasPressed = false;
        private Keys key;
        public UButtonView(Keys key)
        {
            this.key = key;
        }
        public bool Read()
        {
            var result = _wasPressed;
            return result;
        }
        public bool Accepted ()
        {
            return _accepted;
        }
        public void Accept(Keys key)
        {
            if (key != this.key)
                return;
            _accepted = true;
        }
        public void KeyDown(Keys key)
        {
            if (key != this.key)
                return;
            _pressed = true;
        }
        public void KeyUp(Keys key)
        {
            if (key != this.key)
                return;
            _pressed = false;
        }
        public void Update(double deltaTime)
        {
            lock (Meta.Controller.InputLock)
            {
                _accepted = false;
                _wasPressed = _pressed;
            }
        }
        public void Reset()
        {
            _pressed = false;
        }
    }
}
