using System.Windows;

namespace Referring.Client
{
    public interface IMessageManager
    {
        void ShowMessage(string message);
        void ShowWarning(string warning);
        void ShowError(string error);
    }

    public class MessageManager : IMessageManager
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
