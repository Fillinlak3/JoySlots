﻿using System.Windows;
using System.Windows.Input;

namespace JoySlots_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            _lastWindowState = this.WindowState;
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                slotsGameView.SpinButton_Click(sender, e);
        }

        // Keep track of the state for restoration when needed.
        private WindowState _lastWindowState;
        private async void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Always restore to it's minimum size.
            if (this.WindowState == WindowState.Normal && _lastWindowState == WindowState.Maximized)
            {
                this.Width = this.MinWidth;
                this.Height = this.MinHeight;
                _lastWindowState = this.WindowState;
                return;
            }    


            double width = e.NewSize.Width;
            double height = e.NewSize.Height;

            // Calculate the desired aspect ratio
            double aspectRatio = 1920.0 / 1080.0; // Assuming 1920x1080 aspect ratio

            if (width / height > aspectRatio)
            {
                // Window is wider than the aspect ratio, adjust height
                double newHeight = width / aspectRatio;
                this.Height = newHeight;
            }
            else
            {
                // Window is taller than the aspect ratio, adjust width
                double newWidth = height * aspectRatio;
                this.Width = newWidth;
            }

            _lastWindowState = this.WindowState;
            await Task.Delay(16);
        }
    }
}