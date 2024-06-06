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

        private static ResourceDictionary _images { get; set; }
        public static ResourceDictionary Images
        {
            get => _images;
        }

        private static ResourceDictionary _audio { get; set; }
        public static ResourceDictionary Audio
        {
            get => _audio;
        }

        private static ResourceDictionary _animations { get; set; }
        public static ResourceDictionary Animations
        {
            get => _animations;
        }

        private void InitializeResourceDictionaries()
        {
            _images = Application.Current.Resources.MergedDictionaries
                        .FirstOrDefault(d => d.Source != null && d.Source.ToString().EndsWith("Images.xaml"))!;

            _audio = Application.Current.Resources.MergedDictionaries
                        .FirstOrDefault(d => d.Source != null && d.Source.ToString().EndsWith("Audios.xaml"))!;

            _animations = Application.Current.Resources.MergedDictionaries
                        .FirstOrDefault(d => d.Source != null && d.Source.ToString().EndsWith("Animations.xaml"))!;
        }

        // Pragma disable _pcInfo unitialized warning.
#pragma warning disable CS8618
        static App()
        {
            /* 
                Need to do this to avoid MainWindow error not loading in the designer.
                It may seem ambiguous but i didn't find anything else to solve this bug.
                If it's working, then leave it as it is.. :)
             */
            _images = _audio = _animations = new ResourceDictionary();

            _gameSettings = new GameSettings();
            _currentPlayer = new Player();
        }
#pragma warning restore CS8618

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Logger.LogInfo("App", "Application Started");

            InitializeResourceDictionaries();

            this.MainWindow = new MainWindow();
            _pcInfo = new PCInfo(this.MainWindow);

            // Start the UI.
            this.MainWindow.Show();
        }
    }
}
