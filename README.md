# How to Contribute

Ikuti panduan langkah demi langkah ini untuk berkontribusi ke proyek ini menggunakan Git CLI. Panduan ini dirancang khusus untuk meminimalkan dan menghindari konflik pada file, terutama `README.md`.

## Langkah-Langkah Berkontribusi

### 1. Perbarui Branch Utama
Sebelum mulai mengerjakan sesuatu, pastikan branch utama (`main` atau `master`) di komputer Anda sudah sinkron dengan versi terbaru di server.
```bash
git checkout main
git pull origin main
```

### 2. Buat Branch Fitur
Jangan pernah mengedit langsung di branch `main`. Buatlah branch baru untuk setiap fitur atau perbaikan.
```bash
git checkout -b fitur-baru-anda
```

### 3. Lakukan Perubahan
Edit file (seperti `README.md`) sesuai kebutuhan Anda.

### 4. Simpan Perubahan (Commit)
Simpan perubahan Anda secara lokal dengan pesan commit yang jelas.
```bash
git add .
git commit -m "Menambahkan panduan kontribusi di README"
```

### 5. Ambil Perubahan Terbaru (Anti-Konflik)
**PENTING:** Sebelum melakukan push, tarik perubahan terbaru dari `main` untuk memastikan tidak ada perubahan orang lain yang bertabrakan dengan milik Anda.
```bash
git checkout main
git pull origin main
git checkout fitur-baru-anda
git merge main
```
*Jika terjadi konflik saat merge, Git akan memberitahu Anda. Selesaikan konflik tersebut di editor, simpan, lalu commit.*

### 6. Push Branch Anda
Kirimkan branch fitur Anda ke repositori pusat.
```bash
git push origin fitur-baru-anda
```

### 7. Buat Pull Request
Buka halaman repositori di browser (GitHub/GitLab) dan klik tombol **Compare & pull request**. Berikan deskripsi mengenai apa yang Anda ubah.

---
*Tips: Selalu lakukan `git pull` secara rutin untuk menjaga repositori Anda tetap up-to-date.*
