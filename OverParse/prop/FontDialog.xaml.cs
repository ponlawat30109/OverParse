using System;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace OverParse
{
    /// <summary>
    /// FontDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class FontDialog : Window
    {
        public string fontsize;
        public string fontcolor;

        public FontDialog()
        {
            InitializeComponent();
            Language = XmlLanguage.GetLanguage(Thread.CurrentThread.CurrentCulture.Name);
            FontList.DataContext = Fonts.SystemFontFamilies.ToList();

            SampleList.Items.Add(null);
            SampleList.FontFamily = new FontFamily(Properties.Settings.Default.Font);
            FontSizeBox.TextChanged += FontSizeBox_TextChanged;
            TextColorBox.TextChanged += TextColorBox_TextChanged;
            FontSizeBox.Text = "12.0";
            TextColorBox.Text = Properties.Settings.Default.FontColor;
        }

        private void FontList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FontList.Items.Count < 1) { return; }
            Properties.Settings.Default.Font = FontList.SelectedItem.ToString();
            SampleList.FontFamily = (FontFamily)FontList.SelectedItem;
        }

        private void OK_button_Click(object sender, RoutedEventArgs e)
        {
            if (FontList.SelectedItem != null) { Properties.Settings.Default.Font = FontList.SelectedItem.ToString(); }

            if (TextColorBox.Text == null) { fontcolor = "#FFFFFFFF"; return; }
            try
            {
                Color color = (Color)ColorConverter.ConvertFromString(TextColorBox.Text);
                fontcolor = TextColorBox.Text;
                Properties.Settings.Default.FontColor = TextColorBox.Text;
            } catch {
                fontcolor = "#FFFFFFFF";
            }
            DialogResult = true;
        }

        private void FontSizeBox_TextChanged(object sender, RoutedEventArgs e)
        {
            if (FontSizeBox.Text == null) { return; }
            if (double.TryParse(FontSizeBox.Text, out double resultvalue) && 1 < resultvalue) { SampleList.FontSize = resultvalue; }
        }

        private void TextColorBox_TextChanged(object sender, RoutedEventArgs e)
        {;
            if (TextColorBox.Text == null) { return; }
            try {
                Color color = (Color)ColorConverter.ConvertFromString(TextColorBox.Text);
                SampleList.Foreground = new SolidColorBrush(color);
            } catch {
            }
        }

    }

    public class FontNameConverter : IValueConverter
    {
        public object Convert(object value, Type type,object parameter, System.Globalization.CultureInfo culture)
        {
            var v = value as FontFamily;
            var currentLang = XmlLanguage.GetLanguage(culture.IetfLanguageTag);
            return v.FamilyNames.FirstOrDefault(o => o.Key == currentLang).Value ?? v.Source;
        }

        public object ConvertBack(object value, Type type, object parameter, System.Globalization.CultureInfo culture) => throw new NotImplementedException();
    }

}
