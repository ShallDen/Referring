namespace Referring.Client
{
    public static class MessageManager
    {
        private static PopupWindow popup;

        public static PopupWindow Popup
        {
            get { return popup; }
        }

        public static void ShowInformation(string message)
        {
            popup = new PopupWindow();
            popup.ShowMessage("Информация", message);
        }

        public static void ShowWarning(string warning)
        {
            popup = new PopupWindow();
            popup.ShowMessage("Предупреждение", warning);
        }

        public static void ShowError(string error)
        {
            popup = new PopupWindow();
            popup.ShowMessage("Ошибка", error);
        }
    }
}
