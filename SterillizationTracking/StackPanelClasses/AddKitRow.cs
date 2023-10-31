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
        public Button add_use_button, remove_use_button, reorder_button;
        public Label current_use_label, kit_label, kit_number_label, override_label, status_label, uses_left_label, last_updated, description_label;
        public CheckBox override_checkbox;
        public TextBox text_box;

        public AddKitRow(BaseOnePartKit new_kit)
        {
            Orientation = Orientation.Horizontal;
            Binding colorBinding = new Binding("StatusColor");
            colorBinding.Source = new_kit;

            kit_label = new Label();
            kit_label.Width = 150;
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

            text_box = new TextBox();
            Binding description_binding = new Binding(path: "Description");
            description_binding.Mode = BindingMode.TwoWay;
            description_binding.Source = new_kit;
            description_binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            text_box.SetBinding(TextBox.TextProperty, description_binding);
            text_box.LostFocus += new_kit.update;
            text_box.Width = 150;
            text_box.Padding = new Thickness(10);
            text_box.IsReadOnly = true;
            Children.Add(text_box);

            current_use_label = new Label();
            current_use_label.Width = 150;
            Binding myBinding = new Binding("CurrentUseString");
            myBinding.Source = new_kit;
            current_use_label.SetBinding(Label.ContentProperty, myBinding);
            current_use_label.SetBinding(Label.BackgroundProperty, colorBinding);
            current_use_label.Padding = new Thickness(10);
            Children.Add(current_use_label);

            add_use_button = new Button();
            Binding canAddBinding = new Binding("CanAdd");
            canAddBinding.Source = new_kit;
            add_use_button.Click += new_kit.add_use;
            add_use_button.Click += disable_add_use_button;
            add_use_button.SetBinding(Button.IsEnabledProperty, canAddBinding);
            add_use_button.Content = "Add use";
            add_use_button.Padding = new Thickness(10);
            Children.Add(add_use_button);

            remove_use_button = new Button();
            remove_use_button.Click += new_kit.remove_use;
            remove_use_button.Click += disable_remove_use_button;
            remove_use_button.Content = "Remove use";
            remove_use_button.Padding = new Thickness(10);
            remove_use_button.IsEnabled = false;
            Children.Add(remove_use_button);

            reorder_button = new Button();
            reorder_button.Width = 100;
            Binding reorderBinding = new Binding("CanReorder");
            reorderBinding.Source = new_kit;
            reorder_button.Click += new_kit.reorder;
            reorder_button.Click += reordered;
            reorder_button.Content = "Reorder";
            reorder_button.Padding = new Thickness(10);
            reorder_button.SetBinding(Button.IsEnabledProperty, reorderBinding);
            Children.Add(reorder_button);

            uses_left_label = new Label();
            Binding usesleft_binding = new Binding("UsesLeftString");
            usesleft_binding.Source = new_kit;
            uses_left_label.SetBinding(Label.ContentProperty, usesleft_binding);
            uses_left_label.Padding = new Thickness(10);
            Children.Add(uses_left_label);

            status_label = new Label();
            status_label.Content = "Status";
            status_label.SetBinding(Label.BackgroundProperty, colorBinding);
            status_label.Padding = new Thickness(10);
            Children.Add(status_label);

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
                text_box.IsReadOnly = false;
            }
        }
        private void CheckBox_UnChecked(object sender, RoutedEventArgs e)
        {
            remove_use_button.IsEnabled = false;
            text_box.IsReadOnly = true;
        }
    }
}
