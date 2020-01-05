using Microsoft.WindowsAPICodePack.Shell;
using System;
using System.Collections.Generic;
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
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Drawing;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;

namespace UMJA
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region ------------------------- Properties -------------------------

        private bool windowLoaded = false;

        public string Ordnername { get; set; }
        public string Speicherort { get; set; } = $@"C:\Users\{Environment.UserName}\Documents\";
        public string AusgangsdateiPfad { get; set; }

        #endregion

        #region ------------------ Konstruktoren ----------------------------

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtSpeicherort.Text = $@"C:\Users\{Environment.UserName}\Documents\";
            windowLoaded = true;
        }

        #endregion 

        #region --------------------------- Drag & Drop ------------------------------

        private void ImagePanel_Drop(object sender, DragEventArgs e)
        {

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                lsbLogConsole.Items.Add($"Die Ausgangsdatei {files[0].Split('\\').Last()} (Pfad: {files[0]}) wurde per Drag&Drop ausgewählt.");
                SetSourceFile(files[0]);
            }
        }

        private void TextBox_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }

        #endregion

        #region --------------------------- Events -----------------------------

        private void BtnSpeicherort_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            dialog.InitialDirectory = $@"C:\Users\{Environment.UserName}\Documents\";

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                txtSpeicherort.Text = $"{dialog.FileName}\\";
                lsbLogConsole.Items.Add($"Der Speicherort wurde unter {dialog.FileName} festgelegt");
            }
        }

        private void BtnSourceFile_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "graphml files (*.graphml)|*.graphml";

            if (dialog.ShowDialog() == true)
            {
                SetSourceFile(dialog.FileName);
                lsbLogConsole.Items.Add($"Die Ausgangsdatei {dialog.FileName.Split('\\').Last()} (Pfad: {dialog.FileName}) wurde per FileChooser ausgewählt.");
            }
        }

        private void BtnConvert_Click(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(Speicherort) && AusgangsdateiPfad.Split('.').Last().Equals("graphml"))
            {
                var parsedJavaObjects = Logic.ReadDocument(AusgangsdateiPfad);
                lsbLogConsole.Items.Add($"Die Ausgangsdatei wurde erfolgreich eingelesen und verarbeitet.");
                Logic.CreateProject(parsedJavaObjects, Speicherort, Ordnername);
            }
            else
                lsbLogConsole.Items.Add($"Die Ausgangsdatei konnte aufgrund falscher Angaben nicht konvertiert werden.");
        }

        private void Ordnername_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (windowLoaded)
            {
                Ordnername = txtOrdnername.Text;
                //lsbLogConsole.Items.Add($"Der Ordnername wurde durch die Textbox auf {Ordnername} geändert.");
            }
        }

        private void Speicherort_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (windowLoaded)
            {
                Speicherort = txtSpeicherort.Text;
                lsbLogConsole.Items.Add($"Der Speicherort wurde durch die Textbox auf {Speicherort} geändert.");
            }
        }

        private void Ausgangsdatei_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (windowLoaded)
            {
                AusgangsdateiPfad = txtSourcePath.Text;
                lsbLogConsole.Items.Add($"Der Ausgangsdatei wurde durch die Textbox auf {AusgangsdateiPfad} geändert.");
            }
        }

        #endregion

        #region --------------------------------- Helper ----------------------------------

        private void SetSourceFile(string speicherort)
        {
            txtSourcePath.FontStyle = FontStyles.Normal;
            txtSourcePath.Text = speicherort;
            imgFile.Visibility = Visibility.Visible;
            imgFile.Source = ShellFile.FromFilePath(speicherort).Thumbnail.BitmapSource;
        }

        #endregion

    }
}
