using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SterillizationTracking.Services
{
    class CheckNumberList
    {
        public List<string> out_list;
        public List<string> return_list(string kit_name, string file_path)
        {
            out_list = new List<String> { "Select a number"};
            string kit_directory = Path.Combine(file_path, kit_name);
            if (!Directory.Exists(kit_directory))
            {
                Directory.CreateDirectory(kit_directory);
            }
            string[] kit_inventory_list = Directory.GetDirectories(kit_directory);
            int counter = 0;
            for (int i = 1; i < 999; i ++)
            {
                string string_kit_number = i.ToString("D2");
                string new_directory = Path.Combine(kit_directory, $"Kit {string_kit_number}");
                if (!kit_inventory_list.Contains(new_directory))
                {
                    out_list.Add(string_kit_number);
                    counter += 1;
                    if (counter == 5)
                    {
                        return out_list;
                    }
                }
            }
            return out_list;
        }

    }
}
