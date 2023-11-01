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
        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                OnPropertyChanged("Description");
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
        public BaseKit(string name, string kitnumber, string file_path, string description)
        {
            Name = name;
            Description = description;
            KitNumber = $"Kit #: {kitnumber}";
            KitDirectoryPath = Path.Combine(file_path, name, $"Kit {kitnumber}");
            UseFileLocation = Path.Combine(KitDirectoryPath, "Uses.txt");
            UsageDates = new List<string>();
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
                int ind = 0;
                foreach (string line in lines)
                {
                    if (ind == 0)
                    {
                        Description = line.Split("Description:")[1];
                    }
                    else if (line.Contains("Current"))
                    {
                        int current_use = Convert.ToInt32(line.Split(':')[1].Split('$')[0]);
                        int total_use = Convert.ToInt32(line.Split(':')[2].Split('$')[0]);
                        int warning_use = Convert.ToInt32(line.Split(':')[3]);
                        string description = line.Split('_')[1].Split(':')[0];
                        BaseOnePartKit new_kit = new BaseOnePartKit(current_use, total_use, warning_use, description);
                        Kits.Add(new_kit);
                    }
                    else if (!line.Contains("Last"))
                    {
                        UsageDates.Add(line);
                    }
                    ind++;
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
            List<string> lines = new List<string>
            {
                $"Description:{Description}"
            };
            foreach (BaseOnePartKit kit in Kits)
            {
                string kit_desc = kit.Description_;
                lines.Add($"Current Use_{kit_desc}:{kit.CurrentUse}$Total Use_{kit_desc}:{kit.Total_Uses}$Warning Uses_{kit_desc}:{kit.Warning_Uses}");
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
    }
}
