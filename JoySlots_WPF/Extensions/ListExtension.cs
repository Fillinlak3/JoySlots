using JoySlots_WPF;
using JoySlots_WPF.Model;
using System.Windows.Media;

public static class ListExtension
{
    public static ImageSource Random(this List<Symbol> symbolsList, params string[] exclude)
    {
        Random rand = new Random();

        var remainingSymbols = symbolsList.Where(x => !exclude.Contains(x.Name)).ToList();
        var wildSymbol = remainingSymbols.FirstOrDefault(x => x.Rarity == Symbol.RarityTag.Wild);
        var scatterSymbols = remainingSymbols.Where(x => x.Rarity == Symbol.RarityTag.Scatter).ToList();
        var veryrareSymbol = remainingSymbols.FirstOrDefault(x => x.Rarity == Symbol.RarityTag.VeryRare);
        var rareSymbols = remainingSymbols.Where(x => x.Rarity == Symbol.RarityTag.Rare).ToList();
        var commonSymbols = remainingSymbols.Where(x => x.Rarity == Symbol.RarityTag.Common).ToList();

        if (remainingSymbols.Count == 0)
            throw new ArgumentNullException("List is null or all elements removed.");

        double symbolChanceGenerated = rand.NextDouble();
        Symbol symbol = commonSymbols[rand.Next(commonSymbols.Count)];

        bool wildSymbolChance = symbolChanceGenerated < App.GameSettings.WildSymbolChance &&
            wildSymbol is not null;
        bool scatterSymbolChance = symbolChanceGenerated < App.GameSettings.ScatterSymbolChance &&
            scatterSymbols.Count > 0;
        bool veryrareSymbolChance = symbolChanceGenerated < App.GameSettings.VeryRareSymbolChance &&
            veryrareSymbol is not null;
        bool rareSymbolChance = symbolChanceGenerated < App.GameSettings.RareSymbolChance &&
            rareSymbols.Count > 0;

        if (scatterSymbolChance)
            symbol = scatterSymbols[rand.Next(scatterSymbols.Count)];
        else if (wildSymbolChance)
            symbol = wildSymbol!;
        else if(veryrareSymbolChance)
            symbol = veryrareSymbol!;
        else if(rareSymbolChance)
            symbol = rareSymbols[rand.Next(rareSymbols.Count)];

        return symbol.ImageSource;
    }

}
