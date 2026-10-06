using System;

namespace BuddyMinecraft
{
    public enum MinecraftPose { Idle, Crafting, Impressed }
    public enum MineralVisual { Diamond, Emerald, AncientDebris }

    // New performances are separate from existing Goal/Victory/Defeat sprites.
    // Canvas: 260 square. Crafting loops every six seconds; Impressed plays once (4.2 s).
    public sealed class MinecraftBuddyArt
    {
        readonly IPremiumCanvas c;
        readonly PetKind kind;
        readonly MinecraftPose pose;
        readonly double t, motion, strength;
        readonly MineralVisual mineral;
        const string Ink = "#28304A";
        static readonly string[] Hands = { "#AB87ED", "#94E6C4", "#FFE099", "#514060", "#F39869", "#C9E9FC", "#CFA0E3" };
        MinecraftBuddyArt(IPremiumCanvas canvas, PetKind buddy, MinecraftPose state, double seconds, double movement, double reveal, MineralVisual visual)
        {
            c=canvas; kind=buddy; pose=state; t=seconds; motion=movement; mineral=visual;
            double phase=Clamp(t/4.2);
            strength=state==MinecraftPose.Impressed ? Ease(phase/.16)*Ease((1-phase)/.22) : state==MinecraftPose.Crafting ? reveal : 0;
        }
        public static void Draw(IPremiumCanvas canvas, PetKind kind, MinecraftPose pose, double seconds, double motion, MineralVisual mineral, double reveal=1)
        {
            if(canvas==null) throw new ArgumentNullException("canvas");
            if(!Enum.IsDefined(typeof(PetKind),kind) || !Enum.IsDefined(typeof(MinecraftPose),pose) || !Enum.IsDefined(typeof(MineralVisual),mineral)) throw new ArgumentOutOfRangeException("kind");
            seconds=Finite(seconds) ? Math.Max(0,seconds) : 0;
            motion=Finite(motion) ? Math.Max(0,Math.Min(1,motion)) : 1;
            reveal=Finite(reveal) ? Clamp(reveal) : 1;
            new MinecraftBuddyArt(canvas,kind,pose,seconds,motion,reveal,mineral).Render();
        }
        static bool Finite(double n) { return !Double.IsNaN(n)&&!Double.IsInfinity(n); }
        static double Clamp(double n) { return Math.Max(0,Math.Min(1,n)); }
        static double Ease(double n) { n=Clamp(n);return n*n*(3-2*n); }
        void P(string d,string f,string s=null,double w=2.5) { c.Path(d,f,s,w); }
        void E(double x,double y,double rx,double ry,string f,string s=null,double w=2.5) { c.Ellipse(x,y,rx,ry,f,s,w); }
        void R(double x,double y,double w,double h,double r,string f,string s=null) { c.Round(x,y,w,h,r,f,s,2.5); }
        void L(double x,double y,double xx,double yy,string color,double width=2) { c.Line(x,y,xx,yy,color,width); }
        void Star(double x,double y,double radius,string color)
        {
            c.Translate(x,y);c.Scale(radius/10,radius/10,0,0);
            P("M0,-10 L3,-3 L10,0 L3,3 L0,10 L-3,3 L-10,0 L-3,-3 Z",color);c.Pop();c.Pop();
        }
        void Render()
        {
            double lift=pose==MinecraftPose.Impressed ? -Math.Sin(Math.Min(1,t/4.2)*Math.PI)*8*strength*motion : 0;
            double tilt=pose==MinecraftPose.Crafting ? Math.Sin(t*Math.PI/3)*1.8*strength*motion : strength*2*motion;
            c.Translate(0,lift);c.Rotate(tilt,130,205);
            if(BuddyCatalog.HasPremiumArt(kind))
                PremiumBuddyArt.DrawMinecraft(c,kind,pose,t%6,motion,strength);
            else
            {
                c.Opacity(.14);E(130,229,45,6,"#141B32");c.Pop();
                c.Translate(0,4);c.Scale(.86,.86,130,216);
                if(kind==PetKind.Astro) Astro(); else if(kind==PetKind.Mochi) Mochi(); else if(kind==PetKind.Bolt) Bolt(); else Drako();
                c.Pop();c.Pop();
            }
            c.Translate(0,4);c.Scale(.86,.86,130,216);
            if(strength>0)
            {
                if(pose==MinecraftPose.Crafting) Blueprint();
                else if(pose==MinecraftPose.Impressed) Wonder();
            }
            c.Pop();c.Pop();c.Pop();c.Pop();
        }
        void Face(double y,bool robot=false)
        {
            DrawFace(c,130,y,21,robot ? "#A6FFE0" : Ink,pose,t,strength,robot,motion);
        }
        public static void DrawFace(IPremiumCanvas c,double x,double y,double spacing,string ink,MinecraftPose state,double time,double power,bool robot,double movement=1)
        {
            bool impressed=state==MinecraftPose.Impressed && power>.08;
            bool crafting=state==MinecraftPose.Crafting && power>.08;
            double gaze=crafting ? Math.Sin(time*Math.PI)*3*movement : 0;
            bool blink=!impressed && time%6>4.82 && time%6<5.03;
            foreach(int side in new[] {-1,1})
            {
                double eye=x+side*spacing;
                if(blink) c.Line(eye-5,y+2,eye+5,y+2,ink,3);
                else if(impressed)
                {
                    c.Ellipse(eye,y-1,8+power*2,10+power*2,robot ? "#A6FFE0" : "#FFFAEF",ink,1.5);
                    c.Ellipse(eye+side*1.5,y,4,6,robot ? "#1D2A3C" : ink,null,0);
                    c.Ellipse(eye-2,y-4,2,2,"#FFFFFF",null,0);
                    c.Line(eye-6,y-19,eye+5,y-20,ink,2);
                }
                else
                {
                    c.Ellipse(eye+gaze,y+(crafting?3:0),4,6,ink,null,0);
                    c.Ellipse(eye+gaze-1,y+(crafting?1:-2),1.5,1.5,"#FFFFFF",null,0);
                    if(crafting) c.Line(eye-7,y-11,eye+5,y-9,ink,2);
                }
            }
            if(impressed) c.Ellipse(x,y+19,5,7,ink,null,0);
            else
            {
                c.Translate(x,y+15);
                c.Path(crafting ? "M-5,3 Q0,7 5,3" : "M-6,0 Q0,8 6,0",null,ink,2.3);c.Pop();
            }
        }
        void Astro()
        {
            c.Rotate(Math.Sin(t*Math.PI/3)*3*motion,130,80);
            P("M107,80 C109,60 97,53 97,44 M156,80 C155,60 170,55 170,42",null,"#513B7A",4);
            E(97,42,8,8,"#E6D4FF","#513B7A");Star(170,40,10,"#FCE8A0");c.Pop();
            E(71,166,12,21,"#AB87ED","#513B7A");E(190,166,12,21,"#AB87ED","#513B7A");
            E(105,207,21,11,"#8F69D5","#513B7A");E(157,207,21,11,"#8F69D5","#513B7A");
            P("M130,70 C166,70 190,97 190,136 C190,158 200,181 181,199 C165,213 97,217 79,199 C62,183 73,159 72,140 C71,100 91,70 130,70 Z","#B799F0","#513B7A",3);
            P("M83,139 C83,108 94,88 115,84",null,"#E7D6FF",7);
            E(137,181,30,22,"#CDB5F9");
            P("M125,90 C121,99 127,105 138,102 C132,111 119,108 117,100 C117,96 120,92 125,90 Z","#F7EDFF");
            Face(137);E(91,153,10,5,"#E79BDD");E(171,153,10,5,"#E79BDD");
        }
        void Mochi()
        {
            c.Rotate(Math.Sin(t*Math.PI/3)*6*motion,169,193);
            P("M169,198 C207,202 216,178 207,165 C202,157 190,163 194,172 C199,181 188,184 176,178 Z","#80DDB8","#376958",3);c.Pop();
            E(104,208,20,10,"#62BDA0","#376958");E(156,208,20,10,"#62BDA0","#376958");
            P("M130,122 C162,122 181,150 179,182 C177,210 83,213 81,184 C79,152 97,122 130,122 Z","#8DE1BC","#376958",3);
            E(130,179,24,27,"#E4FFF0");E(84,175,11,19,"#94E6C4","#376958");E(177,175,11,19,"#94E6C4","#376958");
            P("M76,97 L73,57 C72,51 77,49 82,53 L106,75 C121,70 141,70 155,75 L180,53 C185,49 190,52 188,59 L183,99 C194,113 193,137 181,149 C164,168 98,170 80,152 C65,137 65,113 76,97 Z","#9CE7C5","#376958",3);
            P("M83,63 L99,79 L83,87 Z M178,63 L162,80 L178,87 Z","#F5B8BE");
            P("M89,99 C96,85 113,82 127,82",null,"#DBFFEC",6);
            P("M124,76 L123,88 M136,77 L134,87",null,"#5FBE9D",4);
            Face(121);E(89,139,10,5,"#F4B7BD");E(172,139,10,5,"#F4B7BD");
            P("M79,134 L62,130 M79,141 L62,144 M181,134 L198,130 M181,141 L198,144",null,"#376958",2);
            P("M109,166 Q130,173 151,166",null,"#477F70",5);E(130,174,7,7,"#FFE99D","#376958",1.5);
        }
        void Bolt()
        {
            L(130,66,130,46,"#775330",4);E(130,42,7,7,"#7CE7C5","#775330");E(128,40,2,2,"#D8FFFF");
            R(90,199,33,18,6,"#F9CC73","#775330");R(137,199,33,18,6,"#F9CC73","#775330");
            R(90,145,80,59,17,"#F1BF65","#775330");R(102,156,56,32,10,"#FFF0BE");
            P("M131,157 L119,174 L129,174 L126,186 L142,169 L132,169 L138,157 Z","#D99A35");
            E(83,180,11,11,"#FFE099","#775330");E(177,180,11,11,"#FFE099","#775330");
            R(64,93,13,35,6,"#A17645","#775330");R(183,93,13,35,6,"#A17645","#775330");
            R(72,66,116,91,27,"#FFD982","#775330");
            P("M84,94 Q85,77 101,77 L157,77",null,"#FFF0C4",5);R(83,89,94,55,18,"#233047","#92683D");
            Face(113,true);
        }
        void Drako()
        {
            c.Rotate(Math.Sin(t*Math.PI/3)*4*motion,159,188);
            P("M154,181 C176,183 192,188 195,173 C198,159 192,145 205,136 C199,153 216,168 211,185 C205,213 173,211 153,199 Z","#292738","#B996ED");
            P("M202,145 C190,132 205,126 205,114 C222,129 219,143 202,145 Z","#B67CEC","#E0ABFA",1.7);c.Pop();
            foreach(int side in new[] {-1,1})
            {
                c.Translate(side<0?101:160,145);c.Scale(side,1,0,0);c.Rotate(-13+Math.Sin(t*Math.PI/3)*3*motion,0,0);
                P("M0,0 C12,-14 19,-42 46,-48 L39,-25 Q51,-30 55,-32 L47,-6 Q55,-6 58,-4 L35,30 Q22,16 5,23 Z","#7048A3","#B996ED");
                P("M1,1 L44,-45 M1,1 L39,-24 M1,1 L46,-5 M1,1 L35,27",null,"#C292F0",1.8);c.Pop();c.Pop();c.Pop();
            }
            E(108,209,18,10,"#282536","#B996ED");E(153,209,18,10,"#282536","#B996ED");
            P("M128,128 C154,128 169,151 168,177 C169,199 153,211 130,211 C106,211 91,200 92,178 C91,151 103,131 128,128 Z","#302B3B","#B996ED");
            E(130,177,23,27,"#9C77C5","#5C427E");
            P("M112,169 Q130,177 148,169 M109,179 Q130,188 151,179 M111,190 Q130,198 148,190",null,"#5F4484",2);
            E(94,175,10,18,"#353044","#B996ED");E(168,175,10,18,"#353044","#B996ED");
            P("M95,82 C83,76 80,60 85,47 C90,61 101,64 106,71 Z M155,73 C160,63 172,60 178,47 C181,63 175,78 166,85 Z","#E8C7A2","#8C7297");
            P("M130,70 C159,70 175,90 174,112 C174,136 157,151 131,153 C105,154 84,139 83,115 C82,89 101,70 130,70 Z","#342D43","#B996ED",2.8);
            P("M91,106 Q94,86 113,80",null,"#8D70AC",4);
            P("M118,73 L128,59 L139,73 L131,82 Z","#9E70D0","#563B78",1.5);
            // The muzzle is drawn before the expressive eyes and mouth.
            P("M111,121 Q130,116 149,121 Q160,126 155,137 Q145,148 128,147 Q110,147 102,138 Q99,127 111,121 Z","#6B506F","#AA83B9",1.8);
            DrawFace(c,130,109,21,"#FFD486",pose,t,strength,false,motion);
            E(116,128,3,2,"#2A2335");E(143,128,3,2,"#2A2335");
        }
        void Blueprint()
        {
            c.Opacity(strength);c.Translate(0,(1-strength)*20);
            // Rolled edges, dimension lines and the outline of a little block house.
            R(72,169,121,57,4,"#245EA0","#94D7FF");
            for(int i=0;i<8;i++) L(80+i*14,174,80+i*14,219,"#427DB3",.7);
            for(int i=0;i<4;i++) L(78,176+i*13,186,176+i*13,"#427DB3",.7);
            R(69,167,8,62,4,"#477FB6","#A7E2FF");R(188,168,8,61,4,"#477FB6","#A7E2FF");
            P("M99,199 L127,180 L157,199 M106,195 L106,216 L151,216 L151,195 M121,216 L121,202 L135,202 L135,216",null,"#E2F6FF",1.6);
            L(92,222,164,222,"#BEEAFF",1);L(92,220,92,224,"#BEEAFF",1);L(164,220,164,224,"#BEEAFF",1);
            double stroke=Math.Sin(t*Math.PI)*motion;
            double x=153+stroke*9, y=192+Math.Cos(t*Math.PI*2)*2*motion;
            // Each buddy's hand or tentacle holds the sheet; the other guides the pencil.
            string skin=Hands[(int)kind];
            P("M92,162 Q84,182 87,192",null,Ink,13);P("M92,162 Q84,182 87,192",null,skin,9);
            E(87,193,8,7,skin,Ink,2);
            c.Translate(x,y);c.Rotate(-32+stroke*8,0,0);
            R(-3,-32,6,30,1,"#FFD277","#805B39");R(-3,-35,6,5,1,"#F6A0AA");
            P("M-3,-2 L3,-2 L0,6 Z","#F2D4B0","#805B39",1);P("M-1,3 L1,3 L0,6 Z",Ink);c.Pop();c.Pop();
            c.Translate(x-6,y-10);c.Rotate(stroke*5,0,0);
            E(0,0,9,7,skin,Ink,2);if(kind==PetKind.Marina) { E(-3,3,1.8,1.8,"#FFE2E6");E(3,3,1.8,1.8,"#FFE2E6"); }
            if(kind==PetKind.Drako) P("M-5,4 L-3,9 L0,4 M2,4 L4,9 L6,3","#F7D7B0");
            c.Pop();c.Pop();
            double progress=(1+Math.Sin(t*Math.PI/3))/2;
            L(156,207,156+progress*22,207,"#ECF8FF",1.5);
            if(kind==PetKind.Astro) Star(85,180,4,"#FFE99A");
            if(kind==PetKind.Mochi) { E(176,215,3,2,"#BAF5E7");E(173,211,1.3,1.3,"#BAF5E7");E(178,211,1.3,1.3,"#BAF5E7"); }
            if(kind==PetKind.Bolt) { L(105,180,105,188,"#85F5D9",1);L(102,184,108,184,"#85F5D9",1); }
            if(kind==PetKind.Drako) P("M82,213 L86,205 L89,213 Z","#DEC0EF");
            if(kind==PetKind.Kitsu) P("M171,213 Q166,204 179,201 Q182,212 171,213 Z","#A7E7CA");
            if(kind==PetKind.Nimbo) { E(175,181,6,3,"#BAE7FF");E(175,178,3,3,"#BAE7FF"); }
            if(kind==PetKind.Marina) P("M172,216 L172,206 L179,204 L179,213 M169,217 L172,216 M176,214 L179,213",null,"#F4B5F0",1.4);
            c.Pop();c.Pop();
        }
        void Wonder()
        {
            c.Opacity(strength);
            string skin=Hands[(int)kind];
            // One hand goes to the mouth, while the find floats beside the character.
            double mouth=new double[] { 159,144,134,132,136,148,154 }[(int)kind];
            c.Translate(0,-strength*4*motion);
            string arm="M92,177 Q87,157 108,"+mouth.ToString("0",System.Globalization.CultureInfo.InvariantCulture);
            P(arm,null,Ink,13);P(arm,null,skin,9);
            E(112,mouth,9,7,skin,Ink,2);c.Pop();
            double y=99+Math.Sin(t*Math.PI)*3*motion;
            c.Translate(204,y);c.Rotate(Math.Sin(t*Math.PI/2)*8*motion,0,0);
            if(mineral==MineralVisual.AncientDebris)
            {
                P("M-16,-12 L5,-20 L20,-10 L20,13 L0,22 L-16,11 Z","#755143","#D9AB8F");
                P("M-16,-12 L0,0 L20,-10 M0,0 L0,22",null,"#C28E70",2);
                P("M-10,-10 L-3,-7 L-8,-3 M5,4 L14,0 L14,9 L7,13",null,"#E0B799",2);
            }
            else
            {
                string color=mineral==MineralVisual.Emerald ? "#6BE5A4" : "#78E6ED";
                P("M-17,-7 L-8,-20 L10,-20 L19,-7 L0,19 Z",color,"#E2FFF7");
                P("M-17,-7 L19,-7 M-8,-20 L-5,-7 L0,19 L6,-7 L10,-20",null,"#EDFFFA",1.4);
            }
            c.Pop();c.Pop();
            Star(197,65,7,"#FFE5A1");Star(227,114,6,"#D1FFF1");
            Star(56,91,8,"#FFE5A1");Star(67,68,4,"#E5D1FF");
            L(57,121,57,132,"#FFE5A1",3);E(57,139,2,2,"#FFE5A1");
            c.Pop();
        }
    }
}
