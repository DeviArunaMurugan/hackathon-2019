using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Flutter_Publish_Utility
{
    /// <summary>
    /// Interaction logic for ScreenCompare.xaml
    /// </summary>
    public partial class ScreenCompare : Page
    {
        public ScreenCompare()
        {
            InitializeComponent();
            ComboBox1.SelectedIndex = 0;
        }

        private void ComboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ComboBox).SelectedIndex == 0)
            {
                screen_compare.Content = "Take Screenshot";
            }
            else
            {
                screen_compare.Content = "Compare";
            }
        }
    }

  
}
