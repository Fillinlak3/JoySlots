using JoySlots_WPF.Model;
using System.Windows.Controls;
using JoySlots_WPF.Extensions;
using System.Windows.Media;

namespace JoySlots_WPF.ViewModel
{
    public class SlotsGameViewModel : BaseViewModel
    {
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

        public async Task<List<WinningLine>> CheckWinningLines(Grid ReelsGrid)
        {
            List<WinningLine> WinningLines = new List<WinningLine>();

            // Scatter Star
            ImageSource ScatterStar = Symbols.FirstOrDefault(x => x.Name == "Jumi")!.ImageSource;
            List<SymbolLocation> ScatterStarsLocation = new List<SymbolLocation>();
            int stars = 0;
            for (int reel = 0; reel < ReelsGrid.ColumnDefinitions.Count; reel += 2)
            {
                bool stillCounting = false;
                for (int i = 0; i < ReelsGrid.RowDefinitions.Count; i++)
                {
                    if (ReelsGrid.GetChild(i, reel)!.Source == ScatterStar)
                    {
                        ScatterStarsLocation.Add(new SymbolLocation(i, reel));
                        stillCounting = true;
                        stars++;
                    }

                    if (stillCounting == false) break;
                }
            }
            if (stars == 3)
            {
                WinningLines.Add(new WinningLine(stars,
                    Symbols.FirstOrDefault(x => x.Name == "Jumi")!, 0, ScatterStarsLocation));
            }
            ScatterStarsLocation.Clear();

            // Scatter Dollar
            ImageSource ScatterDollar = Symbols.FirstOrDefault(x => x.Name == "Ali")!.ImageSource;
            List<SymbolLocation> ScatterDollarsLocation = new List<SymbolLocation>();
            int dollars = 0;
            for (int reel = 0; reel < ReelsGrid.ColumnDefinitions.Count; reel++)
            {
                for (int i = 0; i < ReelsGrid.RowDefinitions.Count; i++)
                {
                    if (ReelsGrid.GetChild(i, reel)!.Source == ScatterDollar)
                    {
                        ScatterDollarsLocation.Add(new SymbolLocation(i, reel));
                        dollars++;
                    }
                }
            }
            if (dollars >= 3)
            {
                WinningLines.Add(new WinningLine(dollars,
                    Symbols.FirstOrDefault(x => x.Name == "Ali")!, 0, ScatterDollarsLocation));
            }
            ScatterDollarsLocation.Clear();

            // Wilds
            // int = the reel, bool = true => need to display anim.
            Dictionary<int, bool> reelsHavingWild = new Dictionary<int, bool>();
            ImageSource Wild = Symbols.FirstOrDefault(x => x.Name == "Iris")!.ImageSource;
            for (int reel = 1; reel < ReelsGrid.ColumnDefinitions.Count - 1; reel++)
            {
                for (int i = 0; i < ReelsGrid.RowDefinitions.Count; i++)
                {
                    if (ReelsGrid.GetChild(i, reel)!.Source == Wild && !reelsHavingWild.ContainsKey(reel))
                        reelsHavingWild.Add(reel, true);
                }
            }

            // Commons
            foreach (var line in MapWinningLines)
            {
                int symbolsCount = 1;
                ImageSource startingSymbol = ReelsGrid.GetChild(line.Value[0].row, line.Value[0].column)!.Source;

                // Avoid scatters.
                if (startingSymbol == ScatterStar || startingSymbol == ScatterDollar)
                    continue;

                for(int i = 1; i <  line.Value.Count; i++)
                {
                    // Get all symbols from the winning line. Wild is included as wiring for symbols.
                    ImageSource currentSymbol = ReelsGrid.GetChild(line.Value[i].row, line.Value[i].column)!.Source;
                    if (reelsHavingWild.ContainsKey(i) || currentSymbol == startingSymbol)
                    {
                        symbolsCount++;
                    }
                    else break;
                }

                if (symbolsCount >= 2)
                {
                    WinningLines.Add(new WinningLine(symbolsCount,
                        Symbols.FirstOrDefault(x => x.ImageSource == startingSymbol)!, line.Key));

                    // Check if it's a winning line.
                    if (WinningLines.Last().IsWinningLine == false)
                    // Filter just active winning lines having an amount won.
                    { WinningLines.Remove(WinningLines.Last()); continue; }

                    /*
                        If a wild was used to wire up a line, animate its appearance if wasn't done already.
                        Check if isn't already animated by the state of it's boolean value. (true = need to animate)
                        Animate the wilds only when it's a winning line.
                    */
                    for (int i = 1; i < symbolsCount && i < ReelsGrid.ColumnDefinitions.Count - 1; i++)
                    {
                        if (reelsHavingWild.ContainsKey(i) && reelsHavingWild[i])
                        {
                            reelsHavingWild[i] = false;
                            for (int j = 0; j < ReelsGrid.RowDefinitions.Count; j++)
                            {
                                Image symbol = ReelsGrid.GetChild(j, i)!;
                                if (symbol.Source != Wild)
                                {
                                    symbol.Source = Symbols.FirstOrDefault(x => x.Name == "Iris")!.ImageSource;
                                    await Task.Delay(300);
                                }
                            }
                        }
                    }
                }
            }
            reelsHavingWild.Clear();

            if (WinningLines.Count > 0)
            {
                string lineWon = string.Empty;
                string AmountWon = string.Empty;
                foreach (var line in WinningLines)
                {
                    lineWon += $"{line.Line}, ";
                    AmountWon += $"{line.CashValue}, ";
                }
                lineWon = lineWon.Remove(lineWon.Length - 2);
                AmountWon = AmountWon.Remove(AmountWon.Length - 2);
                App.Logger.LogInfo("SlotsGameViewModel/CheckWinningLines", $"Winning lines\nCount={WinningLines.Count}" +
                    $"\nLines={lineWon}\nAmountWon={AmountWon}");
            }

            return WinningLines;
        }
    }
}
