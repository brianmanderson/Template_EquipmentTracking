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
using DocumentFormat.OpenXml.Office.CustomDocumentInformationPanel;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Security.Cryptography;
using System.Xml.Linq;
using DocumentFormat.OpenXml.Drawing.Diagrams;

namespace SterillizationTracking.Kit_Classes
{
    public class BaseOnePartKit : INotifyPropertyChanged
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
        public int Total_Uses;
        public int Warning_Uses;
        public int StatusInt;

        private int currentUse;
        private string currentUse_string, usesLeft_string;
        private System.Windows.Media.Brush statusColor;
        private int usesLeft;
        private bool can_reorder, canAdd;
        private string _description;

        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                OnPropertyChanged("Description");
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
        public bool CanAdd
        {
            get { return canAdd; }
            set
            {
                canAdd = value;
                OnPropertyChanged("CanAdd");
            }
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
        public BaseOnePartKit(int current_use, int total_uses, int warning_uses, string description) //string name, int allowed_steralizaitons, int warning_use
        {
            StatusColor = statusColor;
            CurrentUse = current_use;
            Warning_Uses = warning_uses;
            Total_Uses = total_uses;
            Description = description;
        }

        public void add_use()
        {
            if (CurrentUse < Total_Uses)
            {
                CurrentUse += 1;
            }
            check_status();
        }

        public void remove_use()
        {
            CurrentUse -= 1;
            check_status();
        }
        public void reset()
        {
            CurrentUse = 0;
            check_status();
        }
        public void reorder()
        {
            if (CurrentUse == Total_Uses)
            {
                reset();
            }
        }
        public void check_status()
        {
            CurrentUseString = $"Current use: {CurrentUse}";
            UsesLeft = Total_Uses - CurrentUse;
            UsesLeftString = $"Uses left: {UsesLeft}";
            if (CurrentUse >= Total_Uses)
            {
                StatusColor = System.Windows.Media.Brushes.Red;
                CanReorder = true;
                CanAdd = false;
                StatusInt = 3;
            }
            else if (CurrentUse >= Warning_Uses * 0.75)
            {
                StatusColor = System.Windows.Media.Brushes.Yellow;
                CanReorder = false;
                CanAdd = true;
                StatusInt = 2;
            }
            else
            {
                StatusColor = System.Windows.Media.Brushes.Green;
                CanReorder = false;
                CanAdd = true;
                StatusInt = 1;
            }
        }
    }
}
