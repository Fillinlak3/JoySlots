//#define WAIT_MONEY_GROWING_ANIM
#define BYPASS_MONEY_GROWING_ANIM

using JoySlots.Extensions;
using JoySlots.Model;
using JoySlots.View.custom_controls;
using Services.Logging;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using WpfAnimatedGif;

namespace JoySlots.View
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
                        _ => Symbol.RarityTag.Common
                    };
                    Game.Symbols.Add(new Symbol(keyname, bitmapImage, rarity));
                }
            }
            Logger.LogInfo("SlotsGameView/LoadView", "Gathered symbols from Resources.");

            for (int j = 0; j < ReelsGrid.ColumnDefinitions.Count; j++)
            {
                for (int i = 0; i < ReelsGrid.RowDefinitions.Count; i++)
                {
                    ReelsGrid.GetChild(i, j)!.Source = Game.Symbols.Random("Iris", "Ali", "Jumi");
                }
            }

            BalanceCash_LB.Content = $"{App.Player.Balance:F2}";
            App.GameSettings.CanSpin = true;
            Logger.LogInfo("SlotsGameView/LoadView", "Loaded reels by default.");
        }

        private void UnloadView(object sender, RoutedEventArgs e)
        {
            App.GameSettings.CanSpin = false;
            CancellationTokenSources.Clear();
            Game.Symbols.Clear();
        }
        #endregion

        #region Buttons
        public async void SpinButton_Click(object sender, RoutedEventArgs e)
        {
#if WAIT_MONEY_GROWING_ANIM
            if (CancellationTokenSources.Count > 0 && CancellationTokenSources.Count <= 2 &&
                App.GameSettings.BurningLinesAnimation)
            {
                /*
                    The animations that can be are:
                    1. Money Growing for Winning. [1]
                    2. Burning Winning Lines. [0]
                    3. Money Growing for CashIn. [1]

                    <I>: If there are any money growing animations, it will need to stop them if space pressed. Then wait
                         for a second press of the spin btn to roll the reels.
                         Otherwise,stop the Burning Winning Lines Anim and forcefully spin the reels.
                 */
                if(App.GameSettings.MoneyGrowingAnimation)
                {
                    // Send the signal to cancel any Money Growing Animation.
                    CancellationTokenSources[1].Cancel();
                    return;
                }
                // If only the Burning Winning Lines Anim is active.
                else
                {
                    foreach (var cts in CancellationTokenSources)
                    {
                        cts.Cancel();
                    }
                    CancellationTokenSources.Clear();
                    await Task.Delay(200);
                }
            }
            else if (CancellationTokenSources.Count > 0 &&
                CancellationTokenSources.Any(t => t.IsCancellationRequested == false))
            {
                await Task.Delay(50);
                foreach (var tokenSource in CancellationTokenSources)
                    tokenSource.Cancel();
                CancellationTokenSources.Clear();

                // Delay when spamming the button with this.
                await Task.Delay(200);
                return;
            }
#elif BYPASS_MONEY_GROWING_ANIM
            if (CancellationTokenSources.Count > 0 &&
                CancellationTokenSources.Any(t => t.IsCancellationRequested == false))
            {
                Logger.LogInfo("SlotsGameView/SpinButton_Click", "Forcefully stopping all animations.");
                await Task.Delay(50);
                foreach (var tokenSource in CancellationTokenSources)
                    tokenSource.Cancel();
                CancellationTokenSources.Clear();

                Logger.Log("SlotsGameView/SpinButton_Click", "All animations were stopped.");
                /*
                    <!> Delay when spamming the button with this.
                    <!> Informative: When it's an animation this also waits for the CheckWin to terminate.
                    And by terminate means that CanSpin = true. So dont move.
                 */
                await Task.Delay(200);

                /*
                    This forces the reels to spin and bypass Money Growing & Winning Lines animations.
                    This asks for a second press only when you stop the reels spinning animation.
                 */
                if(App.GameSettings.BurningLinesAnimation == false)
                    return;
            }
#endif
            if (App.GameSettings.CanSpin == false) return;

            // Currently spinning.
            App.GameSettings.BurningLinesAnimation = false;
            App.GameSettings.CanSpin = false;
            CashInButton.Visibility = Visibility.Collapsed;
            Status_LB.Content = " MULT NOROC! ";
            if (LastWin_VB.Visibility == Visibility.Visible)
                LastWin_LB.Content = "ULTIMUL CÂȘTIG:";

            // Paid the bet for spinning.
            App.Player.Balance -= App.GameSettings.BetValue;
            BalanceCash_LB.Content = $"{App.Player.Balance:F2}";

            // Remove all animations.
            if (ReelsGrid.Children.Count > 15)
                ReelsGrid.Children.RemoveRange(15, ReelsGrid.Children.Count);

            Logger.LogInfo("SlotsGameView/SpinButton", "Reels currently spinning.");
            await SpinReelsAsync();
            Logger.Log("SlotsGameView/SpinButton", "Stopped spinning reels.");
            await CheckWin();
            Status_LB.Content = " FACEȚI CLICK PE ROTIRE PENTRU A JUCA ";
        }

        private async void CashInButton_Click(object sender, RoutedEventArgs e)
        {
            if (CancellationTokenSources[1].IsCancellationRequested == false)
            { CancellationTokenSources[1].Cancel(); await Task.Delay(10); }

            CancellationTokenSources[1] = new CancellationTokenSource();
            await AnimateMoneyGrowingToBalanceAsync(LastWinCash_LB, BalanceCash_LB, CancellationTokenSources[1].Token);

            CashInButton.Visibility = Visibility.Collapsed;
        }
        
        private void ChangeVolume(object sender, RoutedEventArgs e)
        {
            if (sender is null || sender is not Button)
                throw new Exception("Volume can be changed only from button press");

            if(Game.VolumeState == App.Current.Resources["img_VolumeMax"])
            {
                Game.VolumeState = (App.Current.Resources["img_VolumeMid"] as ImageSource)!;
                App.GameSettings.SetVolume(GameSettings.Volume_State.Mid);
            }
            else if (Game.VolumeState == App.Current.Resources["img_VolumeMid"])
            {
                Game.VolumeState = (App.Current.Resources["img_VolumeMin"] as ImageSource)!;
                App.GameSettings.SetVolume(GameSettings.Volume_State.Min);
            }
            else if (Game.VolumeState == App.Current.Resources["img_VolumeMin"])
            {
                Game.VolumeState = (App.Current.Resources["img_VolumeMute"] as ImageSource)!;
                App.GameSettings.SetVolume(GameSettings.Volume_State.Mute);
            }
            else if (Game.VolumeState == App.Current.Resources["img_VolumeMute"])
            {
                Game.VolumeState = (App.Current.Resources["img_VolumeMax"] as ImageSource)!;
                App.GameSettings.SetVolume(GameSettings.Volume_State.Max);
            }
        }
        
        public void SelectBetAmount(object sender, RoutedEventArgs e)
        {
            /*
                If the Bet Button is used as Spin Button to stop the animations and spin the reels.
             */
            if (App.GameSettings.CanSpin == false && App.GameSettings.BurningLinesAnimation == false)
            { SpinButton_Click(sender, e); return; }

            if (sender is not BetButton button || button is null)
                throw new ArgumentException("Invalid bet button tried to be accessed.");

            App.GameSettings.SetBetValue(button.Name switch
            {
                "BetButton_1" => Math.Round(App.GameSettings.CreditValue * 20, 2),
                "BetButton_2" => Math.Round(App.GameSettings.CreditValue * 50, 2),
                "BetButton_3" => Math.Round(App.GameSettings.CreditValue * 100, 2),
                "BetButton_4" => Math.Round(App.GameSettings.CreditValue * 300, 2),
                "BetButton_5" => Math.Round(App.GameSettings.CreditValue * 500, 2),
                _ => throw new Exception("Couldn't set bet value.")
            });

            BetButton_1.ClearBackgroundColor();
            BetButton_2.ClearBackgroundColor();
            BetButton_3.ClearBackgroundColor();
            BetButton_4.ClearBackgroundColor();
            BetButton_5.ClearBackgroundColor();
            button.BackgroundColor = new SolidColorBrush(Colors.Green);

            SpinButton_Click(sender, e);
        }
        #endregion

        #region Animations
        private void LoadBurningLinesAnimation(List<WinningLine> WinningLines, out double CashAmountWon)
        {
            ImageSource burningLinesAnim = (this.FindResource("anim_BurningLines") as BitmapImage)!;

            bool AddAnimationImage(SymbolLocation symbolLocation)
            {
                /*
                            BUG SOLVE:
                            Without this, if an animation was already placed on the same cell, it would place another
                            on top of it, so this prevents multiple images of animations on the same grid cell.
                         */
                if (ReelsGrid.GetChild(symbolLocation.row, symbolLocation.column, 1) is not null)
                    return false;

                ReelsGrid.SetChild(symbolLocation.row, symbolLocation.column, new Image(), true);
                int index = ReelsGrid.Children.Count - 1;
                Image animImage = (ReelsGrid.Children[index] as Image)!;
                Grid.SetZIndex(animImage, 1);
                ImageBehavior.SetAnimatedSource(animImage, burningLinesAnim);
                return true;
            }

            CashAmountWon = 0;
            foreach (var line in WinningLines)
            {
                for (int i = 0; i < line.SymbolsCount; i++)
                {
                    SymbolLocation symbolLocation;

                    // For symbols that were wired.
                    if (line.SymbolsLocation is null)
                        symbolLocation = Game.MapWinningLines[line.Line][i];
                    // For SCATTERS ONLY.
                    else symbolLocation = line.SymbolsLocation[i];

                    /*
                        Try adding the image where the animation will be displayed
                        over the winning symbol's image.
                     */
                    if (AddAnimationImage(symbolLocation) == false)
                        continue;
                }

                // Retrieve the winning line cash amount.
                CashAmountWon += line.CashValue;
            }
        }
        
        private async Task AnimateMoneyGrowingToBalanceAsync(Label From, Label Where, CancellationToken cancellationToken)
        {
            Logger.Log("SlotsGameView/AnimateMoneyGrowingToBalanceAsync", "Animation running.");
            App.GameSettings.MoneyGrowingAnimation = true;

            /*
                Trebuie sa IA banii de la From
                Trebuie sa PUNA banii la Where
                Cat timp ruleaza task ul animatiei
             */
            double startAmount = 0.1;
            double endAmount = Convert.ToDouble(From.Content);
            int numIncrements = 100; // Number of increments in the animation

            // Calculate the increment value based on total_winning.
            double incrementValue = (endAmount - startAmount) / numIncrements;

            try
            {
                for (int i = 0; i <= numIncrements && !cancellationToken.IsCancellationRequested; i++)
                {
                    From.Content = $"{endAmount}";
                    Where.Content = $"{App.Player.Balance}";

                    double sum = startAmount + i * incrementValue;
                    From.Content = $"{Convert.ToDouble(From.Content) - sum:F2}";
                    Where.Content = $"{Convert.ToDouble(Where.Content) + sum:F2}";
                    if (sum >= endAmount - 0.001)
                        break;
                    await Task.Delay(10, cancellationToken);
                }
            }
            catch(Exception) { }

            From.Content = $"{endAmount:F2}";
            Where.Content = $"{App.Player.Balance + endAmount:F2}";
            App.GameSettings.MoneyGrowingAnimation = false;
            Logger.Log("SlotsGameView/AnimateMoneyGrowingToBalanceAsync", "Animation stopped.");
        }

        private async Task AnimateMoneyGrowingToLastWinAsync(double From, Label Where, CancellationToken cancellationToken)
        {
            Logger.Log("SlotsGameView/AnimateMoneyGrowingToLastWinAsync", "Animation running.");
            App.GameSettings.MoneyGrowingAnimation = true;

            /*
                Trebuie sa IA banii de la From
                Trebuie sa PUNA banii la Where
                Cat timp ruleaza task ul animatiei
             */
            double startAmount = 0.1;
            double endAmount = From;
            int numIncrements = 100; // Number of increments in the animation

            // Calculate the increment value based on total_winning.
            double incrementValue = (endAmount - startAmount) / numIncrements;
            try
            {

                for (int i = 0; i <= numIncrements && !cancellationToken.IsCancellationRequested; i++)
                {
                    double sum = startAmount + i * incrementValue;
                    Where.Content = $"{sum:F2}";
                    if (sum >= endAmount - 0.001)
                        break;
                    await Task.Delay(10, cancellationToken);
                }
            }
            catch(Exception) { }

            Where.Content = $"{endAmount:F2}";
            App.GameSettings.MoneyGrowingAnimation = false;
            Logger.Log("SlotsGameView/AnimateMoneyGrowingToLastWinAsync", "Animation stopped.");
        }

        private async Task AnimateWinningLinesAsync(List<WinningLine> WinningLines, CancellationToken cancellationToken)
        {
            Logger.Log("SlotsGameView/AnimateWinningLinesAsync", "Animation running.");
            App.GameSettings.BurningLinesAnimation = true;
            ImageSource burningLinesAnim = (this.FindResource("anim_BurningLines") as BitmapImage)!;
            ImageSource burningLinesOutlinedAnim = (this.FindResource("anim_BurningLinesOutlined") as BitmapImage)!;
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    foreach (var line in WinningLines)
                    {
                        if (cancellationToken.IsCancellationRequested) break;

                        if(line.SymbolsLocation is null)
                            Status_LB.Content = $" LINIA {line.Line}   CÂȘTIG {line.CashValue} LEI ";
                        else Status_LB.Content = $" SCATTER   CÂȘTIG {line.CashValue} LEI ";

                        for (int i = 0; i < line.SymbolsCount && !cancellationToken.IsCancellationRequested; i++)
                        {
                            SymbolLocation symbolLocation;

                            // For symbols that were wired.
                            if (line.SymbolsLocation is null)
                                symbolLocation = Game.MapWinningLines[line.Line][i];
                            // For SCATTERS ONLY.
                            else symbolLocation = line.SymbolsLocation[i];

                            Image animImage = ReelsGrid.GetChild(symbolLocation.row, symbolLocation.column, 1)!;
                            ImageBehavior.SetAnimatedSource(animImage, burningLinesOutlinedAnim);
                        }

                        await Task.Delay(2500, cancellationToken);

                        for (int i = 0; i < line.SymbolsCount && !cancellationToken.IsCancellationRequested; i++)
                        {
                            SymbolLocation symbolLocation;

                            // For symbols that were wired.
                            if (line.SymbolsLocation is null)
                                symbolLocation = Game.MapWinningLines[line.Line][i];
                            // For SCATTERS ONLY.
                            else symbolLocation = line.SymbolsLocation[i];

                            Image animImage = ReelsGrid.GetChild(symbolLocation.row, symbolLocation.column, 1)!;
                            ImageBehavior.SetAnimatedSource(animImage, burningLinesAnim);
                        }
                    }
                }
            }
            catch(Exception) { }
            Logger.Log("SlotsGameView/AnimateWinningLinesAsync", "Animation stopped.");
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
                await Task.Delay(delayOffset, CancellationTokenSources[0].Token);
                for (int i = 0; i < 5 && CancellationTokenSources.Count > 0; i++)
                {
                    await Task.Delay(delayOffset);
                    CancellationTokenSources[i].Cancel();
                }
            }
            catch (Exception) { }
            finally
            {
                /*
                    Is recommended to not remove this line from here because what if the user doesn't force stop
                    the spinning reels? The animations cancellation token source will not be headed right. So better
                    to leave this here to clear the list after the reels animation is over. It doesn't matter whatsoever
                    if it's forced stopped or not.
                 */
                CancellationTokenSources.Clear();
            }
        }

        private async Task SpinReelAsync(int reel, CancellationToken cancellationToken, TimeSpan rollingDelay)
        {
            ImageSource Wild = Game.Symbols.FirstOrDefault(x => x.Name == "Iris")!.ImageSource;
            ImageSource ScatterStar = Game.Symbols.FirstOrDefault(x => x.Name == "Jumi")!.ImageSource;
            ImageSource ScatterDollar = Game.Symbols.FirstOrDefault(x => x.Name == "Ali")!.ImageSource;

            for (int i = 0; i < 3; i++)
                ReelsGrid.GetChild(i, reel)!.Effect = new BlurEffect { Radius = App.GameSettings.MotionBlurIntensity,
                KernelType = KernelType.Gaussian, RenderingBias = RenderingBias.Performance };

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

            for (int i = 0; i < 3; i++)
                ReelsGrid.GetChild(i, reel)!.Effect = null;
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
        
        private async Task CheckWin()
        {
            Logger.Log("SlotsGameView/CheckWin", "Check winning lines.");
            List<WinningLine> WinningLines = await Game.CheckWinningLines(ReelsGrid);
            await Task.Delay(100);

            // Setup, load and run the animations and set player's win amount.
            if (WinningLines.Count > 0)
            {
                Logger.Log("SlotsGameView/CheckWin", "Preparing the animations.");
                Status_LB.Content = string.Empty;

                // Setup for the Burning Lines Anim.
                double CashAmountWon;
                LoadBurningLinesAnimation(WinningLines, out CashAmountWon);

                // Setup for Money Growing Anim.
                LastWin_LB.Content = "   CÂȘTIG:  ";
                LastWinCash_LB.Content = "0.00";
                LastWin_VB.Visibility = Visibility.Visible;
                CashInButton.Visibility = Visibility.Visible;

                // Create a cancellation token for each animation task.
                CancellationTokenSources.Add(new CancellationTokenSource());
                CancellationTokenSources.Add(new CancellationTokenSource());

                // Launch the animations.
                Logger.Log("SlotsGameView/CheckWin", "Launching Money Growing Last Win & Winning Lines animations.");
                Task animateMoneyGrowing = new Task(async () =>
                await AnimateMoneyGrowingToLastWinAsync(CashAmountWon, LastWinCash_LB, CancellationTokenSources[1].Token));
                animateMoneyGrowing.RunSynchronously();
                await AnimateWinningLinesAsync(WinningLines, CancellationTokenSources[0].Token);

                // Don't forget to add the winning amount to player's balance.
                App.Player.Balance += CashAmountWon;
                WinningLines.Clear();
            }

            App.GameSettings.CanSpin = true;
            Logger.Log("SlotsGameView/CheckWin", "Game Win succeeded. CanSpin = True");
        }
    }
}