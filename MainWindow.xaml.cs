using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
//using Nakov.TurtleGraphics;
using TurtleGraphics;

namespace TurtleLair
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void Me_Click(object sender, RoutedEventArgs e)
        {
            Turtle t = new Turtle(canvas);

        }

        private void Demo1_Click(object sender, RoutedEventArgs e)
        {
            Turtle t = new Turtle(canvas);
            t.TimerDelay = 10;
       
            ColorCircle(t);
        }

        private void Demo2_Click(object sender, RoutedEventArgs e)
        {
            Turtle t = new Turtle(canvas);
            t.TimerDelay = 20;

            DrawPolygons(t);
        }

        private void Demo3_Click(object sender, RoutedEventArgs e)
        {
            Turtle t = new Turtle(canvas);
            t.TimerDelay = 20;

            DrawTree(t);

        }
        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            for (int i = canvas.Children.Count-1; i >= 0; i--)
            {
                UIElement c = canvas.Children[i];
                if (c is Line || c is Polygon)
                    canvas.Children.Remove(c);
            }
        }

        private void ColorCircle(Turtle t)
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

        private void DrawTree(Turtle t)
        {
            int minBranch = 5;
            t.SetAngle(-90);
            t.SetBrush(Brushes.Green);
            buildTree(t, minBranch, 50, 5, 30);
        }

        private void buildTree(Turtle t, int minBranchLength, int branchLength, int shortenBy, int angle)
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

        private void DrawPolygons(Turtle t)
        {
            int sides = 3;
            int x = (int)(canvas.ActualWidth / 2);
            int y = (int)(canvas.ActualHeight / 2);
            List<Brush> brushes = new() { Brushes.Red, Brushes.Green, Brushes.Blue, Brushes.Orange, Brushes.Purple };
            while (sides < 11)
            {
                t.Clear();
                t.PenUp();
                t.GoTo(x, y);
                t.SetAngle(0);
                t.PenDown();

                star(t, 200, sides, 4, brushes, 0);
                
                sides++;

                t.Pause(20);
            }
        }

        private void star(Turtle t, int size, int sides, int minSize, List<Brush> colors, int level = 0)
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
