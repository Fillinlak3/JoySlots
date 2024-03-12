using JoySlots_WPF.Extensions;
using System.Windows.Controls;
using System.Windows.Media;

namespace JoySlots_WPF.Model
{
    public class SlotLogic
    {
        public enum SymbolRarity
        {
            Common = 0,
            Rare,
            VeryRare,
            Wild
        }

        public readonly struct Symbol
        {
            public readonly string Name;
            public readonly ImageSource ImageSource;
            public readonly SymbolRarity Rarity;

            public Symbol(string name, ImageSource imageSource, SymbolRarity rarity)
            {
                Name = name;
                ImageSource = imageSource;
                Rarity = rarity;
            }
        }
        /// <summary>
        /// Assign all slot's symbols here.
        /// </summary>
        public List<Symbol> Symbols;

        public readonly struct SymbolLocation
        {
            public readonly int row;
            public readonly int column;

            public SymbolLocation(int row, int column)
            {
                this.row = row;
                this.column = column;
            }
        }

        /// <summary>
        /// This stores the map of the winning lines based on symbols location on the grid.
        /// </summary>
        private Dictionary<int, List<SymbolLocation>> MapWinningLines;

        public SlotLogic()
        {
            MapWinningLines = new Dictionary<int, List<SymbolLocation>>()
            {
                { 1, new List<SymbolLocation> { new SymbolLocation(1, 0), new SymbolLocation(1, 1),
                new SymbolLocation(1, 2), new SymbolLocation(1, 3), new SymbolLocation(1, 4)} },
                { 2, new List<SymbolLocation> { new SymbolLocation(0, 0), new SymbolLocation(0, 1),
                new SymbolLocation(0, 2), new SymbolLocation(0, 3), new SymbolLocation(0, 4)} },
                { 3, new List<SymbolLocation> { new SymbolLocation(2, 0), new SymbolLocation(2, 1),
                new SymbolLocation(2, 2), new SymbolLocation(2, 3), new SymbolLocation(2, 4)} },
                { 4, new List<SymbolLocation> { new SymbolLocation(0, 0), new SymbolLocation(1, 1),
                new SymbolLocation(2, 2), new SymbolLocation(1, 3), new SymbolLocation(0, 4)} },
                { 5, new List<SymbolLocation> { new SymbolLocation(2, 0), new SymbolLocation(1, 1),
                new SymbolLocation(0, 2), new SymbolLocation(1, 3), new SymbolLocation(2, 4)} },
                { 6, new List<SymbolLocation> { new SymbolLocation(0, 0), new SymbolLocation(0, 1),
                new SymbolLocation(1, 2), new SymbolLocation(2, 3), new SymbolLocation(2, 4)} },
                { 7, new List<SymbolLocation> { new SymbolLocation(2, 0), new SymbolLocation(2, 1),
                new SymbolLocation(1, 2), new SymbolLocation(0, 3), new SymbolLocation(0, 4)} },
                { 8, new List<SymbolLocation> { new SymbolLocation(1, 0), new SymbolLocation(2, 1),
                new SymbolLocation(2, 2), new SymbolLocation(2, 3), new SymbolLocation(1, 4)} },
                { 9, new List<SymbolLocation> { new SymbolLocation(1, 0), new SymbolLocation(0, 1),
                new SymbolLocation(0, 2), new SymbolLocation(0, 3), new SymbolLocation(1, 4)} },
                { 10, new List<SymbolLocation> { new SymbolLocation(0, 0), new SymbolLocation(1, 1),
                new SymbolLocation(1, 2), new SymbolLocation(1, 3), new SymbolLocation(0, 4)} }
            };

            Symbols = new List<Symbol>();
        }

        public Dictionary<int, List<SymbolLocation>> CheckForWinningLines(Grid ReelsGrid)
        {
            var winningLines = new Dictionary<int, List<SymbolLocation>>();

            /*
                Scatter Star
             */
            

            /*
                Scatter Dollar
             */

            return winningLines;
        }
    }
}
