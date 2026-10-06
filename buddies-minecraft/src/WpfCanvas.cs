using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace BuddyMinecraft
{
    // The portable choreography emits the same drawing commands to WPF and the review exporter.
    public sealed class WpfPremiumCanvas : IPremiumCanvas
    {
        readonly DrawingContext dc;
        static readonly Dictionary<string, Brush> brushes = new Dictionary<string, Brush>();
        static readonly Dictionary<string, Pen> pens = new Dictionary<string, Pen>();
        static readonly Dictionary<string, Geometry> paths = new Dictionary<string, Geometry>();
        public WpfPremiumCanvas(DrawingContext context) { dc = context; }
        static Brush Brush(string color)
        {
            if (color == null) return null;
            Brush brush;
            if (!brushes.TryGetValue(color, out brush))
            {
                brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));
                brush.Freeze(); brushes.Add(color, brush);
            }
            return brush;
        }
        static Pen Pen(string color, double width)
        {
            if (color == null) return null;
            string key = color + ":" + width.ToString(CultureInfo.InvariantCulture);
            Pen pen;
            if (pens.TryGetValue(key, out pen)) return pen;
            pen = new Pen(Brush(color), width);
            pen.StartLineCap = pen.EndLineCap = PenLineCap.Round; pen.LineJoin = PenLineJoin.Round;
            pen.Freeze(); pens.Add(key, pen);
            return pen;
        }
        public void Path(string data, string fill, string stroke, double width)
        {
            Geometry shape;
            if (!paths.TryGetValue(data, out shape)) { shape = Geometry.Parse(data); shape.Freeze(); paths.Add(data, shape); }
            dc.DrawGeometry(Brush(fill), Pen(stroke, width), shape);
        }
        public void Ellipse(double x, double y, double rx, double ry, string fill, string stroke, double width) { dc.DrawEllipse(Brush(fill), Pen(stroke, width), new Point(x,y), rx,ry); }
        public void Round(double x, double y, double w, double h, double radius, string fill, string stroke, double width) { dc.DrawRoundedRectangle(Brush(fill), Pen(stroke,width), new Rect(x,y,w,h), radius,radius); }
        public void Line(double x1, double y1, double x2, double y2, string color, double width) { dc.DrawLine(Pen(color,width),new Point(x1,y1),new Point(x2,y2)); }
        public void Translate(double x, double y) { dc.PushTransform(new TranslateTransform(x,y)); }
        public void Rotate(double angle, double x, double y) { dc.PushTransform(new RotateTransform(angle,x,y)); }
        public void Scale(double x, double y, double cx, double cy) { dc.PushTransform(new ScaleTransform(x,y,cx,cy)); }
        public void Opacity(double value) { dc.PushOpacity(value); }
        public void Pop() { dc.Pop(); }
    }
}
