﻿using System;
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
            view.ShowEssayClick += view_ShowEssayClick;
            view.ChangeCollapseModeClick += view_ChangeCollapseModeClick;
        }

        private void view_FileSelectClick(object sender, RoutedEventArgs e)
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

        private void view_FileOpenClick(object sender, RoutedEventArgs e)
        {
            if(FileManager.IsExist(FileManager.FileFullPath))
            {
                Logger.LogInfo("Opening file: " + FileManager.FileFullPath);
                ReferringManager.Instance.OriginalText = FileManager.GetContent(FileManager.FileFullPath);
                Logger.LogInfo("File was opened.");
                view.FocusOnRunReferringButton();
            }
            else
            {
                MessageManager.ShowError(string.Format("Path: {0} isn't valid", FileManager.FileFullPath));
                Logger.LogError("Path: " + FileManager.FileFullPath + " isn't valid");
            }
        }

        private void view_FileSaveClick(object sender, RoutedEventArgs e)
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

                MessageManager.ShowInformation(string.Format("Essay was saved. \nYou can find it at {0}", FileManager.FileDirectory));
                Logger.LogInfo("Essay was saved.");
            }
            else
            {
                MessageManager.ShowError(string.Format("Path: {0} isn't valid", FileManager.FileFullPath));
                Logger.LogError(string.Format("Path: {0} isn't valid", FileManager.FileFullPath));
            }
        }

        private void view_CoefficientChanged(object sender, SelectionChangedEventArgs e)
        {
            ReferringManager.Instance.ReferringCoefficient = (double)e.AddedItems[0];
            Logger.LogInfo("Referring coefficient was changed. New value: " + ReferringManager.Instance.ReferringCoefficient);
        }

        private void view_RunRefferingClick(object sender, RoutedEventArgs e)
        {
            ReferringProcess referring = new ReferringProcess();
            referring.RunReferrengProcess();
        }

        private void view_ShowEssayClick(object sender, RoutedEventArgs e)
        {
            
        }

        private void view_ChangeCollapseModeClick(object sender, RoutedEventArgs e)
        {
            view.ChangeCollapseMode();
        }
    }
}
