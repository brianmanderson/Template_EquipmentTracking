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
using SterillizationTracking.Kit_Classes;

namespace SterillizationTracking.StackPanelClasses
{
    class AddKitRow : StackPanel
    {
        public Button add_use_button, remove_use_button, reorder_button, reset_button, remove_button;
        public List<TextBox> text_boxes;
        public Label current_use_label, kit_label, kit_number_label, override_label, status_label, uses_left_label, last_updated, description_label;
        public CheckBox override_checkbox;
        public TextBox text_box;

        public AddKitRow(BaseKit new_kit)
        {
            Orientation = Orientation.Horizontal;

            kit_label = new Label();
            kit_label.Width = 200;
            Binding label_binding = new Binding("Name");
            label_binding.Source = new_kit;
            kit_label.SetBinding(Label.ContentProperty, label_binding);
            kit_label.Padding = new Thickness(10);
            Children.Add(kit_label);

            kit_number_label = new Label();
            Binding kit_number_label_binding = new Binding("KitNumber");
            kit_number_label_binding.Source = new_kit;
            kit_number_label.SetBinding(Label.ContentProperty, kit_number_label_binding);
            kit_number_label.Padding = new Thickness(10);
            Children.Add(kit_number_label);

            StackPanel stackPanel_all = new StackPanel();
            stackPanel_all.Orientation = Orientation.Vertical;
            text_boxes = new List<TextBox>();
            foreach (BaseOnePartKit kit in new_kit.Kits)
            {
                StackPanel stackPanel = new StackPanel();
                stackPanel.Orientation = Orientation.Horizontal;
                text_box = new TextBox();
                Binding description_binding = new Binding(path: "Description");
                description_binding.Mode = BindingMode.TwoWay;
                description_binding.Source = kit;
                description_binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                text_box.SetBinding(TextBox.TextProperty, description_binding);
                text_box.LostFocus += new_kit.update;
                text_box.Width = 150;
                text_box.Padding = new Thickness(10);
                text_box.IsReadOnly = true;
                text_boxes.Add(text_box);
                stackPanel.Children.Add(text_box);

                current_use_label = new Label();
                current_use_label.Width = 125;
                Binding myBinding = new Binding("CurrentUseString");
                myBinding.Source = kit;
                current_use_label.SetBinding(Label.ContentProperty, myBinding);

                Binding kit_colorBinding = new Binding("StatusColor");
                kit_colorBinding.Source = kit;
                current_use_label.SetBinding(Label.BackgroundProperty, kit_colorBinding);
                current_use_label.Padding = new Thickness(10);
                stackPanel.Children.Add(current_use_label);

                uses_left_label = new Label();
                uses_left_label.Width = 125;
                Binding usesleft_binding = new Binding("UsesLeftString");
                usesleft_binding.Source = kit;
                uses_left_label.SetBinding(Label.ContentProperty, usesleft_binding);
                stackPanel.Children.Add(uses_left_label);
                stackPanel_all.Children.Add(stackPanel);
            }
            Children.Add(stackPanel_all);
            add_use_button = new Button();
            Binding canAddBinding = new Binding("CanAdd");
            canAddBinding.Source = new_kit;
            add_use_button.Click += new_kit.add_use;
            add_use_button.Click += disable_add_use_button;
            add_use_button.SetBinding(Button.IsEnabledProperty, canAddBinding);
            add_use_button.Content = "Used?";
            add_use_button.Padding = new Thickness(10);
            Children.Add(add_use_button);

            remove_use_button = new Button();
            remove_use_button.Click += new_kit.remove_use;
            remove_use_button.Click += disable_remove_use_button;
            remove_use_button.Content = "Add 1";
            remove_use_button.Padding = new Thickness(10);
            remove_use_button.IsEnabled = false;
            Children.Add(remove_use_button);

            reorder_button = new Button();
            reorder_button.Width = 80;
            Binding reorderBinding = new Binding("CanReorderKit");
            reorderBinding.Source = new_kit;
            reorder_button.Click += new_kit.reorder;
            reorder_button.Click += reordered;
            reorder_button.Content = "Reorder";
            reorder_button.Padding = new Thickness(10);
            reorder_button.SetBinding(Button.IsEnabledProperty, reorderBinding);
            Children.Add(reorder_button);

            Binding colorBinding = new Binding("StatusColor");
            colorBinding.Source = new_kit;

            status_label = new Label();
            status_label.Content = "Status";
            status_label.VerticalContentAlignment = VerticalAlignment.Center;
            status_label.SetBinding(Label.BackgroundProperty, colorBinding);
            status_label.Padding = new Thickness(10);
            Children.Add(status_label);

            reset_button = new Button();
            reset_button.Width = 80;
            reset_button.IsEnabled = false;
            reset_button.Click += new_kit.reset;
            reset_button.Content = "Reset?";
            reset_button.Padding = new Thickness(10);
            Children.Add(reset_button);

            remove_button = new Button();
            remove_button.Width = 80;
            remove_button.IsEnabled = false;
            remove_button.Click += new_kit.remove_click;
            remove_button.Click += remove_click;
            remove_button.Content = "Remove?";
            remove_button.Padding = new Thickness(10);
            Children.Add(remove_button);

            override_label = new Label();
            override_label.Content = "Override?";
            override_label.Padding = new Thickness(10);
            Children.Add(override_label);

            override_checkbox = new CheckBox();
            override_checkbox.Padding = new Thickness(10);
            override_checkbox.Checked += CheckBox_Checked;
            override_checkbox.Unchecked += CheckBox_UnChecked;
            override_checkbox.Unchecked += new_kit.update;
            Children.Add(override_checkbox);

            last_updated = new Label();
            Binding last_updated_binding = new Binding("Present");
            last_updated_binding.Source = new_kit;
            last_updated.SetBinding(Label.ContentProperty, last_updated_binding);
            last_updated.Padding = new Thickness(10);
            Children.Add(last_updated);

        }
        public void remove_click(object sender, RoutedEventArgs e)
        {
            Children.Clear();
        }
        public void disable_add_use_button(object sender, RoutedEventArgs e)
        {
            add_use_button.IsEnabled = false;
            CheckBox_Checked(sender, e);
        }

        public void disable_remove_use_button(object sender, RoutedEventArgs e)
        {
            remove_use_button.IsEnabled = false;
            CheckBox_Checked(sender, e);
        }

        public void reordered(object sender, RoutedEventArgs e)
        {
            remove_use_button.IsEnabled = false;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            bool check = override_checkbox.IsChecked ?? false;
            if (check)
            {
                add_use_button.IsEnabled = true;
                remove_use_button.IsEnabled = true;
                reorder_button.IsEnabled = true;
                reset_button.IsEnabled = true;
                remove_button.IsEnabled = true;
                foreach (TextBox tb in text_boxes)
                {
                    tb.IsReadOnly = false;
                }
            }
        }
        private void CheckBox_UnChecked(object sender, RoutedEventArgs e)
        {
            remove_use_button.IsEnabled = false;
            reorder_button.IsEnabled = false;
            reset_button.IsEnabled = false;
            remove_button.IsEnabled = false;
            foreach (TextBox tb in text_boxes)
            {
                tb.IsReadOnly = true;
            }
        }
    }
}
