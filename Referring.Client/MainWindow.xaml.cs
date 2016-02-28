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
        string SourceText { get; set; }
        void FocusOnRunReferringButton();
        event RoutedEventHandler FileOpenClick;
        event RoutedEventHandler FileSaveClick;
        event SelectionChangedEventHandler ReferringCoefficientChanged;
        event RoutedEventHandler RunRefferingClick;
    }

    public partial class MainWindow : Window, IMainWindow
    {
        public MainWindow()
        {
            Logger.LogInfo("Loading main window...");
            InitializeComponent();
            FillReferringCoefficientCombobox();

            selectFileButton.Click += selectFileButton_Click;
            saveSummaryButton.Click += saveSummaryButton_Click;
            referringCoefficientCombobox.SelectionChanged += coefficientCombobox_SelectionChanged;
            runReferringButton.Click += runReferringButton_Click;

            MainPresenter presenter = new MainPresenter(this);
        }

        public event RoutedEventHandler FileOpenClick;
        public event RoutedEventHandler FileSaveClick;
        public event SelectionChangedEventHandler ReferringCoefficientChanged;
        public event RoutedEventHandler RunRefferingClick;

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
                FileManager.FileFullPath = dlg.FileName;
                FileManager.FileName = dlg.SafeFileName;

                if (FileOpenClick != null)
                    FileOpenClick(this, e);
            }
        }

        void saveSummaryButton_Click(object sender, RoutedEventArgs e)
        {
            if (FileSaveClick != null)
                FileSaveClick(sender, e);
        }

        void coefficientCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ReferringManager.ReferringCoefficient = coefficients[(sender as ComboBox).SelectedIndex];

            if (ReferringCoefficientChanged != null)
                ReferringCoefficientChanged(sender, e);
        }

        void runReferringButton_Click(object sender, RoutedEventArgs e)
        {
            if (RunRefferingClick != null)
                RunRefferingClick(sender, e);
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
