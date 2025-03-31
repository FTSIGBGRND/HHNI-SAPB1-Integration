using SAPbobsCOM;
using System;
using System.ComponentModel;
using System.ServiceProcess;

namespace FTSISAPB1iService
{
    public partial class SAPB1Service : ServiceBase
    {
        private static DateTime dteStart;
        private static bool blStartNew = true;
        public SAPB1Service()
        {
            InitializeComponent();
        }

        public void OnDebug()
        {
            OnStart(null);
        }
        protected override void OnStart(string[] args)
        {
            dteStart = DateTime.Now;

            runInitialization.RunWorkerAsync();
        }
        protected override void OnStop()
        {
            if (GlobalVariable.oCompany.Connected)
                if (GlobalVariable.oCompany.InTransaction)
                    GlobalVariable.oCompany.EndTransaction(BoWfTransOpt.wf_RollBack);

            SAPB1Service myService = new SAPB1Service();
            myService.Stop();

            Environment.Exit(0);
        }
        private void runInitialization_DoWork(object sender, DoWorkEventArgs e)
        {

            if (Initialization.onInit() == false)
            {
                if (GlobalVariable.oCompany.Connected)
                    if (GlobalVariable.oCompany.InTransaction)
                        GlobalVariable.oCompany.EndTransaction(BoWfTransOpt.wf_RollBack);

                SAPB1Service myService = new SAPB1Service();

                myService.Stop();

                Environment.Exit(0);
            }
        }
        private void runInitialization_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            runProcService.RunWorkerAsync();
        }
        private void runProcService_DoWork(object sender, DoWorkEventArgs e)
        {
            ProcServices._SAPB1Services();
        }
        private void RunProcService_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            runInitialization.RunWorkerAsync();
        }
    }
}
