namespace JoySlots.Model
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
}
