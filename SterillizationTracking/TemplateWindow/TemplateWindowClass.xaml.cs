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
using System.Windows.Shapes;

namespace SterillizationTracking.TemplateWindow
{
    /// <summary>
    /// Interaction logic for TemplateWindowClass.xaml
    /// </summary>
    public partial class TemplateWindowClass : Window
    {
        public TemplateWindowClass()
        {
            InitializeComponent();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            TemplateComboBox.IsEnabled = true;
            DeleteTemplateButton.IsEnabled = true;
        }
        private void CheckBox_UnChecked(object sender, RoutedEventArgs e)
        {
            TemplateComboBox.IsEnabled = false;
            DeleteTemplateButton.IsEnabled = false;
        }

        private void BuildTemplateButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
