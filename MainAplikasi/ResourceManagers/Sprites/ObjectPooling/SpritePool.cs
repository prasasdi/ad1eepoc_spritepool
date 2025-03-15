using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace MainAplikasi.ResourceManagers.Sprites.ObjectPooling
{
    /// <summary>
    /// SpritePool digunakan untuk mengelola WriteableBitmap agar bisa digunakan ulang,
    /// mengurangi alokasi memori berulang, dan meningkatkan performa rendering sprite.
    /// </summary>
    public class SpritePool
    {
        private readonly Queue<WriteableBitmap> _pool;
        private readonly int _width, _height;
        private readonly PixelFormat _format;
        private readonly int _maxSize;

        /// <summary>
        /// Konstruktor untuk SpritePool. Membuat pool berisi `WriteableBitmap` dengan jumlah maksimum tertentu.
        /// </summary>
        /// <param name="width">Lebar bitmap.</param>
        /// <param name="height">Tinggi bitmap.</param>
        /// <param name="format">Format pixel untuk bitmap.</param>
        /// <param name="poolSize">Jumlah maksimum bitmap dalam pool.</param>
        public SpritePool(int width, int height, PixelFormat format, int poolSize)
        {
            _width = width;
            _height = height;
            _format = format;
            _maxSize = poolSize;
            _pool = new Queue<WriteableBitmap>(poolSize);

            // Inisialisasi pool dengan jumlah yang ditentukan
            for (int i = 0; i < poolSize; i++)
            {
                _pool.Enqueue(new WriteableBitmap(_width, _height, 96, 96, _format, null));
            }
        }

        /// <summary>
        /// Mengambil bitmap dari pool.
        /// Jika pool kosong, akan memberikan peringatan dan mengembalikan `null`.
        /// </summary>
        /// <returns>WriteableBitmap jika tersedia, null jika tidak.</returns>
        public WriteableBitmap GetBitmap()
        {
            if (_pool.Count > 0)
            {
                return _pool.Dequeue();
            }
            else
            {
                Console.WriteLine($"[WARNING] Pool limit reached ({_maxSize}), cannot allocate new bitmap.");
                return null; // Bisa diubah ke exception jika perlu
            }
        }

        /// <summary>
        /// Mengembalikan bitmap ke dalam pool untuk digunakan kembali.
        /// </summary>
        /// <param name="bitmap">Bitmap yang akan dikembalikan ke pool.</param>
        public void ReturnBitmap(WriteableBitmap bitmap)
        {
            if (_pool.Count < _maxSize)
            {
                _pool.Enqueue(bitmap);
            }
            else
            {
                Console.WriteLine($"[WARNING] Trying to return bitmap, but pool is full ({_maxSize}).");
            }
        }

        /// <summary>
        /// Mengecek jumlah bitmap yang masih tersedia dalam pool.
        /// </summary>
        /// <returns>Jumlah bitmap yang tersedia.</returns>
        public int AvailableCount()
        {
            return _pool.Count;
        }
    }
}
