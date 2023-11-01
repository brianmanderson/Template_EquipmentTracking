using System;
using System.IO;
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
using SterillizationTracking.Kit_Classes;
using SterillizationTracking.StackPanelClasses;
using SterillizationTracking.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SterillizationTracking
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private List<string> _kit_numbers = new List<string> { "" };
        private List<string> _kit_names = new List<string> { "Select an applicator", "Cylinder", "Cervix Applicator Set", "Needle Kit", "Segmented Cylinder", 
            "Tandem and Ovoid", "Tandem and Ring", "Y Applicator"};
        private List<string> _filter_kit_names = new List<string> { "All applicators", "Cylinder", "Cervix Applicator Set", "Needle Kit", "Segmented Cylinder",
            "Tandem and Ovoid", "Tandem and Ring", "Y Applicator"};

        public string applicator_directory = @"\\lantis1.unch.unc.edu\research_non_phi\Brachy";  //\\ucsdhc-varis2\radonc$\HDR updates\Steralization_Kits_Tracking\Kit_Status
        public string kit_name;
        public string kit_number;
        public List<string> Kit_Numbers
        {
            get {
                return _kit_numbers; 
            }
            set
            {
                _kit_numbers = value;
                OnPropertyChanged("Kit_Numbers");
            }
        }
        public List<string> Filter_Kit_Names
        {
            get
            {
                return _filter_kit_names;
            }
            set
            {
                _filter_kit_names = value;
                OnPropertyChanged("Filter_Kit_Names");
            }
        }
        public List<string> Kit_Names
        {
            get
            {
                return _kit_names;
            }
            set
            {
                _kit_names = value;
                OnPropertyChanged("Kit_Names");
            }
        }
        public MainWindow()
        {
            InitializeComponent();
            if (!Directory.Exists(applicator_directory))
            {
                Directory.CreateDirectory(applicator_directory);
            }
            Rebuild_From_Files();
            Binding number_binding = new Binding("Kit_Numbers");
            number_binding.Source = this;
            KitNumber_ComboBox.SetBinding(ComboBox.ItemsSourceProperty, number_binding);

            Binding kit_name_binding = new Binding("Kit_Names");
            Kit_Names = _kit_names;
            kit_name_binding.Source = this;
            Kit_ComboBox.SetBinding(ComboBox.ItemsSourceProperty, kit_name_binding);

            Binding filter_kit_binding = new Binding("Filter_Kit_Names");
            Filter_Kit_Names = _filter_kit_names;
            filter_kit_binding.Source = this;
            FilterNameComboBox.SetBinding(ComboBox.ItemsSourceProperty, filter_kit_binding);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }

        public void Add_Kit(string kit_name, string kit_number, string file_path)
        {
            BaseKit new_kit = new BaseKit(name: kit_name, kitnumber: kit_number, file_path: file_path);
            AddKitRow new_row = new AddKitRow(new_kit);
            KitStackPanel.Children.Add(new_row);
        }
        private void Add_Kit_Button_Click(object sender, RoutedEventArgs e)
        {
            kit_name = Kit_Names[Kit_ComboBox.SelectedIndex];
            kit_number = Kit_Numbers[KitNumber_ComboBox.SelectedIndex];
            Add_Kit(kit_name: kit_name, kit_number: kit_number, file_path: applicator_directory);
            Kit_ComboBox.SelectedIndex = 0;
        }

        public void Rebuild_From_Files()
        {
            KitStackPanel.Children.Clear();
            string[] applicator_list = Directory.GetDirectories(applicator_directory);
            string[] kit_list;
            string actual_kit_number;
            string directory_kit_number;
            foreach (string directory_kit_name in applicator_list)
            {
                string[] temp_list = directory_kit_name.Split('\\');
                string applicator_name = temp_list[temp_list.Length - 1];

                string full_applicator_path = System.IO.Path.Combine(applicator_directory, directory_kit_name);
                kit_list = Directory.GetDirectories(full_applicator_path);
                if (kit_list.Length > 0)
                {
                    foreach (string directory_kit_number_path in kit_list)
                    {
                        directory_kit_number = directory_kit_number_path.Split(full_applicator_path)[1];
                        if (directory_kit_number.Contains("Kit"))
                        {
                            actual_kit_number = directory_kit_number.Split(' ')[1];
                            Add_Kit(kit_name: applicator_name, kit_number: actual_kit_number, file_path: applicator_directory);
                        }
                    }
                }
            }

        }
        public void Rebuild_From_Files(string filter_applicator)
        {
            KitStackPanel.Children.Clear();
            string[] applicator_list = Directory.GetDirectories(applicator_directory);
            string[] kit_list;
            string actual_kit_number;
            string directory_kit_number;
            foreach (string directory_kit_name in applicator_list)
            {
                string actual_kit_name = directory_kit_name.Split(applicator_directory + '\\')[1];
                if (actual_kit_name != filter_applicator)
                {
                    continue;
                }
                string[] temp_list = directory_kit_name.Split('\\');
                string applicator_name = temp_list[temp_list.Length - 1];

                string full_applicator_path = System.IO.Path.Combine(applicator_directory, directory_kit_name);
                kit_list = Directory.GetDirectories(full_applicator_path);
                if (kit_list.Length > 0)
                {
                    foreach (string directory_kit_number_path in kit_list)
                    {
                        directory_kit_number = directory_kit_number_path.Split(full_applicator_path)[1];
                        if (directory_kit_number.Contains("Kit"))
                        {
                            actual_kit_number = directory_kit_number.Split(' ')[1];
                            Add_Kit(kit_name: applicator_name, kit_number: actual_kit_number, file_path: applicator_directory);
                        }
                    }
                }
            }

        }
        private void Kit_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            kit_name = Kit_Names[Kit_ComboBox.SelectedIndex];
            if (kit_name.Contains("Select an applicator"))
            {
                if (KitNumber_ComboBox != null)
                {
                    KitNumber_ComboBox.IsEnabled = false;
                    Kit_Numbers = new List<string> { "" };
                }
                Add_Kit_Button.IsEnabled = false;
            }
            else
            {
                KitNumber_ComboBox.IsEnabled = true;
                CheckNumberList number_returner = new CheckNumberList();
                Kit_Numbers = number_returner.return_list(kit_name: kit_name, file_path: applicator_directory);
                KitNumber_ComboBox.SelectedIndex = 0;
            }
        }

        private void KitNumber_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string kitnumber_selection_info = Convert.ToString(KitNumber_ComboBox.SelectedItem);
            if (kitnumber_selection_info.Contains("Select"))
            {
                Add_Kit_Button.IsEnabled = false;

            }
            else
            {
                Add_Kit_Button.IsEnabled = true;
            }
        }

        private void FilterNameComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string filter_kit_selection_info = Convert.ToString(FilterNameComboBox.SelectedItem);
            if (filter_kit_selection_info.Contains("applicators"))
            {
                Rebuild_From_Files();

            }
            else
            {
                Rebuild_From_Files(filter_kit_selection_info);
            }
        }

    }
}
