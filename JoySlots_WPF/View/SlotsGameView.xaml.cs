using JoySlots_WPF.Extensions;
using JoySlots_WPF.Model;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace JoySlots_WPF.View
{
    /// <summary>
    /// Interaction logic for SlotsGameView.xaml
    /// </summary>
    public partial class SlotsGameView : UserControl
    {
        private List<CancellationTokenSource> CancellationTokenSources;

        #region LoadingView
        public SlotsGameView()
        {
            InitializeComponent();

            CancellationTokenSources = new List<CancellationTokenSource>();
        }

        private void LoadView(object sender, RoutedEventArgs e)
        {
            foreach (var key in App.Current.Resources.MergedDictionaries[0].Keys)
            {
                // Get the resource from the merged resource dictionary
                var resource = App.Current.Resources.MergedDictionaries[0][key];

                // Check if the resource is of type BitmapImage
                if (resource is BitmapImage bitmapImage && key is string keyname &&
                    keyname.Contains("SY"))
                {
                    // Add to list
                    keyname = keyname.Remove(0, keyname.LastIndexOf("_") + 1);
                    Symbol.RarityTag rarity;
                    switch(keyname)
                    {
                        case "Iris":
                            rarity = Symbol.RarityTag.Wild;
                            break;
                        case "Jumi":
                            rarity = Symbol.RarityTag.Scatter;
                            break;
                        case "Ali":
                            rarity = Symbol.RarityTag.Scatter;
                            break;
                        case "Robert":
                            rarity = Symbol.RarityTag.VeryRare;
                            break;
                        case "Bucurie":
                            rarity = Symbol.RarityTag.Rare;
                            break;
                        case "Teo":
                            rarity = Symbol.RarityTag.Rare;
                            break;
                        default:
                            rarity = Symbol.RarityTag.Common;
                            break;
                    }
                    Game.Symbols.Add(new Symbol(keyname, bitmapImage, rarity));
                }
            }
            App.Logger.LogInfo("SlotsGameView/LoadView", "Gathered symbols from Resources.");

            for (int j = 0; j < ReelsGrid.ColumnDefinitions.Count; j += 2)
            {
                for (int i = 0; i < ReelsGrid.RowDefinitions.Count; i++)
                {
                    ReelsGrid.SetChild(i, j, new Image() { Source = Game.Symbols.Random("Iris", "Ali", "Jumi") });
                }
            }
            App.GameSettings.CanSpin = true;
            App.Logger.LogInfo("SlotsGameView/LoadView", "Loaded reels by default.");
        }

        private void UnloadView(object sender, RoutedEventArgs e)
        {
            App.GameSettings.CanSpin = false;
            CancellationTokenSources.Clear();
            Game.Symbols.Clear();
        }
        #endregion

        public async void SpinButton_Click(object sender, RoutedEventArgs e)
        {
            if (CancellationTokenSources.Count > 0 &&
            CancellationTokenSources.Any(t => t.IsCancellationRequested == false))
            {
                await Task.Delay(50);
                foreach (var tokenSource in CancellationTokenSources)
                    tokenSource.Cancel();

                CancellationTokenSources.Clear();
                await Task.Delay(100);
                return;
            }
            if (App.GameSettings.CanSpin == false) return;
            else App.GameSettings.CanSpin = false;

            Debug.WriteLine("[SPINNING]");
            await SpinReelsAsync();
            await CheckWin();
            Debug.WriteLine("[STOP]");
        }

        private async Task CheckWin()
        {
            Debug.WriteLine("<CHECKING Game WIN>");
            await Task.Delay(200);
            App.GameSettings.CanSpin = true;
            Debug.WriteLine("<ENDED checking>");
            App.Logger.LogInfo("SlotsGameView/CheckWin", "Game Win succeeded. CanSpin = True");
        }

        #region SpinReelsAnim
        private async Task SpinReelsAsync()
        {
            TimeSpan delay = TimeSpan.FromSeconds(0.2);
            TimeSpan delayRolling = TimeSpan.FromMilliseconds(App.PCInfo.FrameDuration *
                App.GameSettings.ReelsSpinningSpeed);
            TimeSpan delayOffset = TimeSpan.FromMilliseconds(App.PCInfo.FrameDuration *
                App.GameSettings.ReelsStoppingSpeed);

            // Load animation.
            for (int i = 0; i < 5; i++)
            {
                CancellationTokenSources.Add(new CancellationTokenSource());
                Task reel = new Task(async () => await
                SpinReelAsync(i * 2, CancellationTokenSources[i].Token, delayRolling));
                reel.RunSynchronously();
            }

            // Stop animation.
            try
            {
                await Task.Delay(delay, CancellationTokenSources[0].Token);
                for (int i = 0; i < 5 && CancellationTokenSources.Count > 0; i++)
                {
                    await Task.Delay(delayOffset);
                    CancellationTokenSources[i].Cancel();
                }
            }
            catch (Exception) { }
            finally
            {
                CancellationTokenSources.Clear();
            }
        }

        private async Task SpinReelAsync(int reel, CancellationToken cancellationToken, TimeSpan rollingDelay)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                ReelsGrid.SetChild(2, reel, new Image() { Source = (ReelsGrid.GetChild(1, reel) as Image)!.Source });
                ReelsGrid.SetChild(1, reel, new Image() { Source = (ReelsGrid.GetChild(0, reel) as Image)!.Source });

                // Reels 1 & 5 cant have special symbol IRIS
                if (reel / 2 == 0 || reel / 2 == 4)
                    ReelsGrid.SetChild(0, reel, new Image() { Source = Game.Symbols.Random("Iris") });
                // Reels 2 & 4 cant have special symbol JUMI
                else if (reel / 2 == 1 || reel / 2 == 3)
                    ReelsGrid.SetChild(0, reel, new Image() { Source = Game.Symbols.Random("Jumi") });
                // Any symbol is allowed.
                else
                    ReelsGrid.SetChild(0, reel, new Image() { Source = Game.Symbols.Random() });

                await Task.Delay(rollingDelay);
            }
        }

        // With delta time
        //private async Task SpinReelAsync(int reel, CancellationToken cancellationToken, TimeSpan rollingDelay)
        //{
        //    // Initialize variables for tracking elapsed time
        //    DateTime startTime = DateTime.Now;
        //    TimeSpan elapsedTime = TimeSpan.Zero;

        //    while (!cancellationToken.IsCancellationRequested)
        //    {
        //        // Calculate delta time
        //        TimeSpan deltaTime = DateTime.Now - startTime;
        //        elapsedTime += deltaTime;
        //        startTime = DateTime.Now;

        //        // Update animation based on delta time
        //        // For example, you can adjust the rollingDelay based on elapsed time
        //        double rollingDelayMilliseconds = rollingDelay.TotalMilliseconds - elapsedTime.TotalMilliseconds;
        //        if (rollingDelayMilliseconds < 0)
        //        {
        //            rollingDelayMilliseconds = 0;
        //            elapsedTime = TimeSpan.Zero; // Reset elapsed time for the next frame
        //        }

        //        // Perform animation tasks
        //        ReelsGrid.SetChild(2, reel, new Image() { Source = (ReelsGrid.GetChild(1, reel) as Image)!.Source });
        //        ReelsGrid.SetChild(1, reel, new Image() { Source = (ReelsGrid.GetChild(0, reel) as Image)!.Source });

        //        // Reels 1 & 5 cant have special symbol IRIS
        //        if (reel / 2 == 0 || reel / 2 == 4)
        //            ReelsGrid.SetChild(0, reel, new Image() { Source = Symbols.Random("Iris") });
        //        // Reels 2 & 4 cant have special symbol JUMI
        //        else if (reel / 2 == 1 || reel / 2 == 3)
        //            ReelsGrid.SetChild(0, reel, new Image() { Source = Symbols.Random("Jumi") });
        //        // Any symbol is allowed.
        //        else
        //            ReelsGrid.SetChild(0, reel, new Image() { Source = Symbols.Random() });

        //        // Delay using the adjusted rollingDelay
        //        await Task.Delay(TimeSpan.FromMilliseconds(rollingDelayMilliseconds));
        //    }
        //}
        #endregion
    }
}