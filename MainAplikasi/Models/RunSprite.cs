using MainAplikasi.Enums;
using MainAplikasi.ResourceManagers.Sprites.ObjectPooling;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MainAplikasi.Models
{
    public enum E_RunDirection
    {
        RunRight,
        RunLeft
    }

    public class RunSprite : INotifyPropertyChanged
    {
        private readonly WriteableBitmap spriteSheet;
        private readonly int frameWidth = 108;  // 864 / 8
        private readonly int frameHeight = 140; // 280 / 2
        private readonly int totalFrames = 8;   // Satu arah punya 8 frame
        private int currentFrame = 0;

        private readonly WriteableBitmap[] frameBuffer;
        private readonly E_FrameRate frameRate;
        private int frameSkip = 0; // Untuk handling FPS rendah di layar 60 Hz

        private readonly SpritePool spritePool;

        public WriteableBitmap SpriteFrame { get; private set; }

        public RunSprite(E_FrameRate fps = E_FrameRate.FPS12, E_RunDirection direction = E_RunDirection.RunRight)
        {
            frameRate = fps;
            spritePool = new SpritePool(frameWidth, frameHeight, PixelFormats.Bgra32, totalFrames);

            BitmapImage image = new BitmapImage(new Uri("pack://application:,,,/Assets/scottpilgrim_run.png", UriKind.Absolute));
            spriteSheet = new WriteableBitmap(image);

            frameBuffer = new WriteableBitmap[totalFrames];

            // Ambil sprite dari baris tertentu
            // karena left adalah invert dari right, ini ga perlu. Kecuali ada yang perlu ambil sprite berbeda
            int yOffset = (direction == E_RunDirection.RunLeft) ? 0 : 0; // RunRight di baris kedua

            for (int i = 0; i < totalFrames; i++)
            {
                Int32Rect sourceRect = new Int32Rect(i * frameWidth, yOffset, frameWidth, frameHeight);
                frameBuffer[i] = spritePool.GetBitmap() ?? new WriteableBitmap(frameWidth, frameHeight, 96, 96, PixelFormats.Bgra32, null);

                int stride = (frameWidth * 32 + 7) / 8;
                byte[] pixelData = new byte[frameHeight * stride];
                spriteSheet.CopyPixels(sourceRect, pixelData, stride, 0);

                if (direction == E_RunDirection.RunLeft)
                {
                    FlipHorizontal(pixelData, frameWidth, frameHeight, stride);
                }

                frameBuffer[i].WritePixels(new Int32Rect(0, 0, frameWidth, frameHeight), pixelData, stride, 0);
            }

            SpriteFrame = frameBuffer[0];
            CompositionTarget.Rendering += UpdateFrame;
        }

        private void UpdateFrame(object sender, EventArgs e)
        {
            if ((frameRate == E_FrameRate.FPS30 && frameSkip % 2 != 0) ||
                (frameRate == E_FrameRate.FPS12 && frameSkip % 5 != 0))
            {
                frameSkip++;
                return;
            }

            // Simpan frame sebelumnya
            var prevFrame = SpriteFrame;

            // Update ke frame berikutnya
            currentFrame = (currentFrame + 1) % totalFrames;
            SpriteFrame = frameBuffer[currentFrame];

            // Kembalikan frame sebelumnya ke pool jika masih dari pool
            if (prevFrame != null && spritePool.Contains(prevFrame))
            {
                spritePool.ReturnBitmap(prevFrame);
            }

            OnPropertyChanged(nameof(SpriteFrame));
            frameSkip++;
        }

        private void FlipHorizontal(byte[] pixelData, int width, int height, int stride)
        {
            int bytesPerPixel = (PixelFormats.Bgra32.BitsPerPixel + 7) / 8;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width / 2; x++) // Hanya sampai setengah, karena swapping
                {
                    int leftIndex = (y * stride) + (x * bytesPerPixel);
                    int rightIndex = (y * stride) + ((width - 1 - x) * bytesPerPixel);

                    for (int b = 0; b < bytesPerPixel; b++)
                    {
                        byte temp = pixelData[leftIndex + b];
                        pixelData[leftIndex + b] = pixelData[rightIndex + b];
                        pixelData[rightIndex + b] = temp;
                    }
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
