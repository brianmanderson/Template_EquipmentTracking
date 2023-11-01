using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SterillizationTracking.Kit_Classes
{
    public class BaseKit : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string info)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(info));
            }
        }
        public List<BaseOnePartKit> Kits;
        public string KitDirectoryPath;
        public string ReorderDirectoryPath;

        private string name;
        private string kitnumber;
        private string useFileLocation;
        private bool can_reorder_kit, canAdd;
        private string _present;
        private string _description;
        private List<string> usageDates = new List<string>();

        public bool CanReorderKit
        {
            get { return can_reorder_kit; }
            set
            {
                can_reorder_kit = value;
                OnPropertyChanged("CanReorderKit");
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
        public string Present
        {
            get { return _present; }
            set
            {
                _present = value;
                OnPropertyChanged("Present");
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
        public BaseKit(string name, string kitnumber, string file_path)
        {
            Name = name;
            KitNumber = $"Kit #: {kitnumber}";
            KitDirectoryPath = Path.Combine(file_path, name, $"Kit {kitnumber}");
            UseFileLocation = Path.Combine(KitDirectoryPath, "Uses.txt");
            UsageDates = new List<string>();
        }
        public void reset()
        {
            UsageDates = new List<string>();
            foreach (BaseOnePartKit kit in Kits) { kit.reset(); }
        }
        public void add_use(object sender, RoutedEventArgs e)
        {
            DateTime moment = DateTime.Now;
            Present = moment.ToLongDateString() + " " + moment.ToLongTimeString();
            foreach (BaseOnePartKit base_kit in Kits)
            {
                base_kit.add_use();
            }
            update_file();
            check_status();
        }
        public void check_status()
        {
            CanReorderKit = false;
            CanAdd = true;
            foreach (BaseOnePartKit base_kit in Kits)
            {
                if (base_kit.CanReorder)
                {
                    CanReorderKit = true;
                }
                if (!base_kit.CanAdd)
                {
                    CanAdd = false;
                }
            }
        }
        public void build_read_use_file()
        {
            if (File.Exists(UseFileLocation))
            {
                List<string> lines = File.ReadAllLines(UseFileLocation).ToList();
                foreach (string line in lines)
                {
                    if (line.Contains("Description"))
                    {
                        string description = line.Split("Description:")[1].Split('$')[0];
                        int current_use = Convert.ToInt32(line.Split("Current Use:")[1].Split('$')[0]);
                        int total_use = Convert.ToInt32(line.Split("Total Use:")[2].Split('$')[0]);
                        int warning_use = Convert.ToInt32(line.Split("Warning Uses:")[3]);
                        BaseOnePartKit new_kit = new BaseOnePartKit(current_use, total_use, warning_use, description);
                        Kits.Add(new_kit);
                    }
                    else if (line.Contains("Last"))
                    {
                        Present = line.Split("updated:")[1];
                    }
                    else
                    {
                        UsageDates.Add(line);
                    }
                }
            }
            else
            {
                List<string> lines = create_file_lines();
                if (!Directory.Exists(KitDirectoryPath))
                {
                    Directory.CreateDirectory(KitDirectoryPath);
                }
                File.WriteAllLines(UseFileLocation, lines);
            }
            check_status();
        }

        public List<string> create_file_lines()
        {
            List<string> lines = new List<string>();
            foreach (BaseOnePartKit kit in Kits)
            {
                lines.Add($"Description:{kit.Description}$Current Use:{kit.CurrentUse}$Total Use:{kit.Total_Uses}$Warning Uses:{kit.Warning_Uses}");
            }
            lines.Add($"Last updated:{Present}");
            return lines;
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
            List<string> info = create_file_lines();
            info.AddRange(UsageDates);
            if (!Directory.Exists(KitDirectoryPath))
            {
                Directory.CreateDirectory(KitDirectoryPath);
            }
            File.WriteAllLines(UseFileLocation, info);
        }
        public void update(object sender, RoutedEventArgs e)
        {
            update_file();
        }
        public void remove_use(object sender, RoutedEventArgs e)
        {
            foreach (BaseOnePartKit kit in Kits)
            {
                kit.remove_use();
            }
            UsageDates.RemoveAt(UsageDates.Count - 1);
            update_file();
            check_status();
        }
        public void reorder(object sender, RoutedEventArgs e)
        {
            foreach(BaseOnePartKit kit in Kits) { kit.reset(); }
            create_reorder_file();
        }
    }
}
