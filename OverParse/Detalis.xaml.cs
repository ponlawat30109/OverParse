using System.Windows;

namespace OverParse
{
    /// <summary>
    /// Detalis.xaml の相互作用ロジック
    /// </summary>
    public partial class Detalis : Window
    {
        public Detalis(Combatant data)
        {
            InitializeComponent();
            //Testing...
            Data1.Content = data.ID;
            Data2.Content = data.Name;
            Data3.Content = data.Damage;
            Data4.Content = data.MaxHit;
        }
    }
}
