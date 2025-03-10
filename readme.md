# **Kesimpulan Gw Sampai Sekarang**

Awalnya, gw mau bikin animasi loading di WPF tanpa GIF. Gw pilih sprite sheet karena ini teknik lama yang banyak dipakai di aplikasi desktop sebelum GIF bisa di-handle dengan baik. Supaya lebih rapi, gw pakai MVVM, jadi nggak ada code-behind.

Di awal implementasi, gw pakai **`CroppedBitmap`** buat motong frame dari sprite sheet. Animasi jalan, tapi masalahnya memori terus naik. Setelah dicek, ternyata tiap update frame bikin objek baru, dan itu bikin alokasi memori numpuk.

Gw coba ganti pendekatan pakai **`WriteableBitmap`**, biar bisa update gambar langsung tanpa bikin objek baru. Hasilnya lebih bagus, tapi tetap ada naik-turun di penggunaan memori. Setelah gw selidiki, ada beberapa penyebabnya:
- **GC masih jalan**, jadi naik turunnya karena alokasi kecil yang dibersihin.
- **WPF pakai Deferred Rendering**, jadi ada cache tambahan sebelum gambar ditampilkan.
- **`WritePixels()` tetap butuh buffer sementara**, jadi tetap ada sedikit alokasi tiap frame.

Terus, CPU juga kelihatan naik-turun di angka 1-2%. Setelah gw pikir-pikir, ini wajar karena tiap frame tetap butuh proses buat update tampilan. Biar lebih efisien, gw coba beberapa optimasi:
- **Set `RenderOptions.BitmapScalingMode="NearestNeighbor"`** biar nggak ada overhead dari smoothing.
- **Timer diubah ke `DispatcherPriority.Render`** biar pas dengan rendering WPF.
- **Pastikan `WriteableBitmap` di-reuse**, bukan bikin baru tiap frame.

Setelah optimasi ini, memori masih naik turun, tapi turun lagi setelah GC jalan. CPU juga tetap di 1-2%, yang masih masuk akal buat animasi ringan. **Intinya, performanya udah jauh lebih stabil dan nggak ada kebocoran memori.**

---

## **Update: Preload Gambar**  

Gw notice kalau dengan pendekatan sebelumnya memanfaatkan timer dan secara terus menerus men-generate gambar, ini makan resource, baik di memori maupun CPU. Maka dari itu, pendekatan selanjutnya adalah **melakukan preload gambar** sebelum animasi jalan.  

Dengan preload, gambar frame-frame animasi sudah ada di memori sejak awal, jadi waktu render nggak perlu alokasi tambahan. Harapannya, ini bisa ngurangin beban CPU dan bikin animasi lebih mulus.  

![Preload Gambar](gambar/preload1.PNG)

---

## **Update: Buffering Tambahan**  

Setelah preload gambar, gw nemu kalau masih ada sedikit spike di CPU dan memori, walaupun udah jauh lebih stabil. Solusinya, gw coba **buffering tambahan** buat nyiapin beberapa frame di depan sebelum ditampilkan.  

Cara kerjanya:  
1. **Load beberapa frame sekaligus ke buffer**, bukan cuma satu frame per tick.  
2. **Gunakan buffer yang di-reuse** buat nampilin animasi, jadi nggak ada alokasi baru tiap frame.  
3. **Optimasi cache WPF** dengan memastikan nggak ada re-render yang nggak perlu.  

Hasilnya? CPU usage lebih konsisten, dan memori lebih stabil tanpa spike yang aneh. Gw bakal terus lihat apakah ada optimasi lain yang bisa diterapin. 🚀
