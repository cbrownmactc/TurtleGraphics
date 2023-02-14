using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using TurtleGraphics;

namespace TurtleLair
{
    class Examples
    {
        public static void ColorCircle(Turtle t)
        {
            List<Brush> brushes = new List<Brush>()
            {
                Brushes.Orange, Brushes.Red, Brushes.Pink, Brushes.Yellow, Brushes.Blue,
                Brushes.Green
            };

            for (int x = 0; x < 300; x++)
            {
                t.SetBrush(brushes[x % 6]);
                t.SetWidth(1);
                t.Forward(x / 5 + 1);
                t.Left(20);
            }
        }

        public static void DrawTree(Turtle t)
        {
            int minBranch = 5;
            t.SetAngle(-90);
            t.SetBrush(Brushes.Green);
            buildTree(t, minBranch, 50, 5, 30);
        }

        private static void buildTree(Turtle t, int minBranchLength, int branchLength, int shortenBy, int angle)
        {
            if (branchLength > minBranchLength)
            {
                t.Forward(branchLength);
                int newLength = branchLength - shortenBy;

                t.Left(angle);
                buildTree(t, minBranchLength, newLength, shortenBy, angle);

                t.Right(angle * 2);
                buildTree(t, minBranchLength, newLength, shortenBy, angle);

                t.Left(angle);
                t.SetBrush(Brushes.Red);
                t.Back(branchLength);
                t.SetBrush(Brushes.Green);

            }
        }

        public static void DrawPolygons(Turtle t)
        {
            int sides = 3;
            int x = (int)(t.DrawingSurface.ActualWidth / 2);
            int y = (int)(t.DrawingSurface.ActualHeight / 2);
            List<Brush> brushes = new() { Brushes.Red, Brushes.Green, Brushes.Blue, Brushes.Orange, Brushes.Purple };
            while (sides < 11)
            {
                t.Clear();
                t.Home();

                star(t, 200, sides, 4, brushes, 0);

                sides++;

                t.Pause(20);
            }
        }

        private static void star(Turtle t, int size, int sides, int minSize, List<Brush> colors, int level = 0)
        {
            if (size < minSize)
                return;

            float angle = 360 - (360 / sides);

            for (int i = 0; i < sides; i++)
            {
                t.SetBrush(colors[level]);
                t.Forward(size);
                star(t, Convert.ToInt32(size / 3), sides, minSize, colors, level + 1);
                t.Left(angle);
            }
        }


    }
}
