# Kenapa Gue Pilih Teknik Ini 🖥️🎮

Repo ini kembangan dari [ad1eepoc_spriteanimation](https://github.com/prasasdi/ad1eepoc_spriteanimation). Kali ini, gue mau ngejar performa lebih ngebut buat sprite rendering! ⚡🔥

## Kenapa Nggak Pakai WPF Bawaan? 🤔
WPF bagus buat UI, tapi buat animasi sprite? Meh. Render-nya lambat, scaling kurang smooth, dan buffering-nya nggak optimal.

## Teknik yang Gue Pakai 🚀
✅ **System.Windows.Media.Imaging** – Masih pakai ini buat kelola bitmap, tapi dengan optimasi pooling biar nggak boros resource!  
✅ **Buffering Lebih Efisien** – Nggak bolak-balik load bitmap, jadi lebih cepat.  
✅ **Scaling Smooth** – Sprite lebih tajam, anti-aliasing mantap.  
✅ **Tetap di .NET** – Bisa nikmatin enaknya WPF tanpa sakit kepala performa.  

### Snippet Core Rendering 🎨
```csharp
private BitmapSource LoadBitmap(string path)
{
    BitmapImage bitmap = new BitmapImage();
    bitmap.BeginInit();
    bitmap.UriSource = new Uri(path, UriKind.RelativeOrAbsolute);
    bitmap.CacheOption = BitmapCacheOption.OnLoad;
    bitmap.EndInit();
    bitmap.Freeze(); // Biar thread-safe dan lebih optimal
    return bitmap;
}
```

## Teknologi yang Dipakai 🛠️
- **.NET 6** – Untuk apresiasi .NET 6 yang menjadi awalan karir gue di .NET, jadi gue pakai ini! 💙  
- **WPF** – Buat UI dan event handling.  
- **System.Windows.Media.Imaging** – Untuk optimasi rendering sprite dengan teknik pooling.  
- **C#** – Bahasa utama untuk logic dan rendering.  

## Cara Pakai 🚀
1. **Clone repo ini**
   ```sh
   git clone https://github.com/prasasdi/ad1eepoc_spritepool.git
   cd ad1eepoc_spritepool
   ```
2. **Buka di Visual Studio** (disarankan versi terbaru).  
   - Pastikan punya **.NET 6 SDK** terinstal.  
   - Buka `ad1eepoc_spritepool.sln`.  
3. **Jalankan aplikasi** dengan `F5` atau tombol *Run* di Visual Studio.  
4. **Enjoy! 🎮**

## Kesimpulan 🎯
Gue tetap pakai `System.Windows.Media.Imaging`, tapi dengan teknik pooling buat optimasi performa. Ke depan? Mungkin bakal coba Vulkan! 🚀🤘

