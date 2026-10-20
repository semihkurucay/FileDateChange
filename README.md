# Dosya Tarihi Değiştirici

Dosya ve klasörlerin **oluşturma**, **değiştirme** ve **son erişim** tarihlerini istediğiniz değere ayarlar. C# / WinForms, .NET 9.

## Derleme

Gereksinim: .NET 9 SDK (`dotnet --list-sdks` ile kontrol edin).

```powershell
cd DosyaTarih
dotnet run                                  # direkt çalıştır
dotnet publish -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true -o out
# -> out\DosyaTarih.exe
```

## Kullanım

1. Dosya veya klasörleri pencereye sürükleyin. Butonlarla da ekleyebilirsiniz; `.exe` dosyaları da normal dosya gibi eklenir.
2. Değiştirmek istediğiniz tarih türlerini işaretleyin ve tarih/saati girin.
3. Bir klasörün içindekilerin de değişmesini istiyorsanız "alt klasörlere de uygula" seçeneğini işaretleyin.
4. **UYGULA** butonuna basın.

İpuçları:
- Listedeki bir satıra çift tıklarsanız o öğenin mevcut tarihleri seçicilere yüklenir.
- Delete tuşu seçili satırları listeden kaldırır.
- Salt okunur dosyalar da işlenir; bu özellik işlemden sonra geri konur.
- Klasör tarihleri her zaman içerikten sonra ayarlanır. Böylece içeriğin değişmesi klasör tarihini bozmaz.

## Bilinmesi gerekenler

- **Son erişim** tarihini Windows, dosya açıldığında kendisi güncelleyebilir.
- `C:\Program Files` gibi korumalı klasörler için uygulamayı **yönetici olarak** çalıştırın.
- Uygulama dosyanın kendisine dokunmaz, sadece zaman damgalarını değiştirir. Ancak bir programın içinde gömülü olan derleme tarihi gibi bilgiler aynı kalır.
