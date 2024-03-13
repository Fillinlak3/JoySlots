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

        public double CreditValue { get; private set; } = 0.01f;
        public double BetValue { get; private set; } = 0.20f;
        public bool CanSpin { get; set; } = false;

        public enum Volume_State
        {
            Max,
            Mid,
            Min,
            Mute
        }
        public Volume_State Volume { get; private set; } = Volume_State.Max;
        private double VolumeLevel = 1.0f;

        public void SetVolume(Volume_State volume)
        {
            Volume = volume;
            switch (volume)
            {
                case Volume_State.Max:
                    VolumeLevel = 1.0f;
                    break;
                case Volume_State.Mid:
                    VolumeLevel = 0.75f;
                    break;
                case Volume_State.Min:
                    VolumeLevel = 0.25f;
                    break;
                case Volume_State.Mute:
                    VolumeLevel = 0f;
                    break;
                default:
                    break;
            }
        }

        public void SetCreditValue(double creditValue)
        {
            CreditValue = creditValue;
        }

        public void SetBetValue(double betValue)
        {
            BetValue = betValue;
        }
    }
}
