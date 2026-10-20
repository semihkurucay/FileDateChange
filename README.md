# Dosya Tarihi Değiştirici
 
Windows'ta dosya ve klasörlerin **oluşturma**, **değiştirme** ve **son erişim** tarihlerini istediğiniz tarih ve saate ayarlayan küçük bir masaüstü uygulaması.
 
Bir dosyayı kopyaladığınızda, taşıdığınızda veya düzenlediğinizde Windows tarihini "bugün" yapar. Bu araçla tarihleri istediğiniz değere geri alabilir ya da tamamen yeni bir tarih verebilirsiniz.
 
## Özellikler
 
- Dosya, klasör ve uygulama (`.exe`) tarihlerini değiştirme
- Üç tarih türünü (oluşturma, değiştirme, son erişim) ayrı ayrı seçip ayarlama
- Bir klasörün içindeki **tüm dosya ve alt klasörlere** tek seferde uygulama
- Sürükle-bırak ile toplu ekleme
- Bir öğenin mevcut tarihlerini alıp başka öğelere kopyalama (satıra çift tıklayın)
- Salt okunur dosyaları da işleme; bu özellik işlemden sonra geri konur
- Klasör tarihlerinin içerikten sonra ayarlanması; böylece klasör tarihi tekrar "bugün" olmaz
## İndirme
 
[Releases](../../releases) sayfasından `DosyaTarih.exe` dosyasını indirin.
 
> **Gereksinim:** [.NET 9 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/9.0) (Windows x64)
>
> Exe imzalı olmadığı için Windows SmartScreen uyarı gösterebilir. Çalıştırmak için **Ek bilgi → Yine de çalıştır** seçeneğini kullanın.
 
## Kullanım
 
1. Dosya veya klasörleri pencereye sürükleyin. **Dosya Ekle / Klasör Ekle** butonlarını da kullanabilirsiniz.
2. Değiştirmek istediğiniz tarih türlerini işaretleyin ve tarih/saati girin.
3. Klasörün içindekiler de değişsin istiyorsanız **"alt klasörlere de uygula"** seçeneğini işaretleyin.
4. **UYGULA** butonuna basın.
| Kısayol | İşlev |
|---|---|
| Satıra çift tıklama | O öğenin tarihlerini seçicilere yükler |
| `Delete` | Seçili satırları listeden kaldırır |
| **Şimdi** butonu | İlgili tarihi şu ana ayarlar |
 
## Bilinmesi gerekenler
 
- **Son erişim** tarihi, dosya açıldığında Windows tarafından yeniden güncellenebilir.
- `C:\Program Files` gibi korumalı konumlar için uygulamayı **yönetici olarak** çalıştırın.
- Uygulama yalnızca dosya sisteminin zaman damgalarını değiştirir; dosyanın içeriğine dokunmaz. Bir dosyanın içine gömülü tarihler (ör. EXIF, belge özellikleri) aynı kalır.
## Kaynaktan derleme
 
Gereksinim: [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
 
```powershell
git clone https://github.com/semihkurucay/FileDateChange.git
cd FileDateChange
dotnet run                     # doğrudan çalıştır
 
# Tek dosya exe üret -> out\DosyaTarih.exe
dotnet publish -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true -o out
```
 
Runtime gerektirmeyen bağımsız bir exe için `--self-contained true` kullanın. Bu durumda dosya boyutu büyür.
 
## Teknolojiler
 
- C# / .NET 9
- Windows Forms
## Geliştirme
 
Bu proje **[Claude](https://claude.ai)** (Anthropic) yardımıyla geliştirilmiştir. Kod Claude tarafından yazılmış; test ve yayınlama tarafımdan yapılmıştır.
