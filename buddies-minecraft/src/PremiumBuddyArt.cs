using System;

namespace BuddyMinecraft
{
    public interface IPremiumCanvas
    {
        void Path(string data, string fill, string stroke, double width);
        void Ellipse(double x, double y, double rx, double ry, string fill, string stroke, double width);
        void Round(double x, double y, double w, double h, double radius, string fill, string stroke, double width);
        void Line(double x1, double y1, double x2, double y2, string color, double width);
        void Translate(double x, double y);
        void Rotate(double angle, double x, double y);
        void Scale(double x, double y, double cx, double cy);
        void Opacity(double value);
        void Pop();
    }

    // Original art and choreography, in a 260 x 260 canvas. No assets, services or packages required.
    // Effects resolve back to the resting silhouette; only Idle loops. Reduced motion keeps the story.
    public sealed class PremiumBuddyArt
    {
        readonly IPremiumCanvas c;
        MinecraftPose minecraftPose;
        double minecraftStrength;
        readonly PetKind kind;
        readonly PetMood mood;
        readonly double time, motion, phase, act, activityTime;
        readonly DesktopActivity activity;
        const string Ink = "#28304A";
        double goal, win, lost, hurt, scared, nap, phone, curious, wave;

        PremiumBuddyArt(IPremiumCanvas canvas, PetKind pet, PetMood state, double seconds, double movement,
            DesktopActivity desktop, double desktopSeconds)
        {
            c = canvas; kind = pet; mood = state;
            time = Finite(seconds) ? Math.Max(0,seconds) : 0;
            motion = Finite(movement) ? Math.Max(0,Math.Min(2,movement)) : 1;
            phase = Clamp(time / AnimationTiming.Duration(state));
            activity = state == PetMood.Idle ? desktop : DesktopActivity.None;
            activityTime = Finite(desktopSeconds) ? Math.Max(0,desktopSeconds) : 0;
            double duration = AnimationTiming.DesktopDuration(activity);
            act = activity == DesktopActivity.None ? 0 : Smooth(Clamp(activityTime / 1.1)) * Smooth(Clamp((duration - activityTime) / 1.1));
        }

        public static void Draw(IPremiumCanvas canvas, PetKind kind, PetMood mood, double seconds, double motion,
            DesktopActivity activity, double activitySeconds)
        {
            if (canvas == null) throw new ArgumentNullException("canvas");
            if (!BuddyCatalog.HasPremiumArt(kind)) throw new ArgumentOutOfRangeException("kind");
            new PremiumBuddyArt(canvas,kind,mood,seconds,motion,activity,activitySeconds).Draw();
        }

        public static void DrawMinecraft(IPremiumCanvas canvas, PetKind kind, MinecraftPose pose, double seconds, double motion, double strength)
        {
            var art = new PremiumBuddyArt(canvas, kind, PetMood.Idle, seconds, motion, DesktopActivity.None, 0);
            art.minecraftPose = pose; art.minecraftStrength = strength; art.Draw();
        }

        static bool Finite(double v) { return !Double.IsNaN(v) && !Double.IsInfinity(v); }
        static double Clamp(double v) { return Math.Max(0,Math.Min(1,v)); }
        static double Smooth(double v) { return v*v*(3-2*v); }
        double Beat(double start, double end)
        {
            return Smooth(Clamp((phase-start)/.13)) * Smooth(Clamp((end-phase)/.17));
        }
        void Draw()
        {
            goal = mood == PetMood.Goal ? Beat(.04,.94) : 0;
            win = mood == PetMood.Victory ? Beat(.05,.96) : 0;
            lost = mood == PetMood.Defeat ? Beat(.07,.94) : 0;
            hurt = mood == PetMood.Conceded ? Beat(.04,.91) : 0;
            scared = mood == PetMood.Scared ? Beat(.02,.90) : 0;
            nap = activity == DesktopActivity.Nap ? act : 0;
            phone = activity == DesktopActivity.Phone ? act : 0;
            curious = activity == DesktopActivity.Curious ? act : 0;
            wave = Math.Sin((mood == PetMood.Idle ? time : phase*6) * Math.PI/3) * motion;
            c.Opacity(.14); E(130,229,45,6,"#141B32"); c.Pop();
            c.Translate(0,4); c.Scale(.86,.86,130,216);
            if (kind == PetKind.Kitsu) Kitsu();
            else if (kind == PetKind.Nimbo) Nimbo();
            else Marina();
            c.Pop(); c.Pop();
        }

        void P(string data,string fill,string stroke,double width) { c.Path(data,fill,stroke,width); }
        void E(double x,double y,double rx,double ry,string fill) { c.Ellipse(x,y,rx,ry,fill,null,0); }
        void O(double x,double y,double rx,double ry,string fill,string edge) { c.Ellipse(x,y,rx,ry,fill,edge,2.4); }
        void R(double x,double y,double w,double h,double radius,string fill,string edge) { c.Round(x,y,w,h,radius,fill,edge,2.4); }
        void L(double x1,double y1,double x2,double y2,string color,double width) { c.Line(x1,y1,x2,y2,color,width); }
        void Star(double x,double y,double size,string color)
        {
            c.Translate(x,y); c.Scale(size/10,size/10,0,0);
            P("M0,-10 L3,-3 L10,0 L3,3 L0,10 L-3,3 L-10,0 L-3,-3 Z",color,null,0);
            c.Pop(); c.Pop();
        }
        void Leaf(double x,double y,double angle,double size)
        {
            c.Translate(x,y); c.Rotate(angle,0,0); c.Scale(size,size,0,0);
            P("M-11,6 Q-15,-10 10,-12 Q17,5 -11,6 Z","#7CE0C1","#276B67",1.8);
            P("M-13,10 Q-1,-4 7,-8",null,"#276B67",1.5);
            c.Pop();c.Pop();c.Pop();
        }
        void Note(double x,double y,double rotation,string color)
        {
            c.Translate(x,y);c.Rotate(rotation,0,0);
            P("M0,8 L0,-10 L14,-13 L14,5 M0,-5 L14,-8",null,color,3);
            E(-4,8,5,3.5,color);E(10,5,5,3.5,color); c.Pop();c.Pop();
        }
        void Face(double x,double y,double spacing,string eye)
        {
            if (minecraftPose != MinecraftPose.Idle && minecraftStrength > .08)
            {
                MinecraftBuddyArt.DrawFace(c, x, y, spacing, eye, minecraftPose, time, minecraftStrength, false, motion);
                return;
            }
            bool sleepy = nap > .45 || (mood == PetMood.Idle && time % 6 > 4.8 && time % 6 < 5.03);
            bool happy = win > .25 || goal > .3 || curious > .3;
            if (sleepy || happy)
            {
                c.Translate(x,y);
                c.Translate(-spacing,0); P(happy ? "M-6,1 Q0,-9 6,1" : "M-6,0 Q0,5 6,0",null,eye,3); c.Pop();
                c.Translate(spacing,0); P(happy ? "M-6,1 Q0,-9 6,1" : "M-6,0 Q0,5 6,0",null,eye,3); c.Pop();
                c.Pop();
            }
            else
            {
                E(x-spacing,y,4.5,scared > .2 ? 9 : 6.5,eye); E(x+spacing,y,4.5,scared > .2 ? 9 : 6.5,eye);
                E(x-spacing+1,y-2,1.6,2,"#FFFFFF");E(x+spacing+1,y-2,1.6,2,"#FFFFFF");
                if (hurt+lost > .3)
                {
                    L(x-spacing-6,y-11,x-spacing+4,y-14,eye,2.2);
                    L(x+spacing-4,y-14,x+spacing+6,y-11,eye,2.2);
                }
            }
            c.Opacity(.55); E(x-spacing-8,y+11,7,3,"#F48FA5");E(x+spacing+8,y+11,7,3,"#F48FA5");c.Pop();
            c.Translate(x,y+13);
            if (scared > .3) O(0,3,4,5,eye,eye);
            else if (happy) P("M-6,0 Q0,3 6,0 Q4,12 0,11 Q-5,10 -6,0 Z",eye,null,0);
            else P(hurt+lost > .3 ? "M-5,5 Q0,-1 5,5" : "M-5,1 Q0,7 5,1",null,eye,2.1);
            c.Pop();
        }

        void Kitsu()
        {
            double hop = -Math.Sin(phase*Math.PI)*goal*13*motion - win*5*motion;
            c.Translate((goal-win)*wave*3,hop+nap*9+lost*5);
            c.Rotate(wave*(1+goal*5+win*4)+hurt*5-lost*7,130,207);
            // Three separately hinged tails, with cream tips and a teal talisman.
            for(int i=0;i<3;i++)
            {
                c.Translate(152,187); c.Rotate(-18+i*35 + wave*(1+i) + win*(i-1)*5 + nap*(20-i*20),0,0);
                c.Scale(1-nap*.25,1-nap*.3,0,0);
                P("M0,10 C30,17 75,-10 79,-44 C79,-62 64,-65 68,-80 C42,-74 29,-53 27,-31 C25,-15 -9,-10 0,10 Z","#E97D56",Ink,2.7);
                P("M68,-80 C42,-74 29,-53 27,-31 L42,-39 L47,-26 L57,-40 L73,-34 Q87,-60 68,-80 Z","#FFF0D6",null,0);
                P("M16,-12 Q45,-20 51,-53",null,"#FFB886",2.5);c.Pop();c.Pop();c.Pop();
            }
            O(109,211,17,8,"#563A4E",Ink); O(151,211,17,8,"#563A4E",Ink);
            P("M105,148 Q130,132 155,148 L159,185 Q162,207 130,212 Q99,210 100,187 Z","#F39869",Ink,2.8);
            P("M112,147 Q130,159 148,147 L145,184 Q131,201 115,184 Z","#FFEBD2",null,0);
            // Ears retain an unmistakable fox silhouette even at the smallest overlay size.
            c.Rotate(nap*-10+lost*5,130,136);
            P("M81,100 Q74,75 85,48 Q110,60 118,83 M143,83 Q158,56 178,49 Q187,78 176,106","#EF9367",Ink,3);
            P("M85,59 L91,92 L109,83 Z M174,60 L150,84 L173,94 Z","#754357",null,0);
            P("M84,99 Q98,76 130,78 Q163,77 179,104 L190,120 L181,137 Q167,153 130,158 Q95,154 81,137 L70,121 Z","#FFA878",Ink,3);
            P("M77,118 Q93,111 111,126 Q130,137 149,126 Q168,111 184,118 L178,138 Q152,160 128,155 Q97,153 83,136 Z","#FFF0D9",null,0);
            P("M101,93 Q115,85 123,88",null,"#FFD5AB",4);
            Face(130,113,22,Ink);
            P("M125,127 Q130,124 135,127 L130,131 Z",Ink,null,0);
            Leaf(132,89,-40,.56);
            c.Pop();
            P("M108,151 Q130,163 152,151 L150,165 Q131,175 109,165 Z","#45BDA8","#276B67",2);
            P("M146,161 L158,173 L170,166 L158,187 L143,168 Z","#57CEB4","#276B67",2);
            O(132,168,6,7,"#F9D47F","#805738");
            c.Rotate(goal*70+win*110+curious*(70+wave*10),101,161);
            O(99,177,10,18,"#F39869",Ink);E(97,188,7,6,"#FFF0D9");c.Pop();
            c.Rotate(-goal*70-win*110,160,161);O(162,177,10,18,"#F39869",Ink);E(164,188,7,6,"#FFF0D9");c.Pop();
            if (goal+win > 0)
            {
                c.Opacity(Math.Max(goal,win));
                for(int i=0;i<5;i++)
                {
                    double angle=(i*72+phase*150*motion)*Math.PI/180;
                    Leaf(130+Math.Cos(angle)*83,129+Math.Sin(angle)*57,angle*180/Math.PI,.6);
                }
                if (goal > 0)
                {
                    double x=70+Smooth(Clamp((phase-.22)/.46))*124;
                    O(x,72-Math.Sin(phase*Math.PI)*15*motion,12,12,"#B5FFDA","#3CBAA7");
                    Star(x,72-Math.Sin(phase*Math.PI)*15*motion,7,"#FFFFFF");
                }
                if (win > 0) { Star(130,54,17,"#FFE091"); Star(103,57,7,"#9FF2CF");Star(157,57,7,"#9FF2CF"); }
                c.Pop();
            }
            if (hurt > 0)
            {
                c.Opacity(hurt); Leaf(194,156,60+wave*14,.9);
                P("M185,174 L190,170 L197,178 L201,173",null,"#68D2BA",2.8);
                E(188,194+phase*8,3,4,"#8DE6DF");c.Pop();
            }
            if (lost > 0)
            {
                c.Opacity(lost); Leaf(171,195,80,.9);
                double hope=Smooth(Clamp((phase-.48)/.22));
                c.Opacity(hope); Star(181,180,9,"#C2FFE3");c.Pop();c.Pop();
            }
            if (scared > 0)
            {
                c.Opacity(scared);
                // A giant leaf is the fox's failed disappearing trick: the eyes still peek out.
                c.Translate(130,139);c.Rotate(wave*4,0,0);
                P("M-33,34 Q-54,-19 22,-43 Q68,9 -33,34 Z","#6ED4AD","#276B67",3);
                P("M-30,38 Q-3,5 21,-31",null,"#276B67",2.4);
                E(-9,-5,4,7,Ink);E(13,-12,4,7,Ink);c.Pop();c.Pop();c.Pop();
            }
            if (nap > 0)
            {
                c.Opacity(nap);P("M89,179 Q107,210 151,188 Q178,174 176,196 Q161,229 111,214 Q91,209 89,179 Z","#E97D56",Ink,2.8);
                P("M150,188 Q174,170 177,194 Q176,205 161,211 L150,205 L143,209 Z","#FFF0D9",null,0);c.Pop();
            }
            if (phone > 0)
            {
                c.Opacity(phone);c.Translate(-5,Math.Sin(activityTime*2)*3*motion);
                Tablet(181,158,"#68E0C1");Leaf(185,146,-35,.6);c.Pop();c.Pop();
            }
            if (curious > 0) { c.Opacity(curious); Leaf(67,146,35+wave*15,.8);Star(67,122,6,"#D6FFD4");c.Pop(); }
            c.Pop();c.Pop();
        }

        void Nimbo()
        {
            // A rainbow physically unfolds behind the cloud on victory.
            if (win > 0)
            {
                c.Opacity(win);c.Scale(.5+.5*win,.5+.5*win,130,174);
                P("M43,170 C20,5 240,5 217,170",null,"#FF9FAD",8);
                P("M53,170 C35,19 225,19 207,170",null,"#FFE08C",8);
                P("M63,170 C50,33 210,33 197,170",null,"#8DE0C5",8);
                P("M73,170 C65,47 195,47 187,170",null,"#BBA8F5",8);c.Pop();c.Pop();
            }
            c.Translate(wave*(1+hurt*2),-wave*3-win*10*motion+lost*7+nap*6);
            c.Rotate(goal*wave*4-scared*wave*5,130,175);
            // Feet trail on soft lightning-shaped legs.
            P("M111,171 L108,190 L116,194 L110,207 M149,171 L153,190 L145,194 L151,207",null,"#6485BA",4);
            O(107,210,15,8,"#FFD984",Ink);O(155,210,15,8,"#FFD984",Ink);
            c.Rotate(-curious*(30+wave*12)-goal*50,78,151);O(70,168,9,18,"#D3ECFF","#577FAC");c.Pop();
            c.Rotate(goal*65+win*60,184,151);O(191,168,9,18,"#D3ECFF","#577FAC");c.Pop();
            const string cloud="M79,108 C73,79 107,67 125,86 C148,62 181,82 178,106 C207,100 217,126 202,143 C212,166 187,184 166,172 C153,190 124,190 112,179 C85,190 61,174 68,153 C43,143 51,112 79,108 Z";
            P(cloud,"#C9E9FC","#577FAC",3);
            P("M68,147 Q88,166 107,154 Q133,176 156,158 Q183,169 202,145 C205,168 185,180 166,172 C153,190 124,190 112,179 Q71,192 68,147 Z","#97C8E9",null,0);
            c.Opacity(hurt*.7+lost*.45);P(cloud,"#6E85B8",null,0);c.Pop();
            P("M83,103 Q84,85 105,87 M135,90 Q155,80 167,96",null,"#F1FAFF",4);
            Face(133,126,21,Ink);
            P("M124,164 L138,164 L131,173 L139,173 L125,187 L128,175 L120,175 Z","#FFD77F","#BD9855",1.7);
            if (goal > 0)
            {
                c.Opacity(goal);c.Translate(7*Math.Sin(phase*Math.PI)*motion,0);
                P("M186,80 L164,112 L180,111 L166,137 L203,103 L186,104 L206,80 Z","#FFE18C","#B08043",2.6);
                Star(213,105,9,"#FFF1B2");Star(163,68,7,"#FFE18C");c.Pop();c.Pop();
            }
            if (hurt+lost > 0)
            {
                c.Opacity(Math.Max(hurt,lost));
                for(int i=0;i<5;i++)
                {
                    double fall=(phase*2+i*.21)%1;
                    c.Opacity(Math.Sin(fall*Math.PI));c.Translate(86+i*22,183+fall*33);
                    P("M0,-5 Q-8,6 0,7 Q8,6 0,-5 Z","#78C9F0",null,0);c.Pop();c.Pop();
                }
                if (lost > 0)
                {
                    double grow=Smooth(Clamp((phase-.35)/.3));c.Scale(1,.15+grow*.85,189,223);
                    P("M189,223 L189,201 M189,214 Q171,214 174,201 Q188,199 189,210 M190,209 Q204,206 204,196 Q190,195 190,209","#89D8AF","#437D6C",2);
                    c.Pop();
                }
                c.Pop();
            }
            if (scared > 0)
            {
                c.Opacity(scared);
                P("M124,77 L124,125 Q124,137 133,130",null,"#886CAD",3);
                P("M80,83 Q121,31 168,83 Q155,72 143,84 Q125,73 112,84 Q96,72 80,83 Z","#BFA6ED","#675589",2.8);
                P("M112,83 Q112,58 124,56 Q141,63 143,82",null,"#E6D9FF",2);c.Pop();
            }
            if (win+curious > 0)
            {
                c.Opacity(Math.Max(win,curious));c.Rotate(wave*4,74,91);
                for(int i=0;i<8;i++)
                {
                    double a=i*Math.PI/4;
                    L(73+Math.Cos(a)*20,88+Math.Sin(a)*20,73+Math.Cos(a)*26,88+Math.Sin(a)*26,"#FFD982",2.5);
                }
                O(73,88,15,15,"#FFE49E","#BE9D66");E(69,87,1.7,2,Ink);E(78,87,1.7,2,Ink);c.Pop();c.Pop();
            }
            if (phone > 0) { c.Opacity(phone);Tablet(186,172,"#A8DFFD");Star(186,174,7,"#FFE190");c.Pop(); }
            if (nap > 0)
            {
                c.Opacity(nap);P("M90,89 Q114,54 154,67 Q178,64 182,90 Q173,81 164,84 Q123,72 105,97 Z","#A69ADB","#675D96",2.5);
                O(183,92,8,8,"#FFF1BC","#C8B17B");Star(138,77,6,"#FFEBAC");c.Pop();
            }
            c.Pop();c.Pop();
        }

        void Marina()
        {
            if (scared > 0)
            {
                c.Opacity(scared*.7);
                for(int i=0;i<7;i++)
                {
                    double a=i*Math.PI*2/7;
                    E(130+Math.Cos(a)*(48+phase*15*motion),150+Math.Sin(a)*47,20,19,"#635886");
                }
                c.Pop();
            }
            c.Translate(wave*(goal*4+win*6),-win*6*motion+lost*5+nap*9);
            c.Rotate(wave*(1+win*5)+lost*7,130,182);
            // Eight articulated arms; four sit behind the mantle, four in front.
            for(int i=0;i<8;i++)
            {
                bool back=i<4;
                if(i==4) MarinaHead();
                double x=back ? 86+i*29 : 92+(i-4)*25;
                double angle=(i%2==0 ? -1 : 1)*(8+(Math.Sin(time*Math.PI/3+i)-Math.Sin(i))*4*motion);
                if(mood!=PetMood.Idle) angle=(i%2==0 ? -1 : 1)*(8+(Math.Sin(phase*Math.PI*4+i)-Math.Sin(i))*4*motion*Math.Sin(phase*Math.PI));
                angle += (i<4 ? win*(i<2 ? 120 : -120) : goal*(i<6 ? 15 : -15));
                c.Translate(x,back ? 157 : 173);c.Rotate(angle,0,0);c.Scale(i%2==0 ? 1 : -1,1-nap*.2,0,0);
                P("M-8,0 C-12,17 -12,35 -24,32 C-33,31 -28,23 -25,24 C-29,12 -44,23 -36,36 C-21,58 1,33 7,12 L9,0 Z",back ? "#9670BA" : "#BE8DD8",Ink,2.6);
                P("M-28,38 Q-15,46 -6,20",null,"#E5B8ED",3);
                E(-21,39,2.1,2,"#FFE2E6");E(-14,34,2.1,2,"#FFE2E6");E(-10,27,2,2,"#FFE2E6");c.Pop();c.Pop();c.Pop();
            }
            if(goal+win > 0)
            {
                c.Opacity(Math.Max(goal,win));
                Deck();
                // A scratch hand crosses the vinyl; the second hand works the fader.
                c.Rotate(Math.Sin(phase*Math.PI*8)*goal*12*motion,99,181);
                P("M95,163 Q86,184 104,192 Q115,198 119,190 Q107,188 110,180", "#CFA0E3",Ink,2.6);c.Pop();
                P("M165,162 Q176,179 155,190",null,Ink,12);
                P("M165,162 Q176,179 155,190",null,"#CFA0E3",7);
                for(int i=0;i<3;i++) Note(64+i*66,89-Math.Sin(phase*Math.PI+i)*12*motion,-15+i*12,i==1 ? "#FFE394" : "#8EE8D3");
                if(win > 0)
                {
                    L(130,43,130,57,"#B5A9DA",2);O(130,69,15,15,"#D4CEF5","#827CA5");
                    P("M117,64 L143,64 M116,72 L144,72 M122,58 L122,80 M131,55 L131,84 M139,58 L139,80",null,"#948DCD",1.5);
                    Star(154,59,6,"#FFEFC3");
                }
                c.Pop();
            }
            if(hurt > 0)
            {
                c.Opacity(hurt);
                P("M76,159 C49,161 58,219 89,204 C115,189 78,177 77,193 C76,218 184,219 191,189",null,"#72CECA",3.5);
                R(185,177,10,17,3,"#FFE59A",Ink);L(188,174,188,178,Ink,2);L(192,174,192,178,Ink,2);c.Pop();
            }
            if(lost > 0)
            {
                c.Opacity(lost);c.Rotate(Smooth(Clamp((phase-.47)/.2))*150*motion,172,192);
                O(172,192,21,21,"#34384F",Ink);O(172,192,8,8,"#ECA8CE","#FFE0DF");
                P("M160,179 Q171,168 184,181",null,"#747597",2);E(172,192,2,2,"#34384F");c.Pop();c.Pop();
            }
            if(scared > 0)
            {
                c.Opacity(scared);
                P("M86,184 Q91,145 111,133 M174,184 Q169,145 151,133",null,Ink,15);
                P("M86,184 Q91,145 111,133 M174,184 Q169,145 151,133",null,"#CCA0E2",10);c.Pop();
            }
            if(phone > 0) { c.Opacity(phone);Tablet(131,183,"#98E8D3");Note(132,184,0,"#72578F");c.Pop(); }
            if(curious > 0) { c.Opacity(curious);Note(192,117,wave*9,"#95E8D5");Star(71,119,7,"#FFE094");c.Pop(); }
            if(nap > 0)
            {
                c.Opacity(nap);P("M74,186 Q130,232 186,186 L176,209 Q126,234 84,207 Z","#82CECA","#3A737C",2.5);
                P("M86,197 Q130,221 175,199",null,"#C6F0DA",2.5);c.Pop();
            }
            c.Pop();c.Pop();
        }

        void MarinaHead()
        {
            P("M86,134 C83,96 105,83 131,82 C165,81 184,105 179,135 Q192,170 161,181 Q130,194 100,180 Q73,169 86,134 Z","#CFA0E3",Ink,3);
            P("M86,142 Q96,166 123,162 Q156,178 179,143 Q191,170 161,181 Q125,194 100,180 Q78,171 86,142 Z","#AD81CB",null,0);
            P("M104,103 Q117,91 139,95",null,"#EFD0F4",4);
            Face(132,132,21,Ink);
            P("M83,132 Q75,72 130,72 Q188,71 185,132",null,Ink,10);
            P("M83,126 Q82,78 130,78 Q180,77 184,128",null,"#72D5CB",5);
            R(75,115,19,33,8,"#3D4058",Ink);R(172,115,19,33,8,"#3D4058",Ink);
            R(77,121,9,20,4,"#81E3D2",null);R(181,121,8,20,4,"#81E3D2",null);
            P("M182,143 Q183,163 152,159",null,Ink,2.5);O(151,159,5,3,"#FFE19B",Ink);
            Star(153,104,7,"#FFE098");
        }
        void Deck()
        {
            P("M79,178 L181,178 L195,206 L66,206 Z","#3E435F",Ink,2.6);
            R(66,203,129,10,4,"#252D46",Ink);
            O(97,193,19,9,"#20273C","#9E9ACE");O(97,193,7,3.5,"#F4ADD6",null);
            O(166,193,19,9,"#20273C","#9E9ACE");O(166,193,7,3.5,"#8DE7D3",null);
            L(124,187,125,201,"#8E96B7",2);L(133,187,134,201,"#8E96B7",2);L(142,187,143,201,"#8E96B7",2);
            L(122,193,129,193,"#F5DDA6",3);L(131,197,138,197,"#F5DDA6",3);L(140,190,146,190,"#F5DDA6",3);
            for(int i=0;i<5;i++) L(106+i*7,209,109+i*7,209,i%2==0 ? "#8EEBD3" : "#D3A2F0",2);
        }
        void Tablet(double x,double y,string color)
        {
            c.Translate(x,y);c.Rotate(-8,0,0);
            R(-15,-24,30,45,5,"#34364F",Ink);R(-11,-18,22,31,2,color,null);
            L(-5,17,5,17,"#B6BCD2",2);L(-6,-9,5,-9,"#F6FBFF",2);L(-6,-3,2,-3,"#F6FBFF",2);
            c.Pop();c.Pop();
        }
    }
}
