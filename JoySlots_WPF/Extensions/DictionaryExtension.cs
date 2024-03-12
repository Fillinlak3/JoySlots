using JoySlots_WPF;

public static class DictionaryExtension
{
    public static TValue Random<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, params TKey[] exclude)
    where TKey : notnull
    {
        // Create a random number generator
        Random rand = new Random();

        // Exclude specified keys from consideration
        HashSet<TKey> excludedKeys = new HashSet<TKey>(exclude);

        // Separate special symbols from regular symbols
        List<TKey> specialSymbols = new List<TKey>();
        List<TKey> regularSymbols = new List<TKey>();
        TKey? mostValuableSymbol = default;

        foreach (var kvp in dictionary)
        {
            if (!excludedKeys.Contains(kvp.Key))
            {
                if (kvp.Key.Equals("Iris") || kvp.Key.Equals("Jumi") || kvp.Key.Equals("Ali"))
                    specialSymbols.Add(kvp.Key);
                else if (kvp.Key.Equals("Robert"))
                    mostValuableSymbol = kvp.Key;
                else regularSymbols.Add(kvp.Key);
            }
        }

        // Check if special symbol chance is True.
        bool specialSymbolChance = rand.NextDouble() < App.GameSettings.SpecialSymbolChance && specialSymbols.Count > 0;
        bool mostValuableSymbolChance = rand.NextDouble() < App.GameSettings.MostValuableSymbolChance;

        // Choose a random default symbol.
        TKey selectedKey = regularSymbols[rand.Next(regularSymbols.Count)];

        if (mostValuableSymbolChance)
            selectedKey = mostValuableSymbol!;
        else if (specialSymbolChance)
            selectedKey = specialSymbols[rand.Next(specialSymbols.Count)];

        // Return the corresponding value
        return dictionary[selectedKey];
    }

}
