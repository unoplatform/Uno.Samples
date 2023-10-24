using Microsoft.UI.Dispatching;

using System.ComponentModel;

namespace UnoBackgroundWorker.Presentation
{
    public sealed partial class MainPage : Page
    {
        // Create a BackgroundWorker instance
        private BackgroundWorker worker = new BackgroundWorker();

        private DispatcherQueue dispatcherQueue => DispatcherQueue.GetForCurrentThread();

        public MainPage()
        {
            this.InitializeComponent();

            // Set the properties of the BackgroundWorker
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;

            // Handle the events of the BackgroundWorker
            worker.DoWork += Worker_DoWork; ;
            worker.ProgressChanged += Worker_ProgressChanged;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
        }


        private void StartWorkerButton_Click(object sender, RoutedEventArgs e)
        {
            // Start the BackgroundWorker
            if (!worker.IsBusy)
            {
                worker.RunWorkerAsync();
                StatusLabel.Text = "Working...";
            }
        }

        private void CancelWorkerButton_Click(object sender, RoutedEventArgs e)
        {
            // Cancel the BackgroundWorker
            if (worker.IsBusy)
            {
                worker.CancelAsync();
                StatusLabel.Text = "Cancelling...";
            }
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            // Perform the task on a separate thread
            for (int i = 0; i <= 100; i++)
            {
                // Check for cancellation
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }

                // Simulate some work
                System.Threading.Thread.Sleep(100);

                // Report progress
                worker.ReportProgress(i);
            }
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // Update the UI with the progress on the main thread
            DispatcherQueue.TryEnqueue(DispatcherQueuePriority.Normal, () =>
            {
                ProgressBarControl.Value = e.ProgressPercentage;
                ProgressLabel.Text = $"{e.ProgressPercentage}%";
            });
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // Handle the completion or cancellation of the task on the main thread
            DispatcherQueue.TryEnqueue(DispatcherQueuePriority.Normal, () =>
            {
                if (e.Cancelled)
                {
                    StatusLabel.Text = "Cancelled";
                }
                else if (e.Error != null)
                {
                    StatusLabel.Text = "Error: " + e.Error.Message;
                }
                else
                {
                    StatusLabel.Text = "Done";
                }
            });
        }
    }
}