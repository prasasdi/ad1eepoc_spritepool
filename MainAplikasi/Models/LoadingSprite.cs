using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace MainAplikasi.Models
{
    public class LoadingSprite : INotifyPropertyChanged
    {
        private readonly WriteableBitmap spriteSheet;
        private readonly int frameWidth = 256;
        private readonly int frameHeight = 256;
        private readonly int totalFrames = 1;
        private int currentFrame = 0;
        private readonly DispatcherTimer timer;
        private double rotationAngle = 0;

        private readonly WriteableBitmap[] frameBuffer; // Buffer frame

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
            // Load sprite sheet sekali saja
            BitmapImage image = new BitmapImage(new Uri("pack://application:,,,/Assets/loading.png", UriKind.Absolute));
            spriteSheet = new WriteableBitmap(image);

            // Inisialisasi buffer frame
            frameBuffer = new WriteableBitmap[totalFrames];

            for (int i = 0; i < totalFrames; i++)
            {
                Int32Rect sourceRect = new Int32Rect(i * frameWidth, 0, frameWidth, frameHeight);
                frameBuffer[i] = new WriteableBitmap(frameWidth, frameHeight, 96, 96, PixelFormats.Bgra32, null);

                int stride = (frameWidth * 32 + 7) / 8;
                byte[] pixelData = new byte[frameHeight * stride];
                spriteSheet.CopyPixels(sourceRect, pixelData, stride, 0);

                frameBuffer[i].WritePixels(new Int32Rect(0, 0, frameWidth, frameHeight), pixelData, stride, 0);
            }

            // Set frame awal
            SpriteFrame = frameBuffer[0];

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
            if (currentFrame >= totalFrames)
                currentFrame = 0;

            // Ganti referensi langsung tanpa salin pixel
            SpriteFrame = frameBuffer[currentFrame];

            // Efek rotasi
            RotationAngle = (RotationAngle + 30) % 360;

            currentFrame++;
            OnPropertyChanged(nameof(SpriteFrame));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
