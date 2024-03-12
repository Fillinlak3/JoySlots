namespace JoySlots_WPF.Model
{
    public class GameSettings
    {
        public double SpecialSymbolChance { get; private set; } = 0.05f;
        public double MostValuableSymbolChance { get; private set; } = 0.15f;

        public Volume_State Volume { get; private set; } = Volume_State.Max;
        public bool CanSpin { get; set; } = false;
        public uint ReelsSpinningSpeed { get; set; } = 1;
        public uint ReelsStoppingSpeed { get; set; } = 10;

        public enum Volume_State
        {
            Max,
            Mid,
            Min,
            Mute
        }

        public GameSettings()
        { }

        public void SetVolume(Volume_State volume)
        {
            Volume = volume;
        }
    }
}
