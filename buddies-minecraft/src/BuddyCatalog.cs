using System;

namespace BuddyMinecraft
{
    // Preserve the original IDs: saved preferences and the sprite protocol use these names.
    public enum PetKind { Astro = 0, Mochi = 1, Bolt = 2, Drako = 3, Kitsu = 4, Nimbo = 5, Marina = 6 }
    public enum PetMood { Idle, Goal, Conceded, Victory, Defeat, Scared }

    public static class BuddyCatalog
    {
        public static readonly string[] Names = { "Astro", "Mochi", "Bolt", "Drako", "Kitsu", "Nimbo", "Marina" };
        public static readonly string[] Colors = { "#BCA4FF", "#8FE4C4", "#FFD083", "#D8ACFF", "#FFB589", "#A6DAFF", "#F2A6DE" };
        public static readonly string[] Descriptions = {
            "Seu pequeno explorador de grandes partidas.", "Nove vidas. Uma torcida inteira por você.",
            "Carregado de energia. Programado para torcer.", "Seu pequeno caos. Fogo na vitória.",
            "Três caudas. Mil truques. Uma dupla imbatível.", "Pode vir a tempestade. A gente abre o céu.",
            "Oito braços e uma batida só: a da sua vitória."
        };

        public static bool IsPremium(PetKind kind) { return kind >= PetKind.Drako && kind <= PetKind.Marina; }
        public static bool HasPremiumArt(PetKind kind) { return kind >= PetKind.Kitsu && kind <= PetKind.Marina; }
        public static string Slug(PetKind kind) { return Names[(int)kind].ToLowerInvariant(); }

    }
}
