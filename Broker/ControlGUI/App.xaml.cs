using ControlClient;
using System.Windows;

namespace EzControlGUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private void App_OnExit(object sender, ExitEventArgs e)
        {
            // If the server is still listening when we close the gui we ask the user if they want to shut it down.
            var client = new Client(-999);
            if (!client.IsListening()) return;
            var result = MessageBox.Show("The message broker server process is still active.  Would you like to shut down the server?", "Shut it down?", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                client.KillServer();
            }

        }




    }
}
