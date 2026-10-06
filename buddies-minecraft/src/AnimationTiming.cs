namespace BuddyMinecraft
{
    // One full performance per event. The sprite player and native renderer share this contract.
    public static class AnimationTiming
    {
        public const int Frames = 96;
        public static double DesktopDuration(DesktopActivity activity) { return 0; }

        public static double Duration(PetMood mood)
        {
            switch (mood)
            {
                case PetMood.Goal: return 4.2;
                case PetMood.Conceded: return 4.8;
                case PetMood.Victory: return 6.4;
                case PetMood.Defeat: return 6.0;
                case PetMood.Scared: return 4.2;
                default: return 6.0;
            }
        }
    }
}
