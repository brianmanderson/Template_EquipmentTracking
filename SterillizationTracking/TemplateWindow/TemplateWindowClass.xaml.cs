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
    public class TopRow : StackPanel
    {
        public TopRow()
        {
            Orientation = Orientation.Horizontal;
            Label desc = new Label();
            desc.Width = 150;
            desc.Content = "Template Name";

            Label add_part = new Label();
            add_part.Content = "Add Part?";
            add_part.Width = 75;

            Label desc_part = new Label();
            desc_part.Width = 150;
            desc_part.Content = "Part Description";

            Label uses = new Label();
            uses.Width = 50;
            uses.Content = "Uses?";

            Label warning = new Label();
            warning.Width = 70;
            warning.Content = "Warning?";

            Label delete_part = new Label();
            delete_part.Width = 75;
            delete_part.Content = "X Part?";

            Label delete_template = new Label();
            delete_template.Width = 100;
            delete_template.Content = "Delete Template?";

            Children.Add(desc);
            Children.Add(add_part);
            Children.Add(desc_part);
            Children.Add(uses);
            Children.Add(warning);
            Children.Add(delete_part);
            Children.Add(delete_template);
        }
    }
    public class TemplateClassStackPanel : StackPanel
    {
        Brush red = new SolidColorBrush(Color.FromRgb(255, 0, 0));
        public TextBox TotalUseText, WarningUseText, DescriptionText, text_box;
        public ObservableCollection<TemplateKit> Templates;
        public TemplateKit template;

        public TemplateClassStackPanel(ObservableCollection<TemplateKit> templates, TemplateKit template)
        {
            Orientation = Orientation.Horizontal;
            this.template = template;
            Templates = templates;
            build();
        }

        private void build()
        {
            Children.Clear();

            DescriptionText = new TextBox();
            DescriptionText.Width = 150;
            DescriptionText.Text = "Template Name";
            DescriptionText.TextAlignment = TextAlignment.Center;
            DescriptionText.VerticalContentAlignment = VerticalAlignment.Center;
            Binding name_binding = new Binding(path: "Name");
            name_binding.Mode = BindingMode.TwoWay;
            name_binding.Source = template;
            DescriptionText.SetBinding(TextBox.TextProperty, name_binding);
            Children.Add(DescriptionText);

            Button add_button = new Button();
            add_button.Content = "Add Part?";
            add_button.Width = 75;
            add_button.Click += Add_button_Click;
            Children.Add(add_button);


            StackPanel stackPanel_all = new StackPanel();
            stackPanel_all.Orientation = Orientation.Vertical;
            foreach (TemplatePart part in template.Parts)
            {
                StackPanel stackPanel = new StackPanel();
                stackPanel.Orientation = Orientation.Horizontal;
                text_box = new TextBox();
                text_box.Text = "Description";
                Binding description_binding = new Binding(path: "Description");
                description_binding.Mode = BindingMode.TwoWay;
                description_binding.Source = part;
                text_box.SetBinding(TextBox.TextProperty, description_binding);
                text_box.Width = 150;
                text_box.Padding = new Thickness(10);
                stackPanel.Children.Add(text_box);

                text_box = new TextBox();
                Binding total_use_binding = new Binding(path: "Total_Uses");
                total_use_binding.Mode = BindingMode.TwoWay;
                total_use_binding.Source = part;
                text_box.SetBinding(TextBox.TextProperty, total_use_binding);
                text_box.Width = 50;
                text_box.Padding = new Thickness(10);
                stackPanel.Children.Add(text_box);

                text_box = new TextBox();
                Binding warning_use_binding = new Binding(path: "Warning_Uses");
                warning_use_binding.Mode = BindingMode.TwoWay;
                warning_use_binding.Source = part;
                text_box.SetBinding(TextBox.TextProperty, warning_use_binding);
                text_box.Width = 70;
                text_box.Padding = new Thickness(10);
                stackPanel.Children.Add(text_box);

                Button part_delete_button = new Button();
                part_delete_button.Content = "X Part";
                part_delete_button.Background = red;
                part_delete_button.Width = 75;
                part_delete_button.Tag = part;
                part_delete_button.Click += Delete_part_button_Click;
                stackPanel.Children.Add(part_delete_button);
                stackPanel_all.Children.Add(stackPanel);
            }
            Children.Add(stackPanel_all);

            Button delete_button = new Button();
            delete_button.Content = "Delete Template?";
            delete_button.Background = red;
            delete_button.Width = 100;
            delete_button.Click += Delete_button_Click;
            Children.Add(delete_button);
        }
        private void Delete_part_button_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            TemplatePart part = (TemplatePart)button.Tag;
            template.Parts.Remove(part);
            build();
        }
        private void Add_button_Click(object sender, RoutedEventArgs e)
        {
            TemplatePart part = new TemplatePart();
            part.Total_Uses = 0;
            part.Warning_Uses = 0;
            part.Description = "";
            template.Parts.Add(part);
            build();
        }
        private void Delete_button_Click(object sender, RoutedEventArgs e)
        {
            Templates.Remove(template);
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
        public TemplateWindowClass(string base_directory)
        {
            InitializeComponent();
            template_path = Path.Join(base_directory, "Templates.json");
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
            update_view();
        }
        private void update_view()
        {
            KitStackPanel.Children.Clear();
            KitStackPanel.Children.Add(new TopRow());
            foreach (TemplateKit template in TemplateKitList)
            {
                TemplateClassStackPanel panel = new TemplateClassStackPanel(TemplateKitList, template);
                KitStackPanel.Children.Add(panel);
            }
        }
        private void AddTemplate_Click(object sender, RoutedEventArgs e)
        {
            TemplateKit kit = new TemplateKit();
            kit.Name = "";
            kit.Parts = new List<TemplatePart>();
            TemplatePart part = new TemplatePart();
            part.Total_Uses = 0;
            part.Warning_Uses = 0;
            part.Description = "";
            kit.Parts.Add(part);
            TemplateKitList.Add(kit);
            update_view();
        }
        private void Update(object sender, RoutedEventArgs e)
        {
            update_view();
        }

        private void DeleteTemplateButton_Click(object sender, RoutedEventArgs e)
        {
            //TemplateKit selected_kit = (TemplateKit)TemplateComboBox.SelectedItem;
        }

        private void TemplateNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            update_view();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SaveExitButton_Click(object sender, RoutedEventArgs e)
        {
            TemplateKitLibrary library = new TemplateKitLibrary
            {
                Kits = TemplateKitList
            };
            string json_string = JsonSerializer.Serialize(library);
            File.WriteAllText(template_path, json_string);
            Close();
        }
    }
}
