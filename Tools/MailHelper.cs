using SharedModel.Models.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools
{
    public class MailHelper
    {
        public static string CreateTransactionLotMailBody(TransactionModel.TransactionLot.Response transactionLot) 
        {
            //CREATE EMAIL START
            string messageBody = "<font>The following transactions were made:</font><br><br>";
            string htmlTableStart = "<table align ='center'  style=\"border-collapse:collapse; text-align:center;\"";
            string htmlTableHead = "<thead background-color: #dee2e6;><tr style =\"background-color:#dee2e6;\"><th>Merchant</th><th>User</th><th>Country</th><th>Total Transactions</th><th>Total Amount</th></tr></thead><tbody>";
            string htmlTableEnd = "</tr></tbody></table>";
            string htmlTrStart = "<tr align ='center' style =\"color:#555555;\">";
            string htmlTdStart = "<td style=\" border-color:#dee2e6; border-style:solid; border-width:thin; padding: 5px;\">";
            string htmlTdEnd = "</td>";

            messageBody += htmlTableStart;
            messageBody += htmlTableHead;
            messageBody += htmlTrStart;
            messageBody += htmlTdStart + transactionLot.Merchant + htmlTdEnd;
            messageBody += htmlTdStart + transactionLot.User + htmlTdEnd;
            messageBody += htmlTdStart + transactionLot.Country + htmlTdEnd;
            messageBody += htmlTdStart + transactionLot.TotalTransactions + htmlTdEnd;
            messageBody += htmlTdStart + String.Format("{0} {1}", transactionLot.CurrencyType, Math.Round(transactionLot.TotalAmount, 2)) + htmlTdEnd;
            messageBody += htmlTableEnd;

            return messageBody;
        }
    }
}
