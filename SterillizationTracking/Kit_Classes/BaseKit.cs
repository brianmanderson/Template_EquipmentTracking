using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SterillizationTracking.Kit_Classes
{
    public class TemplatePart
    {
        public int Total_Uses { get; set; }
        public int Warning_Uses { get; set; }
        public string Description { get; set; }
    }
    public class TemplateKit
    {
        public string Name { get; set; }
        public List<TemplatePart> Parts { get; set; }
    }
    public class TemplateKitLibrary
    {
        public ObservableCollection<TemplateKit> Kits { get; set; }
    }
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
        public List<BaseOnePartKit> Kits = new List<BaseOnePartKit>();
        public string KitDirectoryPath;
        public string ReorderDirectoryPath;

        private string name;
        private string kitnumber;
        private string useFileLocation;
        private bool can_reorder_kit, canAdd;
        private string _present;
        private List<string> usageDates = new List<string>();
        private System.Windows.Media.Brush statusColor;

        public System.Windows.Media.Brush StatusColor
        {
            get { return statusColor; }
            set
            {
                statusColor = value;
                OnPropertyChanged("StatusColor");
            }
        }
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
        public BaseKit(string name)
        {
            Name = name;
        }
        public void add_kit(int total_uses, int warning_uses, string description)
        {
            Kits.Add(new BaseOnePartKit(0, total_uses, warning_uses, description));
        }
        public void build_template_file(string path)
        {
            List<string> lines = create_file_lines();
            if (!Directory.Exists(KitDirectoryPath))
            {
                Directory.CreateDirectory(KitDirectoryPath);
            }
            File.WriteAllLines(path, lines);
        }
        public void reset()
        {
            UsageDates = new List<string>();
            foreach (BaseOnePartKit kit in Kits) { kit.reset(); }
        }
        public void remove_click(object sender, RoutedEventArgs e)
        {
            remove();
        }
        private void remove()
        {
            EnumerationOptions options = new EnumerationOptions();
            options.RecurseSubdirectories = true;
            foreach (string file in Directory.EnumerateFiles(KitDirectoryPath, "", options))
            {
                File.Delete(file);
            }
            Directory.Delete(KitDirectoryPath, true);
        }
        public void check_status()
        {
            CanReorderKit = false;
            CanAdd = true;
            int status_int = 0;
            foreach (BaseOnePartKit base_kit in Kits)
            {
                base_kit.check_status();
                if (base_kit.CanReorder)
                {
                    CanReorderKit = true;
                }
                if (!base_kit.CanAdd)
                {
                    CanAdd = false;
                }
                if (status_int < base_kit.StatusInt)
                {
                    StatusColor = base_kit.StatusColor;
                    status_int = base_kit.StatusInt;
                }
            }
        }
        public void write_file()
        {
            List<string> lines = create_file_lines();
            if (!Directory.Exists(KitDirectoryPath))
            {
                Directory.CreateDirectory(KitDirectoryPath);
            }
            File.WriteAllLines(UseFileLocation, lines);
        }
        public void build_from_file()
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
                        int total_use = Convert.ToInt32(line.Split("Total Use:")[1].Split('$')[0]);
                        int warning_use = Convert.ToInt32(line.Split("Warning Uses:")[1]);
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
                write_file();
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
        public void add_use(object sender, RoutedEventArgs e)
        {
            DateTime moment = DateTime.Now;
            Present = moment.ToLongDateString() + " " + moment.ToLongTimeString();
            foreach (BaseOnePartKit base_kit in Kits)
            {
                base_kit.add_use();
            }
            UsageDates.Add(Present);
            update_file();
            check_status();
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
            create_reorder_file();
            bool CanReorderAll = true;
            foreach (BaseOnePartKit kit in Kits)
            {
                if (!kit.CanReorder)
                {
                    CanReorderAll = false;
                }
                kit.reorder();
            }
            if (CanReorderAll)
            {
                reset();
            }
            check_status();
            update_file();
        }
        public void reset(object sender, RoutedEventArgs e)
        {
            reset();
            check_status();
            update_file();
        }
    }
}
