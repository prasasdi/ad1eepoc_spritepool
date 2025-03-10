using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace MainAplikasi.Models
{
    public class LoadingSprite : INotifyPropertyChanged
    {
        private readonly List<WriteableBitmap> frames = new();
        private readonly int frameWidth = 256;
        private readonly int frameHeight = 256;
        private readonly int totalFrames = 1;
        private int currentFrame = 0;
        private readonly DispatcherTimer timer;
        private double rotationAngle = 0;

        public WriteableBitmap SpriteFrame { get; private set; }
        public double RotationAngle
        {
            get => rotationAngle;
            set
            {
                rotationAngle = value;
                OnPropertyChanged(nameof(RotationAngle));
            }
        }

        public LoadingSprite()
        {
            // Load sprite sheet
            BitmapImage image = new BitmapImage(new Uri("pack://application:,,,/Assets/loading.png", UriKind.Absolute));
            WriteableBitmap spriteSheet = new WriteableBitmap(image);

            // Preload semua frame ke dalam list
            for (int i = 0; i < totalFrames; i++)
            {
                Int32Rect sourceRect = new Int32Rect(i * frameWidth, 0, frameWidth, frameHeight);
                WriteableBitmap frame = new WriteableBitmap(frameWidth, frameHeight, 96, 96, PixelFormats.Bgra32, null);
                byte[] pixelData = new byte[frameHeight * ((frameWidth * 32 + 7) / 8)];
                spriteSheet.CopyPixels(sourceRect, pixelData, (frameWidth * 32 + 7) / 8, 0);
                frame.WritePixels(new Int32Rect(0, 0, frameWidth, frameHeight), pixelData, (frameWidth * 32 + 7) / 8, 0);
                frames.Add(frame);
            }

            // Set frame awal
            SpriteFrame = frames[0];

            // Timer update animasi
            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(100)
            };
            timer.Tick += UpdateFrame;
            timer.Start();
        }

        private void UpdateFrame(object sender, EventArgs e)
        {
            currentFrame = (currentFrame + 1) % totalFrames;
            SpriteFrame = frames[currentFrame];

            // Update rotasi
            RotationAngle = (RotationAngle + 30) % 360;

            OnPropertyChanged(nameof(SpriteFrame));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
