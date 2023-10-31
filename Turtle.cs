using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.Xml;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace TurtleGraphics
{
    public class Turtle 
    {
        // Fields
        private Queue<TurtleAction> _actionQueue = new();
        private float       _angle = 0;
        private Brush       _brush = Brushes.Black;
        private Canvas      _canvas;
        private List<Line>  _history = new();
        private bool        _isDown = true;
        private Shape       _pointer;
        private Brush       _pointerFill = Brushes.Blue;
        private System.Timers.Timer _timer;
        private double      _timerDelay = 2;
        private Dispatcher  _uiDispatcher;
        private double      _width = 1;
        private int         _x, _y = 0;

        public Turtle(Canvas canvas)
        {
            _canvas = canvas;
            _uiDispatcher = canvas.Dispatcher;
            //_debug = debug; 

            // Configure timer
            _timer = new System.Timers.Timer(_timerDelay);
            _timer.Elapsed += processActions;
            _timer.AutoReset = true;
            _timer.Enabled = true;

            Home();

            // Add pointer
            _pointer = getTriangle(0, 0, 10, 10, Brushes.Black, _pointerFill);
            _canvas.Children.Add(_pointer);
        }

        ~Turtle()
        {
            // Stop processing events
            _timer.Enabled = false;

            // Remove pointer from canvas
            _canvas.Children.Remove(_pointer);

        }

        #region --- Public Methods ---
        /// <summary>
        /// Erase all items created by this turtle.
        /// </summary>
        public void Clear()
        {
            _actionQueue.Enqueue(new TurtleAction()
            {
                ActionToRun = () => clear(),
                SkipDelay = true
            });
        }

        /// <summary>
        /// Move turtle back by specified number of pixels.
        /// </summary>
        /// <param name="distance"></param>
        public void Back(int distance)
        {
            _actionQueue.Enqueue(new TurtleAction()
            {
                ActionToRun = () => { back(distance); }
            });
        }

        /// <summary>
        /// Move turtle forward by specified number of pixels.
        /// </summary>
        /// <param name="distance"></param>
        public void Forward(int distance)
        {
            _actionQueue.Enqueue(new TurtleAction()
            {
                ActionToRun = () => { forward(distance); }
            });
        }

        /// <summary>
        /// Will move the location drawing a line if pendown. Forward should use this
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void GoTo(int x, int y)
        {
            _actionQueue.Enqueue(new TurtleAction()
            {
                ActionToRun = () => { goTo(x, y); },
                SkipDelay = true
            });
        }

        /// <summary>
        /// Move pen to the center of the canvas and reset angle to 0.
        /// </summary>
        public void Home()
        {
            PenUp();
            GoTo(
                (int)_canvas.ActualWidth / 2,
                (int)_canvas.ActualHeight / 2);
            SetAngle(0);
            PenDown();
        }

        /// <summary>
        /// Turn turtle right by specified number of degrees.
        /// </summary>
        /// <param name="degrees"></param>
        public void Left(float degrees)
        {
            _actionQueue.Enqueue(new TurtleAction()
            {
                ActionToRun = () => left(degrees),
                SkipDelay = true
            });
        }

        /// <summary>
        /// Move to a specific location, this will NOT write regardless of pen state.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void MoveTo(int x, int y)
        {
            _actionQueue.Enqueue(new TurtleAction()
            {
                ActionToRun = () => moveTo(x, y),
                SkipDelay = false
            });
        }

        /// <summary>
        /// Pause for specified number of steps.
        /// </summary>
        public void Pause(int numberOfSteps)
        {
            for (int i = 0; i < numberOfSteps; i++)
            {
                _actionQueue.Enqueue(new TurtleAction()
                {
                    ActionToRun = () => { },
                    SkipDelay = false
                });
            }
        }

        /// <summary>
        /// Place the pen down so it's ready for writing.
        /// </summary>
        public void PenDown()
        {
            _actionQueue.Enqueue(new TurtleAction()
            {
                ActionToRun = () => { _isDown = true; },
                SkipDelay = true
            });
        }

        /// <summary>
        /// Lift pen up so nothing will be written.
        /// </summary>
        public void PenUp()
        {
            _actionQueue.Enqueue(new TurtleAction()
            {
                ActionToRun = () => { _isDown = false; },
                SkipDelay = true
            });
        }

        /// <summary>
        /// Turn turtle right by specified number of degrees.
        /// </summary>
        /// <param name="degrees"></param>
        public void Right(float degrees)
        {
            _actionQueue.Enqueue(new TurtleAction()
            {
                ActionToRun = () => right(degrees),
                SkipDelay = true
            });
        }

        /// <summary>
        /// Set angle of turtle to specified degrees (0-359). 0 = east.
        /// </summary>
        /// <param name="degrees"></param>
        public void SetAngle(float degrees)
        {
            _actionQueue.Enqueue(new TurtleAction()
            {
                ActionToRun = () =>
                { setAngle(degrees); },
                SkipDelay = true
            });
        }

        /// <summary>
        /// Change brush to specified brush (Ie. Brushes.Red)
        /// </summary>
        /// <param name="brush"></param>
        public void SetBrush(Brush brush)
        {
            _actionQueue.Enqueue(new TurtleAction()
            {
                ActionToRun = () => { _brush = brush; },
                SkipDelay = true
            });
        } 

        /// <summary>
        /// Set brush color to aRGB value
        /// </summary>
        /// <param name="alpha"></param>
        /// <param name="red"></param>
        /// <param name="green"></param>
        /// <param name="blue"></param>
        public void SetBrush(byte alpha, byte red, byte green, byte blue)
        {
            _actionQueue.Enqueue(new TurtleAction()
            {
                ActionToRun = () =>
                {
                    _brush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(alpha, red, green, blue));
                }
            });
        }

        /// <summary>
        /// Change the color of the pointer.
        /// </summary>
        /// <param name="brush"></param>
        public void SetPointerColor(Brush brush)
        {
            _actionQueue.Enqueue(new TurtleAction()
            {
                ActionToRun = () => {
                    _pointerFill = brush;
                    _canvas.Children.Remove(_pointer);
                    _pointer = getTriangle(0, 0, 10, 10, Brushes.Black, _pointerFill);
                    setAngle(_angle); // So that it rotates the right way
                    _canvas.Children.Add(_pointer);
                },
                SkipDelay = true
            });
        }

        /// <summary>
        /// Set width of brush to specified number of pixels.
        /// </summary>
        /// <param name="width"></param>
        public void SetWidth(double width)
        {
            _actionQueue.Enqueue(new TurtleAction()
            {
                ActionToRun = () => { _width = width; },
                SkipDelay = true
            });
        }

        #endregion --- Public Methods ---

        #region --- Private Methods ---
        private void back(int distance)
        {
            (int x2, int y2) = getEndCoordinates(X, Y, distance, 180 + _angle);
            goTo(x2, y2);
        }

        private void clear()
        {
            // Remove all lines and clear history
            foreach (Line l in _history)
            {
                _canvas.Children.Remove(l);
            }

            _history = new();
        }

        private void forward(int distance)
        {
            (int x2, int y2) = getEndCoordinates(X, Y, distance, _angle);
            goTo(x2, y2);
        }

        private void goTo(int x, int y)
        {
            if (_isDown)
            {
                Line l = new Line()
                {
                    X1 = _x,
                    Y1 = _y,
                    X2 = x,
                    Y2 = y,
                    Stroke = Brush,
                    StrokeThickness = _width,
                };
                DrawingSurface.Children.Add(l);
                _history.Add(l);
            }

            _x = x;
            _y = y;

            int ptrXCenter = (int)(x - _pointer.ActualWidth / 2);
            int ptrYCenter = (int)(y - _pointer.ActualHeight / 2);

            Canvas.SetLeft(_pointer, ptrXCenter);
            Canvas.SetTop(_pointer, ptrYCenter);
        }
        
        private void left(float degrees)
        {
            setAngle(_angle - degrees);
        }

        private void moveTo(int x, int y)
        {
            bool isDown = _isDown;
            _isDown = false;
            goTo(x, y);
            _isDown = isDown;
        }

        private void processActions(Object source, ElapsedEventArgs e)
        {
            try
            {
                _uiDispatcher.Invoke(() =>
                {
                    // Skip delay not implemented yet
                    // Putting in while loop hung display for some reason
                    bool processNext = true;
                    if (_actionQueue.Count > 0)
                    {
                        TurtleAction a = _actionQueue.Dequeue();
                        a.ActionToRun();
                        processNext = a.SkipDelay;
                    }

                });
            } catch (TaskCanceledException ex)
            {
                // Nothing to see here, task canceled because app is terminating
            }

        }
        
        private void right(float degrees)
        {
            setAngle(_angle + degrees);
        }

        private void setAngle(float degrees)
        {
            if (degrees <= -360 || degrees >= 360)
                degrees = degrees % 360;

            if (degrees < 0)
                degrees = 360 + degrees;

            _angle = degrees;

            RotateTransform rt = new RotateTransform(degrees, _pointer.ActualWidth/2, _pointer.ActualHeight/2);
            _pointer.RenderTransform = rt;
        }

        private static (int, int) getEndCoordinates(int x1, int y1, int distance, float heading)
        {
            double r = Math.PI / 180;
            int x = Convert.ToInt32((distance * Math.Cos(heading * r)));
            int y = Convert.ToInt32((distance * Math.Sin(heading * r)));

            return (x1+x, y1+y);
        }

        private static Shape getTriangle(double x, double y, int width, int height, Brush stroke, Brush fill)
        {
            PointCollection pts = new();
            pts.Add(new System.Windows.Point(0, 0));
            pts.Add(new System.Windows.Point(2, 1));
            pts.Add(new System.Windows.Point(0, 2));
            Polygon p = new()
            {
                Points = pts,
                Stroke = stroke,
                Fill = fill,
                StrokeThickness = 1,
                Width = width,
                Height = height,
                Stretch = Stretch.Fill,

            };
            p.SetValue(Canvas.TopProperty, y);
            p.SetValue(Canvas.LeftProperty, x);

            return p;
        }
        #endregion --- Private Methods ---

        #region --- Properties ---
        /// <summary>
        /// The direction the turtle is facing. Will always be 0-359 degrees, 0 = East.
        /// </summary>
        public float Angle { get { return _angle; } }

        /// <summary>
        /// The brush used to draw lines.
        /// </summary>
        public Brush Brush { get { return _brush; } }
 
        /// <summary>
        /// Milliseconds to delay between instructions.
        /// </summary>
        public double TimerDelay
        {
            get { return _timerDelay; }
            set { _timerDelay = value; _timer.Interval = value; }
        }

        /// <summary>
        /// The canvas we will write to.
        /// </summary>
        public Canvas DrawingSurface
        { 
            get { return _canvas; }
            set { _canvas = value; } 
        }

        /// <summary>
        /// Determine if the pen is down (writing) or not.
        /// </summary>
        public bool IsDown
        {
            get { return _isDown;  }
            set { _isDown = value; }
        }

        /// <summary>
        /// Width (pixels) of the pen.
        /// </summary>
        public double Width { get { return _width; } }

        /// <summary>
        /// X position of turtle.
        /// </summary>
        public int X { get { return _x; } }

        /// <summary>
        /// Y position of turtle.
        /// </summary>
        public int Y { get { return _y; } }
        #endregion

    }
}

/*
Some References:
Info about shape stuff
https://learn.microsoft.com/en-us/dotnet/api/system.windows.shapes.path?view=windowsdesktop-7.0
 
Some animation examples:
https://learn.microsoft.com/en-us/dotnet/desktop/wpf/graphics-multimedia/how-to-animate-an-object-along-a-path-double-animation?view=netframeworkdesktop-4.8

 A potentially better way to handle objects / canvas
https://stackoverflow.com/questions/16273490/best-way-to-add-shapes-to-canvas-using-wpf
 */
