using JoySlots_WPF.Extensions;
using JoySlots_WPF.Model;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WpfAnimatedGif;

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
                    var rarity = keyname switch
                    {
                        "Iris" => Symbol.RarityTag.Wild,
                        "Jumi" => Symbol.RarityTag.Scatter,
                        "Ali" => Symbol.RarityTag.Scatter,
                        "Robert" => Symbol.RarityTag.VeryRare,
                        "Bucurie" => Symbol.RarityTag.Rare,
                        "Teo" => Symbol.RarityTag.Rare,
                        _ => Symbol.RarityTag.Common,
                    };
                    Game.Symbols.Add(new Symbol(keyname, bitmapImage, rarity));
                }
            }
            App.Logger.LogInfo("SlotsGameView/LoadView", "Gathered symbols from Resources.");

            for (int j = 0; j < ReelsGrid.ColumnDefinitions.Count; j++)
            {
                for (int i = 0; i < ReelsGrid.RowDefinitions.Count; i++)
                {
                    ReelsGrid.GetChild(i, j)!.Source = Game.Symbols.Random("Iris", "Ali", "Jumi");
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

                // Delay when spamming the button with this.
                await Task.Delay(200);

                /*
                    -> When spinning reels and force stop, this is used to press again the spin to spin the reels.
                    -> But when it's animating the winning lines, when pressed again to spin, it will automatically start
                    spinning the reels.
                (it's animating the winning lines when it's just 1 cancellation token)
                 */
                if(App.GameSettings.BurningLinesAnimation == false)
                    return;
            }
            if (App.GameSettings.CanSpin == false) return;

            // Currently spinning.
            App.GameSettings.BurningLinesAnimation = false;
            App.GameSettings.CanSpin = false;
            Status_LB.Content = " MULT NOROC! ";

            // Remove all animations.
            if (ReelsGrid.Children.Count > 15)
                ReelsGrid.Children.RemoveRange(15, ReelsGrid.Children.Count);

            Debug.WriteLine("[SPINNING]");
            await SpinReelsAsync();
            Debug.WriteLine("[STOPPED SPINNING]");
            await CheckWin();
            Status_LB.Content = " FACEȚI CLICK PE ROTIRE PENTRU A JUCA ";
        }

        private async Task CheckWin()
        {
            Debug.WriteLine("<CHECKING Game WIN>");
            List<WinningLine> WinningLines = await Game.CheckWinningLines(ReelsGrid);
            Debug.WriteLine("<ENDED checking>");
            await Task.Delay(100);

            // Antimation BURNING the winning lines.
            if (WinningLines.Count > 0)
            {
                Debug.WriteLine("<Animation BURNING Winning Lines>");
                Status_LB.Content = string.Empty;
                App.GameSettings.BurningLinesAnimation = true;
                ImageSource burningLinesAnim = (this.FindResource("anim_BurningLines") as BitmapImage)!;
                foreach (var line in WinningLines)
                {
                    if (Game.MapWinningLines.ContainsKey(line.Line))
                    {
                        for (int i = 0; i < line.SymbolsCount; i++)
                        {
                            SymbolLocation symbolLocation = Game.MapWinningLines[line.Line][i];
                            ReelsGrid.SetChild(symbolLocation.row, symbolLocation.column, new Image(), true);
                            int index = ReelsGrid.Children.Count - 1;
                            Image animImage = (ReelsGrid.Children[index] as Image)!;
                            Grid.SetZIndex(animImage, 1);
                            ImageBehavior.SetAnimatedSource(animImage, burningLinesAnim);
                        }
                    }
                }

                CancellationTokenSources.Add(new CancellationTokenSource());
                // Animation BURN each winning line using the animated outline.
                await AnimateWinningLinesAsync(WinningLines, CancellationTokenSources.First().Token);
                WinningLines.Clear();
            }

            // Removed cuz it's better to be placed in the spinning button.
            // Task.Delay(200);
            App.GameSettings.CanSpin = true;
            App.Logger.LogInfo("SlotsGameView/CheckWin", "Game Win succeeded. CanSpin = True");
        }

        #region Animations
        private async Task AnimateWinningLinesAsync(List<WinningLine> WinningLines, CancellationToken cancellationToken)
        {
            ImageSource burningLinesAnim = (this.FindResource("anim_BurningLines") as BitmapImage)!;
            ImageSource burningLinesOutlinedAnim = (this.FindResource("anim_BurningLinesOutlined") as BitmapImage)!;
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    int index = 15;
                    foreach (var line in WinningLines)
                    {
                        if (cancellationToken.IsCancellationRequested) break;

                        Status_LB.Content = $" LINIA {line.Line}   CÂȘTIG {line.CashValue} LEI ";
                        if (Game.MapWinningLines.ContainsKey(line.Line))
                        {
                            for (int i = 0; i < line.SymbolsCount && !cancellationToken.IsCancellationRequested; i++, index++)
                            {
                                SymbolLocation symbolLocation = Game.MapWinningLines[line.Line][i];
                                Image animImage = (ReelsGrid.Children[index] as Image)!;
                                ImageBehavior.SetAnimatedSource(animImage, burningLinesOutlinedAnim);
                            }

                            await Task.Delay(2500, cancellationToken);
                            index -= line.SymbolsCount;

                            for (int i = 0; i < line.SymbolsCount && !cancellationToken.IsCancellationRequested; i++, index++)
                            {
                                SymbolLocation symbolLocation = Game.MapWinningLines[line.Line][i];
                                Image animImage = (ReelsGrid.Children[index] as Image)!;
                                ImageBehavior.SetAnimatedSource(animImage, burningLinesAnim);
                            }
                        }
                    }
                }
            }
            catch(Exception) { }
        }

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
                SpinReelAsync(i, CancellationTokenSources[i].Token, delayRolling));
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
            ImageSource Wild = Game.Symbols.FirstOrDefault(x => x.Name == "Iris")!.ImageSource;
            ImageSource ScatterStar = Game.Symbols.FirstOrDefault(x => x.Name == "Jumi")!.ImageSource;
            ImageSource ScatterDollar = Game.Symbols.FirstOrDefault(x => x.Name == "Ali")!.ImageSource;

            while (!cancellationToken.IsCancellationRequested)
            {
                Image TopImage = ReelsGrid.GetChild(0, reel)!;
                Image MiddleImage = ReelsGrid.GetChild(1, reel)!;
                Image BottomImage = ReelsGrid.GetChild(2, reel)!;

                BottomImage.Source = MiddleImage.Source;
                MiddleImage.Source = TopImage.Source;

                // Reels 1 & 5 cant have special symbol IRIS
                if (reel == 0 || reel == 4)
                    TopImage.Source = Game.Symbols.Random("Iris");
                // Reels 2 & 4 cant have special symbol JUMI
                else if (reel == 1 || reel == 3)
                    TopImage.Source = Game.Symbols.Random("Jumi");
                // Any symbol is allowed.
                else TopImage.Source = Game.Symbols.Random();

                /*
                    Avoid multiple symbols of type Wild / Scatter on the same reel.
                    Also avoid adding another `special symbol` if any already exists.
                */
                if (MiddleImage.Source == Wild || BottomImage.Source == Wild ||
                    MiddleImage.Source == ScatterStar || BottomImage.Source == ScatterStar ||
                    MiddleImage.Source == ScatterDollar || BottomImage.Source == ScatterDollar)
                    TopImage.Source = Game.Symbols.Random("Iris", "Jumi", "Ali");

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