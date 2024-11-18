using Serial_COM.Models;
using System.Windows;

namespace Serial_COM
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            EnvironmentSet envSet = new EnvironmentSet();
            envSet.IntializeLogFile(); // [로그 파일] 초기화!
        }

    }

}
