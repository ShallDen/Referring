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

namespace Referring.Client
{
    /// <summary>
    /// Логика взаимодействия для EssayWindow.xaml
    /// </summary>
    public partial class EssayWindow : Window
    {
        public EssayWindow()
        {
            InitializeComponent();

            essayTextBox.DataContext = ReferringManager.Instance;
            referredTextSentenceCount.DataContext = ReferringManager.Instance;
            essayComparisonPercentage.DataContext = ReferringManager.Instance;

            closeButton.Click += CloseButton_Click;
            saveEssayButton.Click += SaveEssayButton_Click;
            compareEssayButton.Click += CompareEssayButton_Click;
        }

        private void CompareEssayButton_Click(object sender, RoutedEventArgs e)
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
                ReferringManager.Instance.ManualEssayPath = dlg.FileName;

                if (FileManager.IsExist(ReferringManager.Instance.ManualEssayPath))
                {
                    Logger.LogInfo("Opening file: " + ReferringManager.Instance.ManualEssayPath);
                    ReferringManager.Instance.ManualEssayText = FileManager.GetContent(ReferringManager.Instance.ManualEssayPath);
                    Logger.LogInfo("File was opened.");

                    EssayComparer comparer = new EssayComparer(ReferringManager.Instance.AutoEssayPath, ReferringManager.Instance.ManualEssayPath);
                    ReferringManager.Instance.EssayComparisonPercentage = comparer.Compare();

                    if(ReferringManager.Instance.IsComparisonCompete)
                    {

                    }
                }
                else
                {
                    MessageManager.ShowError(string.Format("Путь: {0} не верный.", ReferringManager.Instance.ManualEssayPath));
                    Logger.LogError("Path: " + ReferringManager.Instance.ManualEssayPath + " isn't valid.");
                }
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
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
    }
}
