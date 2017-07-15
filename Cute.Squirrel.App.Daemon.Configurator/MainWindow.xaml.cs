using System;
using System.IO;
using System.Linq;
using System.Windows;

namespace Cute.Squirrel.App.Daemon.Configurator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly string destinationDirectory;


        public MainWindow()
        {
            this.InitializeComponent();

            // Retrieve destination directory
            var args = Environment.GetCommandLineArgs();
            if (args.Length != 2)
            {
                MessageBox.Show(
                    "Invalid arguments passed. Pass configuration destination directory.",
                    "App Daemon Configurator",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                this.Close();
            }
            else
            {
                this.destinationDirectory = Path.GetDirectoryName(args.Skip(1).FirstOrDefault());
            }
        }

        private void ConfirmButtonOnClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.destinationDirectory) ||
                !Directory.Exists(this.destinationDirectory))
            {
                MessageBox.Show(this.destinationDirectory, "App Daemon Configurator", MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            File.WriteAllText(Path.Combine(destinationDirectory, "daemon.conf"), this.ConfigurationUrlInput.Text);

            this.Close();
        }
    }
}
