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
using System.Collections.ObjectModel;
using System.Configuration;

namespace Referring.Client
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    /// 
    public interface IMainWindow
    {
        void FireFileOpenEvent(object sender, RoutedEventArgs e);
        void FocusOnRunReferringButton();
        void ChangeCollapseMode();
        void SetProgressBarValue(double value);
        event RoutedEventHandler FileOpenClick;
        event RoutedEventHandler FileSaveClick;
        event RoutedEventHandler FileSelectClick;
        event SelectionChangedEventHandler ReferringCoefficientChanged;
        event SelectionChangedEventHandler WordCutLenghtChanged;
        event RoutedEventHandler RunRefferingClick;
        event RoutedEventHandler ShowEssayClick;
        event RoutedEventHandler ChangeCollapseModeClick;
    }

    public partial class MainWindow : Window, IMainWindow
    {
        public MainWindow()
        {
            Logger.LogInfo("Loading main window...");

            InitializeComponent();

            HookEvents();
            FillComboboxes();
            BindControls();
            SetReferringValues();
            PerformUIChanges();

            MainPresenter presenter = new MainPresenter(this);
        }

        public event RoutedEventHandler FileOpenClick;
        public event RoutedEventHandler FileSaveClick;
        public event RoutedEventHandler FileSelectClick;
        public event SelectionChangedEventHandler ReferringCoefficientChanged;
        public event SelectionChangedEventHandler WordCutLenghtChanged;
        public event RoutedEventHandler RunRefferingClick;
        public event RoutedEventHandler ShowEssayClick;
        public event RoutedEventHandler ChangeCollapseModeClick;

        public bool IsCollapsed { get; set; }

        private static List<double> coefficients = new List<double> { 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9 };
        private static List<int> wordLengths = new List<int> { 3, 4, 5 };

        public void FocusOnRunReferringButton()
        {
            runReferringButton.Focus();
        }

        public void ChangeCollapseMode()
        {
            if (this.IsCollapsed)
            {
                this.Width = 1003;
                this.IsCollapsed = false;
                this.changeCollapseModeButton.Content = "<<";
            }
            else
            {
                this.Width = 661;
                this.IsCollapsed = true;
                this.changeCollapseModeButton.Content = ">>";
            }
        }

        public void SetProgressBarValue(double value)
        {
            progressBar.Value = value;
        }

        private void HookEvents()
        {
            inputTextBox.TextChanged += InputTextBox_TextChanged;
            selectFileButton.Click += SelectFileButton_Click;
            referringCoefficientCombobox.SelectionChanged += CoefficientCombobox_SelectionChanged;
            wordCutCombobox.SelectionChanged += WordCutCombobox_SelectionChanged;
            runReferringButton.Click += RunReferringButton_Click;
            showEssayButton.Click += ShowEssayButton_Click;
            saveEssayButton.Click += SaveEssayButton_Click;
            changeCollapseModeButton.Click += ChangeCollapseModeButton_Click;
        }

        private void BindControls()
        {
            inputTextBox.DataContext = ReferringManager.Instance;
            usePOSDetectionCheckBox.DataContext = ReferringManager.Instance;
            useStemmingForAllTextCheckBox.DataContext = ReferringManager.Instance;
            useWordNet.DataContext = ReferringManager.Instance;
            useWordCut.DataContext = ReferringManager.Instance;
            wordGrid.DataContext = ReferringManager.Instance;
            originalTextSentenceCount.DataContext = ReferringManager.Instance;
        }

        private void FillComboboxes()
        {
            referringCoefficientCombobox.ItemsSource = coefficients;
            referringCoefficientCombobox.SelectedIndex = coefficients.Count / 2;

            wordCutCombobox.ItemsSource = wordLengths;
            wordCutCombobox.SelectedItem = wordLengths.First();
        }

        private void PerformUIChanges()
        {
            wordCutCombobox.Visibility = Visibility.Hidden;

            ChangeCollapseMode();
        }

        private void SetReferringValues()
        {
            ReferringManager.Instance.IsPOSDetectionActivated = true;
            ReferringManager.Instance.IsStemmingActivated = false;
            ReferringManager.Instance.IsWordNetActivated = true;
            ReferringManager.Instance.IsWordCutActivated = false;
            ReferringManager.Instance.ReferringCoefficient = 0.5;
            ReferringManager.Instance.WordCutLength = 3;

            AssignWordNetDirectory();
        }

        private static void AssignWordNetDirectory()
        {
            var configValue = ConfigurationManager.AppSettings["WordNetDirectory"];

            //Use 32bit Program Files path in case of 32bit system
            ReferringManager.Instance.WordNetDirectory = Environment.Is64BitOperatingSystem ? configValue : configValue.Replace(" (x86)", "");
        }

        private void UpdateSentenceCount()
        {
            var sentenceCount = ReferringManager.Instance.OriginalText.ClearUnnecessarySymbolsInText()
                            .DivideTextToSentences()
                            .ClearWhiteSpacesInList()
                            .RemoveEmptyItemsInList()
                            .ToLower().Count;
            ReferringManager.Instance.OriginalTextSentenceCount = sentenceCount;
        }

        public void FireFileOpenEvent(object sender, RoutedEventArgs e)
        {
            if (FileOpenClick != null)
                FileOpenClick(this, e);
        }

        private void SelectFileButton_Click(object sender, RoutedEventArgs e)
        {
            if (FileSelectClick != null)
                FileSelectClick(sender, e);
        }

        private void CoefficientCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ReferringCoefficientChanged != null)
                ReferringCoefficientChanged(sender, e);
        }

        private void WordCutCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (WordCutLenghtChanged != null)
                WordCutLenghtChanged(sender, e);
        }

        private void ShowEssayButton_Click(object sender, RoutedEventArgs e)
        {
            if (ShowEssayClick != null)
                ShowEssayClick(sender, e);
        }

        private void SaveEssayButton_Click(object sender, RoutedEventArgs e)
        {
            if (FileSaveClick != null)
                FileSaveClick(sender, e);
        }

        private void RunReferringButton_Click(object sender, RoutedEventArgs e)
        {
            if (RunRefferingClick != null)
                RunRefferingClick(sender, e);
        }

        private void ChangeCollapseModeButton_Click(object sender, RoutedEventArgs e)
        {
            if (ChangeCollapseModeClick != null)
                ChangeCollapseModeClick(sender, e);
        }

        private void InputTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateSentenceCount();
        }

        private void MainForm_Loaded(object sender, RoutedEventArgs e)
        {
            Logger.LogInfo("Main window was loaded.");
        }

        private void useWordCut_Checked(object sender, RoutedEventArgs e)
        {
            wordCutCombobox.Visibility = Visibility.Visible;
        }

        private void useWordCut_Unchecked(object sender, RoutedEventArgs e)
        {
            wordCutCombobox.Visibility = Visibility.Hidden;
        }

        private void usePOSDetectionCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            useWordCut.IsEnabled = false;
            useWordCut.IsChecked = false;
        }

        private void usePOSDetectionCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            useWordCut.IsEnabled = true;
        }
    }
}
