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

            // Inisialisasi frame kosong untuk menampung animasi
            SpriteFrame = new WriteableBitmap(frameWidth, frameHeight, 96, 96, PixelFormats.Bgra32, null);

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

            // Ambil data pixel dari sprite sheet
            Int32Rect sourceRect = new Int32Rect(currentFrame * frameWidth, 0, frameWidth, frameHeight);
            int stride = (frameWidth * 32 + 7) / 8;
            byte[] pixelData = new byte[frameHeight * stride];

            spriteSheet.CopyPixels(sourceRect, pixelData, stride, 0);

            // Update WriteableBitmap dengan pixel baru (tanpa alokasi baru)
            SpriteFrame.WritePixels(new Int32Rect(0, 0, frameWidth, frameHeight), pixelData, stride, 0);

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
