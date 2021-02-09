using System.Windows;
using Windows.ApplicationModel.Background;

namespace Net5App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            string exampleTaskName = "BgTask";

            var allTasks = BackgroundTaskRegistration.AllTasks;

            bool taskRegistered = false;
            foreach (var task in allTasks)
            {
                if (task.Value.Name == exampleTaskName)
                {
                    taskRegistered = true;
                    break;
                }
            }

            if (!taskRegistered)
            {
                var builder = new BackgroundTaskBuilder();
                builder.Name = exampleTaskName;
                builder.TaskEntryPoint = "Net5BgTaskComponent.BgTask";
                builder.SetTrigger(new SystemTrigger(SystemTriggerType.TimeZoneChange, false));
                builder.Register();
            }
        }
    }
}
