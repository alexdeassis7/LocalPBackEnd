using System;
using System.ComponentModel.DataAnnotations;

namespace SharedModelDTO.Models.Transaction.PayWay.PayIn.CashPayment.Distributed.Types
{
    public class BaproModel : BarCodeModel
    {
        [Required(ErrorMessage = "Parameter :: FirstExpirtationDate :: is required.")]
        [RegularExpression("^[0-9]{5}$", ErrorMessage = "Parameter :: FirstExpirtationDate :: invalid format, only allow: numbers and length: five digits.")]
        public override string FirstExpirtationDate { get; set; }
        [Required(ErrorMessage = "Parameter :: SecondExpirationAmount :: is required.")]
        [RegularExpression("^[0-9]{6}$", ErrorMessage = "Parameter :: SecondExpirationAmount :: invalid format, only allow: numbers and length: six digits.")]
        public override string SecondExpirationAmount { get; set; }
        [Required(ErrorMessage = "Parameter :: SecondExpirtationDate :: is required.")]
        [RegularExpression("^[0-9]{2}$", ErrorMessage = "Parameter :: SecondExpirtationDate :: invalid format, only allow: numbers and length: two digits.")]
        public override string SecondExpirtationDate { get; set; }

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
            string CodeNumberWithOutCheckDigit = this.BusinessCode + this.ReferenceVoucher + this.FirstExpirtationDate + this.FirstExpirationAmount + this.SecondExpirtationDate + this.SecondExpirationAmount;
            string CheckDigit01 = this.CalculationOfTheVerifierDigit(CodeNumberWithOutCheckDigit);

            this.CheckDigit = CheckDigit01;

            string CodeNumber = CodeNumberWithOutCheckDigit + this.CheckDigit;

            return CodeNumber;
        }
        #endregion
    }
}
