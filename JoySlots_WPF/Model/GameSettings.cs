namespace JoySlots_WPF.Model
{
    public class GameSettings
    {
        public double WildSymbolChance { get; private set; } = 0.03f;
        public double ScatterSymbolChance { get; private set; } = 0.02f;
        public double VeryRareSymbolChance { get; private set; } = 0.10f;
        public double RareSymbolChance { get; private set; } = 0.35f;
        public double CommonSymbolChance { get; private set; } = 0.50f;

        public uint ReelsSpinningSpeed { get; set; } = 1;
        public uint ReelsStoppingSpeed { get; set; } = 10;

        public bool CanSpin { get; set; } = false;

        public enum Volume_State
        {
            Max,
            Mid,
            Min,
            Mute
        }
        public Volume_State Volume { get; private set; } = Volume_State.Max;

        public void SetVolume(Volume_State volume)
        {
            Volume = volume;
        }
    }
}
