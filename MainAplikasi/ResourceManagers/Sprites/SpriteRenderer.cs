using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MainAplikasi.ResourceManagers.Sprites.ObjectPooling;

namespace MainAplikasi.ResourceManagers.Sprites
{
    /// <summary>
    /// SpriteRenderer bertugas untuk mengambil sprite dari spritesheet dan merendernya ke UI.
    /// Menggunakan SpritePool untuk menghindari alokasi berulang pada WriteableBitmap.
    /// </summary>
    public class SpriteRenderer
    {
        private readonly SpritePool _spritePool;   // Pooling WriteableBitmap untuk hemat memori
        private readonly BitmapSource _spriteSheet; // Spritesheet sumber gambar animasi
        private readonly Image _targetImage; // Elemen UI tempat sprite akan dirender
        private int _spriteWidth, _spriteHeight; // Ukuran tiap sprite dalam spritesheet

        /// <summary>
        /// Constructor untuk SpriteRenderer.
        /// </summary>
        /// <param name="targetImage">Elemen UI (Image) yang akan menampilkan sprite.</param>
        /// <param name="spriteSheet">BitmapSource dari spritesheet yang digunakan.</param>
        /// <param name="spriteWidth">Lebar tiap sprite dalam spritesheet.</param>
        /// <param name="spriteHeight">Tinggi tiap sprite dalam spritesheet.</param>
        /// <param name="poolSize">Jumlah bitmap dalam pool untuk optimasi memori.</param>
        public SpriteRenderer(Image targetImage, BitmapSource spriteSheet, int spriteWidth, int spriteHeight, int poolSize)
        {
            _targetImage = targetImage;
            _spriteSheet = spriteSheet;
            _spriteWidth = spriteWidth;
            _spriteHeight = spriteHeight;
            _spritePool = new SpritePool(spriteWidth, spriteHeight, PixelFormats.Bgra32, poolSize);
        }

        /// <summary>
        /// Merender frame dari spritesheet berdasarkan koordinat x dan y.
        /// </summary>
        /// <param name="x">Koordinat X dari sprite dalam spritesheet.</param>
        /// <param name="y">Koordinat Y dari sprite dalam spritesheet.</param>
        public void RenderFrame(int x, int y)
        {
            // Ambil bitmap dari pool (reusable untuk menghindari alokasi baru)
            WriteableBitmap frame = _spritePool.GetBitmap();
            frame.Lock();

            // Definisikan area dari spritesheet yang akan diambil
            Int32Rect rect = new Int32Rect(x, y, _spriteWidth, _spriteHeight);

            // Salin piksel dari spritesheet ke bitmap yang diambil dari pool
            _spriteSheet.CopyPixels(rect, frame.BackBuffer, frame.BackBufferStride * frame.PixelHeight, frame.BackBufferStride);

            // Tandai area yang diperbarui agar WPF hanya merender bagian ini
            frame.AddDirtyRect(new Int32Rect(0, 0, _spriteWidth, _spriteHeight));
            frame.Unlock();

            // Set bitmap hasil render ke UI
            _targetImage.Source = frame;

            // Balikin bitmap ke pool supaya bisa dipakai ulang
            _spritePool.ReturnBitmap(frame);
        }
    }
}
