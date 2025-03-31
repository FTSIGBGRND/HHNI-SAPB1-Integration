using System;

namespace FTSISAPB1iService
{
    class Initialization
    {
        public static bool onInit()
        {
            try
            {
                if (SystemInitialization.initFolders())
                    SystemFunction.filewrite();
                else
                    return false;

                return true;
            }
            catch (Exception ex)
            {
                SystemFunction.errorAppend(ex.Message.ToString());
                return false;
            }
        }
        public static bool connectSBO(string strConSettings)
        {

            SystemFunction.reconnectSAP();

            if (SystemFunction.connectSAP(strConSettings))
            {
                SystemInitialization.initFolders();

                if (!(SystemInitialization.initTables()))
                {
                    SystemFunction.errorAppend(string.Format("Error Creating User Define Tables using {0} Connection Settings.", strConSettings));
                    return false;
                }

                if (!(SystemInitialization.initFields()))
                {
                    SystemFunction.errorAppend(string.Format("Error Creating User Define Fields using {0} Connection Settings.", strConSettings));
                    return false;
                }

                if (!(SystemInitialization.initUDO()))
                {
                    SystemFunction.errorAppend(string.Format("Error Creating User Define Objects using {0} Connection Settings.", strConSettings));
                    return false;
                }

                if (!(SystemInitialization.initStoreProcedure()))
                {
                    SystemFunction.errorAppend(string.Format("Error Executing SQL Scripts using {0} Connection Settings.", strConSettings));
                    return false;
                }

            }
            else
            {
                return false;
            }

            return true;
        }
    }
}
