using System;
using System.ComponentModel.DataAnnotations;

namespace SharedModelDTO.Models.Transaction.PayWay.PayIn.CashPayment.Distributed.Types
{
    public class PagoFacilModel : BarCodeModel
    {
        [Required(ErrorMessage = "Parameter :: Customer :: is required.")]
        [RegularExpression("^[0-9]{10}$", ErrorMessage = "Parameter :: Customer :: invalid format, only allow: numbers and length: ten digits.")]
        public override string Customer { get; set; }
        public string Currency { get { return "0"; /* DE MOMENTO ES UN UNICO DIGITO, 0 QUE ES EN PESOS. */ } }

        #region Methods::Public
        public override void Create()
        {
            base.Create();
        }
        public override void Update()
        {
            base.Update();
        }
        public override void Delete(Int64 id)
        {
            base.Delete(id);
        }
        public override void List() { }
        #endregion

        #region Methods::Barcode
        public override string CodeBarFormatNumber()
        {
            string CodeNumberWithOutCheckDigit = this.BusinessCode + this.FirstExpirationAmount + this.FirstExpirtationDate + this.Customer + this.Currency + this.SecondExpirationAmount + this.SecondExpirtationDate;
            string CheckDigit01 = this.CalculationOfTheVerifierDigit(CodeNumberWithOutCheckDigit);
            string CheckDigit02 = this.CalculationOfTheVerifierDigit(CodeNumberWithOutCheckDigit + CheckDigit01);

            this.CheckDigit = CheckDigit01 + CheckDigit02;

            string CodeNumber = CodeNumberWithOutCheckDigit + this.CheckDigit;

            return CodeNumber;
        }
        #endregion
    }
}
