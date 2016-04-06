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

            closeButton.Click += CloseButton_Click;
            saveEssayButton.Click += SaveEssayButton_Click;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SaveEssayButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ReferringManager.Instance.IsReferringCompete)
            {
                MessageManager.ShowWarning("Please, perform referring process before saving summary.");
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

                MessageManager.ShowInformation(string.Format("Essay was saved. \nYou can find it at {0}", FileManager.FileDirectory));
                Logger.LogInfo("Essay was saved.");
            }
            else
            {
                MessageManager.ShowError(string.Format("Path: {0} isn't valid", FileManager.FileFullPath));
                Logger.LogError(string.Format("Path: {0} isn't valid", FileManager.FileFullPath));
            }
        }
    }
}
