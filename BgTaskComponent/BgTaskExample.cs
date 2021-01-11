using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Background;
using Windows.System.Threading;

namespace BgTaskComponent
{
    public sealed class BgTaskExample: IBackgroundTask
    {
        BackgroundTaskCancellationReason _cancelReason = BackgroundTaskCancellationReason.Abort;
        volatile bool _cancelRequested = false;
        BackgroundTaskDeferral _deferral = null;
        ThreadPoolTimer _periodicTimer = null;
        uint _progress = 0;
        IBackgroundTaskInstance _taskInstance = null;

        //
        // The Run method is the entry point of a background task.
        //
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            Debug.WriteLine("Background " + taskInstance.Task.Name + " Starting...");

            //
            // Query BackgroundWorkCost
            // Guidance: If BackgroundWorkCost is high, then perform only the minimum amount
            // of work in the background task and return immediately.
            //
            var cost = BackgroundWorkCost.CurrentBackgroundWorkCost;

            //
            // Associate a cancellation handler with the background task.
            //
            taskInstance.Canceled += new BackgroundTaskCanceledEventHandler(OnCanceled);

            //
            // Get the deferral object from the task instance, and take a reference to the taskInstance;
            //
            _deferral = taskInstance.GetDeferral();
            _taskInstance = taskInstance;

            // Pop a toast
            SendToast();

            _periodicTimer = ThreadPoolTimer.CreatePeriodicTimer(new TimerElapsedHandler(PeriodicTimerCallback), TimeSpan.FromSeconds(1));

        }

        private void SendToast()
        {
            string title = "Sample Toast";
            string content = "Hello here's my toast content";
            // Construct the visuals of the toast
            ToastVisual visual = new ToastVisual()
            {
                BindingGeneric = new ToastBindingGeneric()
                {
                    Children =
                    {
                        new AdaptiveText()
                        {
                            Text = title
                        },
                        new AdaptiveText()
                        {
                            Text = content
                        },
                        //new AdaptiveImage()
                        //{
                        //    Source = image2
                        //},
                    },
                    AppLogoOverride = new ToastGenericAppLogo()
                    {
                        Source = image1,
                        HintCrop = ToastGenericAppLogoCrop.Circle
                    }
                }
            };

            // Construct the actions for the toast (inputs and buttons)
            ToastActionsCustom actions = new ToastActionsCustom()
            {
                Inputs =
                {
                    new ToastSelectionBox("snoozeTime")
                    {
                        DefaultSelectionBoxItemId = "15",
                        Items =
                        {
                            new ToastSelectionBoxItem("5", "5 minutes"),
                            new ToastSelectionBoxItem("15", "15 minutes"),
                            new ToastSelectionBoxItem("60", "1 hour"),
                            new ToastSelectionBoxItem("240", "4 hours"),
                            new ToastSelectionBoxItem("1440", "1 day")
                        }
                    }
                },
                Buttons =
                {
                    //new ToastButton("Drink some water!!", new QueryString()
                    //{
                    //    { "action", "reply" }

                    //}.ToString())
                    //{
                    //    ActivationType = ToastActivationType.Background,
                    //    ImageUri = image1,
                    //},

                    //new ToastButton("Stretch!", new QueryString()
                    //{
                    //    { "action", "like" }

                    //}.ToString())
                    //{
                    //    ActivationType = ToastActivationType.Background,
                    //    ImageUri = image2,
                    //},

                    new ToastButtonSnooze()
                    {
                        SelectionBoxId = "snoozeTime"
                    },

                    new ToastButtonDismiss()
                }
            };
            // construct the final toast content
            ToastContent toastContent = new ToastContent()
            {
                Visual = visual,
                Actions = actions,

                // Arguments when the user taps body of toast
                Launch = new QueryString()
                {
                    { "action", "viewConversation" },

                }.ToString()
            };
            // And create the toast notification
            ToastNotification notification = new ToastNotification(toastContent.GetXml());
            notification.ExpirationTime = DateTime.Now.AddMinutes(5);
            // And then send the toast
            ToastNotificationManager.CreateToastNotifier().Show(notification);
        }

        //
        // Handles background task cancellation.
        //
        private void OnCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            //
            // Indicate that the background task is canceled.
            //
            _cancelRequested = true;
            _cancelReason = reason;

            Debug.WriteLine("Background " + sender.Task.Name + " Cancel Requested...");
        }

        //
        // Simulate the background task activity.
        //
        private void PeriodicTimerCallback(ThreadPoolTimer timer)
        {
            if ((_cancelRequested == false) && (_progress < 100))
            {
                _progress += 10;
                _taskInstance.Progress = _progress;
            }
            else
            {
                _periodicTimer.Cancel();

                //
                // Record that this background task ran.
                //
                String taskStatus = (_progress < 100) ? "Canceled with reason: " + _cancelReason.ToString() : "Completed";
                Debug.WriteLine("Background " + _taskInstance.Task.Name + taskStatus);

                //
                // Indicate that the background task has completed.
                //
                _deferral.Complete();
            }
        }

        public static void Start(BackgroundTaskRegistrationGroup sender, BackgroundActivatedEventArgs args)
        {
            Start(args.TaskInstance);
        }

        public static void Start(IBackgroundTaskInstance taskInstance)
        {
            // Use the taskInstance.Name and/or taskInstance.InstanceId to determine
            // what background activity to perform. In this sample, all of our
            // background activities are the same, so there is nothing to check.
            var activity = new BgTaskExample();
            activity.Run(taskInstance);
        }
    }
}
