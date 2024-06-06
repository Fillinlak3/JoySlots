using JoySlots.Model;
using JoySlots.Services;
using Services.Logging;
using System.Windows;

namespace JoySlots
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static GameSettings _gameSettings { get; set; }
        public static GameSettings GameSettings { get => _gameSettings;}

        private static Player _currentPlayer { get; set; }
        public static Player Player { get => _currentPlayer; }

        private static PCInfo _pcInfo { get; set; }
        public static PCInfo PCInfo { get => _pcInfo; }

        static App()
        {
            _gameSettings = new GameSettings();
            _currentPlayer = new Player();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Logger.LogInfo("App", "Application Started");

            this.MainWindow = new MainWindow();
            _pcInfo = new PCInfo(this.MainWindow);

            // Start the UI.
            this.MainWindow.Show();
        }
    }
}
