using Serilog;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SysDVRClientGUI.ViewLogic
{
    internal class PictureboxBounceAnimation : IDisposable
    {
        private readonly PictureBox animatedPbx;
        private bool isAnimationRunning = false;
        private CancellationTokenSource animationCts;
        private readonly System.Timers.Timer randomStartTimer;
        private short nextAnimationStart = 16;
        private const string animationTag = "bounceAnimatedPicturebox";

        #region Constructor
        public PictureboxBounceAnimation(PictureBox pictureBox)
        {
            if (pictureBox == null || pictureBox.Image == null)
            {
                throw new ArgumentNullException(nameof(pictureBox), "Empty picturebox provided");
            }

            if (pictureBox.Tag != null && (pictureBox.Tag as string).Contains(animationTag))
            {
                throw new ArgumentException("Picturebox is already animated", nameof(pictureBox));
            }

            this.animatedPbx = pictureBox;

            if (this.animatedPbx.Tag == null)
            {
                this.animatedPbx.Tag = animationTag;
            }
            else
            {
                this.animatedPbx.Tag += $";{animationTag}";
            }

            this.randomStartTimer = new()
            {
                Interval = 1000
            };
            this.randomStartTimer.Elapsed += this.RandomStartTimer_Tick;
            this.randomStartTimer.Start();
        }
        #endregion

        private void RandomStartTimer_Tick(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.nextAnimationStart--;

            if (this.nextAnimationStart <= 0)
            {
                this.Start();

                Random rnd = new();
                this.nextAnimationStart = (short)(10 + rnd.Next(4, 24));
                Log.Debug($"Next animation in {this.nextAnimationStart}");
            }
        }

        /// <summary>
        /// Creates and starts a thread for the logo bounce animation
        /// </summary>
        private void LogoBounceAnimationEngine()
        {
            Task.Factory.StartNew(async () =>
            {
                Thread.CurrentThread.Name = "Logo Bounce Animation";
                Stopwatch s = new();
                Log.Debug($"Logo Bounce Animation started...");
                s.Start();

                Point currentLocation = new(this.animatedPbx.Location.X, this.animatedPbx.Location.Y);
                int bounceToY = Convert.ToInt32(Math.Ceiling(this.animatedPbx.Height / (float)4));
                int startY = this.animatedPbx.Location.Y;
                bool direction = true;
                TimeSpan animationDelay = new(0, 0, 0, 0, 1, 200);

                async Task WaitAndSetAsync()
                {
                    await Task.Delay(animationDelay);
                    this.animatedPbx.Invoke(new Action(() => this.animatedPbx.Location = currentLocation));
                }

                async Task DoBounce()
                {
                    while (direction && currentLocation.Y != startY - bounceToY)
                    {
                        currentLocation.Y--;
                        await WaitAndSetAsync();
                    }

                    direction ^= true;

                    while (!direction && currentLocation.Y != startY)
                    {
                        currentLocation.Y++;
                        await WaitAndSetAsync();
                    }

                    animationDelay = new(0,0,0,0,0, animationDelay.Microseconds + 380);
                }

                while (bounceToY >= 1)
                {
                    await DoBounce();
                    bounceToY--;
                }

                s.Stop();
                Log.Debug($"Logo Bounce Animation finished within \"{s.Elapsed:ss\\:ffffff}\"");
                this.isAnimationRunning = false;
            }, this.animationCts.Token);
        }

        /// <summary>
        /// Starts a new animation if theres no ongoing animation.
        /// </summary>
        public void Start()
        {
            if (this.isAnimationRunning)
            {
                return;
            }

            this.isAnimationRunning = true;
            this.animationCts = new();

            this.LogoBounceAnimationEngine();
        }

        /// <summary>
        /// Stops the animation
        /// </summary>
        public void Stop()
        {
            this.animationCts?.Cancel();
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.animatedPbx != null && this.animatedPbx.Tag is string tag)
            {
                this.animatedPbx.Tag = tag.Replace(animationTag, "").TrimEnd(';');
            }
        }
    }
}
