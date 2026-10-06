using System;
using BuddyMinecraft;

internal static class Checks
{
    static int count;
    static void Check(bool value,string text) { if(!value) throw new Exception("FAIL: "+text);count++;Console.WriteLine("PASS: "+text); }
    public static void Run()
    {
        var s=new MinecraftSession();
        Check(s.Snapshot(PetKind.Astro,0).Animation=="Idle","starts idle");
        s.SetCrafting(true,1);var f=s.Snapshot(PetKind.Astro,1.3);
        Check(f.Animation=="Crafting" && f.Reveal>.49 && f.Reveal<.51,"crafting reveal starts at screen open");
        s.SetCrafting(true,1.4);Check(Math.Abs(s.Snapshot(PetKind.Astro,1.5).Seconds-.5)<.001,"repeated open notification does not restart drawing");
        Check(s.Snapshot(PetKind.Astro,7).Speech=="","crafting speech is not repeated on every loop");
        s.SetCrafting(false,8);Check(s.Snapshot(PetKind.Astro,8).Animation=="Idle","closing workbench stops drawing");
        Check(!s.ObtainMineral("stone","minecraft:stone",1,9),"ordinary blocks are not ore");
        Check(!s.ObtainMineral("zero","minecraft:diamond",0,9),"zero-count inventory changes ignored");
        Check(s.ObtainMineral("iron","minecraft:raw_iron",1,10) && s.Snapshot(PetKind.Bolt,10).Animation=="Goal","common ore uses existing celebration");
        Check(!s.ObtainMineral("iron","minecraft:raw_iron",1,10.1),"duplicate pickup ignored");
        Check(!s.ObtainMineral("coal","minecraft:coal",8,11),"common-ore burst does not spam");
        Check(s.ObtainMineral("diamond","minecraft:diamond",1,12) && s.Snapshot(PetKind.Kitsu,12).Animation=="Impressed","rare find preempts common ore");
        Check(s.Snapshot(PetKind.Kitsu,12).Speech=="DIAMANTE ENCANTADO!","specific rare-mineral phrase selected");
        Check(!s.ObtainMineral("emerald-fast","minecraft:emerald",1,13),"rare-ore burst cooldown");
        Check(s.UnlockAdvancement("advance-1",14) && s.Snapshot(PetKind.Marina,14).Animation=="Victory","advancement preempts ore and uses Victory");
        Check(!s.UnlockAdvancement("advance-1",15),"duplicate advancement ignored");
        Check(s.UnlockAdvancement("advance-2",15),"distinct advancement queued");
        Check(s.Snapshot(PetKind.Astro,20.5).Event=="conquista" && s.Snapshot(PetKind.Astro,20.5).Seconds==0,"queued advancement starts once");
        s.SetCrafting(true,21);Check(s.Die("death",22),"death accepted");
        Check(s.Snapshot(PetKind.Drako,22).Animation=="Defeat","death uses existing Defeat");
        Check(!s.Die("death",22.1),"duplicate death does not replay");
        s.SetCrafting(true,23);Check(s.Snapshot(PetKind.Drako,29).Animation=="Idle","death clears crafting and queued reactions");
        s.Reset();s.SetCrafting(true,0);s.ObtainMineral("gem","minecraft:emerald",1,1);
        Check(s.Snapshot(PetKind.Nimbo,6).Animation=="Crafting","crafting resumes after a reaction when screen remains open");
        s.ObtainMineral("gem2","minecraft:diamond",1,7);s.SetCrafting(false,8);
        Check(s.Snapshot(PetKind.Nimbo,12).Animation=="Idle","close during rare reaction prevents stale blueprint");
        s.Reset();s.UnlockAdvancement("old",0);s.UnlockAdvancement("expired",1);
        Check(s.Snapshot(PetKind.Astro,30).Animation=="Idle","stale queued events are not replayed after suspension");
        s.Reset();s.SetCrafting(true,0);s.Reset();Check(s.Snapshot(PetKind.Astro,1).Animation=="Idle","disconnect/world reset clears activity");
        Check(MineralRules.Family("minecraft:deepslate_diamond_ore")==0 && MineralRules.Family("minecraft:emerald_ore")==1 && MineralRules.Family("minecraft:ancient_debris")==2,"Silk Touch ores and debris classified");
        Check(MineralRules.Family("minecraft:amethyst_shard")==-1 && !MineralRules.IsMineral("minecraft:netherite_ore"),"no fictional netherite ore or automatic rare amethyst");
        foreach(PetKind buddy in Enum.GetValues(typeof(PetKind)))
        {
            foreach(string ev in new[]{"minerio_obtido","minerio_raro","conquista","morte","bancada_aberta"})
            {
                string a=MinecraftSpeech.Phrase(buddy,ev,0),b=MinecraftSpeech.Phrase(buddy,ev,1),c=MinecraftSpeech.Phrase(buddy,ev,2);
                Check(a.Length>0 && b.Length>0 && c.Length>0 && a!=b && b!=c && a!=c,buddy+" three distinct phrases: "+ev);
            }
            string crafting=Render(buddy,MinecraftPose.Crafting,2,1,MineralVisual.Diamond);
            string idle=Render(buddy,MinecraftPose.Idle,2,1,MineralVisual.Diamond);
            string impressed=Render(buddy,MinecraftPose.Impressed,2,1,MineralVisual.Diamond);
            Check(crafting!=idle && impressed!=idle && crafting!=impressed,buddy+" distinct new performances");
            Check(crafting!=Render(buddy,MinecraftPose.Crafting,2,.3,MineralVisual.Diamond) && impressed!=Render(buddy,MinecraftPose.Impressed,2,.3,MineralVisual.Diamond),buddy+" reduced-motion performances");
            Check(impressed!=Render(buddy,MinecraftPose.Impressed,2,1,MineralVisual.Emerald) && impressed!=Render(buddy,MinecraftPose.Impressed,2,1,MineralVisual.AncientDebris),buddy+" distinct rare minerals");
            foreach(MinecraftPose pose in Enum.GetValues(typeof(MinecraftPose)))
                for(int frame=0;frame<96;frame++) foreach(double m in new[]{0.0,.3,1.0})
                    Render(buddy,pose,frame*(pose==MinecraftPose.Impressed?4.2:6)/95,m,MineralVisual.Diamond);
        }
        Console.WriteLine("SUCCESS: "+count+" checks; 6,048 drawing samples. WPF and live Minecraft are not exercised.");
    }
    static string Render(PetKind kind,MinecraftPose pose,double seconds,double motion,MineralVisual mineral)
    {
        var c=new SvgCanvas();MinecraftBuddyArt.Draw(c,kind,pose,seconds,motion,mineral);
        string s=c.Document;if(s.Contains("NaN") || s.Contains("Infinity")) throw new Exception("Nonfinite art");return s;
    }
}
