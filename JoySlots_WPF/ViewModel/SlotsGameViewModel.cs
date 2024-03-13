using JoySlots_WPF.Model;

namespace JoySlots_WPF.ViewModel
{
    public class SlotsGameViewModel : BaseViewModel
    {
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
        public Dictionary<int, List<SymbolLocation>> MapWinningLines { get; private set; }
        public List<Symbol> Symbols { get; set; }

        public SlotsGameViewModel()
        {
            MapWinningLines = new Dictionary<int, List<SymbolLocation>>()
            {
                { 1, new List<SymbolLocation> { new(1, 0), new(1, 1), new(1, 2), new(1, 3), new(1, 4)} },
                { 2, new List<SymbolLocation> { new(0, 0), new(0, 1), new(0, 2), new(0, 3), new(0, 4)} },
                { 3, new List<SymbolLocation> { new(2, 0), new(2, 1), new(2, 2), new(2, 3), new(2, 4)} },
                { 4, new List<SymbolLocation> { new(0, 0), new(1, 1), new(2, 2), new(1, 3), new(0, 4)} },
                { 5, new List<SymbolLocation> { new(2, 0), new(1, 1), new(0, 2), new(1, 3), new(2, 4)} },
                { 6, new List<SymbolLocation> { new(0, 0), new(0, 1), new(1, 2), new(2, 3), new(2, 4)} },
                { 7, new List<SymbolLocation> { new(2, 0), new(2, 1), new(1, 2), new(0, 3), new(0, 4)} },
                { 8, new List<SymbolLocation> { new(1, 0), new(2, 1), new(2, 2), new(2, 3), new(1, 4)} },
                { 9, new List<SymbolLocation> { new(1, 0), new(0, 1), new(0, 2), new(0, 3), new(1, 4)} },
                { 10, new List<SymbolLocation> { new(0, 0), new(1, 1), new(1, 2), new(1, 3), new(0, 4)} }
            };

            Symbols = new List<Symbol>();
        }
    }
}
