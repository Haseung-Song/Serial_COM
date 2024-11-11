using Serial_COM.ViewModels;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace Serial_COM
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainVM vm = new MainVM();
        public MainWindow()
        {
            InitializeComponent();
            DataContext = vm;
        }

        /// <summary>
        /// [ToggleButton_Checked]
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton toggleButton)
            {
                switch (toggleButton.Name)
                {
                    case "AltitudeToggle":
                        vm.IsAltitudeOn = true;
                        break;
                    case "HeadingToggle":
                        vm.IsHeadingOn = true;
                        break;
                    case "SpeedToggle":
                        vm.IsSpeedOn = true;
                        break;

                    default:
                        break;
                }

            }

        }

        /// <summary>
        /// [ToggleButton_Unchecked]
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton toggleButton)
            {
                switch (toggleButton.Name)
                {
                    case "AltitudeToggle":
                        vm.IsAltitudeOn = false;
                        break;
                    case "HeadingToggle":
                        vm.IsHeadingOn = false;
                        break;
                    case "SpeedToggle":
                        vm.IsSpeedOn = false;
                        break;

                    default:
                        break;
                }

            }

        }

    }

}
