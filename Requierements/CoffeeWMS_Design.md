# ☕ CoffeeWMS — Frontend Design Document

> **Versi:** 1.0.0 | **Tanggal:** 31 Mei 2026 | **Platform:** Windows Forms (.NET) | **Skala:** UMKM / 1 Gudang

---

## Daftar Isi

1. [Gambaran Desain](#1-gambaran-desain)
2. [Design Tokens (Warna, Font, Spacing)](#2-design-tokens)
3. [Layout & Struktur Navigasi](#3-layout--struktur-navigasi)
4. [Komponen UI Reusable](#4-komponen-ui-reusable)
5. [Wireframe Per Form/Layar](#5-wireframe-per-formlayar)
6. [Alur Navigasi (User Flow)](#6-alur-navigasi-user-flow)
7. [State & Feedback UI](#7-state--feedback-ui)
8. [Panduan Komponen WinForms](#8-panduan-komponen-winforms)
9. [Aksesibilitas & UX Guidelines](#9-aksesibilitas--ux-guidelines)
10. [Checklist Implementasi Frontend](#10-checklist-implementasi-frontend)

---

## 1. Gambaran Desain

### 1.1 Filosofi Desain

CoffeeWMS dirancang dengan prinsip:

- **Clarity First** — Operator gudang harus bisa memahami setiap layar tanpa pelatihan panjang
- **Action-Oriented** — Aksi utama selalu terlihat jelas, tidak tersembunyi di menu dalam
- **Data Density Seimbang** — Menampilkan cukup informasi tanpa membuat layar terlalu padat
- **Konsistensi Visual** — Komponen, warna, dan pola interaksi yang seragam di seluruh form

### 1.2 Tema Visual

Terinspirasi dari nuansa kopi dan alam: hijau tua sebagai warna utama melambangkan perkebunan kopi, aksen coklat kopi untuk elemen sekunder, dan putih bersih sebagai latar untuk keterbacaan maksimal.

---

## 2. Design Tokens

### 2.1 Palet Warna

| Token | Hex | Penggunaan |
|---|---|---|
| `Primary` | `#2C5F2E` | Header, tombol utama, sidebar aktif |
| `PrimaryLight` | `#4A7C4E` | Hover state, heading sekunder |
| `PrimaryDark` | `#1A3D1C` | Pressed state, border penting |
| `Accent` | `#6F4E37` | Ikon kopi, badge, aksen dekoratif |
| `AccentLight` | `#A07855` | Hover accent, label sekunder |
| `Background` | `#F5F5F0` | Latar utama aplikasi (off-white) |
| `Surface` | `#FFFFFF` | Latar form, card, panel |
| `SurfaceAlt` | `#F0F7F0` | Alternating row tabel |
| `Border` | `#CCCCCC` | Garis pembatas, border input |
| `BorderFocus` | `#2C5F2E` | Border input saat fokus |
| `TextPrimary` | `#1A1A1A` | Teks utama |
| `TextSecondary` | `#555555` | Label, keterangan, placeholder |
| `TextDisabled` | `#AAAAAA` | Teks nonaktif |
| `Success` | `#28A745` | Notifikasi sukses, badge aktif |
| `Warning` | `#FFC107` | Peringatan, stok hampir habis |
| `Error` | `#DC3545` | Pesan error, validasi gagal |
| `Info` | `#17A2B8` | Informasi, tooltip |

### 2.2 Tipografi

| Token | Font | Ukuran | Berat | Penggunaan |
|---|---|---|---|---|
| `FontFamily` | Segoe UI | — | — | Font utama seluruh aplikasi |
| `FontFallback` | Arial | — | — | Fallback jika Segoe UI tidak tersedia |
| `Title` | Segoe UI | 18pt | Bold | Judul halaman/form |
| `Heading` | Segoe UI | 14pt | Bold | Heading section |
| `Subheading` | Segoe UI | 12pt | SemiBold | Label grup, judul kolom tabel |
| `Body` | Segoe UI | 10pt | Regular | Teks isi, label field |
| `Small` | Segoe UI | 9pt | Regular | Keterangan kecil, timestamp |
| `Button` | Segoe UI | 10pt | Bold | Teks tombol |

### 2.3 Spacing & Ukuran

| Token | Nilai | Penggunaan |
|---|---|---|
| `SpaceXS` | 4px | Gap sangat kecil |
| `SpaceS` | 8px | Margin dalam komponen |
| `SpaceM` | 16px | Padding form, jarak antar elemen |
| `SpaceL` | 24px | Jarak antar section |
| `SpaceXL` | 40px | Padding halaman |
| `RadiusS` | 3px | Border radius tombol kecil |
| `RadiusM` | 6px | Border radius panel/card |
| `InputHeight` | 32px | Tinggi standar semua input |
| `ButtonHeightPrimary` | 36px | Tinggi tombol utama |
| `ButtonHeightSecondary` | 30px | Tinggi tombol sekunder |
| `SidebarWidth` | 220px | Lebar sidebar navigasi |
| `HeaderHeight` | 56px | Tinggi header aplikasi |

---

## 3. Layout & Struktur Navigasi

### 3.1 Layout Utama Aplikasi

```
┌─────────────────────────────────────────────────────────────────┐
│  HEADER (56px)                                                  │
│  [☕ CoffeeWMS]          [👤 Nama User - Peran]  [🚪 Logout]   │
├──────────────┬──────────────────────────────────────────────────┤
│              │                                                  │
│  SIDEBAR     │  CONTENT AREA                                    │
│  (220px)     │                                                  │
│              │  [Breadcrumb: Beranda > Modul > Sub-modul]       │
│  [Menu Item] │  ┌──────────────────────────────────────────┐   │
│  [Menu Item] │  │  JUDUL HALAMAN                           │   │
│  [Menu Item] │  │                                          │   │
│  ──────────  │  │  [Konten Form / Tabel / Dashboard]       │   │
│  [Menu Item] │  │                                          │   │
│              │  └──────────────────────────────────────────┘   │
│              │                                                  │
│  [v1.0.0]    │  STATUS BAR (informasi koneksi, user aktif)     │
└──────────────┴──────────────────────────────────────────────────┘
```

### 3.2 Struktur Menu Sidebar Per Peran

#### Admin
```
📊 Dashboard
👥 Pengguna
🏪 Supplier
☕ Jenis & Kategori Kopi
📍 Tujuan Pengiriman
📋 Activity Log
📈 Laporan
👤 Profil Saya
```

#### Manager Gudang
```
📊 Dashboard
📈 Laporan Transaksi
🚚 Laporan Pengiriman
📤 Ekspor Data
👤 Profil Saya
```

#### Petugas
```
📥 Input Penerimaan
📤 Input Pengiriman
📦 Cek Stok
👤 Profil Saya
```

### 3.3 Sidebar Visual Spec

- **Lebar:** 220px (fixed)
- **Latar:** `#2C5F2E` (Primary)
- **Teks menu:** Putih `#FFFFFF`, Segoe UI 10pt
- **Item aktif:** Latar `#1A3D1C`, garis kiri 4px warna `#A07855`
- **Hover:** Latar `#4A7C4E` dengan transisi smooth
- **Logo area:** 56px tinggi, teks "☕ CoffeeWMS" bold putih
- **Separator:** Garis horizontal `rgba(255,255,255,0.2)`

---

## 4. Komponen UI Reusable

### 4.1 Tombol (Button)

| Varian | Latar | Teks | Border | Penggunaan |
|---|---|---|---|---|
| **Primary** | `#2C5F2E` | Putih | Tidak ada | Simpan, Konfirmasi, Submit |
| **Secondary** | Transparan | `#2C5F2E` | `#2C5F2E` 1px | Batal, Kembali, Tambah |
| **Danger** | `#DC3545` | Putih | Tidak ada | Hapus, Nonaktifkan |
| **Warning** | `#FFC107` | `#1A1A1A` | Tidak ada | Peringatan, Reset |
| **Disabled** | `#CCCCCC` | `#AAAAAA` | Tidak ada | Semua tombol nonaktif |

**Spec:**
- Padding: 8px 20px
- Border radius: 4px
- Font: Segoe UI 10pt Bold
- Hover: gelap 10% dari warna dasar
- Ikon opsional di kiri teks (16px)

### 4.2 Input Field

```
Label Field *
┌─────────────────────────────────────────┐
│  Placeholder teks...                    │
└─────────────────────────────────────────┘
  ↑ Warna border saat fokus: #2C5F2E
  ✗ Pesan error validasi (merah #DC3545)
```

**Spec:**
- Tinggi: 32px
- Border: 1px solid `#CCCCCC`
- Border saat fokus: 1px solid `#2C5F2E`
- Border error: 1px solid `#DC3545`
- Border radius: 3px
- Padding dalam: 4px 8px
- Label: di atas input, Segoe UI 10pt, `#555555`
- Asterisk (*) untuk field wajib: merah `#DC3545`

### 4.3 DataGridView (Tabel Data)

```
┌────┬──────────────────┬────────────────┬──────────┬──────────┐
│ No │ Nama             │ Keterangan     │ Status   │ Aksi     │
├────┼──────────────────┼────────────────┼──────────┼──────────┤
│  1 │ Arabika Gayo     │ Green Bean     │ ● Aktif  │ ✏️ 🗑️  │
│  2 │ Robusta Lampung  │ Roasted Bean   │ ● Aktif  │ ✏️ 🗑️  │
│  3 │ Liberika         │ Green Bean     │ ○ Nonaktif│ ✏️ 🗑️  │
└────┴──────────────────┴────────────────┴──────────┴──────────┘
                                        [< Prev]  1/5  [Next >]
```

**Spec:**
- Header: Latar `#2C5F2E`, teks putih bold, tinggi 36px
- Row normal: `#FFFFFF`
- Row alternating: `#F0F7F0`
- Row hover: `#E8F5E9`
- Row selected: `#C8E6C9`
- Border sel: `#CCCCCC`
- Font: Segoe UI 10pt
- Badge Status Aktif: bulatan hijau `#28A745` + teks
- Badge Status Nonaktif: bulatan abu `#AAAAAA` + teks
- Kolom Aksi: tombol ikon kecil (Edit, Hapus)
- Paginasi di bawah tabel: 15 baris per halaman

### 4.4 Card / Panel Ringkasan

```
┌─────────────────────────┐
│  📦 Total Stok          │
│                         │
│       12,500 kg         │
│                         │
│  ↑ +200 kg hari ini     │
└─────────────────────────┘
```

**Spec:**
- Latar: `#FFFFFF`
- Border: 1px solid `#CCCCCC`
- Border radius: 8px
- Drop shadow: `0 2px 8px rgba(0,0,0,0.08)`
- Padding: 20px
- Ikon: 32px, warna `#2C5F2E`
- Angka utama: 24pt Bold `#1A1A1A`
- Keterangan bawah: 9pt `#555555`

### 4.5 Dialog & Modal

**Konfirmasi:**
```
┌─────────────────────────────────────┐
│  ⚠️  Konfirmasi Hapus               │
│                                     │
│  Anda yakin ingin menghapus data    │
│  supplier "PT Kopi Nusantara"?      │
│  Tindakan ini tidak dapat dibatalkan│
│                                     │
│              [Batal]  [Ya, Hapus]   │
└─────────────────────────────────────┘
```

**Notifikasi Sukses (Toast):**
```
┌────────────────────────────────┐
│  ✅  Data berhasil disimpan!   │
└────────────────────────────────┘
  (muncul 3 detik, pojok kanan bawah)
```

---

## 5. Wireframe Per Form/Layar

### 5.1 Form Login

```
┌──────────────────────────────────────────────────────┐
│                                                      │
│                    ☕ CoffeeWMS                      │
│             Sistem Manajemen Gudang Kopi             │
│                                                      │
│         ┌────────────────────────────────┐           │
│  👤     │  Username                      │           │
│         └────────────────────────────────┘           │
│                                                      │
│         ┌────────────────────────────────┐           │
│  🔒     │  ••••••••                      │           │
│         └────────────────────────────────┘           │
│                                                      │
│         ┌────────────────────────────────┐           │
│         │         MASUK                  │  ← Primary│
│         └────────────────────────────────┘           │
│                                                      │
│         ✗ Username atau password salah               │
│           (hanya muncul jika error)                  │
│                                                      │
│                    v1.0.0                            │
└──────────────────────────────────────────────────────┘
```

### 5.2 Dashboard (Manager/Admin)

```
┌──────────────┬───────────────────────────────────────────────┐
│  SIDEBAR     │  📊 Dashboard                 [🔄 Refresh]   │
│              ├───────────────────────────────────────────────┤
│              │                                               │
│              │  ┌───────────┐ ┌───────────┐ ┌───────────┐  │
│              │  │ 📦 Stok   │ │ 📥 Terima │ │ 📤 Kirim  │  │
│              │  │ 12,500 kg │ │  Hari Ini │ │  Hari Ini │  │
│              │  │           │ │   +200 kg │ │   -150 kg │  │
│              │  └───────────┘ └───────────┘ └───────────┘  │
│              │                                               │
│              │  Stok per Jenis Kopi                         │
│              │  ┌─────────────────────────────────────────┐ │
│              │  │  [Grafik Batang / Pie Chart]            │ │
│              │  └─────────────────────────────────────────┘ │
│              │                                               │
│              │  Transaksi Terbaru                           │
│              │  ┌───────┬────────────┬────────┬──────────┐  │
│              │  │ Tgl   │ Jenis      │ Jumlah │ Tipe     │  │
│              │  │ 31/05 │ Arabika    │ 200 kg │ Terima   │  │
│              │  │ 31/05 │ Robusta    │ 150 kg │ Kirim    │  │
│              │  └───────┴────────────┴────────┴──────────┘  │
└──────────────┴───────────────────────────────────────────────┘
```

### 5.3 Form Manajemen Data (CRUD Generic)

```
┌──────────────┬───────────────────────────────────────────────┐
│  SIDEBAR     │  👥 Manajemen [Entitas]      [+ Tambah Baru]  │
│              ├─────────────────────┬─────────────────────────┤
│              │ 🔍 Cari...          │  Filter: [Semua Status▼] │
│              ├─────────────────────┴─────────────────────────┤
│              │ No │ Nama       │ Keterangan │ Status │ Aksi  │
│              │  1 │ ...        │ ...        │ ●Aktif │ ✏️🗑️ │
│              │  2 │ ...        │ ...        │ ●Aktif │ ✏️🗑️ │
│              │  3 │ ...        │ ...        │ ○Non   │ ✏️🗑️ │
│              ├────────────────────────────────────────────────┤
│              │              [< Prev]  Hal. 1/5  [Next >]     │
└──────────────┴───────────────────────────────────────────────┘

── Sub-form Tambah/Edit (muncul sebagai Panel atau Child Form) ──
┌──────────────────────────────────────────────────┐
│  Tambah / Edit [Entitas]                    [✕]  │
│                                                  │
│  Nama *         [________________________]       │
│  Keterangan     [________________________]       │
│  Status         (●) Aktif  ( ) Nonaktif          │
│                                                  │
│                         [Batal]  [Simpan]        │
└──────────────────────────────────────────────────┘
```

### 5.4 Form Input Penerimaan Kopi

```
┌──────────────┬───────────────────────────────────────────────┐
│  SIDEBAR     │  📥 Input Penerimaan Kopi                     │
│              ├───────────────────────────────────────────────┤
│              │                                               │
│              │  Tanggal Terima *    [31/05/2026  📅]         │
│              │  Supplier *          [Pilih Supplier    ▼]    │
│              │  Jenis Kopi *        [Pilih Jenis Kopi  ▼]    │
│              │  Jumlah (kg) *       [________] kg            │
│              │  Catatan             [________________________]│
│              │                      [________________________]│
│              │                                               │
│              │                   [Batal]  [💾 Simpan & Cetak]│
│              │                                               │
│              ├───────────────────────────────────────────────┤
│              │  Penerimaan Hari Ini (31 Mei 2026)            │
│              │  ┌──────┬────────────┬──────────┬──────────┐  │
│              │  │ Jam  │ Supplier   │ Jenis    │ Jumlah   │  │
│              │  │ 08:00│ PT Nusant..│ Arabika  │ 200 kg   │  │
│              │  └──────┴────────────┴──────────┴──────────┘  │
└──────────────┴───────────────────────────────────────────────┘
```

### 5.5 Form Input Pengiriman Kopi

```
┌──────────────┬───────────────────────────────────────────────┐
│  SIDEBAR     │  📤 Input Pengiriman Kopi                     │
│              ├───────────────────────────────────────────────┤
│              │                                               │
│              │  Tanggal Kirim *     [31/05/2026  📅]         │
│              │  Tujuan Pengiriman * [Pilih Tujuan      ▼]    │
│              │  Jenis Kopi *        [Pilih Jenis Kopi  ▼]    │
│              │  Stok Tersedia       [  2,500 kg  ] (readonly) │
│              │  Jumlah Kirim (kg) * [________] kg            │
│              │  Catatan             [________________________]│
│              │                                               │
│              │  ⚠️  Stok tidak mencukupi!                    │
│              │     (pesan error muncul saat validasi gagal)  │
│              │                                               │
│              │                   [Batal]  [💾 Simpan & Cetak]│
└──────────────┴───────────────────────────────────────────────┘
```

### 5.6 Halaman Laporan

```
┌──────────────┬───────────────────────────────────────────────┐
│  SIDEBAR     │  📈 Laporan Transaksi                         │
│              ├───────────────────────────────────────────────┤
│              │  Filter:                                      │
│              │  Dari [01/05/2026] Sampai [31/05/2026]        │
│              │  Tipe [Semua ▼]  Jenis Kopi [Semua ▼]        │
│              │  [🔍 Tampilkan]    [📤 Export Excel] [📄 PDF] │
│              ├───────────────────────────────────────────────┤
│              │ Tgl  │ Tipe  │ Jenis    │ Jumlah │ Petugas   │
│              │ ...  │ Terima│ Arabika  │ 200 kg │ Budi      │
│              │ ...  │ Kirim │ Robusta  │ 100 kg │ Siti      │
│              ├───────────────────────────────────────────────┤
│              │ TOTAL PENERIMAAN: 1,200 kg                    │
│              │ TOTAL PENGIRIMAN:   800 kg                    │
│              │ SELISIH BERSIH:     400 kg                    │
└──────────────┴───────────────────────────────────────────────┘
```

### 5.7 Halaman Profil & Ubah Password

```
┌──────────────┬───────────────────────────────────────────────┐
│  SIDEBAR     │  👤 Profil Saya                               │
│              ├───────────────────────────────────────────────┤
│              │  ┌──────────────────────────────────────────┐ │
│              │  │              👤                          │ │
│              │  │         Budi Santoso                     │ │
│              │  │         Petugas Gudang                   │ │
│              │  │         Terakhir login: 31/05/2026 08:00 │ │
│              │  └──────────────────────────────────────────┘ │
│              │                                               │
│              │  Username        budi.santoso (readonly)      │
│              │  Nama Lengkap    [Budi Santoso_____________]  │
│              │  Peran           Petugas (readonly)           │
│              │                                               │
│              │  ──── Ubah Password ────                      │
│              │  Password Lama    [••••••••]                  │
│              │  Password Baru    [••••••••]                  │
│              │  Konfirmasi Baru  [••••••••]                  │
│              │                                               │
│              │                   [Simpan Perubahan]          │
└──────────────┴───────────────────────────────────────────────┘
```

---

## 6. Alur Navigasi (User Flow)

### 6.1 Alur Login

```
[Aplikasi Dibuka]
       │
       ▼
  [Form Login]
       │
   ┌───┴───┐
   │ Valid?│
   └───┬───┘
       │ Ya                        │ Tidak (≤5x)
       ▼                           ▼
  [Cek Peran]              [Tampilkan Error]
   ┌───┼───┐
   │   │   │
   ▼   ▼   ▼
 Admin Mgr Petugas
   │   │   │
   ▼   ▼   ▼
[Dashboard Sesuai Peran]
```

### 6.2 Alur Input Penerimaan Kopi

```
[Sidebar: Input Penerimaan]
          │
          ▼
     [Buka Form]
          │
    [Isi Semua Field]
          │
     [Klik Simpan]
          │
    ┌─────┴─────┐
    │  Validasi  │
    └─────┬─────┘
          │ Lolos             │ Gagal
          ▼                   ▼
  [Dialog Konfirmasi]   [Highlight Field
          │              Error + Pesan]
    [Klik Ya]
          │
    [Simpan ke DB]
          │
  [Notifikasi Sukses]
          │
  [Dialog: Cetak Bukti?]
     ┌────┴────┐
     │ Ya  │ Tidak
     ▼         ▼
 [Print]  [Reset Form]
```

### 6.3 Alur Pengiriman dengan Validasi Stok

```
[Form Pengiriman]
       │
 [Pilih Jenis Kopi]
       │
 [Sistem Auto-Load Stok Tersedia]
       │
 [Input Jumlah Kirim]
       │
  [Klik Simpan]
       │
  ┌────┴────┐
  │ Jumlah ≤│
  │  Stok?  │
  └────┬────┘
       │ Ya                   │ Tidak
       ▼                      ▼
 [Dialog Konfirmasi]    [Error: "Stok tidak
       │                 mencukupi. Stok
  [Simpan ke DB]         tersedia: X kg"]
       │
 [Stok Berkurang Otomatis]
       │
 [Notifikasi Sukses]
```

---

## 7. State & Feedback UI

### 7.1 State Tombol

| State | Visual |
|---|---|
| Default | Latar `#2C5F2E`, teks putih |
| Hover | Latar `#1A3D1C` |
| Pressed | Latar `#0F2410`, sedikit turun 1px |
| Disabled | Latar `#CCCCCC`, teks `#AAAAAA`, kursor default |
| Loading | Teks "Menyimpan...", ProgressBar/spinner, disabled |

### 7.2 State Input

| State | Visual |
|---|---|
| Default | Border `#CCCCCC` |
| Fokus | Border `#2C5F2E` 2px |
| Error | Border `#DC3545`, label error merah di bawah |
| Disabled | Latar `#F5F5F5`, teks `#AAAAAA` |
| Read-only | Latar `#EEEEEE`, border tipis |

### 7.3 Notifikasi & Pesan

| Tipe | Ikon | Warna | Durasi |
|---|---|---|---|
| Sukses | ✅ | `#28A745` | 3 detik, auto-hilang |
| Error | ❌ | `#DC3545` | Tetap (dismiss manual) |
| Warning | ⚠️ | `#FFC107` | Tetap atau 5 detik |
| Info | ℹ️ | `#17A2B8` | 3 detik, auto-hilang |

### 7.4 Loading State

- Operasi < 1 detik: tidak perlu indikator
- Operasi 1–5 detik: ProgressBar di status bar bawah
- Operasi > 5 detik (ekspor laporan): dialog progress dengan persentase

---

## 8. Panduan Komponen WinForms

### 8.1 Mapping Desain ke Komponen .NET

| Elemen Desain | Komponen WinForms |
|---|---|
| Sidebar navigasi | `Panel` + `Button` dengan custom painting |
| Header aplikasi | `MenuStrip` atau custom `Panel` |
| Input teks | `TextBox` dengan custom border via `WM_NCPAINT` |
| Dropdown | `ComboBox` (DropDownStyle = DropDownList) |
| Tanggal | `DateTimePicker` (Format = Short) |
| Angka/kuantitas | `NumericUpDown` |
| Tabel data | `DataGridView` dengan custom styling |
| Grafik | `System.Windows.Forms.DataVisualization.Charting.Chart` |
| Dialog konfirmasi | `MessageBox.Show()` atau custom Form |
| Notifikasi toast | Custom `Form` tanpa border, animasi fade |
| Cetak | `PrintDocument` + `PrintPreviewDialog` |
| Progress | `ProgressBar` di `StatusStrip` |

### 8.2 Custom Styling DataGridView

```csharp
// Contoh konfigurasi DataGridView standar CoffeeWMS
dataGridView.EnableHeadersVisualStyles = false;
dataGridView.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(44, 95, 46);  // #2C5F2E
dataGridView.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
dataGridView.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
dataGridView.ColumnHeadersHeight = 36;
dataGridView.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 247, 240); // #F0F7F0
dataGridView.DefaultCellStyle.SelectionBackColor = Color.FromArgb(200, 230, 201);
dataGridView.DefaultCellStyle.SelectionForeColor = Color.Black;
dataGridView.BorderStyle = BorderStyle.FixedSingle;
dataGridView.GridColor = Color.FromArgb(204, 204, 204);
dataGridView.RowTemplate.Height = 32;
dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
```

### 8.3 Konvensi Penamaan Kontrol

```
Prefix  │ Tipe Kontrol        │ Contoh
───────────────────────────────────────────
txt     │ TextBox             │ txtUsername
cmb     │ ComboBox            │ cmbJenisKopi
dtp     │ DateTimePicker      │ dtpTanggalTerima
nud     │ NumericUpDown       │ nudJumlahKg
dgv     │ DataGridView        │ dgvListSupplier
btn     │ Button              │ btnSimpan, btnBatal
lbl     │ Label               │ lblError, lblStokTersedia
pnl     │ Panel               │ pnlSidebar, pnlHeader
grp     │ GroupBox            │ grpFilterLaporan
chk     │ CheckBox            │ chkStatusAktif
rad     │ RadioButton         │ radTipePenerimaan
pgb     │ ProgressBar         │ pgbLoading
```

---

## 9. Aksesibilitas & UX Guidelines

### 9.1 Keyboard Navigation
- `Tab` / `Shift+Tab` untuk berpindah antar field
- `Enter` untuk submit form utama
- `Escape` untuk menutup dialog/form sekunder
- `Alt+S` shortcut Simpan, `Alt+B` shortcut Batal
- `F5` untuk refresh data/dashboard

### 9.2 Validasi & Error Handling
- Validasi dilakukan **saat submit**, bukan real-time (kecuali cek stok yang dilakukan saat field jumlah diubah)
- Field error di-highlight dan fokus otomatis ke field pertama yang error
- Pesan error spesifik: "Jumlah harus lebih dari 0" bukan "Input tidak valid"
- Jangan reset seluruh form saat ada error; pertahankan data yang sudah diisi

### 9.3 Konfirmasi Aksi Destruktif
- **Selalu** tampilkan dialog konfirmasi sebelum: hapus data, nonaktifkan user, reset form yang sudah terisi
- Tombol konfirmasi destruktif (Hapus, Nonaktifkan) berwarna merah
- Tombol default di dialog konfirmasi adalah "Batal" (bukan "Ya")

### 9.4 Konsistensi
- Urutan tombol: selalu **[Batal]** di kiri, **[Simpan/Konfirmasi]** di kanan
- Label tombol menggunakan kata kerja: "Simpan", "Hapus", "Tambah" (bukan "OK")
- Tanggal selalu format: `DD/MM/YYYY`
- Angka jumlah selalu ditampilkan dengan pemisah ribuan: `1.200 kg`

### 9.5 Responsivitas Form
- Semua form mendukung resize window (minimum 1366×768)
- DataGridView menggunakan `AutoSizeColumnsMode.Fill` agar menyesuaikan lebar
- Gunakan `Anchor` dan `Dock` properties dengan benar untuk semua kontrol

---

## 10. Checklist Implementasi Frontend

### Fase 1 — Fondasi
- [ ] Setup project WinForms (.NET 6/8)
- [ ] Implementasi color scheme & font sebagai konstanta global
- [ ] Buat `BaseForm` dengan header, sidebar, dan status bar
- [ ] Form Login dengan validasi
- [ ] Sistem routing/navigasi berdasarkan peran user
- [ ] Komponen toast notification

### Fase 2 — Modul Admin
- [ ] Form Manajemen Pengguna (CRUD + DataGridView)
- [ ] Form Kelola Supplier
- [ ] Form Kelola Jenis & Kategori Kopi
- [ ] Form Kelola Tujuan Pengiriman
- [ ] Halaman Activity Log dengan filter

### Fase 3 — Modul Petugas
- [ ] Form Input Penerimaan Kopi
- [ ] Form Input Pengiriman Kopi + validasi stok real-time
- [ ] Halaman Cek Stok
- [ ] Komponen cetak bukti transaksi

### Fase 4 — Modul Manager
- [ ] Dashboard dengan card ringkasan
- [ ] Integrasi Chart control (grafik stok)
- [ ] Form Laporan Transaksi dengan filter
- [ ] Form Laporan Tujuan Pengiriman
- [ ] Fungsi ekspor Excel (EPPlus/NPOI)
- [ ] Fungsi ekspor PDF

### Fase 5 — Fitur General
- [ ] Form Profil Akun
- [ ] Form Ubah Password
- [ ] Fitur pencarian global/kontekstual
- [ ] Auto-logout timer (30 menit idle)

### Fase 6 — QA & Finalisasi
- [ ] Review konsistensi warna dan font di semua form
- [ ] Uji keyboard navigation di semua form
- [ ] Uji semua pesan error dan validasi
- [ ] Uji resize window di resolusi 1366×768 dan 1920×1080
- [ ] Review aksesibilitas (tab order, label kontrol)
- [ ] User acceptance testing dengan operator dan manager

---

*Dokumen ini adalah panduan desain frontend untuk CoffeeWMS v1.0.0. Segala perubahan kebutuhan harus diperbarui di dokumen ini sebelum implementasi.*
