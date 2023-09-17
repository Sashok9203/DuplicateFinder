using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DuplicateFinder.Models
{
    internal class FileInfo:INotifyPropertyChanged
    {
        private bool? moved;
        public bool? Moved 
        {
            get => moved;
            set
            {
                moved = value;
                OnPropertyChanged("Action");
            }
        }
        private bool hasDuplicate;
        public bool HasDuplicate
        {
            get => hasDuplicate;
            set
            {
                hasDuplicate = value;
                OnPropertyChanged("Duplicate");
            }
        }
        public string? Path { get; set; }
        public string? Hash { get; set; }
        public string? Action => Moved == null ? null: Moved.Value? "Moved" :"Can't move";
        public string? Duplicate => HasDuplicate ? "Duplicate" : null;

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string? prop = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
