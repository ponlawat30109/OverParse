using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace OverParse
{
    /// <summary>
    /// Checkdll.xaml の相互作用ロジック
    /// </summary>
    public partial class Checkdll : Window
    {
        public Checkdll() => InitializeComponent();

        private async void Checkdll_Loaded(object sender, RoutedEventArgs e)
        {
            SolidColorBrush okbrush = new SolidColorBrush(Color.FromArgb(255, 0, 251, 0));
            SolidColorBrush passbrush = new SolidColorBrush(Color.FromArgb(255, 225, 225, 79));
            SolidColorBrush errorbrush = new SolidColorBrush(Color.FromArgb(255, 251, 0, 0));
            Task<string> pso2h =  Dllhash("pso2h"); Task<string> ddraw = Dllhash("ddraw"); Task<string> dmgdump = Dllhash("plugins\\PSO2DamageDump");
            await Task.WhenAll(pso2h, ddraw, dmgdump);
            pso2hsha256.Content = "SHA-256 : " + pso2h.Result;
            ddrawsha256.Content = "SHA-256 : " + ddraw.Result;
            dmgdmpsha256.Content = "SHA-256 : " + dmgdump.Result;
            if (pso2h.Result == "601d0b4e8d477fec3c2bab061ab4555af33f7c248d5121fe9211f7103d91a27b") { pso2hchk.Content = "OK (3.1.7)"; pso2hchk.Foreground = okbrush; }
            else if (pso2h.Result == "aaee1145b0bfc202f1a4fbf3b414d8d83a731d2d8d2544735efe6a17cb657fc0") { pso2hchk.Content = "ERROR (3.1.6)"; pso2hchk.Foreground = errorbrush; }
            else { pso2hchk.Content = "Unknown"; pso2hchk.Foreground = passbrush; }

            if (ddraw.Result == "0e2700bba52170f9dc5b4c811a7d7f23e22e9b39eae30006e1947a4acf05f63d") { ddrawchk.Content = "OK (3.1.7)"; ddrawchk.Foreground = okbrush; }
            else if (ddraw.Result == "cb0d66924d2e5f98cc5bb7c59c353b248f7460822f5d6be8c649ef9aecce48b3") { ddrawchk.Content = "ERROR (3.1.6)"; ddrawchk.Foreground = errorbrush; }
            else { ddrawchk.Content = "Unknown"; ddrawchk.Foreground = passbrush; }
            if (dmgdump.Result == "33ddffb54c4267ea62cc4ecf50ea12f19be9304d196e164439cc7091b6d55a41") { dmgdmpchk.Content = "OK (3.1.7)"; dmgdmpchk.Foreground = okbrush; }
            else if (dmgdump.Result == "82bb5d1f9e04827947dc025068b8593f0e5c8e1ec5e4624233b5bcb4e7de3f18") { dmgdmpchk.Content = "ERROR (3.1.6)"; dmgdmpchk.Foreground = errorbrush; }
            else { dmgdmpchk.Content = "Unknown"; dmgdmpchk.Foreground = passbrush; }
        }

        private async Task<string> Dllhash(string name)
        {
            try
            {
                using (var fs = new FileStream(Properties.Settings.Default.Path + $"\\{name}.dll", FileMode.Open, FileAccess.Read))
                {
                    byte[] sha256byte = SHA256.Create().ComputeHash(fs);
                    string sha256string = BitConverter.ToString(sha256byte).Replace("-", "").ToLower();
                    await Task.CompletedTask;
                    return sha256string;
                }
            } catch { await Task.CompletedTask; return "Error."; }
        }

    }
}
