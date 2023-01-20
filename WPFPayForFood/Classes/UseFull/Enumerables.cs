using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFPayForFood.Classes
{
    public enum ELogType
    {
        General = 0,
        Error = 1,
        Device = 2
    }

    public enum EMEIMEssages
    {
        MeiConnected,
        MeiBillEscrow,
        MeiBillStacked,
        MeiBillRejected,
        MeiCashBoxRemoved,
        MeiCashBoxAttached,
        MeiJamDetected,
        MeiJamResolved,
        MeiCheated,
        MeiDisconnected,
        MeiPauseDetected,
        MeiPauseResolved,
        MeiPowerUp,
        MeiPowerUpComplete,
        MeiBillInScrowOnPowerUp,
        MeiStackerFull,
        MeiStackerFullResolved,
        MeiStallDetected,
        MeiStallResolved
    }

    public enum EModalType
    {
        Cancell = 0,
        NotExistAccount = 1,
        Error = 2,
        MaxAmount = 3,
        Information = 4,
        Preload = 5,
        NoPaper = 6
    }

    public enum EError
    {
        Printer = 1,
        Nopapper = 2,
        Device = 3,
        Aplication = 5,
        Api = 6,
        Customer = 7,
        Internet = 8
    }

    public enum ELevelError
    {
        Mild = 3,
        Medium = 2,
        Strong = 1,
    }

    public enum UserControlView
    {
        Login,
        Config,
        Admin,
        Main,
        Menu,
        Consult,
        Detail,
        PaySuccess,
        Pay,
        ReturnMony,
        Products,
        ProductDetail,
        UserPoints
    }

    public enum ETransactionState
    {
        Initial = 1,
        Success = 2,
        CancelError = 6,
        Cancel = 3,
        Error = 5,
        ErrorService = 4
    }

    public enum ETransactionType
    {
        Withdrawal = 15,
        Payment = 3,
        ConsultName = 1,
        ConsultTransact = 2,
    }

    public enum ETypeAdministrator
    {
        Balancing = 1,
        Upload = 2,
        Finished = 3,
        ReUploat = 4
    }
}
