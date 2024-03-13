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

        public Symbol(string name, ImageSource imageSource, RarityTag rarity)
        {
            Name = name;
            ImageSource = imageSource;
            this.Rarity = rarity;
        }
    }
}
