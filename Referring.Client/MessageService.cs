using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Referring.Client
{
    public interface IMessageService
    {
        void ShowMessage(string message);
        void ShowWarning(string warning);
        void ShowError(string error);
    }

    public class MessageService : IMessageService
    {
        public void ShowMessage(string message)
        {
            MessageBox.Show(message, "Сообщение", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void ShowWarning(string warning)
        {
            MessageBox.Show(warning, "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        public void ShowError(string error)
        {
            MessageBox.Show(error, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
