using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Net;

namespace OverParse
{
    /// <summary>
    /// SettingWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class SettingWindow : Window
    {
        public string fontsize;
        public string fontcolor;
        public bool IsSetting = false;
        MainWindow mainwindow = (MainWindow)Application.Current.MainWindow;
        MainWindow m = (MainWindow)Application.Current.MainWindow;

        public SettingWindow()
        {
            InitializeComponent();
            Language = XmlLanguage.GetLanguage(Thread.CurrentThread.CurrentCulture.Name);
            FontList.DataContext = Fonts.SystemFontFamilies.ToList();

            IsSetting = true;
            WindowOpacity.Value = Properties.Settings.Default.WindowOpacity;
            BackOpacity.Value = Properties.Settings.Default.ListOpacity;
            IP.Text = Properties.Settings.Default.BouyomiIP;


            FontSizeBox.TextChanged += FontSizeBox_TextChanged;
            FontSizeBox.Text = Properties.Settings.Default.FontSize.ToString("N1");
            TextColorBox.Content = Properties.Settings.Default.FontColor;
            TextColorBox.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Properties.Settings.Default.FontColor));

            ForegroundUIColor.Content = Properties.Settings.Default.Foreground;
            ForegroundUIColor.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Properties.Settings.Default.Foreground));


            //Column
            PlayerName.IsChecked = Properties.Settings.Default.ListName;
            Percent.IsChecked = Properties.Settings.Default.ListPct;
            TScore.IsChecked = Properties.Settings.Default.ListTS;
            Damage.IsChecked = Properties.Settings.Default.ListDmg;
            Damaged.IsChecked = Properties.Settings.Default.ListDmgd;
            PlayerDPS.IsChecked = Properties.Settings.Default.ListDPS;
            PlayerJA.IsChecked = Properties.Settings.Default.ListJA;
            Critical.IsChecked = Properties.Settings.Default.ListCri;
            MaxHit.IsChecked = Properties.Settings.Default.ListHit;
            AtkName.IsChecked = Properties.Settings.Default.ListAtk;
            Tabchk.IsChecked = Properties.Settings.Default.ListTab;
            Variable.IsChecked = Properties.Settings.Default.Variable;
            // - - - -

            if (Properties.Settings.Default.Language == "ja-JP") { JA.IsChecked = true; } else if (Properties.Settings.Default.Language == "en-US") { EN.IsChecked = true; }

            // - - - -
            Graph.IsChecked = Properties.Settings.Default.ShowDamageGraph;
            Highlight.IsChecked = Properties.Settings.Default.HighlightYourDamage;
            DamageSI.IsChecked = Properties.Settings.Default.DamageSI;
            DPSSI.IsChecked = Properties.Settings.Default.DPSSI;
            MaxSI.IsChecked = Properties.Settings.Default.MaxSI;
            LowResources.IsChecked = Properties.Settings.Default.LowResources;
            CPUdraw.IsChecked = Properties.Settings.Default.CPUdraw;
            Clock.IsChecked = Properties.Settings.Default.Clock;
            Bouyomi.IsChecked = Properties.Settings.Default.Bouyomi;
            BouyomiFormat.IsChecked = Properties.Settings.Default.BouyomiFormat;

            if (Properties.Settings.Default.BackContent == "Color") { RadioColor.IsChecked = true; } else if (Properties.Settings.Default.BackContent == "Image") { RadioImage.IsChecked = true; }
            BackColorInput.Content = Properties.Settings.Default.BackColor;
            PathLabel.Content = "Path : " + Properties.Settings.Default.ImagePath;
            try
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(Properties.Settings.Default.ImagePath);
                bitmap.EndInit();
                PreviewImage.Source = bitmap;
            } catch { }
        }

        private void FontList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FontList.Items.Count < 1) { return; }
            Properties.Settings.Default.Font = FontList.SelectedItem.ToString();
            mainwindow.CombatantData.FontFamily = (FontFamily)FontList.SelectedItem;
        }

        private void OK_button_Click(object sender, RoutedEventArgs e)
        {
            if (FontList.SelectedItem != null) { Properties.Settings.Default.Font = FontList.SelectedItem.ToString(); }

            //Column Settings
            GridLength temp = new GridLength(0);
            m.CombatantView.Columns.Clear();
            if (PlayerName.IsChecked) { m.CombatantView.Columns.Add(m.NameColumn); m.CNameHC.Width = new GridLength(1, GridUnitType.Star); } else { m.CNameHC.Width = temp; }
            if (Variable.IsChecked)
            {
                if (Percent.IsChecked) { m.CombatantView.Columns.Add(m.PercentColumn); m.CPercentHC.Width = new GridLength(0.4, GridUnitType.Star); } else { m.CPercentHC.Width = temp; }
                if (TScore.IsChecked) { m.CombatantView.Columns.Add(m.TScoreColumn); m.CTScoreHC.Width = new GridLength(0.4, GridUnitType.Star); } else { m.CPercentHC.Width = temp; }
                if (Damage.IsChecked) { m.CombatantView.Columns.Add(m.DamageColumn); m.CDmgHC.Width = new GridLength(0.8, GridUnitType.Star); } else { m.CDmgHC.Width = temp; }
                if (Damaged.IsChecked) { m.CombatantView.Columns.Add(m.DamagedColumn); m.CDmgDHC.Width = new GridLength(0.6, GridUnitType.Star); } else { m.CDmgDHC.Width = temp; }
                if (PlayerDPS.IsChecked) { m.CombatantView.Columns.Add(m.DPSColumn); m.CDPSHC.Width = new GridLength(0.6, GridUnitType.Star); } else { m.CDPSHC.Width = temp; }
                if (PlayerJA.IsChecked) { m.CombatantView.Columns.Add(m.JAColumn); m.CJAHC.Width = new GridLength(0.4, GridUnitType.Star); } else { m.CJAHC.Width = temp; }
                if (Critical.IsChecked) { m.CombatantView.Columns.Add(m.CriColumn); m.CCriHC.Width = new GridLength(0.4, GridUnitType.Star); } else { m.CCriHC.Width = temp; }
                if (MaxHit.IsChecked) { m.CombatantView.Columns.Add(m.HColumn); m.CMdmgHC.Width = new GridLength(0.6, GridUnitType.Star); } else { m.CMdmgHC.Width = temp; }
            } else
            {
                if (Percent.IsChecked) { m.CombatantView.Columns.Add(m.PercentColumn); m.CPercentHC.Width = new GridLength(39); } else { m.CPercentHC.Width = temp; }
                if (TScore.IsChecked) { m.CombatantView.Columns.Add(m.TScoreColumn); m.CTScoreHC.Width = new GridLength(39); } else { m.CTScoreHC.Width = temp; }
                if (Damage.IsChecked) { m.CombatantView.Columns.Add(m.DamageColumn); m.CDmgHC.Width = new GridLength(78); } else { m.CDmgHC.Width = temp; }
                if (Damaged.IsChecked) { m.CombatantView.Columns.Add(m.DamagedColumn); m.CDmgDHC.Width = new GridLength(56); } else { m.CDmgDHC.Width = temp; }
                if (PlayerDPS.IsChecked) { m.CombatantView.Columns.Add(m.DPSColumn); m.CDPSHC.Width = new GridLength(56); } else { m.CDPSHC.Width = temp; }
                if (PlayerJA.IsChecked) { m.CombatantView.Columns.Add(m.JAColumn); m.CJAHC.Width = new GridLength(39); } else { m.CJAHC.Width = temp; }
                if (Critical.IsChecked) { m.CombatantView.Columns.Add(m.CriColumn); m.CCriHC.Width = new GridLength(39); } else { m.CCriHC.Width = temp; }
                if (MaxHit.IsChecked) { m.CombatantView.Columns.Add(m.HColumn); m.CMdmgHC.Width = new GridLength(62); } else { m.CMdmgHC.Width = temp; }
            }
            if (AtkName.IsChecked) { m.CombatantView.Columns.Add(m.MaxHitColumn); m.CAtkHC.Width = new GridLength(1.7, GridUnitType.Star); } else { m.CAtkHC.Width = temp; }
            if (Tabchk.IsChecked) { m.TabHC.Width = new GridLength(30); m.CTabHC.Width = new GridLength(30); } else { m.TabHC.Width = temp; m.CTabHC.Width = temp; }
            Properties.Settings.Default.ListName = PlayerName.IsChecked;
            Properties.Settings.Default.ListPct = Percent.IsChecked;
            Properties.Settings.Default.ListTS = TScore.IsChecked;
            Properties.Settings.Default.ListDmg = Damage.IsChecked;
            Properties.Settings.Default.ListDmgd = Damaged.IsChecked;
            Properties.Settings.Default.ListDPS = PlayerDPS.IsChecked;
            Properties.Settings.Default.ListJA = PlayerJA.IsChecked;
            Properties.Settings.Default.ListCri = Critical.IsChecked;
            Properties.Settings.Default.ListHit = MaxHit.IsChecked;
            Properties.Settings.Default.ListAtk = AtkName.IsChecked;
            Properties.Settings.Default.ListTab = Tabchk.IsChecked;
            Properties.Settings.Default.Variable = Variable.IsChecked;
            // - - - -

            Properties.Settings.Default.Save();
            DialogResult = true;
        }

        private void Japanese_Click(object sender, RoutedEventArgs e) => Properties.Settings.Default.Language = "ja-JP";
        private void English_Click(object sender, RoutedEventArgs e) => Properties.Settings.Default.Language = "en-US";


        private void FontSizeBox_TextChanged(object sender, RoutedEventArgs e)
        {
            if (FontSizeBox.Text == null) { return; }
            if (double.TryParse(FontSizeBox.Text, out double resultvalue) && 1 < resultvalue) { mainwindow.CombatantData.FontSize = resultvalue; }
        }

        private void RadioColor_Checked(object sender, RoutedEventArgs e)
        {
            MainWindow mainwindow = (MainWindow)Application.Current.MainWindow;
            Properties.Settings.Default.BackContent = "Color";
            mainwindow.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF0A0A0A"));
        }


        private void BackColor_Click(object sender, RoutedEventArgs e)
        {
            SelectColor color = new SelectColor((Color)ColorConverter.ConvertFromString(Properties.Settings.Default.BackColor)) { Owner = this };
            color.ShowDialog();
            if (color.DialogResult == true)
            {
                BackColorInput.Content = color.ResultColor.ToString();
                BackPreview.Fill = new SolidColorBrush(color.ResultColor);
                Properties.Settings.Default.BackColor = color.ResultColor.ToString();
                mainwindow.CombatantData.Background = new SolidColorBrush(color.ResultColor);
            }
        }

        private void RadioImage_Checked(object sender, RoutedEventArgs e)
        {
            MainWindow mainwindow = (MainWindow)Application.Current.MainWindow;
            Properties.Settings.Default.BackContent = "Image";
            try
            {
                BitmapImage bitmap = new BitmapImage(new Uri(Properties.Settings.Default.ImagePath));
                ImageBrush brush = new ImageBrush
                {
                    ImageSource = bitmap,
                    Stretch = Stretch.UniformToFill
                };
                mainwindow.Background = brush;
            } catch { mainwindow.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF0A0A0A")); }
        }

        private void ImageSelect_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainwindow = (MainWindow)Application.Current.MainWindow;
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Image(*.png,*.jpg,*.jpeg,*.gif,*.bmp)|*.png;*.jpg;*.jpeg;*.gif;*.bmp"
            };
            if (dialog.ShowDialog() == false) { return; }
            Properties.Settings.Default.ImagePath = dialog.FileName;
            try
            {
                BitmapImage bitmap = new BitmapImage(new Uri(Properties.Settings.Default.ImagePath));
                ImageBrush brush = new ImageBrush
                {
                    ImageSource = bitmap,
                    Stretch = Stretch.UniformToFill
                };
                mainwindow.Background = brush;
                //PreviewImage.Source = brush;
            } catch { mainwindow.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF0A0A0A")); }
        }


        private void WindowOpacity_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (IsSetting)
            {
                mainwindow.TheWindow.Opacity = WindowOpacity.Value;
                Properties.Settings.Default.WindowOpacity = WindowOpacity.Value;
            }
        }

        private void BackOpacity_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (IsSetting)
            {
                mainwindow.Background.Opacity = BackOpacity.Value;
                Properties.Settings.Default.ListOpacity = BackOpacity.Value;
            }
        }

        private void Graph_Click(object sender, RoutedEventArgs e) => Properties.Settings.Default.ShowDamageGraph = Graph.IsChecked;
        private void Highlight_Click(object sender, RoutedEventArgs e) => Properties.Settings.Default.HighlightYourDamage = Highlight.IsChecked;
        private void DamageSI_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.DamageSI = DamageSI.IsChecked;
            if (Properties.Settings.Default.ListDmg)
            {
                if (Properties.Settings.Default.DamageSI) { mainwindow.CDmgHC.Width = new GridLength(47); } else { mainwindow.CDmgHC.Width = new GridLength(78); }
            } else { mainwindow.CombatantView.Columns.Remove(mainwindow.DamageColumn); mainwindow.CDmgHC.Width = new GridLength(0); }
        }

        private void DPSSI_Click(object sender, RoutedEventArgs e) => Properties.Settings.Default.DPSSI = DamageSI.IsChecked;

        private void MaxSI_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.MaxSI = MaxSI.IsChecked;
            if (Properties.Settings.Default.ListHit)
            {
                if (Properties.Settings.Default.MaxSI) { mainwindow.CDmgHC.Width = new GridLength(47); } else { mainwindow.CDmgHC.Width = new GridLength(62); }
            } else { mainwindow.CombatantView.Columns.Remove(mainwindow.HColumn); mainwindow.CMdmgHC.Width = new GridLength(0); }
        }

        private void LowResources_Click(object sender, RoutedEventArgs e)
        {
            Process thisProcess = Process.GetCurrentProcess();
            Properties.Settings.Default.LowResources = LowResources.IsChecked;
            if (Properties.Settings.Default.LowResources)
            {
                thisProcess.PriorityClass = ProcessPriorityClass.Idle;
            } else
            {
                thisProcess.PriorityClass = ProcessPriorityClass.Normal;
            }
        }

        private void CPUdraw_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.CPUdraw = CPUdraw.IsChecked;
            if (Properties.Settings.Default.CPUdraw)
            {
                RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;
            } else
            {
                RenderOptions.ProcessRenderMode = RenderMode.Default;
            }
        }

        private void Clock_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Clock = Clock.IsChecked;
            if (Properties.Settings.Default.Clock) { mainwindow.Datetime.Visibility = Visibility.Visible; } else { mainwindow.Datetime.Visibility = Visibility.Collapsed; }
        }

        private void ResetOverParse(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("OverParseをリセットしますか？\n設定は消去されますが、ログは消去されません。", "OverParse Setup", MessageBoxButton.YesNo, MessageBoxImage.Information);
            if (result != MessageBoxResult.Yes) { return; }

            //Resetting
            Properties.Settings.Default.Reset();
            //Properties.Settings.Default.ResetInvoked = true;
            Properties.Settings.Default.Save();

            Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }

        private void LowResources_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e) =>
            Description.Text = "CPUの基本割り込み処理優先度を下げ、他のプロセスへ処理リソースを譲渡します。\n処理が追いついている(アイドル時間が存在している)場合は影響ありません\nOverParseの処理が止まる場合があります。";

        private void CPUdraw_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e) =>
            Description.Text = "強制的にソフトウェアレンダリングへ切り替えます。\n画面出力がdGPUの場合は画面合成時にCPU=>GPUへの転送が発生する為逆効果になります";

        private void Clock_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e) =>
            Description.Text = "デバッグ用\nUpdateForm();が発生したタイミングで現在のPC時刻を表示します";

        //読み上げ設定
        private void Bouyomi_Click(object sender, RoutedEventArgs e) => Properties.Settings.Default.Bouyomi = Bouyomi.IsChecked;
        private void BouyomiFormat_Click(object sender, RoutedEventArgs e) => Properties.Settings.Default.BouyomiFormat = BouyomiFormat.IsChecked;

        private void ListColor_Click(object sender, RoutedEventArgs e)
        {
            SelectColor listcolor = new SelectColor((Color)ColorConverter.ConvertFromString(Properties.Settings.Default.FontColor)) { Owner = this };
            listcolor.ShowDialog();
            if (listcolor.DialogResult == true)
            {
                TextColorBox.Content = listcolor.ResultColor.ToString();
                TextColorBox.Foreground = new SolidColorBrush(listcolor.ResultColor);
                mainwindow.CombatantData.Foreground = new SolidColorBrush(listcolor.ResultColor);
                Properties.Settings.Default.FontColor = listcolor.ResultColor.ToString();
            }
        }

        private void UIColor_Click(object sender, RoutedEventArgs e)
        {
            SelectColor uicolor = new SelectColor((Color)ColorConverter.ConvertFromString(Properties.Settings.Default.Foreground)) { Owner = this };
            uicolor.ShowDialog();
            if (uicolor.DialogResult == true)
            {
                ForegroundUIColor.Content = uicolor.ResultColor.ToString();
                ForegroundUIColor.Foreground = new SolidColorBrush(uicolor.ResultColor);
                Properties.Settings.Default.Foreground = uicolor.ResultColor.ToString();
                mainwindow.BindingGroup.UpdateSources();
            }
        }

        private void GitHub_Click(object sender, RoutedEventArgs e) => Process.Start("https://github.com/Remon-7L");
        private void Twitter_Click(object sender, RoutedEventArgs e) => Process.Start("https://twitter.com/Remon_7L");

        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed) { DragMove(); }
        }

        private void Close_Click(object sender, RoutedEventArgs e) => SystemCommands.CloseWindow(this);

        private void JA_Checked(object sender, RoutedEventArgs e) => Properties.Settings.Default.Language = "ja-JP";
        private void EN_Checked(object sender, RoutedEventArgs e) => Properties.Settings.Default.Language = "en-US";

        private void IP_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (IsSetting)
            {
                if (IPAddress.TryParse(IP.Text, out IPAddress addr))
                {
                    IPResult.Content = "OK"; IPResult.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 255, 0));
                    Properties.Settings.Default.BouyomiIP = addr.ToString();
                    ResultURI.Content = addr.ToString() + " : " + Properties.Settings.Default.BouyomiPort;
                } else
                {
                    IPResult.Content = "NG"; IPResult.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
                }
            }
        }
    }

    public class FontNameConverter : IValueConverter
    {
        public object Convert(object value, Type type, object parameter, System.Globalization.CultureInfo culture)
        {
            var v = value as FontFamily;
            var currentLang = XmlLanguage.GetLanguage(culture.IetfLanguageTag);
            return v.FamilyNames.FirstOrDefault(o => o.Key == currentLang).Value ?? v.Source;
        }

        public object ConvertBack(object value, Type type, object parameter, System.Globalization.CultureInfo culture) => throw new NotImplementedException();
    }

}
