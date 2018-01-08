using System.Windows;

namespace OverParse
{
    /// <summary>
    /// Details.xaml の相互作用ロジック
    /// </summary>
    public partial class Details : Window
    {
        public Details(string data1,string data2)
        {
            InitializeComponent();
            Data1.Content = data1;
            Data2.Content = data2;
        }
    }
}
