using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MainAplikasi.Models
{
    public enum FrameRate
    {
        FPS30,
        FPS60
    }

    public class LoadingSprite : INotifyPropertyChanged
    {
        private readonly WriteableBitmap spriteSheet;
        private readonly int frameWidth = 256;
        private readonly int frameHeight = 256;
        private readonly int totalFrames = 1;
        private int currentFrame = 0;
        private double rotationAngle = 0;

        private readonly WriteableBitmap[] frameBuffer;
        private readonly FrameRate frameRate;
        private int frameSkip = 0; // Untuk handling 30 FPS di layar 60 Hz

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

        public LoadingSprite(FrameRate fps = FrameRate.FPS60)
        {
            frameRate = fps;

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

            // Gunakan CompositionTarget.Rendering untuk sinkronisasi ke layar
            CompositionTarget.Rendering += UpdateFrame;
        }

        private void UpdateFrame(object sender, EventArgs e)
        {
            if (frameRate == FrameRate.FPS30 && frameSkip % 2 != 0) // Untuk 30 FPS, update setiap 2 frame rendering
            {
                frameSkip++;
                return;
            }

            if (currentFrame >= totalFrames)
                currentFrame = 0;

            SpriteFrame = frameBuffer[currentFrame];
            RotationAngle = (RotationAngle + 30) % 360;

            currentFrame++;
            OnPropertyChanged(nameof(SpriteFrame));

            frameSkip++;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
