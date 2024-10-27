using System.Drawing;
using System.Security.AccessControl;

namespace GraphicEditor
{
    public enum BaseType { Point, Line, Rectangle, Arc };

    public abstract class Base
    {
        protected string name;
        protected BaseType type;
        protected Color fillColor;
        protected int size;
        protected bool visible;
        protected int identityZone;
        protected int multiplicity;

        public abstract void SetMultiplicity(int k);
        public abstract bool HitTest(int x, int y);
        public abstract void Draw(Graphics graphics);
        public abstract void Shift(int deltaX, int deltaY = 0);

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public BaseType Type
        {
            get { return type; }
        }

        public Color FillColor
        {
            get { return fillColor; }
            set { fillColor = value; }
        }

        public int Size
        {
            get { return size; }
            set { SetSize(value); }
        }

        public bool Visible
        {
            get { return visible; }
            set { visible = value; }
        }

        public int IdentityZone
        {
            get { return identityZone; }
            set { identityZone = value; }
        }

        public int Multiplicity
        {
            get { return multiplicity; }
            set { multiplicity = value; }
        }

        public void SetSize(int size)
        {
            identityZone = size * 2;
            this.size = size;
        }

    }
}