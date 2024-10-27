using System;
using System.Drawing;

namespace GraphicEditor
{
    public class CustomPoint : Base
    {
        private int x, y;
        public CustomPoint(string name, int x = 0, int y = 0, int size = 3)
        {
            this.name = name;
            this.x = x;
            this.y = y;
            this.name = name;
            this.size = size;
            identityZone = size * 2;
            fillColor = Color.Black;
            type = BaseType.Point;
            visible = true;
            multiplicity = 1;
        }

        public override void SetMultiplicity(int k)
        {
            if (k > 0)
            {
                multiplicity++;
            }
            else
            {
                multiplicity--;
            }
        }

        public override void Draw(Graphics g)
        {
            g.FillEllipse(new SolidBrush(fillColor), x - size, y - size, size * 2, size * 2);
        }

        public override void Shift(int deltaX, int deltaY = 0)
        {
            x += deltaX;
            y += deltaY;
        }

        public override bool HitTest(int x, int y)
        {
            double distance = Math.Sqrt(Math.Pow(x - this.x, 2) + Math.Pow(y - this.y, 2));
            return distance < identityZone;
        }

        public string PointToString()
        {
            return $"Point|{name}|{x}|{y}|{ColorToString()}|{size}|{multiplicity}";
        }

        private string ColorToString()
        {
            string temp = fillColor.ToString();
            int startIndex = temp.IndexOf('[');
            int endIndex = temp.IndexOf(']');
            string result = temp.Substring(startIndex + 1, endIndex - startIndex - 1);
            return result;
        }

        public int X
        {
            get { return x; }
            set { x = value; }
        }

        public int Y
        {
            get { return y; }
            set { y = value; }
        }
    }
}
