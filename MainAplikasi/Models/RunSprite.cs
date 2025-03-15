using MainAplikasi.ResourceManagers.Sprites.ObjectPooling;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MainAplikasi.Models
{
    public class RunSprite : INotifyPropertyChanged
    {
        private readonly WriteableBitmap spriteSheet;
        private readonly int frameWidth = 108;  // 864 / 8
        private readonly int frameHeight = 140; // 280 / 2
        private readonly int totalFrames = 8;   // Animasi ke kanan (bisa 16 kalau termasuk ke kiri)
        private int currentFrame = 0;

        private readonly WriteableBitmap[] frameBuffer;
        private readonly FrameRate frameRate;
        private int frameSkip = 0; // Untuk handling 30 FPS di layar 60 Hz

        private readonly SpritePool spritePool;

        public WriteableBitmap SpriteFrame { get; private set; }

        public RunSprite(FrameRate fps = FrameRate.FPS60)
        {
            frameRate = fps;

            // SpritePool dengan jumlah frame animasi
            spritePool = new SpritePool(frameWidth, frameHeight, PixelFormats.Bgra32, totalFrames);

            // Load sprite sheet sekali saja
            BitmapImage image = new BitmapImage(new Uri("pack://application:,,,/Assets/scottpilgrim_run.png", UriKind.Absolute));
            spriteSheet = new WriteableBitmap(image);

            // Inisialisasi buffer frame
            frameBuffer = new WriteableBitmap[totalFrames];

            for (int i = 0; i < totalFrames; i++)
            {
                Int32Rect sourceRect = new Int32Rect(i * frameWidth, 0, frameWidth, frameHeight);

                // Ambil bitmap dari pool, fallback ke instance baru kalau pool habis
                frameBuffer[i] = spritePool.GetBitmap() ?? new WriteableBitmap(frameWidth, frameHeight, 96, 96, PixelFormats.Bgra32, null);

                int stride = (frameWidth * 32 + 7) / 8;
                byte[] pixelData = new byte[frameHeight * stride];
                spriteSheet.CopyPixels(sourceRect, pixelData, stride, 0);

                frameBuffer[i].WritePixels(new Int32Rect(0, 0, frameWidth, frameHeight), pixelData, stride, 0);
            }

            // Set frame awal
            SpriteFrame = frameBuffer[0];

            // Gunakan CompositionTarget.Rendering untuk animasi
            CompositionTarget.Rendering += UpdateFrame;
        }

        private void UpdateFrame(object sender, EventArgs e)
        {
            if (frameRate == FrameRate.FPS30 && frameSkip % 2 != 0)
            {
                frameSkip++;
                return;
            }

            // Simpan frame sebelumnya
            var prevFrame = SpriteFrame;

            // Update ke frame berikutnya
            currentFrame = (currentFrame + 1) % totalFrames;
            SpriteFrame = frameBuffer[currentFrame];

            // Kembalikan frame sebelumnya ke pool **hanya kalau masih dari pool**
            if (prevFrame != null && spritePool.Contains(prevFrame))
            {
                spritePool.ReturnBitmap(prevFrame);
            }

            OnPropertyChanged(nameof(SpriteFrame));
            frameSkip++;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
