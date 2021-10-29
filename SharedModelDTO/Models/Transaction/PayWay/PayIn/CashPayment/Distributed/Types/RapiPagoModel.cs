using System;
using System.ComponentModel.DataAnnotations;

namespace SharedModelDTO.Models.Transaction.PayWay.PayIn.CashPayment.Distributed.Types
{
    public class RapiPagoModel : BarCodeModel
    {
        [Required(ErrorMessage = "Parameter :: ReferenceVoucher :: is required.")]
        [RegularExpression("^[0-9]{12}$", ErrorMessage = "Parameter :: ReferenceVoucher :: invalid format, only allow: numbers and length: eleven digits.")]
        public override string ReferenceVoucher { get; set; }

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
            string CodeNumberWithOutCheckDigit = this.BusinessCode + this.Customer + this.ReferenceVoucher + this.FirstExpirationAmount + this.FirstExpirtationDate + this.SecondExpirationAmount + this.SecondExpirtationDate;

            string CheckDigit01 = this.CalculationOfTheVerifierDigit(CodeNumberWithOutCheckDigit);

            this.CheckDigit = CheckDigit01;

            string CodeNumber = CodeNumberWithOutCheckDigit + this.CheckDigit;

            return CodeNumber;
        }
        #endregion
    }
}
