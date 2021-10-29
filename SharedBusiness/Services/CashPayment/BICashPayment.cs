using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilityTools = Tools;

namespace SharedBusiness.Services.CashPayment
{
    public class BICashPayment
    {
        public string ListStatusCashPayments(string customer_id)
        {
            string Result = string.Empty;
            try
            {
                DAO.DataAccess.Services.DbCashPayment DbCashPayment = new DAO.DataAccess.Services.DbCashPayment();
                Result = DbCashPayment.ListStatusCashPayments(customer_id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Result;
        }

        public SharedModel.Models.Services.CashPayments.CashPaymentModel.Create.Response BarCodeGenerator(string customer_id, SharedModel.Models.Services.CashPayments.CashPaymentModel.Create.Response data, bool TransactionMechanism, string customer_from, string countryCode)
        {
            SharedModel.Models.Services.CashPayments.CashPaymentModel.Create.Response Result = new SharedModel.Models.Services.CashPayments.CashPaymentModel.Create.Response();
            string BarCodeNumber = string.Empty;
            try
            {
                DAO.DataAccess.Services.DbCashPayment.DbBarcode DbBarCode = new DAO.DataAccess.Services.DbCashPayment.DbBarcode();
                SharedModelDTO.Models.Transaction.PayWay.PayIn.CashPayment.Distributed.Types.Common Common = new SharedModelDTO.Models.Transaction.PayWay.PayIn.CashPayment.Distributed.Types.Common();

                data.first_expiration_amount = (data.first_expiration_amount).Replace(' ', '0').PadLeft(8,'0');
                /* GENERAR BARCODETICKET */
                Common.Ticket = DbBarCode.BarCodeTicketGenerator(data.payment_method, data.invoice, data.additional_info);

                int Year = Convert.ToInt32(data.first_expirtation_date.Substring(0, 4));
                int Month = Convert.ToInt32(data.first_expirtation_date.Substring(4, 2));
                int Day = Convert.ToInt32(data.first_expirtation_date.Substring(6, 2));

                DateTime FirstExpirationDate = new DateTime(Year, Month, Day);
                DateTime SecondExpirationDate = FirstExpirationDate.AddDays(Convert.ToDouble(data.days_to_second_exp_date));

                string FirstDayOfYear = "000" + FirstExpirationDate.DayOfYear.ToString();
                string SecondDayOfYear = "000" + SecondExpirationDate.DayOfYear.ToString();

                string JulianFirstExpirationDate = FirstExpirationDate.Year.ToString().Substring(2, 2) + FirstDayOfYear.Substring(FirstDayOfYear.Length - 3, 3);
                string JulianSecondExpirtationDate = SecondExpirationDate.Year.ToString().Substring(2, 2) + SecondDayOfYear.Substring(SecondDayOfYear.Length - 3, 3);

                string days_to_second_expiration_date = "0" + data.days_to_second_exp_date;                

                switch (data.payment_method)
                {
                    case "RAPA":                        

                        SharedModelDTO.Models.Transaction.PayWay.PayIn.CashPayment.Distributed.Types.RapiPagoModel RapiPago = new SharedModelDTO.Models.Transaction.PayWay.PayIn.CashPayment.Distributed.Types.RapiPagoModel();

                        RapiPago.BusinessCode = "6103";
                        RapiPago.Customer = customer_id.Substring(customer_id.Length - 10);
                        RapiPago.ReferenceVoucher = "0" + Common.Ticket; //data.invoice;
                        RapiPago.FirstExpirationAmount = data.first_expiration_amount;
                        RapiPago.FirstExpirtationDate = JulianFirstExpirationDate;
                        RapiPago.SecondExpirationAmount = data.surcharge.Substring(data.surcharge.Length - 6);
                        RapiPago.SecondExpirtationDate = days_to_second_expiration_date;
                        RapiPago.ClientIdentification = data.identification;

                        BarCodeNumber = RapiPago.CodeBarFormatNumber();

                        if (!RapiPago.IsValidLength(BarCodeNumber, 48))
                            throw new Exception("The barcode number is invalid length.");

                        break;
                    case "PAFA":

                        SharedModelDTO.Models.Transaction.PayWay.PayIn.CashPayment.Distributed.Types.PagoFacilModel PagoFacil = new SharedModelDTO.Models.Transaction.PayWay.PayIn.CashPayment.Distributed.Types.PagoFacilModel();

                        string PagoFacilBusinessCode = "90062500";

                        PagoFacil.BusinessCode = PagoFacilBusinessCode.Substring(PagoFacilBusinessCode.Length - 4);
                        PagoFacil.FirstExpirationAmount = data.first_expiration_amount;
                        PagoFacil.FirstExpirtationDate = JulianFirstExpirationDate;
                        PagoFacil.Customer = "000" + Common.Ticket; //"0000" + customer_id.Substring(customer_id.Length - 10);
                        PagoFacil.SecondExpirationAmount = data.surcharge.Substring(data.surcharge.Length - 6);
                        PagoFacil.SecondExpirtationDate = days_to_second_expiration_date;
                        PagoFacil.ClientIdentification = data.identification;

                        BarCodeNumber = PagoFacil.CodeBarFormatNumber();

                        if (!PagoFacil.IsValidLength(BarCodeNumber, 42))
                            throw new Exception("The barcode number is invalid length.");

                        break;
                    case "BAPR":

                        SharedModelDTO.Models.Transaction.PayWay.PayIn.CashPayment.Distributed.Types.BaproModel Bapro = new SharedModelDTO.Models.Transaction.PayWay.PayIn.CashPayment.Distributed.Types.BaproModel();

                        Bapro.BusinessCode = "0133";
                        Bapro.ReferenceVoucher = Common.Ticket; //data.invoice.Substring(data.invoice.Length - 11);
                        Bapro.FirstExpirtationDate = FirstExpirationDate.ToString("yyMMdd");
                        Bapro.FirstExpirationAmount = data.first_expiration_amount;
                        Bapro.SecondExpirtationDate = SecondExpirationDate.ToString("yyMMdd");
                        Bapro.SecondExpirationAmount = data.surcharge;
                        Bapro.ClientIdentification = data.identification;

                        BarCodeNumber = Bapro.CodeBarFormatNumber();

                        if (!Bapro.IsValidLength(BarCodeNumber, 44))
                            throw new Exception("The barcode number is invalid length.");

                        break;
                    case "COEX":

                        SharedModelDTO.Models.Transaction.PayWay.PayIn.CashPayment.Distributed.Types.CobroExpressModel CobroExpress = new SharedModelDTO.Models.Transaction.PayWay.PayIn.CashPayment.Distributed.Types.CobroExpressModel();

                        CobroExpress.BusinessCode = "0722";
                        CobroExpress.Customer = customer_id.Substring(customer_id.Length - 10);
                        CobroExpress.ReferenceVoucher = "0" + Common.Ticket; //data.invoice;
                        CobroExpress.FirstExpirationAmount = data.first_expiration_amount;
                        CobroExpress.FirstExpirtationDate = JulianFirstExpirationDate;
                        CobroExpress.SecondExpirationAmount = data.surcharge.Substring(data.surcharge.Length - 6);
                        CobroExpress.SecondExpirtationDate = days_to_second_expiration_date;
                        CobroExpress.ClientIdentification = data.identification;

                        BarCodeNumber = CobroExpress.CodeBarFormatNumber();

                        if (!CobroExpress.IsValidLength(BarCodeNumber, 48))
                            throw new Exception("The barcode number is invalid length.");

                        break;
                    default:
                        throw new Exception("Parameter :: payment_method :: invalid value: [RAPA: RapiPago | PAFA: PagoFacil | BAPR: Bapro | COEX: CobroExpress].");
                }

                Common.BarCode = BarCodeNumber;
                Common.Currency = data.currency;
                Common.CustomerID = customer_id;
                Common.ExpirtationDate = FirstExpirationDate.ToString("yyyyMMdd");
                Common.TransactionType = data.payment_method;
                Common.Value = data.first_expiration_amount;
                Common.AdditionalInfo = data.additional_info;
                Common.Invoice = data.invoice;
                Common.ClientIdentification = data.identification;
                Common.ClientName = data.name;
                Common.ClientMail = data.mail;
                Common.CustomerFrom = customer_from;

                //Common.ClientFirstName = data.first_name;
                //Common.ClientLastName = data.last_name;
                Common.ClientAddress = data.address;
                Common.ClientBirthDate = data.birth_date;
                Common.ClientCountry = data.country;
                Common.ClientCity = data.city;
                Common.ClientAnnotation = data.annotation;

                DbBarCode.CreateLotTransaction(Common, TransactionMechanism, countryCode);
                Result.bar_code= UtilityTools.BarCode.BarCode.BarCodeGenerator(BarCodeNumber);
                Result.transaction_id = Common.Ticket;
                Result.bar_code_number = BarCodeNumber;
                //  Result = Tools.BarCode.BarCode.BarCodeGenerator(BarCodeNumber);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return Result;
        }
        public bool ValidFile(string filename)
        {
            bool isValid = false;

            try
            {
                DAO.DataAccess.Services.DbFile DbFile = new DAO.DataAccess.Services.DbFile();
                isValid = DbFile.ValidFile(filename);
            }
            catch (Exception ex)
            {
                isValid = false;
            }

            return isValid;
        }
        public SharedModel.Models.Services.CashPayments.CashPaymentModel.ReadFile ReadFileRAPA(string file, string transaction_type, string datetime, bool TransactionMechanism, string FileName)
        {
            SharedModel.Models.Services.CashPayments.CashPaymentModel.ReadFile ReadFile = new SharedModel.Models.Services.CashPayments.CashPaymentModel.ReadFile();
            List<SharedModel.Models.Services.CashPayments.CashPaymentModel.TransactionResult> TrProcessDetail = new List<SharedModel.Models.Services.CashPayments.CashPaymentModel.TransactionResult>();
            List<SharedModel.Models.Services.CashPayments.CashPaymentModel.TransactionResult> TrDismissDetail = new List<SharedModel.Models.Services.CashPayments.CashPaymentModel.TransactionResult>();
            SharedModel.Models.Services.CashPayments.CashPaymentModel.TransactionResult ObjTrProcessDetail;
            SharedModel.Models.Services.CashPayments.CashPaymentModel.TransactionResult ObjTrDismissDetail;

            /* TOTALIZADORES A 0 */
            Int32 QtyTransactionRead = 0;
            Int32 QtyTransactionProcess = 0;
            Int32 QtyTransactionDismiss = 0;

            /* SE SETEA LA POSICION Y LA LONGITUD DEL ACCOUNTNUMBER(CLIENTE EN LA GENERACION DEL BARCODE, QUE ES EL TICKET MAS TRES CEROS) */
            int HeaderLine = 0;
            int HeaderLineLength = 8;

            int AmountPosIni = 8;
            int AmountLength = 15;

            int BarCodePosIni = 23;
            int BarCodeLength = 48;

            int TicketPosIni = 15;
            int TicketLength = 11;

            bool isValid = false;

            try
            {
                /* BASE64 A BYTES[] */
                byte[] bytes = Convert.FromBase64String(file);
                //byte[] bytes = File.ReadAllBytes(@"C:\Users\FEDE\Downloads\4186G0764\RP060219_4186.txt"); /* PROBAR EN LOCAL */

                using (var stream = new StreamReader(new MemoryStream(bytes)))
                {
                    /* SE DECLARA UN STRING PARA OBTENER CADA LINEA DEL ARCHIVO */
                    string line;

                    /* WHILE MIENTRAS QUE NO SE TERMINE EL ARCHIVO */
                    while (!stream.EndOfStream)
                    {
                        ObjTrProcessDetail = new SharedModel.Models.Services.CashPayments.CashPaymentModel.TransactionResult();
                        ObjTrDismissDetail = new SharedModel.Models.Services.CashPayments.CashPaymentModel.TransactionResult();

                        /* VA LEYENDO LINEA POR LINEA Y SETEANDOLA A LA VARIABLE LINE */
                        line = stream.ReadLine();

                        if (line.Substring(HeaderLine, HeaderLineLength) != "00000000" && line.Substring(HeaderLine, HeaderLineLength) != "99999999")
                        {
                            QtyTransactionRead += 1;
                            
                            /* SE OBTIENE EL MONTO ABONADO (TOTAL) */
                            string Amount = line.Substring(AmountPosIni, AmountLength);

                            string BarCodeNumber = line.Substring(BarCodePosIni, BarCodeLength);

                            /* SE OBTIENE EL TICKET GENERADO POR LP PARA TRACEAR LA OPERACION */
                            string Ticket = BarCodeNumber.Substring(TicketPosIni, TicketLength);

                            /* SE INSTANCIA AL DAO */
                            DAO.DataAccess.Services.DbCashPayment DbCashPayment = new DAO.DataAccess.Services.DbCashPayment();

                            /* SE INVOCA LA VALIDACION DE TICKET */
                            isValid = DbCashPayment.ValidateTicket(Ticket, TransactionMechanism);

                            if (isValid == true)
                            {
                                /* SI ES VALIDO, SE INCREMENTA EL TOTALIZADOR DE TRANSACCIONES PROCESADAS */
                                QtyTransactionProcess += 1;

                                ObjTrProcessDetail.Ticket = Ticket;
                                ObjTrProcessDetail.Amount = Convert.ToDouble(Amount) / 100;
                                ObjTrProcessDetail.BarCodeNumber = BarCodeNumber;
                                TrProcessDetail.Add(ObjTrProcessDetail);
                            }
                            else
                            {
                                /* SINO ES VALIDO, SE INCREMENTA EL TOTALIZADOR DE TRANSACCIONES PERDIDAS */
                                QtyTransactionDismiss += 1;

                                ObjTrDismissDetail.Ticket = Ticket;
                                ObjTrDismissDetail.Amount = Convert.ToDouble(Amount) / 100;
                                ObjTrDismissDetail.BarCodeNumber = BarCodeNumber;
                                TrDismissDetail.Add(ObjTrDismissDetail);
                            }

                        }
                    }
                }

                SharedModelDTO.File.FileModel FileDTO = new SharedModelDTO.File.FileModel();
                FileDTO.file_bytes = bytes;
                FileDTO.transaction_type = transaction_type;
                FileDTO.datetime_process = datetime;

                FileDTO = UtilityTools.File.Zip.FileCompression.ZipFileToBytes(FileDTO);

                DAO.DataAccess.Services.DbFile DbFile = new DAO.DataAccess.Services.DbFile();
                DbFile.SaveFile(FileDTO, string.Empty, FileName, "ARG", null);

                ReadFile.Status = "OK";
                ReadFile.StatusMessage = "OK";
                ReadFile.QtyTransactionRead = QtyTransactionRead;
                ReadFile.QtyTransactionProcess = QtyTransactionProcess;
                ReadFile.QtyTransactionDismiss = QtyTransactionDismiss;
                ReadFile.TrProcessDetail = TrProcessDetail;
                ReadFile.TrDismissDetail = TrDismissDetail;
            }
            catch (Exception ex)
            {
                ReadFile.Status = "ERROR";
                ReadFile.StatusMessage = ex.Message;
                ReadFile.QtyTransactionRead = QtyTransactionRead;
                ReadFile.QtyTransactionProcess = QtyTransactionProcess;
                ReadFile.QtyTransactionDismiss = QtyTransactionDismiss;
                ReadFile.TrProcessDetail = null;
                ReadFile.TrDismissDetail = null;
            }
            return ReadFile;
        }

        public SharedModel.Models.Services.CashPayments.CashPaymentModel.ReadFile ReadFilePAFA(string file, string transaction_type, string datetime, bool TransactionMechanism, string FileName)
        {
            SharedModel.Models.Services.CashPayments.CashPaymentModel.ReadFile ReadFile = new SharedModel.Models.Services.CashPayments.CashPaymentModel.ReadFile();
            List<SharedModel.Models.Services.CashPayments.CashPaymentModel.TransactionResult> TrProcessDetail = new List<SharedModel.Models.Services.CashPayments.CashPaymentModel.TransactionResult>();
            List<SharedModel.Models.Services.CashPayments.CashPaymentModel.TransactionResult> TrDismissDetail = new List<SharedModel.Models.Services.CashPayments.CashPaymentModel.TransactionResult>();
            SharedModel.Models.Services.CashPayments.CashPaymentModel.TransactionResult ObjTrProcessDetail;
            SharedModel.Models.Services.CashPayments.CashPaymentModel.TransactionResult ObjTrDismissDetail;

            /* TOTALIZADORES A 0 */
            Int32 QtyTransactionRead = 0;
            Int32 QtyTransactionProcess = 0;
            Int32 QtyTransactionDismiss = 0;

            try
            {
                /* BASE64 A BYTES[] */
                byte[] bytes = Convert.FromBase64String(file);
                //byte[] bytes = File.ReadAllBytes(@"C:\Users\FEDE\Downloads\PF150119_900_TI.txt"); /* PROBAR EN LOCAL */

                /* SE SETEA LA POSICION Y LA LONGITUD DEL ACCOUNTNUMBER(CLIENTE EN LA GENERACION DEL BARCODE, QUE ES EL TICKET MAS TRES CEROS) */
                int TicketPosIni = 24;
                int TicketLength = 14;

                int AmountPosIni = 48;
                int AmountLength = 10;

                int BarCodePosIni = 1;
                int BarCodeLength = 42;                

                bool isValid = false;

                bool read05 = false;
                bool read06 = false;
                bool read07 = false;

                /* SE CARGAN LOS BYTES PARA EMPEZAR A LEER EL DOCUMENTO */
                using (var stream = new StreamReader(new MemoryStream(bytes)))
                {
                    /* SE DECLARA UN STRING PARA OBTENER CADA LINEA DEL ARCHIVO */
                    string line;

                    /* WHILE MIENTRAS QUE NO SE TERMINE EL ARCHIVO */
                    while (!stream.EndOfStream)
                    {
                        ObjTrProcessDetail = new SharedModel.Models.Services.CashPayments.CashPaymentModel.TransactionResult();
                        ObjTrDismissDetail = new SharedModel.Models.Services.CashPayments.CashPaymentModel.TransactionResult();

                        /* VA LEYENDO LINEA POR LINEA Y SETEANDOLA A LA VARIABLE LINE */
                        line = stream.ReadLine();

                        /* 
                         * OBTIENE EL PRIMER CARACTER DE LA LINEA:
                         * 1.- REGISTRO CABECERA DE ARCHIVO
                         * 3.- REGISTRO CABECERA DEL LOTE (Solo uno por Lote)
                         * 5.- REGISTRO DETALLE (Información detallada de cada Transacción)
                         * 6.- REGISTRO DETALLE (Código de Barras de la Transacción)
                         * 7.- REGISTRO DETALLE (Detalle de los instrumentos de pago)
                         * 8.- REGISTRO COLA DEL LOTE (Información total del Lote, solo uno por Lote)
                         * 9.- REGISTRO COLA DEL ARCHIVO (Información total del Archivo, solo uno por Archivo)
                         */
                        
                        if (Convert.ToInt32(line.Substring(0, 1)) == 5) /* ==> Entonces es una Transacción */
                        {
                            /* SETEA EL TOTALIZADOR DE TRANSACCION LEIDA */
                            QtyTransactionRead += 1;

                            /* SE OBTIENE EL NRO. DE CUENTA */
                            string AccountNumber = line.Substring(TicketPosIni, TicketLength);

                            /* SE VALIDA POR LAS DUDAS */
                            if (!string.IsNullOrEmpty(AccountNumber))
                            {
                                /* SE OBTIENE EL TICKET GENERADO POR LP PARA TRACEAR LA OPERACION */
                                string Ticket = AccountNumber.Substring(3);

                                /* SE OBTIENE EL MONTO ABONADO (TOTAL) */
                                string Amount = line.Substring(AmountPosIni, AmountLength);

                                /* SE INSTANCIA AL DAO */
                                DAO.DataAccess.Services.DbCashPayment DbCashPayment = new DAO.DataAccess.Services.DbCashPayment();

                                /* SE INVOCA LA VALIDACION DE TICKET */
                                isValid = DbCashPayment.ValidateTicket(Ticket, TransactionMechanism);

                                if (isValid == true)
                                {
                                    /* SI ES VALIDO, SE INCREMENTA EL TOTALIZADOR DE TRANSACCIONES PROCESADAS */
                                    QtyTransactionProcess += 1;

                                    ObjTrProcessDetail.Ticket = Ticket;
                                    ObjTrProcessDetail.Amount = Convert.ToDouble(Amount) / 100;
                                }
                                else
                                {
                                    /* SINO ES VALIDO, SE INCREMENTA EL TOTALIZADOR DE TRANSACCIONES PERDIDAS */
                                    QtyTransactionDismiss += 1;

                                    ObjTrDismissDetail.Ticket = Ticket;
                                    ObjTrDismissDetail.Amount = Convert.ToDouble(Amount) / 100;
                                }                                    
                            }
                            else
                            {
                                /* AL NO PODER VALIDAR SE PIERDE */
                                QtyTransactionDismiss += 1;
                            }

                            read05 = true;
                        }
                        if (Convert.ToInt32(line.Substring(0, 1)) == 6) /* ==> Entonces es un Detalle de la Transacción: BarCode */
                        {
                            /* OBTIENE EL NUMERO DE CODIGO DE BARRA */
                            string BarCodeNumber = line.Substring(BarCodePosIni, BarCodeLength);

                            if (isValid == true)
                                ObjTrProcessDetail.BarCodeNumber = BarCodeNumber;
                            else
                                ObjTrDismissDetail.BarCodeNumber = BarCodeNumber;

                            read06 = true;
                        }
                        if (Convert.ToInt32(line.Substring(0, 1)) == 7)
                        {
                            read07 = true;
                        }


                        if(read05 == true && read06 == true && read07 == true)
                        {
                            TrProcessDetail.Add(ObjTrProcessDetail);
                            TrDismissDetail.Add(ObjTrDismissDetail);

                            read05 = false;
                            read06 = false;
                            read07 = false;
                        }                                              
                    }
                };

                SharedModelDTO.File.FileModel FileDTO = new SharedModelDTO.File.FileModel();
                FileDTO.file_bytes = bytes;
                FileDTO.transaction_type = transaction_type;
                FileDTO.datetime_process = datetime;

                FileDTO = UtilityTools.File.Zip.FileCompression.ZipFileToBytes(FileDTO);

                DAO.DataAccess.Services.DbFile DbFile = new DAO.DataAccess.Services.DbFile();
                DbFile.SaveFile(FileDTO, string.Empty, FileName, "ARG", null);

                ReadFile.Status = "OK";
                ReadFile.StatusMessage = "OK";
                ReadFile.QtyTransactionRead = QtyTransactionRead;
                ReadFile.QtyTransactionProcess = QtyTransactionProcess;
                ReadFile.QtyTransactionDismiss = QtyTransactionDismiss;
                ReadFile.TrProcessDetail = TrProcessDetail;
                ReadFile.TrDismissDetail = TrDismissDetail;
            }
            catch (Exception ex)
            {
                ReadFile.Status = "ERROR";
                ReadFile.StatusMessage = ex.Message;
                ReadFile.QtyTransactionRead = QtyTransactionRead;
                ReadFile.QtyTransactionProcess = QtyTransactionProcess;
                ReadFile.QtyTransactionDismiss = QtyTransactionDismiss;
                ReadFile.TrProcessDetail = null;
                ReadFile.TrDismissDetail = null;
            }
            return ReadFile;
        }

        public SharedModel.Models.Services.CashPayments.CashPaymentModel.ReadFile ReadFileBAPR(string file, string transaction_type, string datetime, bool TransactionMechanism, string FileName)
        {
            SharedModel.Models.Services.CashPayments.CashPaymentModel.ReadFile ReadFile = new SharedModel.Models.Services.CashPayments.CashPaymentModel.ReadFile();
            List<SharedModel.Models.Services.CashPayments.CashPaymentModel.TransactionResult> TrProcessDetail = new List<SharedModel.Models.Services.CashPayments.CashPaymentModel.TransactionResult>();
            List<SharedModel.Models.Services.CashPayments.CashPaymentModel.TransactionResult> TrDismissDetail = new List<SharedModel.Models.Services.CashPayments.CashPaymentModel.TransactionResult>();
            SharedModel.Models.Services.CashPayments.CashPaymentModel.TransactionResult ObjTrProcessDetail;
            SharedModel.Models.Services.CashPayments.CashPaymentModel.TransactionResult ObjTrDismissDetail;

            /* TOTALIZADORES A 0 */
            Int32 QtyTransactionRead = 0;
            Int32 QtyTransactionProcess = 0;
            Int32 QtyTransactionDismiss = 0;

            try
            {
                /* BASE64 A BYTES[] */
                byte[] bytes = Convert.FromBase64String(file);
                //byte[] bytes = File.ReadAllBytes(@"C:\Users\FEDE\Downloads\BAPR_TEST.txt"); /* PROBAR EN LOCAL */

                /* SE SETEA LA POSICION Y LA LONGITUD DEL ACCOUNTNUMBER(CLIENTE EN LA GENERACION DEL BARCODE, QUE ES EL TICKET MAS TRES CEROS) */
                int AmountPosIni = 72;
                int AmountLength = 10;

                int BarCodePosIni = 0;
                int BarCodeLength = 60;

                int TicketPosIni = 4;
                int TicketLength = 11;

                bool isValid = false;

                int idx = 0;

                using (var stream = new StreamReader(new MemoryStream(bytes)))
                {
                    /* SE DECLARA UN STRING PARA OBTENER CADA LINEA DEL ARCHIVO */
                    string line;

                    /* WHILE MIENTRAS QUE NO SE TERMINE EL ARCHIVO */
                    while (!stream.EndOfStream)
                    {
                        ObjTrProcessDetail = new SharedModel.Models.Services.CashPayments.CashPaymentModel.TransactionResult();
                        ObjTrDismissDetail = new SharedModel.Models.Services.CashPayments.CashPaymentModel.TransactionResult();

                        /* VA LEYENDO LINEA POR LINEA Y SETEANDOLA A LA VARIABLE LINE */
                        line = stream.ReadLine();

                        idx += 1;

                        if (idx > 1) /* SE EVITA REGISTRO CABECERA */
                        {
                            QtyTransactionRead += 1;

                            /* SE OBTIENE EL MONTO ABONADO (TOTAL) */
                            string Amount = line.Substring(AmountPosIni, AmountLength);

                            string BarCodeNumber = line.Substring(BarCodePosIni, BarCodeLength);

                            /* SE OBTIENE EL TICKET GENERADO POR LP PARA TRACEAR LA OPERACION */
                            string Ticket = BarCodeNumber.Substring(TicketPosIni, TicketLength);

                            /* SE INSTANCIA AL DAO */
                            DAO.DataAccess.Services.DbCashPayment DbCashPayment = new DAO.DataAccess.Services.DbCashPayment();

                            /* SE INVOCA LA VALIDACION DE TICKET */
                            isValid = DbCashPayment.ValidateTicket(Ticket, TransactionMechanism);

                            if (isValid == true)
                            {
                                /* SI ES VALIDO, SE INCREMENTA EL TOTALIZADOR DE TRANSACCIONES PROCESADAS */
                                QtyTransactionProcess += 1;

                                ObjTrProcessDetail.Ticket = Ticket;
                                ObjTrProcessDetail.Amount = Convert.ToDouble(Amount) / 100;
                                ObjTrProcessDetail.BarCodeNumber = BarCodeNumber;
                            }
                            else
                            {
                                /* SINO ES VALIDO, SE INCREMENTA EL TOTALIZADOR DE TRANSACCIONES PERDIDAS */
                                QtyTransactionDismiss += 1;

                                ObjTrDismissDetail.Ticket = Ticket;
                                ObjTrDismissDetail.Amount = Convert.ToDouble(Amount) / 100;
                                ObjTrDismissDetail.BarCodeNumber = BarCodeNumber;
                            }

                            TrProcessDetail.Add(ObjTrProcessDetail);
                            TrDismissDetail.Add(ObjTrDismissDetail);
                        }
                    }
                }

                SharedModelDTO.File.FileModel FileDTO = new SharedModelDTO.File.FileModel();
                FileDTO.file_bytes = bytes;
                FileDTO.transaction_type = transaction_type;
                FileDTO.datetime_process = datetime;

                FileDTO = UtilityTools.File.Zip.FileCompression.ZipFileToBytes(FileDTO);

                DAO.DataAccess.Services.DbFile DbFile = new DAO.DataAccess.Services.DbFile();
                DbFile.SaveFile(FileDTO, string.Empty, FileName, "ARG", null);

                ReadFile.Status = "OK";
                ReadFile.StatusMessage = "OK";
                ReadFile.QtyTransactionRead = QtyTransactionRead;
                ReadFile.QtyTransactionProcess = QtyTransactionProcess;
                ReadFile.QtyTransactionDismiss = QtyTransactionDismiss;
                ReadFile.TrProcessDetail = TrProcessDetail;
                ReadFile.TrDismissDetail = TrDismissDetail;
            }
            catch (Exception ex)
            {
                ReadFile.Status = "ERROR";
                ReadFile.StatusMessage = ex.Message;
                ReadFile.QtyTransactionRead = QtyTransactionRead;
                ReadFile.QtyTransactionProcess = QtyTransactionProcess;
                ReadFile.QtyTransactionDismiss = QtyTransactionDismiss;
                ReadFile.TrProcessDetail = null;
                ReadFile.TrDismissDetail = null;
            }
            return ReadFile;
        }
        public SharedModel.Models.Services.CashPayments.CashPaymentModel.ReadFile ReadFileCOEX(string file, string transaction_type, string datetime, bool TransactionMechanism, string FileName)
        {
            SharedModel.Models.Services.CashPayments.CashPaymentModel.ReadFile ReadFile = new SharedModel.Models.Services.CashPayments.CashPaymentModel.ReadFile();
            List<SharedModel.Models.Services.CashPayments.CashPaymentModel.TransactionResult> TrProcessDetail = new List<SharedModel.Models.Services.CashPayments.CashPaymentModel.TransactionResult>();
            List<SharedModel.Models.Services.CashPayments.CashPaymentModel.TransactionResult> TrDismissDetail = new List<SharedModel.Models.Services.CashPayments.CashPaymentModel.TransactionResult>();
            SharedModel.Models.Services.CashPayments.CashPaymentModel.TransactionResult ObjTrProcessDetail;
            SharedModel.Models.Services.CashPayments.CashPaymentModel.TransactionResult ObjTrDismissDetail;

            /* TOTALIZADORES A 0 */
            Int32 QtyTransactionRead = 0;
            Int32 QtyTransactionProcess = 0;
            Int32 QtyTransactionDismiss = 0;

            /* SE SETEA LA POSICION Y LA LONGITUD DEL ACCOUNTNUMBER(CLIENTE EN LA GENERACION DEL BARCODE, QUE ES EL TICKET MAS TRES CEROS) */
            int HeaderLine = 0;
            int HeaderLineLength = 8;

            int AmountPosIni = 8;
            int AmountLength = 15;

            int BarCodePosIni = 23;
            int BarCodeLength = 48;

            int TicketPosIni = 15;
            int TicketLength = 11;

            bool isValid = false;

            try
            {
                /* BASE64 A BYTES[] */
                byte[] bytes = Convert.FromBase64String(file);
                //byte[] bytes = File.ReadAllBytes(@"C:\Users\FEDE\Downloads\4186G0764\RP060219_4186.txt"); /* PROBAR EN LOCAL */

                using (var stream = new StreamReader(new MemoryStream(bytes)))
                {
                    /* SE DECLARA UN STRING PARA OBTENER CADA LINEA DEL ARCHIVO */
                    string line;

                    /* WHILE MIENTRAS QUE NO SE TERMINE EL ARCHIVO */
                    while (!stream.EndOfStream)
                    {
                        ObjTrProcessDetail = new SharedModel.Models.Services.CashPayments.CashPaymentModel.TransactionResult();
                        ObjTrDismissDetail = new SharedModel.Models.Services.CashPayments.CashPaymentModel.TransactionResult();

                        /* VA LEYENDO LINEA POR LINEA Y SETEANDOLA A LA VARIABLE LINE */
                        line = stream.ReadLine();

                        if (line.Substring(HeaderLine, HeaderLineLength) != "00000000" && line.Substring(HeaderLine, HeaderLineLength) != "99999999")
                        {
                            QtyTransactionRead += 1;

                            /* SE OBTIENE EL MONTO ABONADO (TOTAL) */
                            string Amount = line.Substring(AmountPosIni, AmountLength);

                            string BarCodeNumber = line.Substring(BarCodePosIni, BarCodeLength);

                            /* SE OBTIENE EL TICKET GENERADO POR LP PARA TRACEAR LA OPERACION */
                            string Ticket = BarCodeNumber.Substring(TicketPosIni, TicketLength);

                            /* SE INSTANCIA AL DAO */
                            DAO.DataAccess.Services.DbCashPayment DbCashPayment = new DAO.DataAccess.Services.DbCashPayment();

                            /* SE INVOCA LA VALIDACION DE TICKET */
                            isValid = DbCashPayment.ValidateTicket(Ticket, TransactionMechanism);

                            if (isValid == true)
                            {
                                /* SI ES VALIDO, SE INCREMENTA EL TOTALIZADOR DE TRANSACCIONES PROCESADAS */
                                QtyTransactionProcess += 1;

                                ObjTrProcessDetail.Ticket = Ticket;
                                ObjTrProcessDetail.Amount = Convert.ToDouble(Amount) / 100;
                                ObjTrProcessDetail.BarCodeNumber = BarCodeNumber;
                            }
                            else
                            {
                                /* SINO ES VALIDO, SE INCREMENTA EL TOTALIZADOR DE TRANSACCIONES PERDIDAS */
                                QtyTransactionDismiss += 1;

                                ObjTrDismissDetail.Ticket = Ticket;
                                ObjTrDismissDetail.Amount = Convert.ToDouble(Amount) / 100;
                                ObjTrDismissDetail.BarCodeNumber = BarCodeNumber;
                            }

                            TrProcessDetail.Add(ObjTrProcessDetail);
                            TrDismissDetail.Add(ObjTrDismissDetail);
                        }
                    }
                }

                SharedModelDTO.File.FileModel FileDTO = new SharedModelDTO.File.FileModel();
                FileDTO.file_bytes = bytes;
                FileDTO.transaction_type = transaction_type;
                FileDTO.datetime_process = datetime;

                FileDTO = UtilityTools.File.Zip.FileCompression.ZipFileToBytes(FileDTO);

                DAO.DataAccess.Services.DbFile DbFile = new DAO.DataAccess.Services.DbFile();
                DbFile.SaveFile(FileDTO, string.Empty, FileName, "ARG", null);

                ReadFile.Status = "OK";
                ReadFile.StatusMessage = "OK";
                ReadFile.QtyTransactionRead = QtyTransactionRead;
                ReadFile.QtyTransactionProcess = QtyTransactionProcess;
                ReadFile.QtyTransactionDismiss = QtyTransactionDismiss;
                ReadFile.TrProcessDetail = TrProcessDetail;
                ReadFile.TrDismissDetail = TrDismissDetail;
            }
            catch (Exception ex)
            {
                ReadFile.Status = "ERROR";
                ReadFile.StatusMessage = ex.Message;
                ReadFile.QtyTransactionRead = QtyTransactionRead;
                ReadFile.QtyTransactionProcess = QtyTransactionProcess;
                ReadFile.QtyTransactionDismiss = QtyTransactionDismiss;
                ReadFile.TrProcessDetail = null;
                ReadFile.TrDismissDetail = null;
            }
            return ReadFile;
        }
    }
}
