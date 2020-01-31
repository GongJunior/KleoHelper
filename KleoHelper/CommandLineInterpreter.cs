using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Runtime.CompilerServices;

namespace KleoHelper
{
    class CommandLineInterpreter : INotifyPropertyChanged
    {
		#region EVENTS
		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged([CallerMemberName] string name = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}
        #endregion


        #region PROPERTIES
        private bool isGuiEnabled;
		public bool IsGuiEnabled
		{
			get
			{
				return isGuiEnabled;
			}
			set
			{
				isGuiEnabled = value;
				OnPropertyChanged("IsGuiEnabled");
			}
		}

        public List<string> SourceFiles = new List<string>();
		public string ZipOutput = null;
        #endregion

        public string DecryptFile(string file)
		{
			using Process p = new Process();
			p.StartInfo.FileName = "cmd.exe";
			p.StartInfo.Arguments = $@"/C gpg -d ""{file}"" > ""{file}.csv""";
			p.StartInfo.UseShellExecute = false;
			p.StartInfo.RedirectStandardOutput = true;
			p.StartInfo.RedirectStandardError = true;
			p.StartInfo.CreateNoWindow = true;
			p.Start();
			var output = p.StandardOutput.ReadToEnd();
			output += p.StandardError.ReadToEnd();
			p.WaitForExit();
			return output;
		}

		public void SendToZip()
		{
			if (File.Exists(ZipOutput)) File.Delete(ZipOutput);

			using FileStream zip = new FileStream(ZipOutput, FileMode.Create);
			using ZipArchive archive = new ZipArchive(zip, ZipArchiveMode.Create);
			foreach (var file in SourceFiles)
			{
				var entryName = $"{ParseFileNameToScreen(file)}.csv";
				archive.CreateEntryFromFile($"{file}.csv", entryName);
			}
		}

		public string ParseFileNameToScreen(string fullanme)
		{
			return fullanme.Substring(fullanme.LastIndexOf(@"\") + 1);
		}

	}
}
