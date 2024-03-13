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
        private double ValueFor_2 = 0f, ValueFor_3 = 0f, ValueFor_4 = 0f, ValueFor_5 = 0f;

        public Symbol(string name, ImageSource imageSource, RarityTag rarity)
        {
            Name = name;
            ImageSource = imageSource;
            this.Rarity = rarity;

            switch(name)
            {
                case "Robert":
                    ValueFor_2 = Math.Round(App.GameSettings.BetValue, 2);
                    ValueFor_3 = Math.Round(App.GameSettings.BetValue * 5, 2);
                    ValueFor_4 = Math.Round(App.GameSettings.BetValue * 25, 2);
                    ValueFor_5 = Math.Round(App.GameSettings.BetValue * 500, 2);
                    break;
                case "Bucurie":
                    ValueFor_3 = Math.Round(App.GameSettings.BetValue * 4, 2);
                    ValueFor_4 = Math.Round(App.GameSettings.BetValue * 12, 2);
                    ValueFor_5 = Math.Round(App.GameSettings.BetValue * 70, 2);
                    break;
                case "Teo":
                    ValueFor_3 = Math.Round(App.GameSettings.BetValue * 4, 2);
                    ValueFor_4 = Math.Round(App.GameSettings.BetValue * 12, 2);
                    ValueFor_5 = Math.Round(App.GameSettings.BetValue * 70, 2);
                    break;
                case "Rizea":
                    ValueFor_3 = Math.Round(App.GameSettings.BetValue * 2, 2);
                    ValueFor_4 = Math.Round(App.GameSettings.BetValue * 4, 2);
                    ValueFor_5 = Math.Round(App.GameSettings.BetValue * 20, 2);
                    break;
                case "Rusu":
                    ValueFor_3 = Math.Round(App.GameSettings.BetValue * 1, 2);
                    ValueFor_4 = Math.Round(App.GameSettings.BetValue * 3, 2);
                    ValueFor_5 = Math.Round(App.GameSettings.BetValue * 15, 2);
                    break;
                case "Catanoiu":
                    ValueFor_3 = Math.Round(App.GameSettings.BetValue * 1, 2);
                    ValueFor_4 = Math.Round(App.GameSettings.BetValue * 3, 2);
                    ValueFor_5 = Math.Round(App.GameSettings.BetValue * 15, 2);
                    break;
                case "Gabi":
                    ValueFor_3 = Math.Round(App.GameSettings.BetValue * 1, 2);
                    ValueFor_4 = Math.Round(App.GameSettings.BetValue * 3, 2);
                    ValueFor_5 = Math.Round(App.GameSettings.BetValue * 15, 2);
                    break;
                case "Petru":
                    ValueFor_3 = Math.Round(App.GameSettings.BetValue * 1, 2);
                    ValueFor_4 = Math.Round(App.GameSettings.BetValue * 3, 2);
                    ValueFor_5 = Math.Round(App.GameSettings.BetValue * 15, 2);
                    break;
                case "Jumi":
                    ValueFor_3 = Math.Round(App.GameSettings.BetValue * 20, 2);
                    break;
                case "Ali":
                    ValueFor_3 = Math.Round(App.GameSettings.BetValue * 5, 2);
                    ValueFor_4 = Math.Round(App.GameSettings.BetValue * 20, 2);
                    ValueFor_5 = Math.Round(App.GameSettings.BetValue * 100, 2);
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
                    return ValueFor_2;
                case 3:
                    return ValueFor_3;
                case 4:
                    return ValueFor_4;
                case 5:
                    return ValueFor_5;
                default:
                    return 0;
            }
        }
    }
}
