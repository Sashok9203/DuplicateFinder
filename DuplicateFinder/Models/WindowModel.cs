using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Automation.Peers;
using System.Windows.Forms;


namespace DuplicateFinder.Models
{
    internal class WindowModel : INotifyPropertyChanged
    {
        private enum State
        {
            Idle,
            Scaned,
        }
        private State CurentState { get; set; } = State.Idle;
        private List<FileInfo> dublicates;
        private void openFolder(object o)
        {
            FolderBrowserDialog fbd = new();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                if (o.ToString() == "openSource") Dirs.SourcePath = fbd.SelectedPath;
                else Dirs.DestinationPath = fbd.SelectedPath;
            }
        }

        private async void scan()
        {
            string[] files = Directory.GetFiles(Dirs.SourcePath);
            Files.Clear();
            if (files.Length == 0) return;
            foreach (string file in files)
                Files.Add(new() { Path = file });
            await Parallel.ForEachAsync<FileInfo>(Files, async (file,ct) => 
            {
                using var md5 = MD5.Create();
                using var stream = File.OpenRead(file.Path);
                file.Hash = Encoding.Default.GetString(md5.ComputeHash(stream));
            });
            dublicates.Clear();
            dublicates = Files.GroupBy(x => x.Hash).Where(z => z.Count() > 1).SelectMany(x => x.Take(x.Count() - 1)).ToList();
            foreach (var item in dublicates)
                item.HasDuplicate = true;
            CurentState = State.Scaned;
        }
        private async void move()
        {
            FileInfo[] filesToMove = Files.Except(dublicates).ToArray();
            await Parallel.ForEachAsync<FileInfo>(filesToMove, async (file, ct) => 
            {
                try { File.Move(file.Path, Path.Combine( Dirs.DestinationPath,Path.GetFileName(file.Path)),true); } catch { file.Moved = false; }
                file.Moved = true;
            });
            dublicates.Clear();
            MessageBox.Show($" {filesToMove.Length} files moved to {Dirs.DestinationPath}");
            CurentState = State.Idle;
        }


        public Directories Dirs { get; set; } 

        

        public WindowModel()
        {
            Dirs = new();
            Files = new();
            dublicates = new();
        }

        public ObservableCollection<FileInfo> Files { get; set; }

        public RelayCommand OpenFolder => new((o)=>openFolder(o));
        public RelayCommand Scan => new((o) => scan(),(o)=> Path.Exists(Dirs.SourcePath) && Dirs.SourcePath != Dirs.DestinationPath && CurentState == State.Idle);
        public RelayCommand Move => new((o) => move(), (o) => Path.Exists(Dirs.DestinationPath) && Dirs.SourcePath != Dirs.DestinationPath && Files.Count != 0 && CurentState == State.Scaned);

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string? prop = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
