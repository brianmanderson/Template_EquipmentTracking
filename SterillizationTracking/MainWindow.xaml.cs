using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using SterillizationTracking.Kit_Classes;
using SterillizationTracking.StackPanelClasses;
using SterillizationTracking.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using SterillizationTracking.TemplateWindow;
using System.Text.Json;
using System.Collections.ObjectModel;
using DocumentFormat.OpenXml.ExtendedProperties;

namespace SterillizationTracking
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private List<string> _kit_numbers = new List<string> { "" };

        public string base_directory = @"\\lantis1.unch.unc.edu\research_non_phi\Brachy";
        public string applicator_path_file = @"Applicator_Path.txt";
        public string applicator_directory;
        public string template_file;
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
        private TemplateKitLibrary library;
        public ObservableCollection<TemplateKit> Kits;
        public ObservableCollection<string> Kitnames;
        public MainWindow()
        {
            InitializeComponent();
            if (File.Exists(applicator_path_file))
            {
                read_file();
            }
        }
        public void read_file()
        {
            base_directory = File.ReadLines(applicator_path_file).FirstOrDefault();
            template_file = Path.Combine(base_directory, "Templates.json");
            applicator_directory = base_directory;
            Rebuild_From_Files(applicator_directory);
            rebuild_library();
        }
        public void bind()
        {
            System.Windows.Data.Binding number_binding = new System.Windows.Data.Binding("Kit_Numbers");
            number_binding.Source = this;
            KitNumber_ComboBox.SetBinding(ItemsControl.ItemsSourceProperty, number_binding);
            KitNumber_ComboBox.SelectedIndex = 0;

            System.Windows.Data.Binding kit_name_binding = new System.Windows.Data.Binding("Kits");
            kit_name_binding.Source = library;
            Kit_ComboBox.DisplayMemberPath = "Name";
            Kit_ComboBox.SetBinding(ItemsControl.ItemsSourceProperty, kit_name_binding);

            Kitnames = new ObservableCollection<string>
            {
                "Show all"
            };
            foreach (TemplateKit template in library.Kits)
            {
                Kitnames.Add(template.Name);
            }
            FilterNameComboBox.ItemsSource = Kitnames;
        }

        public void Add_Kit(TemplateKit template_kit, string kit_number, string file_path)
        {
            BaseKit new_kit = new BaseKit(name: template_kit.Name, kitnumber: kit_number, file_path: file_path);
            foreach (TemplatePart part in template_kit.Parts)
            {
                new_kit.add_kit(part.Total_Uses, part.Warning_Uses, part.Description);
            }
            new_kit.write_file();
        }
        public void Add_Existing_Kit(string kit_name, string kit_number, string file_path)
        {
            BaseKit new_kit = new BaseKit(kit_name, kit_number, file_path);
            new_kit.build_from_file();
            AddKitRow new_row = new AddKitRow(new_kit);
            KitStackPanel.Children.Add(new_row);
        }
        private void Add_Kit_Button_Click(object sender, RoutedEventArgs e)
        {
            TemplateKit kit = (TemplateKit)Kit_ComboBox.SelectedItem;
            kit_number = Kit_Numbers[KitNumber_ComboBox.SelectedIndex];
            Add_Kit(kit, kit_number: kit_number, file_path: applicator_directory);
            CheckNumberList number_returner = new CheckNumberList();
            Kit_Numbers = number_returner.return_list(kit_name: kit_name, file_path: applicator_directory);
            Rebuild_From_Files(applicator_directory);
            bind();
        }

        public void rebuild_library()
        {
            if (File.Exists(template_file))
            {
                string jsonString = File.ReadAllText(template_file);
                library = JsonSerializer.Deserialize<TemplateKitLibrary>(jsonString);
            }
            else
            {
                library = new TemplateKitLibrary();
                library.Kits = new ObservableCollection<TemplateKit>();
            }
            bind();
        }
        public void Rebuild_From_Files(string path)
        {
            KitStackPanel.Children.Clear();
            string[] applicator_list = Directory.GetDirectories(path);
            string[] kit_list;
            string actual_kit_number;
            string directory_kit_number;
            foreach (string directory_kit_name in applicator_list)
            {
                string[] temp_list = directory_kit_name.Split('\\');
                string applicator_name = temp_list[temp_list.Length - 1];

                string full_applicator_path = Path.Combine(path, directory_kit_name);
                kit_list = Directory.GetDirectories(full_applicator_path);
                if (kit_list.Length > 0)
                {
                    foreach (string directory_kit_number_path in kit_list)
                    {
                        directory_kit_number = directory_kit_number_path.Split(full_applicator_path)[1];
                        if (directory_kit_number.Contains("Kit"))
                        {
                            actual_kit_number = directory_kit_number.Split(' ')[1];
                            Add_Existing_Kit(applicator_name, actual_kit_number, path);
                        }
                    }
                }
            }
        }
        public void Rebuild_From_Applicator_path(string path, string directory_kit_name)
        {
            KitStackPanel.Children.Clear();
            string[] kit_list;
            string actual_kit_number;
            string directory_kit_number;
            string full_applicator_path = Path.Combine(path, directory_kit_name);
            if (Directory.Exists(full_applicator_path))
            {
                kit_list = Directory.GetDirectories(full_applicator_path);
                if (kit_list.Length > 0)
                {
                    foreach (string directory_kit_number_path in kit_list)
                    {
                        directory_kit_number = directory_kit_number_path.Split(full_applicator_path)[1];
                        if (directory_kit_number.Contains("Kit"))
                        {
                            actual_kit_number = directory_kit_number.Split(' ')[1];
                            Add_Existing_Kit(directory_kit_name, actual_kit_number, path);
                        }
                    }
                }
            }
        }
        private void Kit_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Kit_ComboBox.SelectedIndex == -1) 
            {
                return;
            }
            kit_name = ((TemplateKit)Kit_ComboBox.SelectedItem).Name;
            KitNumber_ComboBox.IsEnabled = true;
            CheckNumberList number_returner = new CheckNumberList();
            Kit_Numbers = number_returner.return_list(kit_name: kit_name, file_path: applicator_directory);
            bind();
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
            string filter_kit_selection_info = (string)FilterNameComboBox.SelectedItem;
            if (filter_kit_selection_info.Contains("Show all"))
            {
                Rebuild_From_Files(applicator_directory);
            }
            else
            {
                Rebuild_From_Applicator_path(applicator_directory, filter_kit_selection_info);
            }
        }

        private void CreateTemplate_Button_Click(object sender, RoutedEventArgs e)
        {
            TemplateWindowClass template_window = new TemplateWindowClass(base_directory);
            template_window.ShowDialog();
            rebuild_library();
            bind();
        }

        private void Change_Directory_Button_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string folder = dialog.SelectedPath;
                List<string> lines = new List<string>
                {
                    folder
                };
                File.WriteAllLines(applicator_path_file, lines);
                read_file();
            }
        }
    }
}
