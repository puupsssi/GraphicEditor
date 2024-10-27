using System;
using System.Collections.Generic;
using System.Drawing;

namespace GraphicEditor
{
    internal class Plex
    {
        private Base root;

        public Plex()
        {
            root = null;
        }

        public Plex(Plex tmp)
        {
            root = new CustomLine((CustomLine)tmp.root);
            Stack<CustomPoint> stackPoint = new Stack<CustomPoint>();
            Stack<CustomLine> stackLine = new Stack<CustomLine>();
            stackLine.Push(null);
            Base current = root;
            while (current != null)
            {
                if (current.Type == BaseType.Point)
                {
                    stackPoint.Push((CustomPoint)current);
                    current = stackLine.Pop();
                    current.SetMultiplicity(1);
                }
                else
                {
                    if (current.Multiplicity == 1)
                    {
                        stackLine.Push((CustomLine)current);
                        current = ((CustomLine)current).Left;
                    }
                    else if (current.Multiplicity == 2)
                    {
                        stackLine.Push((CustomLine)current);
                        current = ((CustomLine)current).Right;
                    }
                    else
                    {
                        CustomPoint right = stackPoint.Pop();
                        CustomPoint left = stackPoint.Peek();
                        switch (current.Type)
                        {
                            case BaseType.Line:
                                CustomLine customLine = new CustomLine(left, right, current.Size);
                                customLine.FillColor = current.FillColor;
                                Add(customLine);
                                break;
                            case BaseType.Rectangle:
                                CustomRectangle customRectangle = new CustomRectangle(left, right, current.Size, ((CustomRectangle)current).Angle);
                                customRectangle.FillColor = current.FillColor;
                                Add(customRectangle);
                                break;
                        }
                        current.SetMultiplicity(1);
                        current = stackLine.Pop();
                        if (current != null)
                        {
                            current.SetMultiplicity(1);
                        }
                    }
                }
            }
        }

        public Plex(string inputString)
        {
            CustomPoint[] multiplePoints = new CustomPoint[100];
            int multiplePointsCount = 0;
            Stack<Base> stack = new Stack<Base>();
            string[] lines = inputString.Split('\n');

            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                string[] parts = line.Split('|');

                switch (parts[0])
                {
                    case "Point":
                        CustomPoint customPoint = new CustomPoint(parts[1], int.Parse(parts[2]), int.Parse(parts[3]), int.Parse(parts[5]));
                        customPoint.Multiplicity = int.Parse(parts[6]);
                        Color fillColor = Color.FromName(parts[4]);
                        customPoint.FillColor = fillColor;
                        if (customPoint.Multiplicity > 1)
                        {
                            int index = -1;
                            int iterator = 0;
                            while (iterator < multiplePointsCount && index != -1)
                            {
                                if (multiplePoints[index].Name == customPoint.Name)
                                {
                                    index = iterator;
                                }
                                iterator++;
                            }
                            if (index == -1)
                            {
                                stack.Push(customPoint);
                                multiplePoints[multiplePointsCount++] = customPoint;
                            }
                            else
                            {
                                stack.Push(multiplePoints[index]);
                            }
                        }
                        else
                        {
                            stack.Push(customPoint);
                        }
                        break;
                    case "Line":
                        Base right = stack.Pop();
                        Base left = stack.Pop();
                        CustomLine customLine = new CustomLine(parts);
                        customLine.Left = left;
                        customLine.Right = right;
                        stack.Push(customLine);
                        break;
                    case "Rectangle":
                        right = stack.Pop();
                        left = stack.Pop();
                        CustomRectangle customRectangle = new CustomRectangle(left, right, int.Parse(parts[3]), int.Parse(parts[4]));
                        Color rectangleColor = Color.FromName(parts[2]);
                        customRectangle.FillColor = rectangleColor;
                        stack.Push(customRectangle);
                        break;
                }
            }
            root = stack.Pop();
        }

        public CustomPoint FindPoint(int x = 0, int y = 0)
        {
            Stack<CustomLine> stack = new Stack<CustomLine>();
            stack.Push(null);
            CustomPoint result = null;
            Base current = root;
            while (current != null)
            {
                if (current.Type == BaseType.Point)
                {
                    if (((CustomPoint)current).HitTest(x, y))
                    {
                        result = (CustomPoint)current;
                    }
                    current = stack.Pop();
                    current.SetMultiplicity(1);
                }
                else
                {
                    if (current.Multiplicity == 1)
                    {
                        stack.Push((CustomLine)current);
                        current = ((CustomLine)current).Left;
                    }
                    else if (current.Multiplicity == 2)
                    {
                        stack.Push((CustomLine)current);
                        current = ((CustomLine)current).Right;
                    }
                    else
                    {
                        current.SetMultiplicity(1);
                        current = stack.Pop();
                        if (current != null)
                        {
                            current.SetMultiplicity(1);
                        }
                    }
                }
            }
            return result;
        }

        public CustomPoint FindPoint(CustomPoint point)
        {
            return FindPoint(point.X, point.Y);
        }

        public CustomLine FindLine(int x = 0, int y = 0)
        {
            Stack<CustomLine> stack = new Stack<CustomLine>();
            stack.Push(null);
            CustomLine result = null;
            Base current = root;
            while (current != null)
            {
                if (current.Type == BaseType.Point)
                {
                    if (((CustomPoint)current).HitTest(x, y))
                    {
                        result = stack.Peek();
                    }
                    current = stack.Pop();
                    current.SetMultiplicity(1);
                }
                else
                {
                    if (current.Multiplicity == 1)
                    {
                        stack.Push((CustomLine)current);
                        current = ((CustomLine)current).Left;
                    }
                    else if (current.Multiplicity == 2)
                    {
                        stack.Push((CustomLine)current);
                        current = ((CustomLine)current).Right;
                    }
                    else
                    {
                        current.SetMultiplicity(1);
                        current = stack.Pop();
                        if (current != null)
                        {
                            current.SetMultiplicity(1);
                        }
                    }
                }
            }
            return result;
        }


        public CustomLine FindLine(CustomPoint point)
        {
            return FindLine(point.X, point.Y);
        }

        public CustomPoint GetPoint(Base line)
        {
            Base current = line;
            while (current.Type != BaseType.Point)
            {
                current = ((CustomLine)current).Left;
            }
            return (CustomPoint)current;
        }

        public void Add(CustomLine tmp)
        {
            if (root == null)
            {
                root = tmp;
                return;
            }
            CustomLine leftLine = null;
            CustomLine rightLine = null;
            CustomPoint leftPoint = FindPoint((CustomPoint)tmp.Left);
            CustomPoint rightPoint = FindPoint((CustomPoint)tmp.Right);
            if (leftPoint != null) leftLine = FindLine(leftPoint);
            if (rightPoint != null) rightLine = FindLine(rightPoint);
            if (leftPoint != null)
            {
                if (leftLine.Left == leftPoint)
                {
                    string[] name = GetName(tmp.Name);
                    name[0] = leftLine.Left.Name;
                    tmp.Left = leftLine.Left;
                    leftLine.Left = tmp;
                    tmp.Name = name[0] + " " + name[1];
                }
                else
                {
                    string[] name = GetName(tmp.Name);
                    name[0] = leftLine.Right.Name;
                    tmp.Left = leftLine.Right;
                    leftLine.Right = tmp;
                    tmp.Name = name[0] + " " + name[1];
                }
                if (rightLine != null)
                {
                    string[] name = GetName(tmp.Name);
                    name[1] = rightPoint.Name;
                    tmp.Right = rightPoint;
                    rightPoint.SetMultiplicity(1);
                    tmp.Name = name[0] + " " + name[1];
                }
            }
            else if (rightLine != null)
            {
                tmp = tmp.Invert();
                if (rightLine.Right == rightPoint)
                {
                    string[] name = GetName(tmp.Name);
                    name[1] = rightLine.Right.Name;
                    tmp.Right = rightLine.Right;
                    rightLine.Right = tmp;
                    tmp.Name = name[0] + " " + name[1];
                }
                else
                {
                    string[] name = GetName(tmp.Name);
                    name[0] = rightLine.Left.Name;
                    tmp.Left = rightLine.Left;
                    rightLine.Left = tmp;
                    tmp.Name = name[0] + " " + name[1];
                }
            }
        }

        public void Draw(Graphics graphics)
        {
            Stack<CustomPoint> stackPoint = new Stack<CustomPoint>();
            Stack<CustomLine> stackLine = new Stack<CustomLine>();
            stackLine.Push(null);
            Base current = root;
            while (current != null)
            {
                if (current.Type == BaseType.Point)
                {
                    stackPoint.Push((CustomPoint)current);
                    current = stackLine.Pop();
                    current.SetMultiplicity(1);
                }
                else
                {
                    if (current.Multiplicity == 1)
                    {
                        stackLine.Push((CustomLine)current);
                        current = ((CustomLine)current).Left;
                    }
                    else if (current.Multiplicity == 2)
                    {
                        stackLine.Push((CustomLine)current);
                        current = ((CustomLine)current).Right;
                    }
                    else
                    {
                        CustomPoint right = stackPoint.Pop();
                        CustomPoint left = stackPoint.Peek();
                        switch (current.Type)
                        {
                            case BaseType.Line:
                                CustomLine customLine = new CustomLine(left, right, current.Size);
                                customLine.FillColor = current.FillColor;
                                customLine.Draw(graphics);
                                break;
                            case BaseType.Rectangle:
                                CustomRectangle customRectangle = new CustomRectangle(left, right, current.Size, ((CustomRectangle)current).Angle);
                                customRectangle.FillColor = current.FillColor;
                                customRectangle.Draw(graphics);
                                break;
                        }
                        current.SetMultiplicity(1);
                        current = stackLine.Pop();
                        if (current != null)
                        {
                            current.SetMultiplicity(1);
                        }
                    }
                }
            }
        }

        public string SaveToString()
        {
            string result = "";
            Stack<CustomLine> stackLine = new Stack<CustomLine>();
            stackLine.Push(null);
            Base current = root;
            while (current != null)
            {
                if (current.Type == BaseType.Point)
                {
                    result += $"{((CustomPoint)current).PointToString()}\n";
                    current = stackLine.Pop();
                    current.SetMultiplicity(1);
                }
                else
                {
                    if (current.Multiplicity == 1)
                    {
                        stackLine.Push((CustomLine)current);
                        current = ((CustomLine)current).Left;
                    }
                    else if (current.Multiplicity == 2)
                    {
                        stackLine.Push((CustomLine)current);
                        current = ((CustomLine)current).Right;
                    }
                    else
                    {
                        switch (current.Type)
                        {
                            case BaseType.Line:
                                result += $"{((CustomLine)current).LineToString()}\n";
                                break;
                            case BaseType.Rectangle:
                                result += $"{((CustomRectangle)current).RectangleToString()}\n";
                                break;
                        }
                        current.SetMultiplicity(1);
                        current = stackLine.Pop();
                        if (current != null)
                        {
                            current.SetMultiplicity(1);
                        }
                    }
                }
            }
            return result;
        }

        private string[] GetName(string name)
        {
            string[] result = name.Split(' ');
            return result;
        }

        public void Shift(int deltaX, int deltaY = 0)
        {
            Base[] multipleShape = new Base[100];
            Stack<CustomLine> stack = new Stack<CustomLine>();
            stack.Push(null);
            Base current = root;
            int j = 0;
            while (current != null)
            {
                if (current.Type == BaseType.Point)
                {
                    bool a = true;
                    for (int i = 0; i < j; i++)
                    {
                        if (multipleShape[i] == current)
                        {
                            a = false;
                        }
                    }

                    if (a)
                    {
                        multipleShape[j] = current;
                        j++;
                        ((CustomPoint)current).Shift(deltaX, deltaY);
                    }

                    current = stack.Pop();
                    current.SetMultiplicity(1);
                }
                else
                {
                    if (current.Multiplicity == 1)
                    {
                        stack.Push((CustomLine)current);
                        current = ((CustomLine)current).Left;
                    }
                    else if (current.Multiplicity == 2)
                    {
                        stack.Push((CustomLine)current);
                        current = ((CustomLine)current).Right;
                    }
                    else
                    {
                        current.SetMultiplicity(1);
                        current = stack.Pop();
                        if (current != null)
                        {
                            current.SetMultiplicity(1);
                        }
                    }
                }
            }
        }

        public void Fill(Color FillColor)
        {
            Base[] multipleShape = new Base[100];
            Stack<CustomLine> stack = new Stack<CustomLine>();
            stack.Push(null);
            Base current = root;

            while (current != null)
            {
                if (current.Type == BaseType.Line)
                {

                    current.FillColor = FillColor;
                }
                if (current.Type == BaseType.Rectangle)
                {

                    current.FillColor = FillColor;
                }

                if (current.Type == BaseType.Point)
                {

                    current = stack.Pop();
                    current.SetMultiplicity(1);
                }
                else
                {
                    if (current.Multiplicity == 1)
                    {
                        stack.Push((CustomLine)current);
                        current = ((CustomLine)current).Left;
                    }
                    else if (current.Multiplicity == 2)
                    {
                        stack.Push((CustomLine)current);
                        current = ((CustomLine)current).Right;
                    }
                    else
                    {
                        current.SetMultiplicity(1);
                        current = stack.Pop();
                        if (current != null)
                        {
                            current.SetMultiplicity(1);
                        }
                    }
                }
            }
        }

    }
}