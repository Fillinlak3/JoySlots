using JoySlots_WPF.Model;
using JoySlots_WPF.Services;
using System.Diagnostics;
using System.Windows;

namespace JoySlots_WPF
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

        private static Logger _logger { get; set; }
        public static Logger Logger { get => _logger; }

        private static PCInfo _pcInfo { get; set; }
        public static PCInfo PCInfo { get => _pcInfo; }

        static App()
        {
            #if DEBUG
            _logger = new Logger(true);
            #else
                _logger = new Logger(false);
            #endif

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
