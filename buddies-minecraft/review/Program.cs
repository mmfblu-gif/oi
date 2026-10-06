using System;
using System.IO;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Encodings.Web;
using BuddyMinecraft;

internal static class Program
{
    static int Main(string[] args)
    {
        try
        {
            Checks.Run();
            if(args.Length==0 || args[0]=="--test") return 0;
            string dir=Path.GetFullPath(args[0]);Directory.CreateDirectory(dir);
            string[] clips={"CraftingIntro","Crafting","ImpressedDiamond","ImpressedEmerald","ImpressedDebris"};
            var data=new Dictionary<string,object>();
            foreach(PetKind buddy in Enum.GetValues(typeof(PetKind)))
            {
                var scenes=new Dictionary<string,object>();
                for(int state=0;state<clips.Length;state++)
                {
                    var pose=state<2?MinecraftPose.Crafting:MinecraftPose.Impressed;
                    var mineral=state==3?MineralVisual.Emerald:state==4?MineralVisual.AncientDebris:MineralVisual.Diamond;
                    int count=state==0?16:64;
                    double duration=state==0?.6:state==1?6:4.2;
                    var modes=new string[2][];
                    for(int mode=0;mode<2;mode++)
                    {
                        modes[mode]=new string[count];
                        for(int i=0;i<count;i++)
                        {
                            double seconds=state==1 ? .6+duration*i/count : duration*i/(count-1);
                            double reveal=state==0 ? seconds/.6 : 1;
                            var c=new SvgCanvas();MinecraftBuddyArt.Draw(c,buddy,pose,seconds,mode==0?1:.3,mineral,reveal);
                            modes[mode][i]=c.Document;
                        }
                    }
                    string item=state==3?"minecraft:emerald":state==4?"minecraft:ancient_debris":"minecraft:diamond";
                    string ev=state<2?"bancada_aberta":"minerio_raro";
                    string[] lines=new string[3];for(int v=0;v<3;v++) lines[v]=MinecraftSpeech.Phrase(buddy,ev,v,item);
                    scenes.Add(clips[state],new{duration=duration,frames=modes,lines=lines,loop=state==1});
                    if(state!=0) File.WriteAllText(Path.Combine(dir,BuddyCatalog.Slug(buddy)+"-"+clips[state]+".svg"),modes[0][count/2]);
                }
                data.Add(BuddyCatalog.Slug(buddy),new{name=buddy.ToString(),color=BuddyCatalog.Colors[(int)buddy],scenes=scenes});
            }
            File.WriteAllText(Path.Combine(dir,"scenes.json"),JsonSerializer.Serialize(data,new JsonSerializerOptions{Encoder=JavaScriptEncoder.UnsafeRelaxedJsonEscaping}));
            Console.WriteLine("Exported seven buddies, normal/gentle, intro + loop + three rare minerals.");
            return 0;
        }
        catch(Exception e) { Console.Error.WriteLine(e);return 1; }
    }
}
