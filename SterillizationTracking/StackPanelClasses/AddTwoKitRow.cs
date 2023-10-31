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
    class AddTwoKitRow : StackPanel
    {
        public Button add_use_button, remove_use_button, reorder_metal_button, reorder_plastic_button;
        public Label current_use_metal_label, current_use_plastic_label, kit_label, kit_number_label, override_label, last_updated, description_label;
        public CheckBox override_checkbox;
        public StackPanel current_use_stackpanel;
        public TextBox text_box;

        public AddTwoKitRow(BaseTwoPartKit new_kit)
        {
            Orientation = Orientation.Horizontal;
            Binding colorBinding_metal = new Binding("StatusColor_Metal");
            Binding colorBinding_plastic = new Binding("StatusColor_Plastic");
            colorBinding_metal.Source = new_kit;
            colorBinding_plastic.Source = new_kit;

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

            current_use_stackpanel = new StackPanel();
            current_use_stackpanel.Orientation = Orientation.Vertical;

            current_use_metal_label = new Label();
            current_use_metal_label.Width = 150;
            Binding current_use_metal_binding = new Binding("CurrentUseStringMetal");
            current_use_metal_binding.Source = new_kit;
            current_use_metal_label.SetBinding(Label.ContentProperty, current_use_metal_binding);
            current_use_metal_label.SetBinding(Label.BackgroundProperty, colorBinding_metal);
            current_use_metal_label.Padding = new Thickness(10);
            current_use_stackpanel.Children.Add(current_use_metal_label);

            current_use_plastic_label = new Label();
            current_use_plastic_label.Width = 150;
            Binding current_use_plastic_binding = new Binding("CurrentUseStringPlastic");
            current_use_plastic_binding.Source = new_kit;
            current_use_plastic_label.SetBinding(Label.ContentProperty, current_use_plastic_binding);
            current_use_plastic_label.SetBinding(Label.BackgroundProperty, colorBinding_plastic);
            current_use_plastic_label.Padding = new Thickness(10);
            current_use_stackpanel.Children.Add(current_use_plastic_label);
            Children.Add(current_use_stackpanel);

            Binding canAddBinding = new Binding("CanAdd");
            canAddBinding.Source = new_kit;
            add_use_button = new Button();
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

            StackPanel reorder_stack_panel = new StackPanel();
            reorder_stack_panel.Orientation = Orientation.Vertical;

            reorder_metal_button = new Button();
            reorder_metal_button.Width = 100;
            Binding reordermetalBinding = new Binding("CanReorderMetal");
            reordermetalBinding.Source = new_kit;
            reorder_metal_button.Click += new_kit.reorder_metal;
            reorder_metal_button.Click += reordered;
            reorder_metal_button.Content = "Reorder Metal";
            reorder_metal_button.Padding = new Thickness(10);
            reorder_metal_button.SetBinding(Button.IsEnabledProperty, reordermetalBinding);
            reorder_stack_panel.Children.Add(reorder_metal_button);

            reorder_plastic_button = new Button();
            reorder_plastic_button.Width = 100;
            Binding reorderplasticBinding = new Binding("CanReorderPlastic");
            reorderplasticBinding.Source = new_kit;
            reorder_plastic_button.Click += new_kit.reorder_plastic;
            reorder_plastic_button.Click += reordered;
            reorder_plastic_button.Content = "Reorder Plastic";
            reorder_plastic_button.Padding = new Thickness(10);
            reorder_plastic_button.SetBinding(Button.IsEnabledProperty, reorderplasticBinding);
            reorder_stack_panel.Children.Add(reorder_plastic_button);
            Children.Add(reorder_stack_panel);

            StackPanel uses_left_stack_panel = new StackPanel();
            uses_left_stack_panel.Orientation = Orientation.Vertical;

            Label uses_left_metal_label = new Label();
            Binding usesleft_metal_binding = new Binding("UsesLeftMetal");
            usesleft_metal_binding.Source = new_kit;
            uses_left_metal_label.SetBinding(Label.ContentProperty, usesleft_metal_binding);
            uses_left_metal_label.Padding = new Thickness(10);
            uses_left_stack_panel.Children.Add(uses_left_metal_label);

            Label uses_left_plastic_label = new Label();
            Binding usesleft_plastic_binding = new Binding("UsesLeftPlastic");
            usesleft_plastic_binding.Source = new_kit;
            uses_left_plastic_label.SetBinding(Label.ContentProperty, usesleft_plastic_binding);
            uses_left_plastic_label.Padding = new Thickness(10);
            uses_left_stack_panel.Children.Add(uses_left_plastic_label);
            Children.Add(uses_left_stack_panel);

            StackPanel status_stack_panel = new StackPanel();
            status_stack_panel.Orientation = Orientation.Vertical;
            Label status_metal_label = new Label();
            status_metal_label.Content = "Status Metal";
            status_metal_label.SetBinding(Label.BackgroundProperty, colorBinding_metal);
            status_metal_label.Padding = new Thickness(10);
            status_stack_panel.Children.Add(status_metal_label);
            Label status_plastic_label = new Label();
            status_plastic_label.Content = "Status Plastic";
            status_plastic_label.SetBinding(Label.BackgroundProperty, colorBinding_plastic);
            status_plastic_label.Padding = new Thickness(10);
            status_stack_panel.Children.Add(status_plastic_label);
            Children.Add(status_stack_panel);

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
                reorder_metal_button.IsEnabled = true;
                reorder_plastic_button.IsEnabled = true;
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
