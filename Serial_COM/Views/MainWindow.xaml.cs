using Serial_COM.ViewModels;
using System.Windows;

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

    }

}
