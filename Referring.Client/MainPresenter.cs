using System;
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
        private readonly IReferringManager referringManager;

        public MainPresenter(IMainWindow _view, IReferringManager _referringgManager)
        {
            view = _view;
            referringManager = _referringgManager;

            view.FileOpenClick += view_FileOpenClick;
            view.FileSaveClick += view_FileSaveClick;
            view.ReferringCoefficientChanged += view_CoefficientChanged;
            view.RunRefferingClick += view_RunRefferingClick;
        }

        void view_FileOpenClick(object sender, EventArgs e)
        {
            if(FileManager.IsExist(view.FileFullPath))
            {
                Logger.LogInfo("Opening file: " + view.FileFullPath);
                view.SourceText = FileManager.GetContent(view.FileFullPath);
                Logger.LogInfo("File was opened.");
                view.FocusOnRunReferringButton();
            }
            else
            {
                MessageManager.ShowError(string.Format("Path: {0} isn't valid", view.FileFullPath));
                Logger.LogError("Path: " + view.FileFullPath + " isn't valid");
            }
        }

        void view_FileSaveClick(object sender, EventArgs e)
        {
            if (!referringManager.IsReferringCompete)
            {
                MessageManager.ShowWarning("Please, perform referring process before saving summary.");
                Logger.LogWarning("Please, perform referring process before saving summary.");
                return;
            }

            if (FileManager.IsExist(view.FileFullPath))
            {
                string fileName = string.Concat(view.FileDirectory + "Summary_" + view.FileName);
                Logger.LogInfo("Saving file: " + fileName);
                
                if(FileManager.IsExist(fileName))
                    FileManager.Delete(fileName);

                FileManager.SaveContent(view.SourceText, fileName); // TODO: change view.SourceText to real summary text after its implementation
                MessageManager.ShowMessage("File was saved.");
                Logger.LogInfo("File was saved.");
            }
            else
            {
                MessageManager.ShowError(string.Format("Path: {0} isn't valid", view.FileFullPath));
                Logger.LogError(string.Format("Path: {0} isn't valid", view.FileFullPath));
            }
        }

        void view_CoefficientChanged(object sender, EventArgs e)
        {
            Logger.LogInfo("Referring coefficient was changed. New value: " + view.ReferringCoefficient);
        }

        void view_RunRefferingClick(object sender, EventArgs e)
        {
            Logger.LogInfo("Starting referring process...");

            var testsent = view.SourceText.DivideToSentences();
            var testwords = view.SourceText.DivideToWords();

            MessageManager.ShowWarning("This feature isn't implemented yet!");
            Logger.LogWarning("This feature isn't implemented yet!");
        }
    }
}
