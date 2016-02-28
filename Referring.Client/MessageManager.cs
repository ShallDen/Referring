namespace Referring.Client
{
    public class MessageManager
    {
        private static PopupWindow popup;

        public static PopupWindow Popup
        {
            get { return popup; }
        }

        public static void ShowInformation(string message)
        {
            popup = new PopupWindow();
            popup.ShowMessage("Information", message);
        }

        public static void ShowWarning(string warning)
        {
            popup = new PopupWindow();
            popup.ShowMessage("Warning", warning);
        }

        public static void ShowError(string error)
        {
            popup = new PopupWindow();
            popup.ShowMessage("Error", error);
        }
    }
}
