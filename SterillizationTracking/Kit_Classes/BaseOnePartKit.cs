using System;
using System.IO;
using System.Collections.Generic;
using System.Windows;
using SterillizationTracking.Services;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SterillizationTracking.Kit_Classes
{
    public class BaseOnePartKit : INotifyPropertyChanged
    {
        private int currentUse;
        private List<string> usageDates = new List<string>();
        private string currentUse_string, usesLeft_string, description;
        private System.Windows.Media.Brush statusColor;
        private string name;
        private string kitnumber;
        private string useFileLocation;
        private string _present;
        private bool can_reorder, canAdd;
        private int usesLeft;

        public int total_uses;
        public int warning_uses;
        public string KitDirectoryPath;
        public string ReorderDirectoryPath;

        public List<string> UsageDates
        {
            get
            {
                return usageDates;
            }
            set
            {
                usageDates = value;
                OnPropertyChanged("UsageDates");
            }
        }
        public string CurrentUseString
        {
            get { return currentUse_string; }
            set
            {
                currentUse_string = value;
                OnPropertyChanged("CurrentUseString");
            }
        }
        public string UsesLeftString
        {
            get { return usesLeft_string; }
            set
            {
                usesLeft_string = value;
                OnPropertyChanged("UsesLeftString");
            }
        }
        public string Description
        {
            get { return description; }
            set
            {
                description = value;
                OnPropertyChanged("Description");
            }
        }
        public string Present
        {
            get { return _present; }
            set
            {
                _present = value;
                OnPropertyChanged("Present");
            }
        }
        public bool CanAdd
        {
            get { return canAdd; }
            set
            {
                canAdd = value;
                OnPropertyChanged("CanAdd");
            }
        }
        public BaseOnePartKit(string name, string kitnumber, string file_path) //string name, int allowed_steralizaitons, int warning_use
        {
            Name = name;
            StatusColor = statusColor;
            KitNumber = $"Kit #: {kitnumber}";
            KitDirectoryPath = Path.Combine(file_path, name, $"Kit {kitnumber}");
            UseFileLocation = Path.Combine(KitDirectoryPath, "Uses.txt");

            warning_uses = 80;
            total_uses = 100;
            CanReorder = false;
            Description = "";
            if (name == "Cylinder")
            {
                total_uses = 500;
                warning_uses = 450;
                Description = "18 Pieces";
            }
            else if (name == "Tandem and Ovoid")
            {
                Description = "18 Pieces";
                total_uses = 100;
                warning_uses = 80;
            }
            else if (name == "Needle Kit")
            {
                total_uses = 25;
                warning_uses = 18;
                Description = "Count: 10, 20 Pieces";
            }
            else if (name == "Segmented Cylinder")
            {
                total_uses = 500;
                warning_uses = 450;
            }
            else if (name == "Cervix Applicator Set")
            {
                Description = "10 Pieces";
                total_uses = 500;
                warning_uses = 450;
            }
            CanAdd = true;
            UsageDates = new List<string>();
            build_read_use_file();
        }

        public void build_read_use_file()
        {
            if (File.Exists(UseFileLocation))
            {
                List<string> lines = File.ReadAllLines(UseFileLocation).ToList();
                Description = lines[0].Split("Description:")[1];
                CurrentUse = Convert.ToInt32(lines[1].Split("Use:")[1]);
                total_uses = Convert.ToInt32(lines[2].Split("Uses:")[1]);
                warning_uses = Convert.ToInt32(lines[3].Split("Uses:")[1]);
                Present = lines[4].Split("updated:")[1];
                if (lines.Count > 5)
                {
                    UsageDates = lines.GetRange(5, lines.Count - 5);
                }
                else
                {
                    UsageDates = new List<string>();
                }

            }
            else
            {
                CurrentUse = 0;
                string[] info ={ $"Description:{Description}", $"Current Use:{0}", $"Total Uses:{total_uses}", $"Warning Uses:{warning_uses}", $"Last updated:{Present}" };
                if (!Directory.Exists(KitDirectoryPath))
                {
                    Directory.CreateDirectory(KitDirectoryPath);
                }
                File.WriteAllLines(UseFileLocation, info);
            }
            check_status();
            CurrentUseString = $"Current use: {CurrentUse}";
            UsesLeft = total_uses - CurrentUse;
            UsesLeftString = $"Uses left: {UsesLeft}";
            CanAdd = true;
            if (total_uses - CurrentUse == 0)
            {
                CanAdd = false;
            }
        }

        public void create_reorder_file()
        {
            ReorderDirectoryPath = Path.Combine(KitDirectoryPath, "Reorders");
            if (!Directory.Exists(ReorderDirectoryPath))
            {
                Directory.CreateDirectory(ReorderDirectoryPath);
            }
            DateTime moment = DateTime.Now;
            Present = (moment.ToLongDateString() + " " + moment.ToLongTimeString()).Replace(":", ".");
            string file_path = Path.Combine(ReorderDirectoryPath, Present.Replace(":", ".") + ".txt");
            File.WriteAllLines(file_path, UsageDates);
        }
        public void update_file()
        {
            List<string> info = new List<string>() { $"Description:{Description}", $"Current Use:{CurrentUse}",
                $"Total Uses:{total_uses}", $"Warning Uses:{warning_uses}", $"Last updated:{Present}" };
            info.AddRange(UsageDates);
            if (!Directory.Exists(KitDirectoryPath))
            {
                Directory.CreateDirectory(KitDirectoryPath);
            }
            File.WriteAllLines(UseFileLocation, info);
        }

        public int CurrentUse
        {
            get { return currentUse; }
            set
            {
                currentUse = value;
                OnPropertyChanged("CurrentUse");
            }
        }

        public int UsesLeft
        {
            get { return usesLeft; }
            set
            {
                usesLeft = value;
                OnPropertyChanged("UsesLeft");
            }
        }

        public bool CanReorder
        {
            get { return can_reorder; }
            set
            {
                can_reorder = value;
                OnPropertyChanged("CanReorder");
            }
        }

        public string UseFileLocation
        {
            get { return useFileLocation; }
            set
            {
                useFileLocation = value;
                OnPropertyChanged("UseFileLocation");
            }
        }

        public string KitNumber
        {
            get { return kitnumber; }
            set
            {
                kitnumber = value;
                OnPropertyChanged("KitNumber");
            }
        }

        public String Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }

        public System.Windows.Media.Brush StatusColor
        {
            get { return statusColor; }
            set
            {
                statusColor = value;
                OnPropertyChanged("StatusColor");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string info)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(info));
            }
        }

        public void add_use(object sender, RoutedEventArgs e)
        {
            DateTime moment = DateTime.Now;
            Present = moment.ToLongDateString() + " " + moment.ToLongTimeString();
            CurrentUse += 1;
            CurrentUseString = $"Current use: {CurrentUse}";
            UsesLeft = total_uses - CurrentUse;
            UsesLeftString = $"Uses left: {UsesLeft}";
            UsageDates.Add($"{CurrentUse}: {Present}");
            update_file();
            check_status();
        }

        public void remove_use(object sender, RoutedEventArgs e)
        {
            CurrentUse -= 1;
            CurrentUseString = $"Current use: {CurrentUse}";
            UsesLeft = total_uses - CurrentUse;
            UsesLeftString = $"Uses left: {UsesLeft}";
            UsageDates.RemoveAt(UsageDates.Count - 1);
            update_file();
            check_status();
        }

        public void update(object sender, RoutedEventArgs e)
        {
            update_file();
        }
        public void reorder(object sender, RoutedEventArgs e)
        {
            create_reorder_file();
            CurrentUse = 0;
            CurrentUseString = $"Current use: {CurrentUse}";
            UsesLeft = total_uses - CurrentUse;
            UsesLeftString = $"Uses left: {UsesLeft}";
            UsageDates = new List<string>();
            update_file();
            check_status();
        }

        public void check_status()
        {
            if (CurrentUse >= total_uses)
            {
                StatusColor = System.Windows.Media.Brushes.Red;
                CanReorder = true;
            }
            else if (CurrentUse >= warning_uses * 0.75)
            {
                StatusColor = System.Windows.Media.Brushes.Yellow;
                CanReorder = false;
            }
            else
            {
                StatusColor = System.Windows.Media.Brushes.Green;
                CanReorder = false;
            }
        }
    }
}
