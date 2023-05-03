using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SnowMapEditor
{
    public class UMapAxis
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
    public class UAxis
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
            var result = last;
            if (queue.Count == 0)
                last = null;
            else
                last = queue.Last();

            if (!result.HasValue)
                return 0;
            if (result.Value == left)
                return -1;
            if (result.Value == right)
                return 1;
            return 0;
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

    public class UClick
    {
        private bool _pressed = false;
        private bool _wasRead = false;
        private Keys key;
        public UClick(Keys key)
        {
            this.key = key;
        }
        public bool IsDown ()
        {
            return _pressed;
        }
        public bool Read()
        {
            var result = _pressed && !_wasRead;
            if (result)
            {
                _wasRead = true;
            }
            return result;
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
        public void Update(double deltaTime)
        {

        }
        public void Reset()
        {
            _wasRead = false;
            _pressed = false;
        }
    }

    public class UMouseClick
    {
        private bool _pressed = false;
        private bool _wasRead = true;
        private MouseButtons key;
        public UMouseClick(MouseButtons key)
        {
            this.key = key;
        }
        public bool Read()
        {
            return _pressed;
        }
        public bool WasPressed()
        {
            if (_wasRead)
                return false;
            if (_pressed)
            {
                _wasRead = true;
                return true;
            }
            return false;
        }
        public bool WasReleased()
        {
            if (_wasRead)
                return false;
            if (!_pressed)
            {
                _wasRead = true;
                return true;
            }
            return false;
        }
        public void KeyDown(MouseButtons key)
        {
            if (key != this.key)
                return;
            _wasRead = false;
            _pressed = true;
        }
        public void KeyUp(MouseButtons key)
        {
            if (key != this.key)
                return;
            _wasRead = false;
            _pressed = false;
        }
        public void Update(double deltaTime)
        {

        }
        public void Reset()
        {
            _wasRead = false;
            _pressed = false;
        }
    }
}
