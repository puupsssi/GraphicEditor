using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace GraphicEditor
{
    internal class CustomRectangle : CustomLine
    {
        private int angle;

        public CustomRectangle(Base left, Base right, int size = 3, int angle = 0)
        {
            this.left = left;
            this.right = right;
            this.angle = angle;
            this.size = size;
            identityZone = size * 2;
            fillColor = Color.Black;
            type = BaseType.Rectangle;
            visible = true;
            multiplicity = 1;
            name = left.Name + " " + right.Name;
        }
        public CustomRectangle(string[] inputString)
        {
            left = null;
            right = null;
            size = int.Parse(inputString[3]);
            identityZone = size * 2;
            fillColor = Color.FromName(inputString[2]);
            visible = true;
            type = BaseType.Rectangle;
            multiplicity = 1;
            angle = int.Parse(inputString[4]);
        }

        public CustomRectangle(CustomRectangle tmp)
        {
            left = null;
            right = null;
            name = tmp.name;
            size = tmp.size;
            identityZone = size * 2;
            fillColor = tmp.fillColor;
            type = BaseType.Rectangle;
            visible = true;
            multiplicity = 1;
        }

        public override void Shift(int deltaX, int deltaY)
        {
            left.Shift(deltaX, deltaY);
            right.Shift(deltaX, deltaY);
        }

        public override void SetMultiplicity(int k = 0)
        {
            multiplicity %= 3;
            multiplicity++;
        }

        public override bool HitTest(int x, int y)
        {
            return false;
        }

        public override void Draw(Graphics graphics)
        {
            Pen pen = new Pen(fillColor, size);

            float centerX = (((CustomPoint)left).X + ((CustomPoint)right).X) / 2;
            float centerY = (((CustomPoint)left).Y + ((CustomPoint)right).Y) / 2;

            float width = Math.Abs(((CustomPoint)right).X - ((CustomPoint)left).X);
            float height = Math.Abs(((CustomPoint)right).Y - ((CustomPoint)left).Y);

            Matrix rotationMatrix = new Matrix();
            rotationMatrix.RotateAt(angle, new PointF(centerX, centerY));
            graphics.Transform = rotationMatrix;

            graphics.DrawRectangle(pen, centerX - width / 2, centerY - height / 2, width, height);

            rotationMatrix.RotateAt(-angle, new PointF(centerX, centerY));
            graphics.Transform = rotationMatrix;
        }

        public string RectangleToString()
        {
            return $"Rectangle|{name}|{ColorToString()}|{size}|{angle}";
        }
        private string ColorToString()
        {
            string temp = fillColor.ToString();
            int startIndex = temp.IndexOf('[');
            int endIndex = temp.IndexOf(']');
            string result = temp.Substring(startIndex + 1, endIndex - startIndex - 1);
            return result;
        }

        public int Angle
        {
            get { return angle; }
        }
    }
}
