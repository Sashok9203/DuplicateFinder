using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;


namespace DuplicateFinder.Models
{
    internal class Directories:IDataErrorInfo,INotifyPropertyChanged
    {
        private string? sourcePath;
        private string? destinationPath;

        public string? SourcePath
        {
            get => sourcePath;
            set 
            {
                sourcePath = value;
                OnPropertyChanged();
                OnPropertyChanged("DestinationPath");
            }
        }
        public string? DestinationPath
        {
            get => destinationPath;
            set
            {
                destinationPath = value;
                OnPropertyChanged();
                OnPropertyChanged("SourcePath");
            }
        }

        public string this[string columnName]
        {
            get 
            {
                return columnName switch
                {
                    "SourcePath" => getError(SourcePath),
                    "DestinationPath" => getError(DestinationPath),
                    _ => string.Empty,
                };
            }
        }
        private string getError(string? property)
        {
            if (!Path.Exists(property)) return "Directory does not exist ...";
            else if (SourcePath == DestinationPath) return "Identical paths of the initial and final catalog ...";
            else return string.Empty;
        }
        public string Error => throw new NotImplementedException();
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string? prop = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
