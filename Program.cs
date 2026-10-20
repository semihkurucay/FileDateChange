namespace DosyaTarih;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new MainForm());
    }
}

/// <summary>Dosya ve klasörlerin oluşturma / değiştirme / erişim tarihlerini ayarlar.</summary>
internal sealed class MainForm : Form
{
    private const string TarihFormati = "dd.MM.yyyy HH:mm:ss";

    private readonly ListView liste = new();
    private readonly CheckBox chkOlusturma = new() { Text = "Oluşturma tarihi", Checked = true, AutoSize = true };
    private readonly CheckBox chkDegistirme = new() { Text = "Değiştirme tarihi", Checked = true, AutoSize = true };
    private readonly CheckBox chkErisim = new() { Text = "Son erişim tarihi", Checked = true, AutoSize = true };
    private readonly DateTimePicker dtOlusturma = YeniTarihSecici();
    private readonly DateTimePicker dtDegistirme = YeniTarihSecici();
    private readonly DateTimePicker dtErisim = YeniTarihSecici();
    private readonly CheckBox chkAltlar = new()
    {
        Text = "Klasörlerin içindeki tüm dosya ve alt klasörlere de uygula",
        AutoSize = true
    };
    private readonly Label lblDurum = new() { AutoSize = true, Text = "Dosya/klasörleri sürükleyip bırakın veya butonlarla ekleyin." };

    public MainForm()
    {
        Text = "Dosya Tarihi Değiştirici";
        MinimumSize = new Size(820, 560);
        Size = new Size(980, 640);
        StartPosition = FormStartPosition.CenterScreen;
        AllowDrop = true;
        DragEnter += (_, e) => e.Effect = e.Data!.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
        DragDrop += (_, e) => Ekle((string[])e.Data!.GetData(DataFormats.FileDrop)!);

        // --- Liste ---
        liste.Dock = DockStyle.Fill;
        liste.View = View.Details;
        liste.FullRowSelect = true;
        liste.GridLines = true;
        liste.Columns.Add("Yol", 420);
        liste.Columns.Add("Tür", 60);
        liste.Columns.Add("Oluşturma", 135);
        liste.Columns.Add("Değiştirme", 135);
        liste.Columns.Add("Son erişim", 135);
        liste.KeyDown += (_, e) => { if (e.KeyCode == Keys.Delete) SeciliKaldir(); };
        liste.DoubleClick += (_, e) => SeciliTarihleriAl();

        // --- Üst butonlar ---
        var ustPanel = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, Padding = new Padding(6) };
        ustPanel.Controls.AddRange(new Control[]
        {
            Buton("Dosya Ekle…", (_, _) => DosyaSec()),
            Buton("Klasör Ekle…", (_, _) => KlasorSec()),
            Buton("Seçileni Kaldır", (_, _) => SeciliKaldir()),
            Buton("Listeyi Temizle", (_, _) => liste.Items.Clear()),
            Buton("Seçilinin tarihlerini al", (_, _) => SeciliTarihleriAl())
        });

        // --- Alt: tarih ayarları ---
        var tarihTablo = new TableLayoutPanel { AutoSize = true, ColumnCount = 3, Padding = new Padding(6) };
        TarihSatiri(tarihTablo, chkOlusturma, dtOlusturma);
        TarihSatiri(tarihTablo, chkDegistirme, dtDegistirme);
        TarihSatiri(tarihTablo, chkErisim, dtErisim);

        var hepsiniEsitle = Buton("Diğerlerini oluşturma tarihine eşitle", (_, _) =>
        {
            dtDegistirme.Value = dtOlusturma.Value;
            dtErisim.Value = dtOlusturma.Value;
        });

        var uygula = Buton("UYGULA", (_, _) => Uygula());
        uygula.Font = new Font(Font, FontStyle.Bold);
        uygula.Height = 40;
        uygula.Width = 160;

        var altAkis = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom, AutoSize = true, FlowDirection = FlowDirection.TopDown,
            WrapContents = false, Padding = new Padding(6)
        };
        altAkis.Controls.Add(tarihTablo);
        altAkis.Controls.Add(hepsiniEsitle);
        altAkis.Controls.Add(chkAltlar);
        altAkis.Controls.Add(uygula);
        altAkis.Controls.Add(lblDurum);

        Controls.Add(liste);
        Controls.Add(ustPanel);
        Controls.Add(altAkis);
    }

    // ---------- UI yardımcıları ----------

    private static DateTimePicker YeniTarihSecici() => new()
    {
        Format = DateTimePickerFormat.Custom,
        CustomFormat = TarihFormati,
        Width = 180,
        Value = DateTime.Now
    };

    private static Button Buton(string metin, EventHandler tik)
    {
        var b = new Button { Text = metin, AutoSize = true, Margin = new Padding(3) };
        b.Click += tik;
        return b;
    }

    private static void TarihSatiri(TableLayoutPanel t, CheckBox chk, DateTimePicker dt)
    {
        chk.CheckedChanged += (_, _) => dt.Enabled = chk.Checked;
        chk.Anchor = AnchorStyles.Left;
        var simdi = Buton("Şimdi", (_, _) => dt.Value = DateTime.Now);
        t.Controls.Add(chk);
        t.Controls.Add(dt);
        t.Controls.Add(simdi);
    }

    // ---------- Liste işlemleri ----------

    private void DosyaSec()
    {
        using var d = new OpenFileDialog { Multiselect = true, Title = "Dosya seçin", Filter = "Tüm dosyalar|*.*" };
        if (d.ShowDialog(this) == DialogResult.OK) Ekle(d.FileNames);
    }

    private void KlasorSec()
    {
        using var d = new FolderBrowserDialog { Description = "Klasör seçin", UseDescriptionForTitle = true };
        if (d.ShowDialog(this) == DialogResult.OK) Ekle(new[] { d.SelectedPath });
    }

    private void Ekle(IEnumerable<string> yollar)
    {
        var mevcut = liste.Items.Cast<ListViewItem>().Select(i => i.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
        liste.BeginUpdate();
        foreach (var yol in yollar)
        {
            if (mevcut.Contains(yol) || !(File.Exists(yol) || Directory.Exists(yol))) continue;
            var item = new ListViewItem { Name = yol, Text = yol };
            item.SubItems.AddRange(new[] { "", "", "", "" });
            liste.Items.Add(item);
            SatiriYenile(item);
        }
        liste.EndUpdate();
    }

    private static void SatiriYenile(ListViewItem item)
    {
        FileSystemInfo fi = Directory.Exists(item.Name) ? new DirectoryInfo(item.Name) : new FileInfo(item.Name);
        fi.Refresh();
        item.SubItems[1].Text = fi is DirectoryInfo ? "Klasör" : "Dosya";
        item.SubItems[2].Text = fi.CreationTime.ToString(TarihFormati);
        item.SubItems[3].Text = fi.LastWriteTime.ToString(TarihFormati);
        item.SubItems[4].Text = fi.LastAccessTime.ToString(TarihFormati);
    }

    private void SeciliKaldir()
    {
        foreach (ListViewItem i in liste.SelectedItems) liste.Items.Remove(i);
    }

    private void SeciliTarihleriAl()
    {
        if (liste.SelectedItems.Count == 0) return;
        var yol = liste.SelectedItems[0].Name;
        FileSystemInfo fi = Directory.Exists(yol) ? new DirectoryInfo(yol) : new FileInfo(yol);
        dtOlusturma.Value = fi.CreationTime;
        dtDegistirme.Value = fi.LastWriteTime;
        dtErisim.Value = fi.LastAccessTime;
    }

    // ---------- Asıl iş ----------

    private void Uygula()
    {
        if (liste.Items.Count == 0) { Bilgi("Listede öğe yok."); return; }
        if (!chkOlusturma.Checked && !chkDegistirme.Checked && !chkErisim.Checked) { Bilgi("En az bir tarih türü seçin."); return; }

        DateTime? olusturma = chkOlusturma.Checked ? dtOlusturma.Value : null;
        DateTime? degistirme = chkDegistirme.Checked ? dtDegistirme.Value : null;
        DateTime? erisim = chkErisim.Checked ? dtErisim.Value : null;

        int basarili = 0;
        var hatalar = new List<string>();
        Cursor = Cursors.WaitCursor;
        try
        {
            foreach (ListViewItem item in liste.Items)
            {
                foreach (var yol in IslenecekYollar(item.Name, chkAltlar.Checked, hatalar))
                {
                    try
                    {
                        TarihAyarla(yol, olusturma, degistirme, erisim);
                        basarili++;
                    }
                    catch (Exception ex)
                    {
                        hatalar.Add($"{yol}: {ex.Message}");
                    }
                }
                SatiriYenile(item);
            }
        }
        finally
        {
            Cursor = Cursors.Default;
        }

        lblDurum.Text = $"{basarili} öğe güncellendi, {hatalar.Count} hata.";
        if (hatalar.Count > 0)
            MessageBox.Show(this, string.Join(Environment.NewLine, hatalar.Take(30)) +
                            (hatalar.Count > 30 ? $"{Environment.NewLine}… ve {hatalar.Count - 30} hata daha" : ""),
                            "Bazı öğeler değiştirilemedi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }

    /// <summary>
    /// Sıralama önemli: içerik değiştikçe Windows klasörün tarihini günceller.
    /// Bu yüzden önce dosyalar, sonra klasörler en derinden yukarı, en son kök klasör işlenir.
    /// </summary>
    private static IEnumerable<string> IslenecekYollar(string yol, bool altlar, List<string> hatalar)
    {
        if (!Directory.Exists(yol) || !altlar) return new[] { yol };

        var secenek = new EnumerationOptions
        {
            RecurseSubdirectories = true,
            IgnoreInaccessible = true,
            AttributesToSkip = FileAttributes.ReparsePoint // junction/sembolik link döngülerini önler
        };
        try
        {
            var dosyalar = Directory.EnumerateFiles(yol, "*", secenek);
            var klasorler = Directory.EnumerateDirectories(yol, "*", secenek)
                                     .OrderByDescending(k => k.Count(c => c == Path.DirectorySeparatorChar));
            return dosyalar.Concat(klasorler).Append(yol).ToList();
        }
        catch (Exception ex)
        {
            hatalar.Add($"{yol} taranamadı: {ex.Message}");
            return new[] { yol };
        }
    }

    private static void TarihAyarla(string yol, DateTime? olusturma, DateTime? degistirme, DateTime? erisim)
    {
        if (Directory.Exists(yol))
        {
            if (olusturma is { } o) Directory.SetCreationTime(yol, o);
            if (degistirme is { } d) Directory.SetLastWriteTime(yol, d);
            if (erisim is { } e) Directory.SetLastAccessTime(yol, e);
            return;
        }

        // Salt okunur dosyalarda yazma reddedilebilir; geçici olarak kaldırıp geri koy.
        var nitelik = File.GetAttributes(yol);
        bool saltOkunur = nitelik.HasFlag(FileAttributes.ReadOnly);
        if (saltOkunur) File.SetAttributes(yol, nitelik & ~FileAttributes.ReadOnly);
        try
        {
            if (olusturma is { } o) File.SetCreationTime(yol, o);
            if (degistirme is { } d) File.SetLastWriteTime(yol, d);
            if (erisim is { } e) File.SetLastAccessTime(yol, e);
        }
        finally
        {
            if (saltOkunur) File.SetAttributes(yol, nitelik);
        }
    }

    private void Bilgi(string mesaj) => MessageBox.Show(this, mesaj, Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
}
