﻿using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace calk
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            updateParam();
        }

        public List<Point> points = new List<Point>();
        public List<decimal> lens = new List<decimal>();
        public bool end;
        public Brush PolyC;
        public Brush DotC;
        public Brush LineC;
        public Brush TextC;
        public int TextS;
        public double LineH;

        public void updateParam()
        {
            PolyC = new SolidColorBrush(Properties.Settings.Default.PolyC);
            DotC = new SolidColorBrush(Properties.Settings.Default.DotC);
            LineC = new SolidColorBrush(Properties.Settings.Default.LineC);
            TextC = new SolidColorBrush(Properties.Settings.Default.FontC);
            TextS = Properties.Settings.Default.FontS;
            LineH = Properties.Settings.Default.LineH;
            draw();
            Properties.Settings.Default.Save();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!end)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    try
                    {
                        Point p = e.GetPosition(this);
                        if (points.Count >= 1)
                        {
                            InputBox.InputBox inputBox = new InputBox.InputBox();
                            lens.Add(decimal.Parse(inputBox.getString().Replace(".", ",")));
                        }
                        points.Add(new Point(p.X - 10, p.Y - 30));
                    }
                    catch {}
                }
                draw();
            }
            else
            {
                points = new List<Point>();
                lens = new List<decimal>();
                gr.Children.Clear();
                end = false;
            }
        }

        private decimal calk()
        {
            decimal height = 0;
            try
            {
                height = decimal.Parse(h.Text);
            }
            catch
            {
                InputBox.InputBox inputBox = new InputBox.InputBox("Введите высоту");
                h.Text = inputBox.getString().Replace(".", ",");
                height = decimal.Parse(h.Text);
            }
            decimal rezult = 0;
            foreach (decimal d in lens)
            {
                rezult += d * height;
            }
            foreach (Grid g in otv.Items)
            {
                rezult -= decimal.Parse(((TextBox)g.Children[0]).Text.Replace(".",",")) * decimal.Parse(((TextBox)g.Children[1]).Text.Replace(".", ","));
            }
            return rezult;
        }

        private void calk(object sender, RoutedEventArgs e)
        {
            if (points.Count != lens.Count)
            {
                InputBox.InputBox inputBox = new InputBox.InputBox();
                lens.Add(decimal.Parse(inputBox.getString().Replace(".", ",")));
            }
            rez.Text = calk().ToString();
            draw();
        }

        private void draw()
        {
            gr.Children.Clear();
            gr.Children.Add(new Polygon() { Points = new PointCollection(points), Fill = PolyC });
            foreach (Point p in points)
            {
                gr.Children.Add(
                    new Ellipse()
                    {
                        Height = 4,
                        Width = 4,
                        Margin = new Thickness(p.X - 2, p.Y - 2, 0, 0),
                        Fill = DotC,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Top
                    }
                    );
                if (!end)
                {
                    gr.Children.Add(new Line() { X1 = 0, Y1 = p.Y, X2 = 10000, Y2 = p.Y, Stroke = LineC, StrokeThickness = LineH });
                    gr.Children.Add(new Line() { X1 = p.X, Y1 = 0, X2 = p.X, Y2 = 10000, Stroke = LineC, StrokeThickness = LineH });
                }
            }
            for (int i = 0; i < points.Count; i++)
            {
                try
                {
                    Point pos = FindTxtPos(points[i], points[i == points.Count - 1 ? 0 : i + 1]);
                    gr.Children.Add(new TextBlock { Text = lens[i].ToString(), Margin = new Thickness(pos.X, pos.Y, 0, 0), FontSize = TextS, Foreground = TextC });
                }catch{}
            }
            gr.Children.Add(new TextBlock { Text = Area(points.ToArray()).ToString() });
        }

        double Area(Point[] polygon)
        {
            double area = 0;
            for (int i = 0; i < polygon.Length; i++)
            {
                int j = (i + 1) % polygon.Length;
                Point a = polygon[i], b = polygon[j];

                area += a.X * b.Y - a.Y * b.X;
            }
            return System.Math.Abs(area) / 2;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void PolyColor(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.PolyC = ((SolidColorBrush)(sender as MenuItem).Background).Color;
            updateParam();
        }

        private void DotColor(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.DotC = ((SolidColorBrush)(sender as MenuItem).Background).Color;
            updateParam();
        }
        private void TxtColor(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.FontC = ((SolidColorBrush)(sender as MenuItem).Background).Color;
            updateParam();
        }

        private void LineColor(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.LineC = ((SolidColorBrush)(sender as MenuItem).Background).Color;
            updateParam();
        }

        private void Button_Plus(object sender, RoutedEventArgs e)
        {
            Grid g = new Grid { Background = Brushes.Black, Width = 105, Height = 35 };
            g.Children.Add(new TextBox { HorizontalAlignment = HorizontalAlignment.Left, Width = 50, Height = 25, Text = "0" });
            g.Children.Add(new TextBox { HorizontalAlignment = HorizontalAlignment.Right, Width = 50, Height = 25, Text = "0" });
            otv.Items.Add(g);
        }

        private void New(object sender, RoutedEventArgs e)
        {
            points.Clear();
            lens.Clear();
            gr.Children.Clear();
            otv.Items.Clear();
        }

        public Point FindTxtPos(Point p1, Point p2) => new Point((p1.X + p2.X) / 2, (p1.Y + p2.Y) / 2);

        private void FontS(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.FontS = int.Parse((sender as MenuItem).Header.ToString());
            updateParam();
        }
        private void LineSize(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.LineH = double.Parse((sender as MenuItem).Header.ToString());
            updateParam();
        }
    }
}
