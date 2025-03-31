using System.ComponentModel;
using System.Configuration.Install;

namespace FTSISAPB1iService
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }
        private void serviceInstaller1_AfterInstall(object sender, InstallEventArgs e)
        {
            //new ServiceController(serviceInstaller1.ServiceName).Start();
        }

        private void serviceProcessInstaller1_AfterInstall(object sender, InstallEventArgs e)
        {

        }
    }
}
