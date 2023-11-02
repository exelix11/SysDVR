using Serilog;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SysDVRClientGUI.ViewLogic
{
    internal class PictureboxSlideAnimation : IDisposable
    {
        private readonly PictureBox animatedPbx;
        private bool isAnimationRunning = false;
        private CancellationTokenSource animationCts;
        private const string animationTag = "slideAnimatedPicturebox";

        public enum Directions
        {
            TopToBottom,
            BottomToTop
        }

        public Directions CurrentDirection { get; private set; }

        #region Constructor
        public PictureboxSlideAnimation(PictureBox pictureBox)
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
        }
        #endregion

        /// <summary>
        /// Creates and starts a thread for the logo slide animation
        /// </summary>
        private void LogoSlideAnimationEngine()
        {
            Task.Factory.StartNew(async () =>
            {
                Thread.CurrentThread.Name = "Logo Slide Animation";
                Stopwatch s = new();
                Log.Debug($"Logo Slide Animation {this.CurrentDirection} started...");
                s.Start();

                Point currentLocation = default;
                int yStartLocation = this.animatedPbx.Location.Y;

                async Task WaitAndSetAsync()
                {
                    await Task.Delay(5);
                    this.animatedPbx.Invoke(new Action(() => this.animatedPbx.Location = currentLocation));
                }

                if (this.CurrentDirection == Directions.TopToBottom)
                {
                    currentLocation = new(this.animatedPbx.Location.X, this.animatedPbx.Location.Y - this.animatedPbx.Size.Height);
                    while (currentLocation.Y <= 3 && !this.animationCts.IsCancellationRequested)
                    {
                        currentLocation.Y++;
                        await WaitAndSetAsync();
                    }
                }
                else
                {
                    currentLocation = new(this.animatedPbx.Location.X, this.animatedPbx.Location.Y);
                    while (currentLocation.Y != yStartLocation - this.animatedPbx.Size.Height && !this.animationCts.IsCancellationRequested)
                    {
                        currentLocation.Y--;
                        await WaitAndSetAsync();
                    }
                }

                s.Stop();
                Log.Debug($"Logo Slide Animation {this.CurrentDirection} finished within \"{s.Elapsed:ss\\:ffffff}\"");
                this.isAnimationRunning = false;
            }, this.animationCts.Token);
        }

        /// <summary>
        /// Starts a new animation if theres no ongoing animation.
        /// </summary>
        /// <param name="direction"></param>
        public void Start(Directions direction = Directions.TopToBottom)
        {
            if (this.isAnimationRunning)
            {
                return;
            }

            this.isAnimationRunning = true;
            this.animationCts = new();
            this.CurrentDirection = direction;

            this.LogoSlideAnimationEngine();
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
