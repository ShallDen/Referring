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
        event RoutedEventHandler FileOpenClick;
        event RoutedEventHandler FileSaveClick;
        event RoutedEventHandler FileSelectClick;
        event SelectionChangedEventHandler ReferringCoefficientChanged;
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
            FillReferringCoefficientCombobox();

            selectFileButton.Click += SelectFileButton_Click;
            referringCoefficientCombobox.SelectionChanged += CoefficientCombobox_SelectionChanged;
            runReferringButton.Click += RunReferringButton_Click;
            showEssayButton.Click += ShowEssayButton_Click;
            saveEssayButton.Click += SaveEssayButton_Click;
            changeCollapseModeButton.Click += ChangeCollapseModeButton_Click;
            
            MainPresenter presenter = new MainPresenter(this);

            inputTextBox.DataContext = ReferringManager.Instance;
            usePOSDetectionCheckBox.DataContext = ReferringManager.Instance;
            useStemmingForAllTextCheckBox.DataContext = ReferringManager.Instance;
            useWordNet.DataContext = ReferringManager.Instance;
            wordGrid.DataContext = ReferringManager.Instance;

            ReferringManager.Instance.IsPOSDetectionActivated = true;
            ReferringManager.Instance.IsStemmingActivated = true;
            ReferringManager.Instance.IsWordNetActivated = true;
            ReferringManager.Instance.ReferringCoefficient = 0.5;

            ChangeCollapseMode();

            //TODO: Delete this code when development will be complete
            ///////////////////////

            string userName = Environment.UserName;
            FileManager.FileFullPath = "C:\\Users\\" + userName + "\\Desktop\\text.txt";
            FileManager.FileName = "text.txt";

            if (FileOpenClick != null)
               FileOpenClick(this, new RoutedEventArgs());

            ///////////////////////
        }

        public event RoutedEventHandler FileOpenClick;
        public event RoutedEventHandler FileSaveClick;
        public event RoutedEventHandler FileSelectClick;
        public event SelectionChangedEventHandler ReferringCoefficientChanged;
        public event RoutedEventHandler RunRefferingClick;
        public event RoutedEventHandler ShowEssayClick;
        public event RoutedEventHandler ChangeCollapseModeClick;


        public bool IsCollapsed { get; set; }

        private static List<double> coefficients = new List<double> { 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9 };

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

        private void FillReferringCoefficientCombobox()
        {
            referringCoefficientCombobox.ItemsSource = coefficients;
            referringCoefficientCombobox.SelectedIndex = coefficients.Count / 2;
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

        private void MainForm_Loaded(object sender, RoutedEventArgs e)
        {
            Logger.LogInfo("Main window was loaded.");
        }
    }
}
