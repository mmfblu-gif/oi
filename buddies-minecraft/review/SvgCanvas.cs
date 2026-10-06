using System;
using System.Globalization;
using System.Text;
using System.Security;
using BuddyMinecraft;

// Review backend for the exact premium drawing commands. Does not emulate WPF or the Windows app.
internal sealed class SvgCanvas : IPremiumCanvas
{
    readonly StringBuilder s = new StringBuilder();
    int depth;
    static string N(double n) { return n.ToString("0.###",CultureInfo.InvariantCulture); }
    static string Esc(string s) { return SecurityElement.Escape(s); }
    static string Paint(string fill,string stroke,double w) { return " fill=\""+(fill ?? "none")+"\" stroke=\""+(stroke ?? "none")+"\" stroke-width=\""+N(w)+"\" stroke-linecap=\"round\" stroke-linejoin=\"round\""; }
    public void Path(string d,string f,string stroke,double w) { s.Append("<path d=\"").Append(Esc(d)).Append('"').Append(Paint(f,stroke,w)).Append("/>"); }
    public void Ellipse(double x,double y,double rx,double ry,string f,string stroke,double w) { s.Append("<ellipse cx=\"").Append(N(x)).Append("\" cy=\"").Append(N(y)).Append("\" rx=\"").Append(N(rx)).Append("\" ry=\"").Append(N(ry)).Append('"').Append(Paint(f,stroke,w)).Append("/>"); }
    public void Round(double x,double y,double w,double h,double r,string f,string stroke,double width) { s.Append("<rect x=\"").Append(N(x)).Append("\" y=\"").Append(N(y)).Append("\" width=\"").Append(N(w)).Append("\" height=\"").Append(N(h)).Append("\" rx=\"").Append(N(r)).Append('"').Append(Paint(f,stroke,width)).Append("/>"); }
    public void Line(double x,double y,double xx,double yy,string color,double w) { s.Append("<path d=\"M").Append(N(x)).Append(',').Append(N(y)).Append(" L").Append(N(xx)).Append(',').Append(N(yy)).Append('"').Append(Paint(null,color,w)).Append("/>"); }
    void Group(string attr) { depth++;s.Append("<g ").Append(attr).Append('>'); }
    public void Translate(double x,double y) { Group("transform=\"translate("+N(x)+" "+N(y)+")\""); }
    public void Rotate(double a,double x,double y) { Group("transform=\"rotate("+N(a)+" "+N(x)+" "+N(y)+")\""); }
    public void Scale(double x,double y,double cx,double cy) { Group("transform=\"translate("+N(cx)+" "+N(cy)+") scale("+N(x)+" "+N(y)+") translate("+N(-cx)+" "+N(-cy)+")\""); }
    public void Opacity(double v) { Group("opacity=\""+N(v)+"\""); }
    public void Pop() { if(--depth<0) throw new InvalidOperationException("SVG stack underflow");s.Append("</g>"); }
    public string Body { get { if(depth!=0) throw new InvalidOperationException("SVG stack not balanced");return s.ToString(); } }
    public string Document { get { return "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 260 260\">"+Body+"</svg>"; } }
}
