using System.Windows.Media;

namespace JoySlots_WPF.Model
{
    public class Symbol
    {
        public enum RarityTag
        {
            Common = 0,
            Rare,
            VeryRare,
            Scatter,
            Wild
        }

        public readonly string Name;
        public readonly ImageSource ImageSource;
        public readonly RarityTag Rarity;
        private readonly double ValueFor_2_Multiplier = 0f, ValueFor_3_Multiplier = 0f;
        private readonly double ValueFor_4_Multiplier = 0f, ValueFor_5_Multiplier = 0f;

        public Symbol(string name, ImageSource imageSource, RarityTag rarity)
        {
            Name = name;
            ImageSource = imageSource;
            this.Rarity = rarity;

            switch(name)
            {
                case "Robert":
                    ValueFor_2_Multiplier = 1;
                    ValueFor_3_Multiplier = 5;
                    ValueFor_4_Multiplier = 25;
                    ValueFor_5_Multiplier = 500;
                    break;
                case "Bucurie":
                    ValueFor_3_Multiplier = 4;
                    ValueFor_4_Multiplier = 12;
                    ValueFor_5_Multiplier = 70;
                    break;
                case "Teo":
                    ValueFor_3_Multiplier = 4;
                    ValueFor_4_Multiplier = 12;
                    ValueFor_5_Multiplier = 70;
                    break;
                case "Rizea":
                    ValueFor_3_Multiplier = 2;
                    ValueFor_4_Multiplier = 4;
                    ValueFor_5_Multiplier = 20;
                    break;
                case "Rusu":
                    ValueFor_3_Multiplier = 1;
                    ValueFor_4_Multiplier = 3;
                    ValueFor_5_Multiplier = 15;
                    break;
                case "Catanoiu":
                    ValueFor_3_Multiplier = 1;
                    ValueFor_4_Multiplier = 3;
                    ValueFor_5_Multiplier = 15;
                    break;
                case "Gabi":
                    ValueFor_3_Multiplier = 1;
                    ValueFor_4_Multiplier = 3;
                    ValueFor_5_Multiplier = 15;
                    break;
                case "Petru":
                    ValueFor_3_Multiplier = 1;
                    ValueFor_4_Multiplier = 3;
                    ValueFor_5_Multiplier = 15;
                    break;
                case "Jumi":
                    ValueFor_3_Multiplier = 20;
                    break;
                case "Ali":
                    ValueFor_3_Multiplier = 5;
                    ValueFor_4_Multiplier = 20;
                    ValueFor_5_Multiplier = 100;
                    break;
                default:
                    break;
            }
        }

        public double GetValue(int symbolsCount)
        {
            switch (symbolsCount)
            {
                case 2:
                    return Math.Round(ValueFor_2_Multiplier * App.GameSettings.BetValue, 2);
                case 3:
                    return Math.Round(ValueFor_3_Multiplier * App.GameSettings.BetValue, 2);
                case 4:
                    return Math.Round(ValueFor_4_Multiplier * App.GameSettings.BetValue, 2);
                case 5:
                    return Math.Round(ValueFor_5_Multiplier * App.GameSettings.BetValue, 2);
                default:
                    return 0;
            }
        }
    }
}
