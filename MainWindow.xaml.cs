﻿using System;
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
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using TurtleGraphics;

namespace TurtleLair
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Turtle t;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void mainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // If we try to do this in the constructor
            // the canvas size won't be correct yet
            t = new Turtle(canvas);
        }

        private void Me_Click(object sender, RoutedEventArgs e)
        {
            t.Home();
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            for (int i = canvas.Children.Count - 1; i >= 0; i--)
            {
                UIElement c = canvas.Children[i];
                if (c is Line)
                    canvas.Children.Remove(c);
            }
        }

        #region Demo Stuff
        private void square(Turtle t, int x, int y, int size)
        {
            t.MoveTo(x, y);
            List<Brush> brushes = new List<Brush>()
            {
                Brushes.Green, Brushes.Blue,Brushes.Red,
                Brushes.Orange
            };

            for (int i = 0; i < 4; i++)
            {
                t.SetBrush(brushes[i]);
                t.Forward(size);
                t.Right(90);
            }
        }

        private void Demo1_Click(object sender, RoutedEventArgs e)
        {
            Turtle t = new Turtle(canvas);
            t.TimerDelay = 10;
       
            Examples.ColorCircle(t);
        }

        private void Demo2_Click(object sender, RoutedEventArgs e)
        {
            Turtle t = new Turtle(canvas);
            t.SetPointerColor(Brushes.Red);
            t.TimerDelay = 20;

            Examples.DrawPolygons(t);
        }

        private void Demo3_Click(object sender, RoutedEventArgs e)
        {
            Turtle t = new Turtle(canvas);
            t.TimerDelay = 20;

            Examples.DrawTree(t);

        }



        #endregion Demo Stuff

    }
}
