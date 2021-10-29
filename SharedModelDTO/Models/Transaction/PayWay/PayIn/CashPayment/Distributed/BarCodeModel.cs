using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SharedModelDTO.Models.Transaction.PayWay.PayIn.CashPayment.Distributed
{
    public class BarCodeModel : CashPaymentModel
    {
        [Required(ErrorMessage = "Parameter :: BusinessCode :: is required.")]
        [RegularExpression("^[0-9]{4}$", ErrorMessage = "Parameter :: BusinessCode :: invalid format, only allow: numbers and length: four digits.")]
        public string BusinessCode { get; set; }
        [Required(ErrorMessage = "Parameter :: Customer :: is required.")]
        [RegularExpression("^[0-9]{10}$", ErrorMessage = "Parameter :: Customer :: invalid format, only allow: numbers and length: ten digits.")]
        public virtual string Customer { get; set; }
        [Required(ErrorMessage = "Parameter :: ReferenceVoucher :: is required.")]
        [RegularExpression("^[0-9]{11}$", ErrorMessage = "Parameter :: ReferenceVoucher :: invalid format, only allow: numbers and length: eleven digits.")]
        public virtual string ReferenceVoucher { get; set; }
        [Required(ErrorMessage = "Parameter :: FirstExpirationAmount :: is required.")]
        [RegularExpression("^[0-9]{8}$", ErrorMessage = "Parameter :: FirstExpirationAmount :: invalid format, only allow: numbers and length: eight digits.")]
        public string FirstExpirationAmount { get; set; }
        [Required(ErrorMessage = "Parameter :: FirstExpirtationDate :: is required.")]
        [RegularExpression("^[0-9]{5}$", ErrorMessage = "Parameter :: FirstExpirtationDate :: invalid format, only allow: numbers and length: five digits.")]
        public virtual string FirstExpirtationDate { get; set; }
        [Required(ErrorMessage = "Parameter :: SecondExpirationAmount :: is required.")]
        [RegularExpression("^[0-9]{6}$", ErrorMessage = "Parameter :: SecondExpirationAmount :: invalid format, only allow: numbers and length: six digits.")]
        public virtual string SecondExpirationAmount { get; set; }
        [Required(ErrorMessage = "Parameter :: SecondExpirtationDate :: is required.")]
        [RegularExpression("^[0-9]{2}$", ErrorMessage = "Parameter :: SecondExpirtationDate :: invalid format, only allow: numbers and length: two digits.")]
        public virtual string SecondExpirtationDate { get; set; }
        public string CheckDigit { get; set; }
        [Required(ErrorMessage = "Parameter :: client_identification :: is required.")]
        [RegularExpression("^(\\b(20|23|24|27|30|33|34)(\\D)?[0-9]{8}(\\D)?[0-9])$", ErrorMessage = "Parameter :: client_identification :: invalid value.")]
        public string ClientIdentification { get; set; }

        public virtual bool IsValidLength(string data, int length)
        {
            if (string.IsNullOrEmpty(data))
            {
                return false;
            }
            else
            {
                if (data.Length == length)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public virtual string CodeBarFormatNumber()
        {
            return string.Empty;
        }
        public virtual string CalculationOfTheVerifierDigit(string data)
        {
            string Result = "";
            string Sequence;
            string SequenceInit = "13579";
            string[] SequenceNTuple = new string[] { "3", "5", "7", "9" };

            if (!string.IsNullOrEmpty(data))
            {
                Sequence = SequenceInit;
                for (int i = 0; i < data.Length - 5; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if (Sequence.Length == data.Length)
                            break;
                        Sequence += SequenceNTuple[j];
                    }
                }

                char[] vData = data.ToCharArray();
                char[] vSequence = Sequence.ToCharArray();

                int[] vProduct;

                if (vData.Length == vSequence.Length)
                {
                    vProduct = new int[data.Length];
                    for (int a = 0; a < vData.Length; a++)
                    {
                        vProduct[a] = int.Parse(vData[a].ToString()) * int.Parse(vSequence[a].ToString());
                    }

                    int ProductSum = vProduct.Sum();

                    string ProductSumDivided = (ProductSum / 2).ToString();

                    Result = ProductSumDivided.Substring(ProductSumDivided.Length - 1, 1);
                }
            }
            return Result;
        }
    }
}
