using System;
using System.Collections.Generic;

namespace BuddyMinecraft
{
    public static class MineralRules
    {
        static readonly HashSet<string> Common = new HashSet<string>(StringComparer.Ordinal) {
            "minecraft:coal","minecraft:raw_iron","minecraft:raw_copper","minecraft:raw_gold",
            "minecraft:iron_ingot","minecraft:copper_ingot","minecraft:gold_ingot",
            "minecraft:lapis_lazuli","minecraft:redstone","minecraft:quartz","minecraft:amethyst_shard",
            "minecraft:coal_ore","minecraft:deepslate_coal_ore","minecraft:iron_ore","minecraft:deepslate_iron_ore",
            "minecraft:copper_ore","minecraft:deepslate_copper_ore","minecraft:gold_ore","minecraft:deepslate_gold_ore",
            "minecraft:redstone_ore","minecraft:deepslate_redstone_ore","minecraft:lapis_ore","minecraft:deepslate_lapis_ore",
            "minecraft:nether_gold_ore","minecraft:nether_quartz_ore"
        };
        public static int Family(string id)
        {
            if(id=="minecraft:diamond" || id=="minecraft:diamond_ore" || id=="minecraft:deepslate_diamond_ore") return 0;
            if(id=="minecraft:emerald" || id=="minecraft:emerald_ore" || id=="minecraft:deepslate_emerald_ore") return 1;
            if(id=="minecraft:ancient_debris") return 2;
            return -1;
        }
        public static bool IsMineral(string id) { return id!=null && (Family(id)>=0 || Common.Contains(id)); }
    }
    public sealed class MinecraftFeedback
    {
        public string Animation { get; internal set; }
        public string Event { get; internal set; }
        public string ItemId { get; internal set; }
        public string Speech { get; internal set; }
        public double Seconds { get; internal set; }
        public int Variant { get; internal set; }
        public double Reveal { get; internal set; }
    }
    // Feed confirmed events for the local player only. This is not a Minecraft mod or a game detector.
    // Polling Snapshot is side-effect free apart from advancing queued performances.
    public sealed class MinecraftSession
    {
        sealed class Reaction { public string Event, Item; public double Created, Duration; public int Priority, Variant; }
        readonly HashSet<string> seen = new HashSet<string>(StringComparer.Ordinal);
        readonly Queue<string> seenOrder = new Queue<string>();
        readonly Queue<Reaction> pending = new Queue<Reaction>();
        readonly Dictionary<string,int> variants = new Dictionary<string,int>();
        Reaction active;
        bool crafting;
        double craftStart, started, lastNow, lastOre=Double.NegativeInfinity, lastRare=Double.NegativeInfinity;
        int craftVariant;
        double Now(double now)
        {
            if(Double.IsNaN(now)||Double.IsInfinity(now)) return lastNow;
            lastNow=Math.Max(lastNow,Math.Max(0,now));return lastNow;
        }
        bool Remember(string id)
        {
            if(String.IsNullOrWhiteSpace(id) || id.Length>256 || !seen.Add(id)) return false;
            seenOrder.Enqueue(id);if(seenOrder.Count>256) seen.Remove(seenOrder.Dequeue());return true;
        }
        int Next(string key)
        {
            int v; if(!variants.TryGetValue(key,out v)) v=0;
            variants[key]=(v+1)%3;return v;
        }
        void Advance(double now)
        {
            if(active!=null && now-started<active.Duration) return;
            active=null;
            while(pending.Count>0)
            {
                var next=pending.Dequeue();
                if(now-next.Created>12) continue;
                active=next;started=now;return;
            }
        }
        bool Submit(string id,string eventName,string item,double now,int priority,double duration)
        {
            now=Now(now);Advance(now);
            if(!Remember(id)) return false;
            if(active!=null && active.Priority==4) return false;
            if(priority==1 && (active!=null || now-lastOre<5)) return false;
            if(priority==2 && now-lastRare<4.2) return false;
            if(active!=null && priority<=active.Priority && pending.Count>=8) return false;
            var r=new Reaction { Event=eventName,Item=item,Created=now,Priority=priority,Duration=duration,Variant=Next(eventName) };
            if(priority==1) lastOre=now; if(priority==2) lastRare=now;
            if(active==null || priority>active.Priority) { active=r;started=now; }
            else pending.Enqueue(r);
            return true;
        }
        public bool ObtainMineral(string eventId,string itemId,int count,double now)
        {
            if(count<=0 || !MineralRules.IsMineral(itemId)) return false;
            bool rare=MineralRules.Family(itemId)>=0;
            return Submit(eventId,rare?"minerio_raro":"minerio_obtido",itemId,now,rare?2:1,4.2);
        }
        public bool UnlockAdvancement(string eventId,double now) { return Submit(eventId,"conquista","",now,3,6.4); }
        public bool Die(string eventId,double now)
        {
            now=Now(now);
            if(!Remember(eventId)) return false;
            crafting=false;pending.Clear();
            active=new Reaction { Event="morte",Item="",Created=now,Priority=4,Duration=6,Variant=Next("morte") };started=now;
            return true;
        }
        public void SetCrafting(bool open,double now)
        {
            now=Now(now);Advance(now);
            if(!open) { crafting=false; return; }
            if(active!=null && active.Priority==4) return;
            if(!crafting) { crafting=true;craftStart=now;craftVariant=Next("bancada_aberta"); }
        }
        public MinecraftFeedback Snapshot(PetKind kind,double now)
        {
            now=Now(now);Advance(now);
            string ev=active!=null ? active.Event : crafting ? "bancada_aberta" : "";
            int variant=active!=null ? active.Variant : craftVariant;
            string item=active!=null ? active.Item : "";
            string animation=ev=="morte" ? "Defeat" : ev=="conquista" ? "Victory" : ev=="minerio_raro" ? "Impressed" : ev=="minerio_obtido" ? "Goal" : crafting ? "Crafting" : "Idle";
            double seconds=active!=null ? now-started : crafting ? now-craftStart : 0;
            // Crafting speech appears once, not on every six-second drawing loop.
            string speech=ev=="bancada_aberta" && seconds>5 ? "" : MinecraftSpeech.Phrase(kind,ev,variant,item);
            return new MinecraftFeedback { Animation=animation,Event=ev,ItemId=item,Speech=speech,Seconds=seconds,Variant=variant,Reveal=animation=="Crafting"?Math.Min(1,seconds/.6):1 };
        }
        public void Reset()
        {
            active=null;pending.Clear();seen.Clear();seenOrder.Clear();variants.Clear();crafting=false;
            craftStart=started=lastNow=0;lastOre=lastRare=Double.NegativeInfinity;craftVariant=0;
        }
    }
}
