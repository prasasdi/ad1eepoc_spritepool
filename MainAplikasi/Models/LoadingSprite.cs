using MainAplikasi.Enums;
using MainAplikasi.ResourceManagers.Sprites.ObjectPooling;
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
        private double rotationAngle = 0;

        private readonly WriteableBitmap[] frameBuffer;
        private readonly E_FrameRate frameRate;
        private int frameSkip = 0; // Untuk handling 30 FPS di layar 60 Hz

        private readonly SpritePool spritePool; // Tambahkan SpritePool untuk pengelolaan memory bitmap

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

        /// <summary>
        /// Konstruktor untuk LoadingSprite. 
        /// Menggunakan SpritePool untuk mengelola frame agar lebih efisien dalam alokasi memori.
        /// </summary>
        /// <param name="fps">Frame rate animasi.</param>
        public LoadingSprite(E_FrameRate fps = E_FrameRate.FPS60)
        {
            frameRate = fps;

            // Inisialisasi SpritePool dengan kapasitas sesuai kebutuhan animasi
            spritePool = new SpritePool(frameWidth, frameHeight, PixelFormats.Bgra32, totalFrames);

            // Load sprite sheet sekali saja
            BitmapImage image = new BitmapImage(new Uri("pack://application:,,,/Assets/loading.png", UriKind.Absolute));
            spriteSheet = new WriteableBitmap(image);

            // Inisialisasi buffer frame
            frameBuffer = new WriteableBitmap[totalFrames];

            for (int i = 0; i < totalFrames; i++)
            {
                Int32Rect sourceRect = new Int32Rect(i * frameWidth, 0, frameWidth, frameHeight);

                // Ambil bitmap dari pool, kalau pool kosong, warning
                frameBuffer[i] = spritePool.GetBitmap();
                if (frameBuffer[i] == null)
                {
                    Console.WriteLine($"[WARNING] No available bitmaps in pool for frame {i}");
                    continue;
                }

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

        /// <summary>
        /// Fungsi untuk mengupdate frame animasi.
        /// Menggunakan SpritePool agar tidak terus-menerus membuat instance baru.
        /// </summary>
        private void UpdateFrame(object sender, EventArgs e)
        {
            if (frameRate == E_FrameRate.FPS30 && frameSkip % 2 != 0) // Untuk 30 FPS, update setiap 2 frame rendering
            {
                frameSkip++;
                return;
            }

            if (currentFrame >= totalFrames)
                currentFrame = 0;

            // Kembalikan frame sebelumnya ke pool sebelum mengganti ke frame baru
            spritePool.ReturnBitmap(SpriteFrame);

            // Ambil frame baru dari buffer
            SpriteFrame = frameBuffer[currentFrame];

            RotationAngle = (RotationAngle + 30) % 360;

            currentFrame++;
            OnPropertyChanged(nameof(SpriteFrame));

            frameSkip++;
        }

        /// <summary>
        /// Event untuk memberitahu perubahan properti ke UI.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
