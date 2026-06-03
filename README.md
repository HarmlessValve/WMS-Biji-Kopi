# CoffeeWMS

## Cara Menambahkan Menu di Sidebar / Dashboard
Menu aplikasi pada CoffeeWMS dibuat secara dinamis menggunakan C# murni tanpa Visual Studio Designer. Jika Anda ingin menambahkan tombol menu baru, ikuti panduan berikut:

1. Buka file `Views/MainForm.cs`.
2. Cari metode `InitializeMenus()`.
3. Gunakan metode `AddMenuItem()` untuk menambahkan tombol baru.
4. Tambahkan `currentTop += 40;` di baris selanjutnya untuk memberi jarak (margin) ke bawah.

**Contoh Penulisan:**
```csharp
// Menambahkan menu Stok Barang
AddMenuItem("📦 Stok Barang", currentTop, ShowStokBarang);
currentTop += 40;
```

---

## Panduan Kontribusi Singkat (Git Cheat Sheet)

Untuk meminimalkan konflik, ikuti alur Git sederhana berikut:

```bash
# 1. Sinkronisasi dengan main terbaru
git checkout main
git pull origin main

# 2. Buat dan masuk ke branch fitur baru
git checkout -b nama-branch-fitur-anda

# 3. (Lakukan perubahan/kodingan di sini)

# 4. Simpan perubahan ke Git (Commit)
git add .
git commit -m "Pesan commit singkat"

# 5. Tarik perubahan terbaru dulu sebelum push (mencegah konflik)
git pull origin main

# 6. Push kode terbaru ke branch Anda
git push origin nama-branch-fitur-anda
```

Setelah selesai, buat **Pull Request** via web repositori Anda (GitHub/GitLab). Biarkan Admin yang menyetujui (Approve).
