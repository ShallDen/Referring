using System;
using System.Threading;
using System.Threading.Tasks;
using Referring.Core;
using System.Windows.Controls;
using System.Windows;
using Microsoft.Win32;

namespace Referring.Client
{
    public class MainPresenter
    {
        private readonly IMainWindow view;

        public MainPresenter(IMainWindow _view)
        {
            view = _view;

            view.FileSelectClick += view_FileSelectClick;
            view.FileOpenClick += view_FileOpenClick;
            view.FileSaveClick += view_FileSaveClick;
            view.ReferringCoefficientChanged += view_CoefficientChanged;
            view.WordCutLenghtChanged += View_WordCutLenghtChanged;
            view.RunRefferingClick += view_RunRefferingClick;
            view.ShowEssayClick += view_ShowEssayClick;
            view.ChangeCollapseModeClick += view_ChangeCollapseModeClick;
        }

        private void view_FileSelectClick(object sender, RoutedEventArgs e)
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
                FileManager.FileFullPath = dlg.FileName;
                FileManager.FileName = dlg.SafeFileName;

                view.FireFileOpenEvent(sender, e);
            }
        }

        private void view_FileOpenClick(object sender, RoutedEventArgs e)
        {
            if (FileManager.IsExist(FileManager.FileFullPath))
            {
                Logger.LogInfo("Opening file: " + FileManager.FileFullPath);
                ReferringManager.Instance.OriginalText = FileManager.GetContent(FileManager.FileFullPath);
                Logger.LogInfo("File was opened.");
                view.FocusOnRunReferringButton();
            }
            else
            {
                MessageManager.ShowError(string.Format("Путь: {0} не верный.", FileManager.FileFullPath));
                Logger.LogError("Path: " + FileManager.FileFullPath + " isn't valid.");
            }
        }

        private void view_FileSaveClick(object sender, RoutedEventArgs e)
        {
            if (!ReferringManager.Instance.IsReferringCompete)
            {
                MessageManager.ShowWarning("Пожалуйста, для начала выполните операцию реферирования.");
                Logger.LogWarning("Please, perform referring process before saving summary.");
                return;
            }

            if (FileManager.IsExist(FileManager.FileFullPath))
            {
                Logger.LogInfo("Saving essay: " + ReferringManager.Instance.AutoEssayPath);
                
                if(FileManager.IsExist(ReferringManager.Instance.AutoEssayPath))
                    FileManager.Delete(ReferringManager.Instance.AutoEssayPath);

                FileManager.SaveContent(ReferringManager.Instance.ReferredText, ReferringManager.Instance.AutoEssayPath);

                MessageManager.ShowInformation(string.Format("Реферат сохранен. \nВы можете найти его в папке {0}", FileManager.FileDirectory));
                Logger.LogInfo("Essay was saved.");
            }
            else
            {
                MessageManager.ShowError(string.Format("Путь: {0} не верный.", FileManager.FileFullPath));
                Logger.LogError(string.Format("Path: {0} isn't valid", FileManager.FileFullPath));
            }
        }

        private void view_CoefficientChanged(object sender, SelectionChangedEventArgs e)
        {
            ReferringManager.Instance.ReferringCoefficient = (double)e.AddedItems[0];
            Logger.LogInfo("Referring coefficient was changed. New value: " + ReferringManager.Instance.ReferringCoefficient);
        }

        private void View_WordCutLenghtChanged(object sender, SelectionChangedEventArgs e)
        {
            ReferringManager.Instance.WordCutLength = (int)e.AddedItems[0];
            Logger.LogInfo("Word cut length was changed. New value: " + ReferringManager.Instance.WordCutLength);
        }

        private void view_RunRefferingClick(object sender, RoutedEventArgs e)
        {
            if (ReferringManager.Instance.IsReferringRunning)
            {
                MessageManager.ShowWarning("Процесс реферирования уже запущен. Пожалуйста, дождитесь окончания операции.");
                return;
            }

            //Check WordNet directory and files
            try
            {
                WordNetManager.CheckWordNetPaths(ReferringManager.Instance.WordNetDirectory);
            }
            catch (Exception ex)
            {
                MessageManager.ShowError(ex.Message);
                Logger.LogError(ex.Message);
                return;
            }

            ReferringProcess referring = null; 

            //Initialize referring and WordNet
            try
            {
                referring = new ReferringProcess();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                MessageManager.ShowError(ex.InnerException.Message);
                Logger.LogError(ex.InnerException.Message);
                return;
            }

            referring.ProgressChanged += Referring_ProgressChanged;
            referring.WorkCompleted += Referring_WorkCompleted;

            //Run referring process
            var syncContext = SynchronizationContext.Current;
            Task task = Task.Factory.StartNew(referring.RunReferrengProcess, syncContext);
        }

        private void view_ShowEssayClick(object sender, RoutedEventArgs e)
        {
            if (!ReferringManager.Instance.IsReferringCompete)
            {
                MessageManager.ShowWarning("Пожалуйста, для начала выполните операцию реферирования.");
                Logger.LogWarning("Please, perform referring process before saving summary.");
                return;
            }

            EssayWindow essayWindow = new EssayWindow();
            essayWindow.Show();
        }

        private void view_ChangeCollapseModeClick(object sender, RoutedEventArgs e)
        {
            view.ChangeCollapseMode();
        }

        private void Referring_ProgressChanged(double percent)
        {
            view.SetProgressBarValue(percent);
        }

        private void Referring_WorkCompleted(string elapsedTime)
        {
            ReferringManager.Instance.IsReferringRunning = false;
            MessageManager.ShowInformation("Реферирование выполнено! \nВремя операции: " + elapsedTime + "\nТеперь Вы можете сохранить реферат.");
        }
    }
}
