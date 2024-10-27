using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace GraphicEditor
{
    public partial class Form1 : Form
    {
        bool Edit = false;
        bool MousePressed = false;
        int pointCounter = 0;
        int angle = 0;
        int size = 5;
        Plex plex = new Plex();
        CustomPoint a;
        CustomPoint b;
        Color color = Color.Black;
        BaseType baseType = BaseType.Line;
        public Form1()
        {
            InitializeComponent();
        }

        private void прямаяToolStripMenuItem_Click(object sender, EventArgs e)
        {
            baseType = BaseType.Line;
        }

        private void прямоугольникToolStripMenuItem_Click(object sender, EventArgs e)
        {
            baseType = BaseType.Rectangle;
        }


        

        private void AddLine(CustomPoint left, CustomPoint right)
        {
            CustomLine customLine = new CustomLine(left,right,size);
            customLine.FillColor = color;
            plex.Add(customLine);
        }

        private void Update(Graphics graphics)
        {
            graphics.Clear(Color.White);
            plex.Draw(graphics);
        }

        private void AddRectangle(CustomPoint left, CustomPoint right)
        {
            CustomRectangle customRectangle = new CustomRectangle(left,right,size,angle);
            customRectangle.FillColor = color;
            plex.Add(customRectangle);
        }

        private void GraphicPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                MousePressed = false;
                if (!Edit)
                {
                    a = new CustomPoint($"A{pointCounter++}", e.X, e.Y, size);
                    a.FillColor = color;
                }
                else
                {
                    CustomPoint c;
                    c = plex.FindPoint(new CustomPoint($"A", e.X, e.Y, size));
                    if (c != null)
                    {
                        int localX = Cursor.Position.X - c.X;
                        int localY = Cursor.Position.Y - c.Y;
                        EditPoint(c, localX, localY);
                    }
                    else
                    {
                        int localX = Cursor.Position.X;
                        int localY = Cursor.Position.Y;
                        EditPlex(localX, localY);

                    }
                }
            }
        }
        void EditPoint(CustomPoint c, int localX, int localY)
        {
            if (!MousePressed)
            {
                c.Shift(
                Cursor.Position.X - c.X - localX,
                Cursor.Position.Y - c.Y - localY);
                Graphics graphics = GraphicPanel.CreateGraphics();
                graphics.Clear(Color.White);
                plex.Draw(graphics);
                Task.Delay(10).ContinueWith(t => EditPoint(c, localX, localY)); ;
            }
        }

        void EditPlex(int localX, int localY)
        {
            if (!MousePressed)
            {
                plex.Shift(
                Cursor.Position.X  - localX,
                Cursor.Position.Y  - localY);
                Graphics graphics = GraphicPanel.CreateGraphics();
                graphics.Clear(Color.White);
                plex.Draw(graphics);
                localX = Cursor.Position.X;
                localY = Cursor.Position.Y;
                Task.Delay(10).ContinueWith(t => EditPlex(localX, localY)); ;
            }
        }



        private void GraphicPanel_MouseUp(object sender, MouseEventArgs e)
        {

            if (e.Button == MouseButtons.Left)
            {
                MousePressed = true;
                if (!Edit)
                {
                    Graphics graphics = GraphicPanel.CreateGraphics();
                    b = new CustomPoint($"A{pointCounter++}", e.X, e.Y, size);
                    if (a != null)
                        switch (baseType)
                        {
                            case BaseType.Line:
                                AddLine(a, b);
                                Update(graphics);
                                break;
                            case BaseType.Rectangle:
                                AddRectangle(a, b);
                                Update(graphics);
                                break;
                        }
                }

            }
        }
        private void clearButton_Click(object sender, EventArgs e)
        {
            plex = new Plex();
            pointCounter = 0;
            Graphics graphics = GraphicPanel.CreateGraphics();
            graphics.Clear(Color.White);
        }

        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string filePath = saveFileDialog1.FileName;
                string textPlex = plex.SaveToString();
                File.WriteAllText(filePath, textPlex);
            }
        }

        private void загрузитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog1.FileName;
                plex = new Plex(File.ReadAllText(filePath));
                Graphics graphics = GraphicPanel.CreateGraphics();
                Update(graphics);
                a = null;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Edit = !(Edit);
            if (Edit)
            {
                button1.Text = "Отмена";
            }
            else
            {
                button1.Text = "Редактировать";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            plex.Fill(color);
            Graphics graphics = GraphicPanel.CreateGraphics();
            graphics.Clear(Color.White);
            plex.Draw(graphics);
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            size = trackBar1.Value;
            label2.Text = size.ToString();
        }

        private void цветToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                color = colorDialog1.Color;
            }
        }
    }
}
