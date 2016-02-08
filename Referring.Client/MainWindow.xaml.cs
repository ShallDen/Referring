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
        void ShowSourceText();
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
            selectFileButton.Click += selectFileButton_Click;
            saveSummaryButton.Click += saveSummaryButton_Click;
            coefficientCombobox.SelectionChanged += coefficientCombobox_SelectionChanged;
            runReferringButton.Click += runReferringButton_Click;

            MessageService service = new MessageService();
            FileManager fileManager = new FileManager();
            MainPresenter presenter = new MainPresenter(this, fileManager, service);
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
            if (ReferringCoefficientChanged != null)
                ReferringCoefficientChanged(sender, EventArgs.Empty);
        }

        void runReferringButton_Click(object sender, RoutedEventArgs e)
        {
            if (RunRefferingClick != null)
                RunRefferingClick(sender, EventArgs.Empty);
        }

        public void ShowSourceText()
        {
            inputTextBox.Text = SourceText;
        }

        private void MainForm_Loaded(object sender, RoutedEventArgs e)
        {
            Logger.LogInfo("Main window was loaded.");
        }
    }
}
