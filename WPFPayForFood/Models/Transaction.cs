using System;
using System.Collections.Generic;
using System.ComponentModel;
using WPFPayForFood.Classes;
using WPFPayForFood.DataModel;
using WPFPayForFood.Services.ObjectIntegration;
using WPFPayForFood.ViewModel;

namespace WPFPayForFood.Models
{
    public class Transaction : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyRaised(string propertyname)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));
        }

        public string Document { get; set; }
        public Comidas lstComidas { get; set; }
        public Datum ComidaSelect { get; set; }
        public List<Datum> productos { get; set; }

        public string reference { get; set; }

        public ResponseRestaurante LstRestaurantes { get; set; }

        public string Enrollment { get; set; }

        public DateTime DateTransaction { get; set; }

        public PaymentViewModel Payment { get; set; }

        public string StatePay { get; set; }

        public bool IsReturn { get; set; }

        public ETransactionState State { get; set; }

        public string Observation { get; set; }

        public string UserPoints { get; set; }

        public string PayerDocument { get; set; }

        public ETransactionType Type { get; set; }

        public bool StateReturnMoney { get; set; }

        public bool statePaySuccess { get; set; }

        public PAYER payer { get; set; }

        public int StateNotification { get; set; }

        public decimal AmountToReturn { get; set; }



        public object File { get; set; }

        private decimal _Amount;

        public decimal Amount
        {
            get
            {
                return _Amount;
            }
            set
            {
                _Amount = value;
                OnPropertyRaised("Amount");
            }
        }

        private int _transactionId { get; set; }

        public int TransactionId
        {
            get
            {
                return _transactionId;
            }
            set
            {
                _transactionId = value;
                OnPropertyRaised("TransactionId");
            }
        }

        private int _consecutivoId { get; set; }

        public int ConsecutivoId
        {
            get
            {
                return _consecutivoId;
            }
            set
            {
                _consecutivoId = value;
                OnPropertyRaised("ConsecutivoId");
            }
        }

        private int _idTransactionAPi { get; set; }

        public int IdTransactionAPi
        {
            get
            {
                return _idTransactionAPi;
            }
            set
            {
                _idTransactionAPi = value;
                OnPropertyRaised("IdTransactionAPi");
            }
        }
    }
}
