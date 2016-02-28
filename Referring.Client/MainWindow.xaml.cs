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
using Microsoft.Win32;
using Referring.Core;

namespace Referring.Client
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    /// 
    public interface IMainWindow
    {
        string FileFullPath { get; }
        string FileName { get; }
        string FileDirectory { get; }
        double ReferringCoefficient { get; }
        string SourceText { get; set; }
        void FocusOnRunReferringButton();
        event EventHandler FileOpenClick;
        event EventHandler FileSaveClick;
        event EventHandler ReferringCoefficientChanged;
        event EventHandler RunRefferingClick;
    }
    public partial class MainWindow : Window, IMainWindow
    {
        public MainWindow()
        {
            Logger.LogInfo("Loading main window...");
            InitializeComponent();
            FillReferringCoefficientCombobox();
            Logger.LogInfo("Main window loaded.");

            selectFileButton.Click += selectFileButton_Click;
            saveSummaryButton.Click += saveSummaryButton_Click;
            referringCoefficientCombobox.SelectionChanged += coefficientCombobox_SelectionChanged;
            runReferringButton.Click += runReferringButton_Click;

            ReferringManager referringManager = new ReferringManager();
            MainPresenter presenter = new MainPresenter(this, referringManager);
        }

        public event EventHandler FileOpenClick;
        public event EventHandler FileSaveClick;
        public event EventHandler ReferringCoefficientChanged;
        public event EventHandler RunRefferingClick;

        public string FileFullPath { get; private set; }
        public string FileName { get; private set; }
        public string FileDirectory { get; private set; }
        public double ReferringCoefficient { get; private set; }

        public string SourceText
        {
            get { return inputTextBox.Text; }
            set { inputTextBox.Text = value; }
        }

        private static List<double> coefficients = new List<double> { 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9 };

        void selectFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Текстовые файлы|*.txt";

            if (dlg.ShowDialog() == true)
            {
                FileFullPath = dlg.FileName;
                FileName = dlg.SafeFileName;
                FileDirectory = FileFullPath.Replace(FileName, "");

                if (FileOpenClick != null)
                    FileOpenClick(this, EventArgs.Empty);
            }
        }

        void saveSummaryButton_Click(object sender, RoutedEventArgs e)
        {
            if (FileSaveClick != null)
                FileSaveClick(sender, EventArgs.Empty);
        }

        void coefficientCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ReferringCoefficient = coefficients[(sender as ComboBox).SelectedIndex];

            if (ReferringCoefficientChanged != null)
                ReferringCoefficientChanged(sender, EventArgs.Empty);
        }

        void runReferringButton_Click(object sender, RoutedEventArgs e)
        {
            if (RunRefferingClick != null)
                RunRefferingClick(sender, EventArgs.Empty);
        }

        public void FocusOnRunReferringButton()
        {
            runReferringButton.Focus();
        }

        private void FillReferringCoefficientCombobox()
        {
            referringCoefficientCombobox.ItemsSource = coefficients;
            referringCoefficientCombobox.SelectedIndex = coefficients.Count / 2;
        }

        private void MainForm_Loaded(object sender, RoutedEventArgs e)
        {
            Logger.LogInfo("Main window was loaded.");
        }
    }
}
