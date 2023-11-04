using SterillizationTracking.Kit_Classes;
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
using System.Text.Json;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SterillizationTracking.TemplateWindow
{
    /// <summary>
    /// Interaction logic for TemplateWindowClass.xaml
    /// </summary>
    /// 
    public class PartClass : StackPanel
    {
        Brush red = new SolidColorBrush(Color.FromRgb(255, 0, 0));
        public Label TotalUse, WarningUse, Description;
        public TextBox TotalUseText, WarningUseText, DescriptionText;
        public List<PartClass> Parts;
        public PartClass(List<PartClass> parts)
        {
            Orientation = Orientation.Horizontal;
            Description = new Label();
            Description.Content = "Description";
            Description.Width = 75;
            Children.Add(Description);

            DescriptionText = new TextBox();
            DescriptionText.Width = 150;
            Children.Add(DescriptionText);

            TotalUse = new Label();
            TotalUse.Content = "Total Uses";
            TotalUse.Width = 65;
            Children.Add(TotalUse);

            TotalUseText = new TextBox();
            TotalUseText.Width = 30;
            Children.Add(TotalUseText);

            WarningUse = new Label();
            WarningUse.Content = "Warning Uses";
            WarningUse.Width = 85;
            Children.Add(WarningUse);

            WarningUseText = new TextBox();
            WarningUseText.Width = 30;
            Children.Add(WarningUseText);

            Button delete_button = new Button();
            delete_button.Content = "Delete?";
            delete_button.Background = red;
            delete_button.Width = 55;
            delete_button.Click += Delete_button_Click;
            Children.Add(delete_button);
            Parts = parts;
        }

        private void Delete_button_Click(object sender, RoutedEventArgs e)
        {
            Parts.Remove(this);
            Children.Clear();
        }
    }
    public partial class TemplateWindowClass : Window
    {
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
        public string template_path;
        public ObservableCollection<TemplateKit> TemplateKitList
        {
            get { return templateKitList; }
            set
            {
                templateKitList = value;
                OnPropertyChanged("TemplateKitList");
            }
        }
        private ObservableCollection<TemplateKit> templateKitList;
        public List<PartClass> stack_panel_parts;
        public TemplateWindowClass(string base_directory)
        {
            InitializeComponent();
            template_path = Path.Join(base_directory, "Templates.json");
            stack_panel_parts = new List<PartClass>();
            if (File.Exists(template_path))
            {
                string jsonString = File.ReadAllText(template_path);
                TemplateKitLibrary library = JsonSerializer.Deserialize<TemplateKitLibrary>(jsonString);
                TemplateKitList = library.Kits;
            }
            else
            {
                TemplateKitList = new ObservableCollection<TemplateKit>();
            }
            TemplateComboBox.ItemsSource = TemplateKitList;
            TemplateComboBox.DisplayMemberPath = "Name";
            check_status();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            DeleteTemplateButton.IsEnabled = true;
        }
        private void CheckBox_UnChecked(object sender, RoutedEventArgs e)
        {
            DeleteTemplateButton.IsEnabled = false;
        }

        private void BuildTemplateButton_Click(object sender, RoutedEventArgs e)
        {
            List<TemplatePart> parts = new List<TemplatePart>();
            foreach (PartClass part_class in stack_panel_parts)
            {
                int total_uses = Convert.ToInt32(part_class.TotalUseText.Text);
                int warning_uses = Convert.ToInt32(part_class.WarningUseText.Text);
                parts.Add(new TemplatePart
                {
                    Total_Uses = total_uses,
                    Warning_Uses = warning_uses,
                    Description = part_class.DescriptionText.Text
                });
            }
            TemplateKit template_kit = new TemplateKit
            {
                Name = TemplateNameTextBox.Text,
                Parts = parts,
            };
            TemplateKitList.Add(template_kit);
            stack_panel_parts.Clear();
            TemplateNameTextBox.Text = "";
            write_template();
            check_status();
        }
        private void write_template()
        {
            TemplateKitLibrary library = new TemplateKitLibrary
            {
                Kits = TemplateKitList
            };
            string json_string = JsonSerializer.Serialize(library);
            File.WriteAllText(template_path, json_string);
        }
        private void update_view()
        {
            KitStackPanel.Children.Clear();
            foreach (PartClass part in stack_panel_parts)
            {
                KitStackPanel.Children.Add(part);
            }
            if (TemplateKitList.Count > 0)
            {
                TemplateComboBox.SelectedIndex = 0;
            }
        }
        private void AddPart_Click(object sender, RoutedEventArgs e)
        {
            stack_panel_parts.Add(new PartClass(stack_panel_parts));
            check_status();
        }

        private void check_status()
        {
            BuildTemplateButton.IsEnabled = false;
            if (KitStackPanel.Children.Count > 0)
            {
                if (TemplateNameTextBox.Text != "")
                {
                    BuildTemplateButton.IsEnabled = true;
                }
            }
            update_view();
        }
        private void check_parts()
        {
            bool is_enabled;
            foreach (PartClass pc in KitStackPanel.Children)
            {
                if (pc.TotalUseText.Text == "")
                {
                    is_enabled = false;
                    break;
                }
                else if (pc.DescriptionText.Text == "")
                {
                    is_enabled = false;
                    break;
                }
                else if (pc.WarningUseText.Text == "")
                {
                    is_enabled = false;
                    break;
                }
            }
        }
        private void Update(object sender, RoutedEventArgs e)
        {
            check_status();
        }

        private void DeleteTemplateButton_Click(object sender, RoutedEventArgs e)
        {
            TemplateKit selected_kit = (TemplateKit)TemplateComboBox.SelectedItem;
            TemplateKitList.Remove(selected_kit);
            write_template();
        }

        private void TemplateNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            check_status();
        }
    }
}
