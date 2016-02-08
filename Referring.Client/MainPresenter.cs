﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Referring.Core;

namespace Referring.Client
{
    public class MainPresenter
    {
        private readonly IMainWindow view;
        private readonly IFileManager fileManager;
        private readonly IMessageService messageService;

        public MainPresenter(IMainWindow _view, IFileManager _manager, IMessageService _messageService)
        {
            view = _view;
            fileManager = _manager;
            messageService = _messageService;

            view.FileOpenClick += view_FileOpenClick;
            view.FileSaveClick += view_FileSaveClick;
            view.ReferringCoefficientChanged += view_CoefficientChanged;
            view.RunRefferingClick += view_RunRefferingClick;
        }

        void view_FileOpenClick(object sender, EventArgs e)
        {
            if(fileManager.IsExist(view.FileFullPath))
            {
                Logger.LogInfo("Opening file: " + view.FileFullPath);
                view.SourceText = fileManager.GetContent(view.FileFullPath);
                Logger.LogInfo("File was opened.");
                view.ShowSourceText();
            }
            else
            {
                Logger.LogError("Path: " + view.FileFullPath + " isn't valid");
                messageService.ShowError("Path: " + view.FileFullPath + " isn't valid");
            }
        }

        void view_FileSaveClick(object sender, EventArgs e)
        {
            if (fileManager.IsExist(view.FileFullPath))
            {
                string fileName = string.Concat(view.FileDirectory + "Summary_" + view.FileName);
                Logger.LogInfo("Saving file: " + fileName);
                
                if(fileManager.IsExist(fileName))
                    fileManager.Delete(fileName);

                fileManager.SaveContent(view.SourceText, fileName); // TODO: change view.SourceText to real summary text after its implementation
                Logger.LogInfo("File was saved.");
            }
            else
            {
                Logger.LogError("Path: " + view.FileFullPath + " isn't valid");
                messageService.ShowError("Path: " + view.FileFullPath + " isn't valid");
            }
        }

        void view_CoefficientChanged(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        void view_RunRefferingClick(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
