using System.Windows;
using System.Windows.Controls;

namespace JoySlots_WPF.Extensions
{
    public static class GridExtension
    {
        public static Image? GetChild(this Grid grid, int row, int column)
        {
            foreach (Image child in grid.Children)
            {
                if (Grid.GetRow(child) == row && Grid.GetColumn(child) == column)
                {
                    return child;
                }
            }
            return null;
        }

        public static void SetChild(this Grid grid, int row, int column, UIElement element, bool forAnimation = false)
        {
            // Check if already exists and need to be replaced => delete old assign new.
            UIElement? _el = GetChild(grid, row, column);
            if (_el != null && forAnimation == false)
                grid.Children.Remove(_el);

            Grid.SetRow(element, row);
            Grid.SetColumn(element, column);
            grid.Children.Add(element);
        }

        public static void RemoveChild(this Grid grid, int row, int column)
        {
            // Find the UIElement at the specified row and column
            UIElement? element = GetChild(grid, row, column);

            // If the element exists, remove it from the grid
            if (element != null)
            {
                grid.Children.Remove(element);
            }
        }
    }
}
