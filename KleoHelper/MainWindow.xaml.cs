using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KleoHelper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly CommandLineInterpreter cli; 
        public MainWindow()
        {
            InitializeComponent();
            cli = new CommandLineInterpreter { IsGuiEnabled = true };
            buttonStack.DataContext = cli;
        }

        private void Source_Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog
            {
                Multiselect = true,
                DefaultExt = ".pgp",
                Filter = "PGP Encryption (.pgp)|*.pgp|Possibly Encrypted Files (.pgp,.gpg,.csv,.txt)|*.pgp;*.gpg;*.csv;*.txt|All Types|*.*"
            };
            cli.SourceFiles.Clear();
            if (dlg.ShowDialog() == true)
            {
                foreach (var _ in dlg.FileNames)
                {
                    cli.SourceFiles.Add(_);
                    TextOutput.Text += $"\n{_} Added!";
                }
            }
        }

        private void Output_Button_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog
            {
                FileName = "Decrypted_Files",
                DefaultExt = ".zip",
                Filter = "Zip File (.zip)|*.zip",
            };
            if (dlg.ShowDialog() == true)
            {
                cli.ZipOutput = dlg.FileName;
            }
            TextOutput.Text += $"\nSending output to: {cli.ZipOutput}";
        }

        private async void Run_Button_Click(object sender, RoutedEventArgs e)
        {
            TextOutput.Text += "\nStarting process....";
            if (cli.SourceFiles.Count > 0)
            {
                PrepareGUI(true);
                foreach (var file in cli.SourceFiles)
                {
                    TextOutput.Text += $"\nWorking {file}";
                    string lineOut = await Task.Run(() => cli.DecryptFile(file));
                    TextOutput.Text += $"\n{lineOut}";
                }
                if (cli.ZipOutput != null)
                {
                    TextOutput.Text += "\nSending to zip....";
                    await Task.Run(() => cli.SendToZip());
                }
                TextOutput.Text += "\nProcess Complete!";
                PrepareGUI(false);
            }
            else TextOutput.Text += "\nPlease select files first";
        }
         private void PrepareGUI(bool isStarting)
        {
            if (isStarting)
            {

                ProcessRunning.Visibility = Visibility.Visible;
                cli.IsGuiEnabled = false;
            }
            else
            {

                ProcessRunning.Visibility = Visibility.Hidden;
                cli.IsGuiEnabled = true;
            }
        }
    }
}
