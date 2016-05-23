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
using System.Windows.Shapes;
using Referring.Core;
using Microsoft.Win32;
using System.Threading;

namespace Referring.Client
{
    /// <summary>
    /// Логика взаимодействия для EssayWindow.xaml
    /// </summary>
    public partial class EssayWindow : Window
    {
        private static List<string> types = new List<string>
        {
            "по полноте",
            "по точности (с учетом погрешности)",
            "по точности (с использованием коэф. значимости)"
        };

        public EssayWindow()
        {
            InitializeComponent();

            essayTextBox.DataContext = ReferringManager.Instance;
            referredTextSentenceCount.DataContext = ReferringManager.Instance;
            essayComparisonPercentage.DataContext = EssayComparisonManager.Instance;
            useCurrentEssayAsFirstFile.DataContext = EssayComparisonManager.Instance;

            saveEssayButton.Click += SaveEssayButton_Click;
            compareEssayButton.Click += CompareEssayButton_Click;
            chooseFisrtFileButton.Click += ChooseFisrtFileButton_Click;
            chooseSecondFileButton.Click += ChooseSecondFileButton_Click;

            useCurrentEssayAsFirstFile.Checked += UseCurrentEssayAsFirstFile_Checked;
            useCurrentEssayAsFirstFile.Unchecked += UseCurrentEssayAsFirstFile_Unchecked;
            fullSentenceTypeRadioButton.Checked += FullSentenceTypeRadioButton_Checked;
            mainWordTypeRadioButton.Checked += MainWordTypeRadioButton_Checked;
            mainWordTypeRadioButton.Unchecked += MainWordTypeRadioButton_Unchecked;
            mainWordTypeSelectionComboBox.SelectionChanged += MainWordTypeSelectionComboBox_SelectionChanged;

            hitLabel.Visibility = Visibility.Hidden;
            essayComparisonPercentage.Visibility = Visibility.Hidden;

            EssayComparisonManager.Instance.ComparisonType = ComparisonType.MainWordFulness;
            EssayComparisonManager.Instance.IsUseCurrentEssayAsFirstFile = true;

            mainWordTypeRadioButton.IsChecked = true;

            FillCombobox();
        }

        private void FillCombobox()
        {
            mainWordTypeSelectionComboBox.ItemsSource = types;
            mainWordTypeSelectionComboBox.SelectedValue = types.First();
        }

        private void MainWordTypeRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            EssayComparisonManager.Instance.ComparisonType = ComparisonType.MainWordFulness;
            mainWordTypeSelectionComboBox.IsEnabled = true;
        }

        private void MainWordTypeRadioButton_Unchecked(object sender, RoutedEventArgs e)
        {
            EssayComparisonManager.Instance.ComparisonType = ComparisonType.FullSentences;
            mainWordTypeSelectionComboBox.IsEnabled = false;
        }

        private void FullSentenceTypeRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            mainWordTypeSelectionComboBox.SelectedValue = types.First();
            EssayComparisonManager.Instance.ComparisonType = ComparisonType.FullSentences;
        }

        private void MainWordTypeSelectionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectionValue = (string)e.AddedItems[0];

            switch (selectionValue)
            {
                case "по полноте":
                    EssayComparisonManager.Instance.ComparisonType = ComparisonType.MainWordFulness;
                    break;
                case "по точности (с учетом погрешности)":
                    EssayComparisonManager.Instance.ComparisonType = ComparisonType.MainWordAccuracyWithError;
                    break;
                case "по точности (с использованием коэф. значимости)":
                    EssayComparisonManager.Instance.ComparisonType = ComparisonType.MainWordAccuracyWithSignificanceKoefficient;
                    break;
                default:
                    EssayComparisonManager.Instance.ComparisonType = ComparisonType.MainWordFulness;
                    break;
            }
        }

        private void ChooseFisrtFileButton_Click(object sender, RoutedEventArgs e)
        {
            if (ReferringManager.Instance.IsReferringRunning)
            {
                MessageManager.ShowWarning("Процесс реферирования уже запущен. Пожалуйста, дождитесь окончания операции.");
                return;
            }

            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Текстовые файлы|*.txt";

            if (dlg.ShowDialog() == true)
            {
                EssayComparisonManager.Instance.FisrtEssayPath = dlg.FileName;
            }
        }

        private void ChooseSecondFileButton_Click(object sender, RoutedEventArgs e)
        {
            if (ReferringManager.Instance.IsReferringRunning)
            {
                MessageManager.ShowWarning("Процесс реферирования уже запущен. Пожалуйста, дождитесь окончания операции.");
                return;
            }

            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Текстовые файлы|*.txt";

            if (dlg.ShowDialog() == true)
            {
                EssayComparisonManager.Instance.SecondEssayPath = dlg.FileName;
            }
        }

        private void CompareEssayButton_Click(object sender, RoutedEventArgs e)
        {
            if (ReferringManager.Instance.IsReferringRunning)
            {
                MessageManager.ShowWarning("Процесс реферирования уже запущен. Пожалуйста, дождитесь окончания операции.");
                return;
            }

            if (!LoadEssays())
                return;

            Logger.LogInfo("Essays were loaded. Starting comparison.");

            EssayComparisonProcess comparisonProcess = new EssayComparisonProcess();

            comparisonProcess.ProgressChanged += EssayComparison_ProgressChanged;
            comparisonProcess.WorkCompleted += EssayComparison_WorkCompleted;

            //Run comparison process
            var syncContext = SynchronizationContext.Current;
            Task task = Task.Factory.StartNew(comparisonProcess.RunComparisonProcess, syncContext);    
        }

        private bool LoadEssays()
        {
            string firstEssay = string.Empty;
            string secondtEssay = string.Empty;

            bool isLoaded = false;
            bool isFirstEssaySpecified = !string.IsNullOrEmpty(EssayComparisonManager.Instance.FisrtEssayPath);
            bool isSecondEssaySpecified = !string.IsNullOrEmpty(EssayComparisonManager.Instance.SecondEssayPath);

            if (EssayComparisonManager.Instance.IsUseCurrentEssayAsFirstFile && isSecondEssaySpecified)
            {
                firstEssay = ReferringManager.Instance.ReferredText;

                if (FileManager.IsExist(EssayComparisonManager.Instance.SecondEssayPath))
                {
                    secondtEssay = FileManager.GetContent(EssayComparisonManager.Instance.SecondEssayPath);
                    isLoaded = true;
                }
            }

            else if (isFirstEssaySpecified && isSecondEssaySpecified)
            {
                if (FileManager.IsExist(EssayComparisonManager.Instance.FisrtEssayPath) && FileManager.IsExist(EssayComparisonManager.Instance.SecondEssayPath))
                {
                    firstEssay = FileManager.GetContent(EssayComparisonManager.Instance.FisrtEssayPath);
                    secondtEssay = FileManager.GetContent(EssayComparisonManager.Instance.SecondEssayPath);
                    isLoaded = true;
                }
            }
            else
            {
                MessageManager.ShowWarning("Пожалуйста выберите оба реферата для сравнения!");
                Logger.LogWarning("Пожалуйста выберите оба реферата для сравнения!");
                isLoaded = false;
            }

            EssayComparisonManager.Instance.FisrtEssay = firstEssay;
            EssayComparisonManager.Instance.SecondEssay = secondtEssay;

            return isLoaded;
        }

        private void EssayComparison_WorkCompleted(string elapsedTime)
        {
            string extraMessage = string.Empty;

            EssayComparisonManager.Instance.IsComparisonCompete = true;

            if (EssayComparisonManager.Instance.ComparisonType != ComparisonType.MainWordAccuracyWithSignificanceKoefficient)
            {
                hitLabel.Visibility = Visibility.Visible;
                essayComparisonPercentage.Visibility = Visibility.Visible;
                extraMessage = "\nРефераты идентичны на " + string.Format("{0:0}%", EssayComparisonManager.Instance.EssayComparisonPercentage);
            }
            else
            {
                hitLabel.Visibility = Visibility.Hidden;
                essayComparisonPercentage.Visibility = Visibility.Hidden;

                extraMessage = "Полученная статистика была сохранена в CSV файл.";
            }

            MessageManager.ShowInformation("Сравнение рефератов завершено. " + extraMessage + "\nВремя операции: " + elapsedTime);
            Logger.LogInfo("Сравнение рефератов завершено. " + extraMessage + "\nВремя операции: " + elapsedTime);
        }

        private void SaveEssayButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ReferringManager.Instance.IsReferringCompete)
            {
                MessageManager.ShowWarning("Пожалуйста, для начала выполните операцию реферирования.");
                Logger.LogWarning("Please, perform referring process before saving summary.");
                return;
            }

            if (FileManager.IsExist(FileManager.FileFullPath))
            {
                string fileName = string.Concat(FileManager.FileDirectory + "Summary_" + FileManager.FileName);
                Logger.LogInfo("Saving essay: " + fileName);

                if (FileManager.IsExist(fileName))
                    FileManager.Delete(fileName);

                FileManager.SaveContent(ReferringManager.Instance.ReferredText, fileName);

                MessageManager.ShowInformation(string.Format("Реферат сохранен. \nВы можете найти его в папке {0}", FileManager.FileDirectory));
                Logger.LogInfo("Essay was saved.");
            }
            else
            {
                MessageManager.ShowError(string.Format("Путь: {0} не верный.", FileManager.FileFullPath));
                Logger.LogError(string.Format("Path: {0} isn't valid", FileManager.FileFullPath));
            }
        }

        private void UseCurrentEssayAsFirstFile_Unchecked(object sender, RoutedEventArgs e)
        {
            chooseFisrtFileButton.IsEnabled = true;
        }

        private void UseCurrentEssayAsFirstFile_Checked(object sender, RoutedEventArgs e)
        {
            chooseFisrtFileButton.IsEnabled = false;
        }

        public void SetProgressBarValue(double value)
        {
            progressBar.Value = value;
        }

        private void EssayComparison_ProgressChanged(double percent)
        {
            SetProgressBarValue(percent);
        }
    }
}
