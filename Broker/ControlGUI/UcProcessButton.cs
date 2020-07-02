using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using ControlClient;

namespace EzControlGUI
{
    public partial class UcProcessButton
    {
        private bool _isRunning;

        private bool _killingProc;
        private readonly Client _client;

        public UcProcessButton(string procName, string procUri, int procId)
        {
            InitializeComponent();

            _client = new Client(-procId);

            ProcName = procName;
            ProcUri = procUri;
            ProcId = procId;
            Task.Run(RunPeriodicAsync);

        }

        private string ProcName { get; }
        private string ProcUri { get; }
        private int ProcId { get; }



        private void LabelLogic(bool running)
        {

            if (running && _killingProc)
            {
                TglBtn.Content = ProcName + " - Killing";
                TglBtn.IsEnabled = false;
            }
            else if (running && !_killingProc)
            {
                TglBtn.Content = ProcName + " - Running";
                TglBtn.IsEnabled = true;
            } else if (!running && !_killingProc)
            {
                TglBtn.Content = ProcName;
                TglBtn.IsEnabled = true;
            }
            else if (!running && _killingProc)
            {
                TglBtn.Content = ProcName;
                TglBtn.IsEnabled = true;
                _killingProc = false;
            }
        }


        private void tglBtn_Click(object sender, RoutedEventArgs e)
        {

            // if process is running we kill it by clicking it's button.  if it isn't running we start it by clicking it's button.
            if(_client.CheckProcessStatus(ProcId))
            {
                // send kill request
                _client.RequestKill(ProcId);
                TglBtn.IsEnabled = false;
                _killingProc = true;
            } else
            {
                // start the process
                StartProcess();
            }

        }


        private void StartProcess()
        {
            var startInfo = new ProcessStartInfo(ProcUri) {Arguments = ProcId.ToString()};
            System.Diagnostics.Process.Start(startInfo);
        }


        private async Task RunPeriodicAsync()
        {

            bool? lastResult = null;
            while (true)
            {

                _isRunning = _client.CheckProcessStatus(ProcId);

                if (lastResult != _isRunning)
                {
                    void Action() => LabelLogic(_isRunning);
                    Dispatcher.Invoke(Action);
                }

                lastResult = _isRunning;
                await Task.Delay(1000);
            }

        }

    }



}
