namespace JoySlots_WPF.Model
{
    public readonly struct WinningLine
    {
        public readonly int SymbolsCount = 0;
        public readonly Symbol Symbol;
        public readonly int Line = 0;
        public readonly double CashValue = 0;
        public readonly List<SymbolLocation>? SymbolsLocation;
        public readonly bool IsWinningLine = true;

        public WinningLine(int symbolsCount, Symbol symbol, int line, List<SymbolLocation>? symbolsLocation = null)
        {
            SymbolsCount = symbolsCount;
            Symbol = symbol;
            Line = line;
            CashValue = symbol.GetValue(symbolsCount);
            SymbolsLocation = symbolsLocation;

            if (CashValue == 0) IsWinningLine = false;
        }
    }
}
