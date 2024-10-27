using System;
using System.Drawing;

namespace GraphicEditor
{
    public class CustomLine : Base
    {
        protected Base left;
        protected Base right;

        public CustomLine(Base left, Base right, int size = 3)
        {
            this.left = left;
            this.right = right;
            this.size = size;
            identityZone = size * 2;
            fillColor = Color.Black;
            type = BaseType.Line;
            visible = true;
            multiplicity = 1;
            name = left.Name + " " + right.Name;
        }

        public CustomLine()
        {
            left = null;
            right = null;
        }

        public CustomLine(string[] inputString)
        {
            left = null;
            right = null;
            size = int.Parse(inputString[3]);
            identityZone = size * 2;
            fillColor = Color.FromName(inputString[2]);
            visible = true;
            type = BaseType.Line;
            multiplicity = 1;
        }

        public CustomLine(CustomLine tmp)
        {
            left = null;
            right = null;
            name = tmp.name;
            size = tmp.size;
            identityZone = size * 2;
            fillColor = tmp.fillColor;
            type = BaseType.Line;
            visible = true;
            multiplicity = 1;
        }

        public CustomLine Invert()
        {
            CustomLine result = this;
            Base shape = result.left;
            result.left = right;
            result.right = shape;
            result.name = left.Name + " " + right.Name;
            return result;
        }

        public override void SetMultiplicity(int k = 0)
        {
            multiplicity++;
            multiplicity = multiplicity % 4;
            if (multiplicity == 0) multiplicity = 1;
        }

        public string LineToString()
        {
            return $"Line|{name}|{ColorToString()}|{size}";
        }

        private string ColorToString()
        {
            string temp = fillColor.ToString();
            int startIndex = temp.IndexOf('[');
            int endIndex = temp.IndexOf(']');
            string result = temp.Substring(startIndex + 1, endIndex - startIndex - 1);
            return result;
        }

        public override void Shift(int deltaX, int deltaY = 0)
        {
            left.Shift(deltaX, deltaY);
            right.Shift(deltaX, deltaY);
        }

        public override bool HitTest(int x, int y)
        {
            double distance;
            if (((CustomPoint)left).X == ((CustomPoint)right).X)
            {
                distance = Math.Abs(x - ((CustomPoint)left).X);
                if (distance <= identityZone)
                {
                    return true;
                }
                return false;
            }

            double k = (((CustomPoint)right).Y - ((CustomPoint)left).Y) / (((CustomPoint)right).X - ((CustomPoint)left).X);
            double b = ((CustomPoint)left).Y - k * ((CustomPoint)left).X;
            distance = Math.Abs(k * x - y + b) / Math.Sqrt(k * k + 1);
            if (distance <= identityZone)
            {
                return true;
            }
            return false;
        }


        public override void Draw(Graphics g)
        {
            Pen pen = new Pen(fillColor, size);
            g.DrawLine(pen, ((CustomPoint)left).X, ((CustomPoint)left).Y, ((CustomPoint)right).X, ((CustomPoint)right).Y);
        }

        public Base Left
        {
            get { return left; }
            set { left = value; }
        }

        public Base Right
        {
            get { return right; }
            set { right = value; }
        }
    }
}
