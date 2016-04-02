using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            view.RunRefferingClick += view_RunRefferingClick;
        }

        void view_FileSelectClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Текстовые файлы|*.txt";

            if (dlg.ShowDialog() == true)
            {
                FileManager.FileFullPath = dlg.FileName;
                FileManager.FileName = dlg.SafeFileName;

                view.FireFileOpenEvent(sender, e);
            }
        }

        void view_FileOpenClick(object sender, RoutedEventArgs e)
        {
            if(FileManager.IsExist(FileManager.FileFullPath))
            {
                Logger.LogInfo("Opening file: " + FileManager.FileFullPath);
                ReferringManager.Instance.OriginalText = FileManager.GetContent(FileManager.FileFullPath);
                Logger.LogInfo("File was opened.");
                view.SourceText = ReferringManager.Instance.OriginalText;
                view.FocusOnRunReferringButton();
            }
            else
            {
                MessageManager.ShowError(string.Format("Path: {0} isn't valid", FileManager.FileFullPath));
                Logger.LogError("Path: " + FileManager.FileFullPath + " isn't valid");
            }
        }

        void view_FileSaveClick(object sender, RoutedEventArgs e)
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
                
                if(FileManager.IsExist(fileName))
                    FileManager.Delete(fileName);

                FileManager.SaveContent(ReferringManager.Instance.ReferredText, fileName);

                MessageManager.ShowInformation(string.Format("Essay was saved. You can find it at {0}", FileManager.FileDirectory));
                Logger.LogInfo("Essay was saved.");
            }
            else
            {
                MessageManager.ShowError(string.Format("Path: {0} isn't valid", FileManager.FileFullPath));
                Logger.LogError(string.Format("Path: {0} isn't valid", FileManager.FileFullPath));
            }
        }

        void view_CoefficientChanged(object sender, SelectionChangedEventArgs e)
        {
            ReferringManager.Instance.ReferringCoefficient = (double)e.AddedItems[0];
            Logger.LogInfo("Referring coefficient was changed. New value: " + ReferringManager.Instance.ReferringCoefficient);
        }

        void view_RunRefferingClick(object sender, RoutedEventArgs e)
        {
            ReferringProcess referring = new ReferringProcess();
            referring.RunReferrengProcess();
        }
    }
}
