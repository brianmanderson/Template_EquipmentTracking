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
    public class BaseTwoPartKit : INotifyPropertyChanged
    {
        private int currentUse_metal;
        private List<string> usageDates = new List<string>();
        private int currentUse_plastic;
        private System.Windows.Media.Brush statusColor_Metal, statusColor_Plastic;
        private string name;
        private string kitnumber;
        private string useFileLocation;
        private string _present;
        private bool can_reorder_metal, canAdd;
        private bool can_reorder_plastic;
        private string currentUse_string_metal, currentUse_string_plastic, usesLeft_string_metal, usesLeft_string_plastic, description;

        public int total_uses_metal;
        public int total_uses_plastic;
        public int warning_uses_metal;
        public int warning_uses_plastic;
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

        public string Present
        {
            get { return _present; }
            set
            {
                _present = value;
                OnPropertyChanged("Present");
            }
        }
        public int CurrentUseMetal
        {
            get { return currentUse_metal; }
            set
            {
                currentUse_metal = value;
                OnPropertyChanged("CurrentUseMetal");
            }
        }
        public string CurrentUseStringMetal
        {
            get { return currentUse_string_metal; }
            set
            {
                currentUse_string_metal = value;
                OnPropertyChanged("CurrentUseStringMetal");
            }
        }
        public string CurrentUseStringPlastic
        {
            get { return currentUse_string_plastic; }
            set
            {
                currentUse_string_plastic = value;
                OnPropertyChanged("CurrentUseStringPlastic");
            }
        }
        public int CurrentUsePlastic
        {
            get { return currentUse_plastic; }
            set
            {
                currentUse_plastic = value;
                OnPropertyChanged("CurrentUsePlastic");
            }
        }
        public string UsesLeftMetal
        {
            get { return usesLeft_string_metal; }
            set
            {
                usesLeft_string_metal = value;
                OnPropertyChanged("UsesLeftMetal");
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
        public string UsesLeftPlastic
        {
            get { return usesLeft_string_plastic; }
            set
            {
                usesLeft_string_plastic = value;
                OnPropertyChanged("UsesLeftPlastic");
            }
        }
        public bool CanReorderMetal
        {
            get { return can_reorder_metal; }
            set
            {
                can_reorder_metal = value;
                OnPropertyChanged("CanReorderMetal");
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
        public bool CanReorderPlastic
        {
            get { return can_reorder_plastic; }
            set
            {
                can_reorder_plastic = value;
                OnPropertyChanged("CanReorderPlastic");
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
        public System.Windows.Media.Brush StatusColor_Metal
        {
            get { return statusColor_Metal; }
            set
            {
                statusColor_Metal = value;
                OnPropertyChanged("StatusColor_Metal");
            }
        }
        public System.Windows.Media.Brush StatusColor_Plastic
        {
            get { return statusColor_Plastic; }
            set
            {
                statusColor_Plastic = value;
                OnPropertyChanged("StatusColor_Plastic");
            }
        }
        public BaseTwoPartKit(string name, string kitnumber, string file_path) //string name, int allowed_steralizaitons, int warning_use
        {
            Name = name;
            StatusColor_Metal = statusColor_Metal;
            StatusColor_Plastic = statusColor_Plastic;
            KitNumber = $"Kit #: {kitnumber}";
            KitDirectoryPath = Path.Combine(file_path, name, $"Kit {kitnumber}");
            UseFileLocation = Path.Combine(KitDirectoryPath, "Uses.txt");

            total_uses_metal = 500;
            total_uses_plastic = 100;
            warning_uses_metal = 450;
            warning_uses_plastic = 80;
            CanReorderMetal = false;
            CanReorderPlastic = false;
            CanAdd = true;
            UsageDates = new List<string>();
            Description = "";
            if (name.Contains("Y Applicator"))
            {
                Description = "13 Pieces";
            }
            else if (name.Contains("Tandem and Ring"))
            {
                Description = "15 Pieces";
            }
            build_read_use_file();
        }

        public void build_read_use_file()
        {
            if (File.Exists(UseFileLocation))
            {
                List<string> lines = File.ReadAllLines(UseFileLocation).ToList();
                Description = lines[0].Split("Description:")[1];
                CurrentUseMetal = Convert.ToInt32(lines[1].Split("Current Use_Metal:")[1]);
                CurrentUsePlastic = Convert.ToInt32(lines[2].Split("Current Use_Plastic:")[1]);
                total_uses_metal = Convert.ToInt32(lines[3].Split("Total Uses_Metal:")[1]);
                total_uses_plastic = Convert.ToInt32(lines[4].Split("Total Uses_Plastic:")[1]);
                warning_uses_metal = Convert.ToInt32(lines[5].Split("Warning Uses_Metal:")[1]);
                warning_uses_plastic = Convert.ToInt32(lines[6].Split("Warning Uses_Plastic:")[1]);
                Present = lines[7].Split("updated:")[1];
                if (lines.Count > 8)
                {
                    UsageDates = lines.GetRange(8, lines.Count - 8);
                }
                else
                {
                    UsageDates = new List<string>();
                }
            }
            else
            {
                CurrentUseMetal = 0;
                CurrentUsePlastic = 0;
                string[] info = { $"Description:{Description}", $"Current Use_Metal:{0}", $"Current Use_Plastic:{0}", $"Total Uses_Metal:{total_uses_metal}",
                    $"Total Uses_Plastic:{total_uses_plastic}", $"Warning Uses_Metal:{warning_uses_metal}", $"Warning Uses_Plastic:{warning_uses_plastic}",
                    $"Last updated:{Present}" };
                if (!Directory.Exists(KitDirectoryPath))
                {
                    Directory.CreateDirectory(KitDirectoryPath);
                }
                File.WriteAllLines(UseFileLocation, info);
            }
            check_status();
            UsesLeftMetal = $"Uses left metal: {total_uses_metal - CurrentUseMetal}";
            UsesLeftPlastic = $"Uses left plastic: {total_uses_plastic - CurrentUsePlastic}";
            CurrentUseStringMetal = $"Current use metal: {CurrentUseMetal}";
            CurrentUseStringPlastic = $"Current use plastic: {CurrentUsePlastic}";
            CanAdd = true;
            if ((total_uses_metal - CurrentUseMetal == 0) || (total_uses_plastic - CurrentUsePlastic == 0))
            {
                CanAdd = false;
            }
        }

        public void create_reorder_file_plastic()
        {
            ReorderDirectoryPath = Path.Combine(KitDirectoryPath, "Reorders");
            if (!Directory.Exists(ReorderDirectoryPath))
            {
                Directory.CreateDirectory(ReorderDirectoryPath);
            }
            DateTime moment = DateTime.Now;
            Present = "Plastic_" + (moment.ToLongDateString() + " " + moment.ToLongTimeString()).Replace(":", ".");
            string file_path = Path.Combine(ReorderDirectoryPath, Present.Replace(":", ".") + ".txt");
            File.WriteAllLines(file_path, UsageDates);
        }
        public void create_reorder_file_metal()
        {
            ReorderDirectoryPath = Path.Combine(KitDirectoryPath, "Reorders");
            if (!Directory.Exists(ReorderDirectoryPath))
            {
                Directory.CreateDirectory(ReorderDirectoryPath);
            }
            DateTime moment = DateTime.Now;
            Present = "Metal_" + (moment.ToLongDateString() + " " + moment.ToLongTimeString()).Replace(":", ".");
            string file_path = Path.Combine(ReorderDirectoryPath, Present.Replace(":", ".") + ".txt");
            File.WriteAllLines(file_path, UsageDates);
        }
        public void update_file()
        {
            List<string> info = new List<string>() {$"Description:{Description}", $"Current Use_Metal:{CurrentUseMetal}", $"Current Use_Plastic:{CurrentUsePlastic}", $"Total Uses_Metal:{total_uses_metal}",
                    $"Total Uses_Plastic:{total_uses_plastic}", $"Warning Uses_Metal:{warning_uses_metal}", $"Warning Uses_Plastic:{warning_uses_plastic}",
                    $"Last updated:{Present}" };
            info.AddRange(UsageDates);
            if (!Directory.Exists(KitDirectoryPath))
            {
                Directory.CreateDirectory(KitDirectoryPath);
            }
            File.WriteAllLines(UseFileLocation, info);
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

        public void update_useage()
        {
            UsesLeftMetal = $"Uses left metal: {total_uses_metal - CurrentUseMetal}";
            UsesLeftPlastic = $"Uses left plastic: {total_uses_plastic - CurrentUsePlastic}";
            CurrentUseStringMetal = $"Current use metal: {CurrentUseMetal}";
            CurrentUseStringPlastic = $"Current use plastic: {CurrentUsePlastic}";
        }
        public void add_use(object sender, RoutedEventArgs e)
        {
            DateTime moment = DateTime.Now;
            Present = moment.ToLongDateString() + " " + moment.ToLongTimeString();
            CurrentUseMetal += 1;
            CurrentUsePlastic += 1;
            UsageDates.Add($"Metal use: {CurrentUseMetal}, Plastic use: {CurrentUsePlastic} @: {Present}");
            update_useage();
            update_file();
            check_status();
        }

        public void remove_use(object sender, RoutedEventArgs e)
        {
            CurrentUseMetal -= 1;
            CurrentUsePlastic -= 1;
            UsageDates.RemoveAt(UsageDates.Count - 1);
            update_useage();
            update_file();
            check_status();
        }

        public void reorder_metal(object sender, RoutedEventArgs e)
        {
            create_reorder_file_metal();
            CurrentUseMetal = 0;
            UsageDates = new List<string>();
            update_useage();
            update_file();
            check_status();
        }
        public void reorder_plastic(object sender, RoutedEventArgs e)
        {
            create_reorder_file_plastic();
            CurrentUsePlastic = 0;
            update_useage();
            update_file();
            check_status();
        }
        public void update(object sender, RoutedEventArgs e)
        {
            update_file();
        }
        public void check_status()
        {
            if (CurrentUseMetal >= total_uses_metal)
            {
                StatusColor_Metal = System.Windows.Media.Brushes.Red;
                CanReorderMetal = true;
            }
            else if (CurrentUseMetal >= warning_uses_metal * 0.75)
            {
                StatusColor_Metal = System.Windows.Media.Brushes.Yellow;
                CanReorderMetal = false;
            }
            else
            {
                StatusColor_Metal = System.Windows.Media.Brushes.Green;
                CanReorderMetal = false;
            }
            if (CurrentUsePlastic >= total_uses_plastic)
            {
                StatusColor_Plastic = System.Windows.Media.Brushes.Red;
                CanReorderPlastic = true;
            }
            else if (CurrentUsePlastic >= warning_uses_plastic * 0.75)
            {
                StatusColor_Plastic = System.Windows.Media.Brushes.Yellow;
                CanReorderPlastic = false;
            }
            else
            {
                StatusColor_Plastic = System.Windows.Media.Brushes.Green;
                CanReorderPlastic = false;
            }
        }
    }
}
