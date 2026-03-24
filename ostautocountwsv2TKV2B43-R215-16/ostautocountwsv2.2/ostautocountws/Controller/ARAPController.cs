
using AutoCount.ARAP.APCN;
using AutoCount.ARAP.APInvoice;
using AutoCount.ARAP.ARCN;
using AutoCount.ARAP.ARDeposit;
using AutoCount.ARAP.ARInvoice;
using AutoCount.ARAP.ARPayment;
using AutoCount.ARAP.ARRefund;
using AutoCount.Authentication;
using Dapper;
using FirebirdSql.Data.FirebirdClient;
using Newtonsoft.Json.Linq;
using ostautocountws.Model;
using ostautocountws.Model.ostautocountws.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace ostautocountws.Controller
{
    public class ARAPController : ApiController
    {

        [Route("arap/apinvoicedel")]
        [HttpPost]
        public IHttpActionResult apinvoicedelete(apinvoice[] invoice)
        {
            try
            {
                if (invoice.Count() > 0)
                {
                    if (!SettingsMain.MyDbSession.IsLogin)
                    {
                        SettingsMain.MyDbSession.Login(SettingsMain.DefaultAutoCountID, SettingsMain.DefaultAutoCountPWD);
                    }
                    APInvoiceDataAccess apInvoiceDA = APInvoiceDataAccess.Create(SettingsMain.MyDbSession, SettingsMain.MydbSetting);


					// List<string> disInv = invoice.Select(c => c.DocNo).Distinct().ToList();
					List<apinvoice> disInv = invoice.Distinct().ToList();
                    foreach (var dis in disInv)
                    {
                         
                    //    int toDelete = int.Parse(dis) - 100000;
                     //   apInvoiceDA.CancelAPInvoice(toDelete.ToString(), SettingsMain.DefaultAutoCountID);
                        apInvoiceDA.CancelAPInvoice(dis.DocNo,dis.CreditorCode, SettingsMain.DefaultAutoCountID);
                       // 
                    }
                }
                return Ok();
            }

       

            catch (Exception ex)
            {

                SettingsMain.MLogger.Error(ex.Message + ex.StackTrace);
                return InternalServerError(ex);
            }

        }

        [Route("arap/apcndel")]
        [HttpPost]
        public IHttpActionResult apcndelete(apinvoice[] invoice)
        {
            try
            {

                if (invoice.Count() > 0)
                {
                    if (!SettingsMain.MyDbSession.IsLogin)
                    {
                        SettingsMain.MyDbSession.Login(SettingsMain.DefaultAutoCountID, SettingsMain.DefaultAutoCountPWD);
                    }
                    APCNDataAccess apCnDa = APCNDataAccess.Create(SettingsMain.MyDbSession, SettingsMain.MydbSetting);

                    try
                    {
  

                        List<apinvoice> disInv = invoice.Distinct().ToList();
                        foreach (var dis in disInv)
                        {
                        //    int toDelete = int.Parse(dis) - 100000;
                       //     apCnDa.CancelAPCN(toDelete.ToString(), SettingsMain.DefaultAutoCountID);
                            apCnDa.CancelAPCN(dis.DocNo, dis.CreditorCode, SettingsMain.DefaultAutoCountID);

                        }

                    }
                    catch (Exception ex)
                    {
                        return InternalServerError(ex);
                    }
                    finally
                    {
                     
                
                    }
                  

                }
               
       
                return Ok();
            }



            catch (Exception ex)
            {

                SettingsMain.MLogger.Error(ex.Message + ex.StackTrace);
                return InternalServerError(ex);
            }

        }

        [Route("arap/apinvoice")]
        [HttpPost]
        public IHttpActionResult apinvoice(apinvoice[] invoice)
        {
            try
            {
                if (invoice.Count() > 0)
                {
                    if (!SettingsMain.MyDbSession.IsLogin)
                    {
                        SettingsMain.MyDbSession.Login(SettingsMain.DefaultAutoCountID, SettingsMain.DefaultAutoCountPWD);
                    }

                    List<string> disInv = invoice.Select(c => c.DocNo).Distinct().ToList();
		
					foreach (var dis in disInv)
                    {
                        List<apinvoice> toProcess = invoice.Where(c => c.DocNo == dis).ToList();
                        APInvoiceDataAccess apInvoiceDA = APInvoiceDataAccess.Create(SettingsMain.MyDbSession, SettingsMain.MydbSetting);
                        APInvoiceEntity _invoiceToAdd = apInvoiceDA.NewAPInvoice();
                        apinvoice _srcInvoice = toProcess.First();
                        _invoiceToAdd.DocNo = _srcInvoice.DocNo;
                        _invoiceToAdd.SupplierInvoiceNo = _srcInvoice.DocNo;
                        _invoiceToAdd.CreditorCode = _srcInvoice.CreditorCode;
                        _invoiceToAdd.DisplayTerm = _srcInvoice.DisplayTerm;
                        _invoiceToAdd.DocDate = _srcInvoice.DocDate;
                        _invoiceToAdd.JournalType = _srcInvoice.JournalType;
                        _invoiceToAdd.RefNo2 = _srcInvoice.INVOICEBATCHNO;
                        _invoiceToAdd.Description = "Batch No:" + _srcInvoice.BATCHFILENO;
                        if (!string.IsNullOrWhiteSpace(_srcInvoice.CURRENCYCODE))
                        {
                            _invoiceToAdd.CurrencyCode = _srcInvoice.CURRENCYCODE;
                            _invoiceToAdd.CurrencyRate = _srcInvoice.EXCHANGERATE;
                        }
                        foreach (var item in toProcess)
                        {
                            APInvoiceDTLEntity apInvoiceDetail = _invoiceToAdd.NewDetail();
                            apInvoiceDetail.AccNo = item.AccNo;
                            apInvoiceDetail.Description = item.LineDescription;
                            apInvoiceDetail.ProjNo = item.ProjNo;
                            apInvoiceDetail.TaxCode = item.TaxType;
                            apInvoiceDetail.TaxRate = item.TaxRate;
                            //         arInvoiceDetail.Tax = item.Tax;
                            apInvoiceDetail.Amount = item.Amount;
                        }
                        apInvoiceDA.SaveAPInvoice(_invoiceToAdd, SettingsMain.DefaultAutoCountID);
                    }

                }


                return Ok();

            }



            catch (Exception ex)
            {

                SettingsMain.MLogger.Error(ex.Message + ex.StackTrace);
                return InternalServerError(ex);
            }

        }

        [Route("arap/apcn")]
        [HttpPost]
        public IHttpActionResult apcn(apinvoice[] invoice)
        {
            try
            {
                if (invoice.Count() > 0)
                {
                    if (!SettingsMain.MyDbSession.IsLogin)
                    {
                        SettingsMain.MyDbSession.Login(SettingsMain.DefaultAutoCountID, SettingsMain.DefaultAutoCountPWD);
                    }

                    List<string> disInv = invoice.Select(c => c.DocNo).Distinct().ToList();
		
					foreach (var dis in disInv)
                    {
                        List<apinvoice> toProcess = invoice.Where(c => c.DocNo == dis).ToList();
                        APCNDataAccess apCnDa = APCNDataAccess.Create(SettingsMain.MyDbSession, SettingsMain.MydbSetting);
                        APCNEntity _invoiceToAdd = apCnDa.NewAPCN();
                        apinvoice _srcInvoice = toProcess.First();
                        _invoiceToAdd.DocNo = _srcInvoice.DocNo;
                        _invoiceToAdd.SupplierCNNo = _srcInvoice.DocNo;
                        _invoiceToAdd.CreditorCode = _srcInvoice.CreditorCode;
                       // _invoiceToAdd.DisplayTerm = _srcInvoice.DisplayTerm;
                        _invoiceToAdd.DocDate = _srcInvoice.DocDate;
                        _invoiceToAdd.JournalType = _srcInvoice.JournalType;
                        _invoiceToAdd.RefNo2 = _srcInvoice.INVOICEBATCHNO;
                        _invoiceToAdd.Description = "Batch No:" + _srcInvoice.BATCHFILENO;
                        if (!string.IsNullOrWhiteSpace(_srcInvoice.CURRENCYCODE))
                        {
                            _invoiceToAdd.CurrencyCode = _srcInvoice.CURRENCYCODE;
                            _invoiceToAdd.CurrencyRate = _srcInvoice.EXCHANGERATE;
                        }
                        foreach (var item in toProcess)
                        {
                            APCNDTLEntity apInvoiceDetail = _invoiceToAdd.NewDetail();
                            apInvoiceDetail.AccNo = item.AccNo;
                            apInvoiceDetail.Description = item.LineDescription;
                            apInvoiceDetail.ProjNo = item.ProjNo;
                            apInvoiceDetail.TaxCode = item.TaxType;
                            apInvoiceDetail.TaxRate = item.TaxRate;
                            //         arInvoiceDetail.Tax = item.Tax;
                            apInvoiceDetail.Amount = -item.Amount;
                        }

                        try
                        {
           

                            apCnDa.SaveAPCN(_invoiceToAdd, SettingsMain.DefaultAutoCountID);
                        }
                        catch (Exception ex)
                        {

                            return InternalServerError(ex);
                        }
                        finally
                        {
                         
                
                        }
            
                    }

                }


                return Ok();

            }



            catch (Exception ex)
            {

                SettingsMain.MLogger.Error(ex.Message + ex.StackTrace);
                return InternalServerError(ex);
            }

        }

        [Route("arap/deposit")]
        [HttpPost]
        public IHttpActionResult ardeposit(cdeposit[] srcpayment)
        {
            try
            {
                if (srcpayment.Count() > 0)
                {
                    if (!SettingsMain.MyDbSession.IsLogin)
                    {
                        SettingsMain.MyDbSession.Login(SettingsMain.DefaultAutoCountID, SettingsMain.DefaultAutoCountPWD);
                    }
                    cdeposit _srcPayment = srcpayment[0];

                    bool executeCancel = false;
                    bool executeUncancel = false;

                    ARDepositCommand cmd = ARDepositCommand.Create(SettingsMain.MyDbSession, SettingsMain.MydbSetting);
                    ARDeposit doc = null;
           
                    try
                    {
                       
                        doc = cmd.Edit(_srcPayment.DocNo); 
                        if(doc.Cancelled)
                        {
                            executeUncancel = true;
                            doc.ClearDetails();
                            ARDepositDetail detail = doc.AddDetail();
                            detail.PaymentAmt = _srcPayment.FXVALUE;
                            detail.PaymentBy = _srcPayment.PAYMENTACCOUNT;
                            detail.ChequeNo = _srcPayment.ChequeNo;
                            //detail.PaymentMethod = _srcPayment.PAYMENTMETHOD;

                            var PaymentMethod = getPaymentMethod(_srcPayment.DebitAcc);
                            if (!string.IsNullOrWhiteSpace(PaymentMethod))
                            {
                                detail.PaymentMethod = PaymentMethod;
                            }

                            if (!string.IsNullOrWhiteSpace(_srcPayment.CURRENCYCODE))
                            {
                                doc.CurrencyCode = _srcPayment.CURRENCYCODE;
                                doc.MasterRow["ToDepositRate"] = _srcPayment.EXCHANGERATE;
                                doc.MasterRow["ToHomeRate"] = _srcPayment.EXCHANGERATE;
                            }
                        }
                        else
                        {
                            executeCancel = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        SettingsMain.MLogger.Error(ex.Message + ex.StackTrace);

                    }
                    
                    if(doc == null)
                    {
                         doc =  cmd.AddNew();
                        doc.DocNo = _srcPayment.DocNo;
                        doc.DebtorCode = _srcPayment.DebtorCode;
                        doc.DocDate = _srcPayment.DocDate;
                  
                        var DepositMethod = getPaymentMethod(_srcPayment.CreditAcc);
                        if (!string.IsNullOrWhiteSpace(DepositMethod))
                        {
                            doc.DepositPaymentMethod = DepositMethod;
                            
                        }
                        ARDepositDetail detail = doc.AddDetail();
                      
                        detail.PaymentAmt = _srcPayment.FXVALUE;
                        detail.PaymentBy = _srcPayment.PAYMENTACCOUNT;
                        detail.ChequeNo = _srcPayment.ChequeNo;
                        var PaymentMethod = getPaymentMethod(_srcPayment.DebitAcc);
                        if (!string.IsNullOrWhiteSpace(PaymentMethod))
                        {
                            detail.PaymentMethod = PaymentMethod;
                            
                        }


                        if (!string.IsNullOrWhiteSpace(_srcPayment.CURRENCYCODE))
                        {
                            doc.CurrencyCode = _srcPayment.CURRENCYCODE;
                            doc.MasterRow["ToDepositRate"] = _srcPayment.EXCHANGERATE;
                            doc.MasterRow["ToHomeRate"] = _srcPayment.EXCHANGERATE;
                            //detail.ToBankRate = _srcPayment.EXCHANGERATE;
                            
                        }
                    }

                    try
                    {

                        doc.Description = _srcPayment.Description.Substring(0,40);
                        doc.Save();
                        if (executeUncancel)
                        {

						//	doc.UncancelDocument();
                           
						}
                        if (executeCancel)
                        {
                            doc.CancelDocument();

                        }


                    }
                    catch (Exception ex)
                    {

                        return InternalServerError(ex);
                    }
                    finally
                    {

                    }









                }

                return Ok();
            }

            catch (Exception ex)
            {

                SettingsMain.MLogger.Error(ex.Message + ex.StackTrace);
                return InternalServerError(ex);
            }

        }


        private string getPaymentMethod(string accno)
        {

            using (SqlConnection cnn = new SqlConnection(SettingsMain.MydbSetting.ConnectionString))
            {
                var myPayment = cnn.Query<cpaymentMethod>("select * from PaymentMethod where upper(BankAccount) = @BankAccount ", new { BankAccount = accno });
                if(myPayment.Count()>0)
                {
                    return myPayment.FirstOrDefault().PaymentMethod;
                }
                else
                {
                    return "";
                }
            }
        }

        //[Route("arap/applydeposit")]
        //[HttpPost]
        //public IHttpActionResult applydeposit(cpayment[] srcpayment)
        //{
        //    try
        //    {
        //        if (srcpayment.Count() > 0)
        //        {
        //            cpayment _srcPayment = srcpayment[0];    
        //            using (SqlConnection cnn = new SqlConnection(SettingsMain.MydbSetting.ConnectionString))
        //            {                  
        //                    var myPayment = cnn.Query<cpayment>("select * from ARPayment where DocNo2 = @DocNo2", new { DocNo2 = _srcPayment.DocNo });
        //                var myDeposit = cnn.Query<cdeposit>("select * from ARDeposit where DocNo = @DocNo", new { DocNo = _srcPayment.DEPOSITNUMBER });
        //                    bool isExistPayment = myPayment.Count() > 0;
        //                     bool isPaymentCancelled;
        //                     bool executeUncancel = false;
        //                ARPaymentDataAccess arPaymentDA = ARPaymentDataAccess.Create(SettingsMain.MyDbSession, SettingsMain.MydbSetting);
        //                ARInvoiceDataAccess arInvoiceDA = ARInvoiceDataAccess.Create(SettingsMain.MyDbSession, SettingsMain.MydbSetting);
        //                    ARPaymentEntity _arPayment = null;
        //                    if (!isExistPayment)
        //                    {
        //                        //Payment Does not Exist
        //                        _arPayment = arPaymentDA.NewARPayment();
        //                        _arPayment.DocNo2 = _srcPayment.DocNo;
        //                        _arPayment.DebtorCode = _srcPayment.DebtorCode;
        //                        _arPayment.DocDate = _srcPayment.DocDate;
        //                       _arPayment.Description = _srcPayment.Description;
                        
        //                        if (!string.IsNullOrWhiteSpace(_srcPayment.CURRENCYCODE))
        //                        {
        //                            _arPayment.CurrencyCode = _srcPayment.CURRENCYCODE;
        //                            _arPayment.ToHomeRate = _srcPayment.EXCHANGERATE;
        //                        }


        //                        ARPaymentDTLEntity arPaymentDetail = _arPayment.NewDetail();
                                
        //                        var PaymentMethod = getPaymentMethod(_srcPayment.DebitAcc);
        //                    var theInvoice = arInvoiceDA.GetARInvoice(_srcPayment.INVOICENUMBER);
                         
        //                    if (!string.IsNullOrWhiteSpace(PaymentMethod))
        //                        {
        //                            arPaymentDetail.PaymentMethod = PaymentMethod;
        //                            arPaymentDetail.PaymentAmt = _srcPayment.PAYMENTAMOUNT;

        //                  //        var depositList =    arPaymentDA.GetARDeposit(_srcPayment.DebtorCode, theInvoice.CurrencyCode, PaymentMethod);
        //                        arPaymentDetail.DepositDocKey = myDeposit.First().DocKey;
        //                        }

        //                        if(_srcPayment.DrAccType == "SDP")
        //                            {
        //                        _arPayment.KnockOff(AutoCount.Document.DocumentType.ARInvoice, _srcPayment.INVOICENUMBER,  _srcPayment.FXVALUE);

        //                             }

        //                    }
        //                    else
        //                    {
        //                        //Payment Exist
        //                        _arPayment = arPaymentDA.GetARPayment(myPayment.First().DocNo);
        //                    isPaymentCancelled = _arPayment.Cancelled;
                               
        //                     if(!isPaymentCancelled)
        //                    {
        //                        if (_srcPayment.DrAccType == "SDP")
        //                        {
        //                            _arPayment.KnockOff(AutoCount.Document.DocumentType.ARInvoice, _srcPayment.INVOICENUMBER, _srcPayment.FXVALUE);
        //                        }
        //                        else
        //                        {
        //                            if (_srcPayment.DEPOSITAPPLIEDAMOUNT == 0)
        //                            {
        //                                _arPayment.ARPaymentTable.Rows[0]["Cancelled"] = "T";
        //                            }
        //                            else
        //                            {
        //                                _arPayment.ClearKnockOff();
        //                            }
                                  
        //                        }
        //                    }
        //                    else
        //                    {
        //                        if(_srcPayment.DrAccType == "SDP")
        //                        {

        //                            executeUncancel = true;
        //                            _arPayment.ARPaymentTable.Rows[0]["Cancelled"] = "F";
        //                            _arPayment.KnockOff(AutoCount.Document.DocumentType.ARInvoice, _srcPayment.INVOICENUMBER, _srcPayment.FXVALUE);
        //                        }
        //                    }
                                
                     
                             
        //                    }
        //                    try
        //                    {

        //                        arPaymentDA.SaveARPayment(_arPayment, SettingsMain.DefaultAutoCountID);
                          


        //                    }
        //                    catch (Exception ex)
        //                    {

        //                        return InternalServerError(ex);
        //                    }
        //                    finally
        //                    {
        //                       
        //                    }
                        
                       
        //            }
        //        }

        //        return Ok();
        //    }

        //    catch (Exception ex)
        //    {

        //        SettingsMain.MLogger.Error(ex.Message + ex.StackTrace);
        //        return InternalServerError(ex);
        //    }

        //}



        [Route("arap/applydeposit")]
        [HttpPost]
        public IHttpActionResult applydeposit(cpayment[] srcpayment)
        {
            try
            {
                if (srcpayment.Count() > 0)
                {
                    if (!SettingsMain.MyDbSession.IsLogin)
                    {
                        SettingsMain.MyDbSession.Login(SettingsMain.DefaultAutoCountID, SettingsMain.DefaultAutoCountPWD);
                    }
                    cpayment _srcPayment = srcpayment[0];
                    using (SqlConnection cnn = new SqlConnection(SettingsMain.MydbSetting.ConnectionString))
                    {
                        var myPayment = cnn.Query<cpayment>("select * from ARPayment where DocNo2 = @DocNo2 and Description like @Description", new { DocNo2 = _srcPayment.DEPOSITNUMBER, Description = "%" + _srcPayment.DocNo + "%" });
                        var myDeposit = cnn.Query<cdeposit>("select * from ARDeposit where DocNo = @DocNo", new { DocNo = _srcPayment.DEPOSITNUMBER });
                        bool isExistPayment = myPayment.Count() > 0;
                        bool isPaymentCancelled;
                        bool executeUncancel = false;
                        ARPaymentDataAccess arPaymentDA = ARPaymentDataAccess.Create(SettingsMain.MyDbSession, SettingsMain.MydbSetting);
                        ARInvoiceDataAccess arInvoiceDA = ARInvoiceDataAccess.Create(SettingsMain.MyDbSession, SettingsMain.MydbSetting);
                        ARPaymentEntity _arPayment = null;
                        if (!isExistPayment)
                        {
                            //Payment Does not Exist
                            _arPayment = arPaymentDA.NewARPayment();
                            _arPayment.DocNo2 = _srcPayment.DEPOSITNUMBER;
                            _arPayment.DebtorCode = _srcPayment.DebtorCode;
                            _arPayment.DocDate = _srcPayment.DocDate;
                            _arPayment.Description = _srcPayment.Description;

                            if (!string.IsNullOrWhiteSpace(_srcPayment.CURRENCYCODE))
                            {
                                _arPayment.CurrencyCode = _srcPayment.CURRENCYCODE;
                                _arPayment.ToHomeRate = _srcPayment.EXCHANGERATE;
                            }


                            ARPaymentDTLEntity arPaymentDetail = _arPayment.NewDetail();

                            var PaymentMethod = getPaymentMethod(_srcPayment.DebitAcc);
                            var theInvoice = arInvoiceDA.GetARInvoice(_srcPayment.INVOICENUMBER);

                            if (!string.IsNullOrWhiteSpace(PaymentMethod))
                            {
                                arPaymentDetail.PaymentMethod = PaymentMethod;
                                arPaymentDetail.PaymentAmt = _srcPayment.PAYMENTAMOUNT;

                                //        var depositList =    arPaymentDA.GetARDeposit(_srcPayment.DebtorCode, theInvoice.CurrencyCode, PaymentMethod);
                                arPaymentDetail.DepositDocKey = myDeposit.First().DocKey;
                            }

                            if (_srcPayment.DrAccType == "SDP")
                            {
                                _arPayment.KnockOff(AutoCount.Document.DocumentType.ARInvoice, _srcPayment.INVOICENUMBER, _srcPayment.FXVALUE);

                            }

                        }
                        else
                        {
                            //Payment Exist
                            _arPayment = arPaymentDA.GetARPayment(myPayment.First().DocNo);

                            var _arPaymentDtlKey = cnn.Query<long>("select top 1 dtlkey from ARPaymentDTL where DocKey = @DocKey", new { Dockey = _arPayment.DocKey }).First();
                            isPaymentCancelled = _arPayment.Cancelled;

                            if (!isPaymentCancelled)
                            {
                                if (_srcPayment.DrAccType == "SDP")
                                {
                                    //_arPayment.ARPaymentTable.Rows[0]["PaymentAmt"] = _srcPayment.PAYMENTAMOUNT;
             
                                    //ARDepositCommand cmd = ARDepositCommand.Create(SettingsMain.MyDbSession,SettingsMain.MydbSetting);
                                    //ARDeposit doc = null;
                                    //doc = cmd.Edit(myDeposit.First().DocNo);
                                    //if (doc.PaymentAmount != _srcPayment.DEPOSITAMOUNT)
                                    //{
                                    //    ARDepositDetail detail = doc.EditDetail(0);
                                    //    detail.PaymentAmt = _srcPayment.DEPOSITAMOUNT;
                                    //    doc.Save();

                                    //}
                                    ARPaymentDTLEntity paymentDtl = _arPayment.GetDetail(_arPaymentDtlKey);
                                    if (paymentDtl.PaymentAmt != _srcPayment.PAYMENTAMOUNT)
                                    {
                                        paymentDtl.PaymentAmt = _srcPayment.PAYMENTAMOUNT;
                                        arPaymentDA.SaveARPayment(_arPayment, SettingsMain.DefaultAutoCountID);
                                    }
                                    _arPayment.KnockOff(AutoCount.Document.DocumentType.ARInvoice, _srcPayment.INVOICENUMBER, _srcPayment.FXVALUE);
                                }
                                else
                                {
                                    if (_srcPayment.DEPOSITAPPLIEDAMOUNT == 0)
                                    {
                                        _arPayment.ARPaymentTable.Rows[0]["Cancelled"] = "T";
                                    }
                                    else
                                    {
                                        // _arPayment.ClearKnockOff();
                                        _arPayment.KnockOff(AutoCount.Document.DocumentType.ARInvoice, _srcPayment.INVOICENUMBER, 0);
                                    }

                                }
                            }
                            else
                            {
                                if (_srcPayment.DrAccType == "SDP")
                                {

                                    executeUncancel = true;
                                    _arPayment.ARPaymentTable.Rows[0]["Cancelled"] = "F";
                              
                                    _arPayment.KnockOff(AutoCount.Document.DocumentType.ARInvoice, _srcPayment.INVOICENUMBER, _srcPayment.FXVALUE);
                                }
                            }



                        }
                        try
                        {
          
                            arPaymentDA.SaveARPayment(_arPayment, SettingsMain.DefaultAutoCountID);



                        }
                        catch (Exception ex)
                        {

                            return InternalServerError(ex);
                        }
                        finally
                        {
                       
                        }


                    }
                }

                return Ok();
            }

            catch (Exception ex)
            {

                SettingsMain.MLogger.Error(ex.Message + ex.StackTrace);
                return InternalServerError(ex);
            }

        }


        [Route("arap/applypayment")]
        [HttpPost]
        public IHttpActionResult arapplypayment(cpayment[] srcpayment)
        {
            try
            {
                if (srcpayment.Count() > 0)
                {
                    if (!SettingsMain.MyDbSession.IsLogin)
                    {
                        SettingsMain.MyDbSession.Login(SettingsMain.DefaultAutoCountID, SettingsMain.DefaultAutoCountPWD);
                    }
                    cpayment _srcPayment = srcpayment[0];


                    using (SqlConnection cnn = new SqlConnection(SettingsMain.MydbSetting.ConnectionString))
                    {

                        if (_srcPayment.PAYMENTAMOUNT > 0)
                        {
                            var myPayment = cnn.Query<cpayment>("select * from ARPayment where DocNo2 = @DocNo2", new { DocNo2 = _srcPayment.DocNo });
                            bool isExistPayment = myPayment.Count() > 0;
                            ARPaymentDataAccess arPaymentDA = ARPaymentDataAccess.Create(SettingsMain.MyDbSession, SettingsMain.MydbSetting);
                            ARPaymentEntity _arPayment = null;
                            if (!isExistPayment)
                            {
                                return Ok();
                            }
                            else
                            {
                                //Payment Exist
                                _arPayment = arPaymentDA.GetARPayment(myPayment.First().DocNo);
                                if (myPayment.First().Cancelled == "F")
                                {
                                    if (_srcPayment.DrAccType == "SDC")
                                    {

                                        // _arPayment.ClearKnockOff();
                                        _arPayment.KnockOff(AutoCount.Document.DocumentType.ARInvoice, _srcPayment.INVOICENUMBER, 0);
                                    }
                                    else
                                    {
                                        _arPayment.KnockOff(AutoCount.Document.DocumentType.ARInvoice, _srcPayment.INVOICENUMBER, _srcPayment.FXVALUE);
                                    }
                                }
                                else
                                {
                                    return Ok();
                                }
                              
                            }
                            try
                            { 
                                arPaymentDA.SaveARPayment(_arPayment, SettingsMain.DefaultAutoCountID);

                            }
                            catch (Exception ex)
                            {

                                return InternalServerError(ex);
                            }
                            finally
                            {
                         
                            }
                        }
                        else
                        {
                            var myRefund = cnn.Query<cpayment>("select * from ARRefund where DocNo2 = @DocNo2", new { DocNo2 = _srcPayment.DocNo });
                            bool isExistPayment = myRefund.Count() > 0;
                            ARRefundDataAccess arRefundDA = ARRefundDataAccess.Create(SettingsMain.MyDbSession, SettingsMain.MydbSetting);
                            ARRefundEntity _arRefund = null;
                            if (!isExistPayment)
                            {
                                return Ok();
                            }
                            else
                            {
                                _arRefund = arRefundDA.GetARRefund(myRefund.First().DocNo);
                                if (myRefund.First().Cancelled == "F")
                                {
                                    
                                  //  if(_srcPayment.DrAccType=="SDC")
                                   if (_srcPayment.DrAccType == "SDC")
                                        {
                                        _arRefund.KnockOff(AutoCount.Document.DocumentType.ARCN, _srcPayment.INVOICENUMBER, -_srcPayment.FXVALUE);
                                    }
                                    else
                                    {
                                        //_arRefund.ClearKnockOff();
                                        _arRefund.KnockOff(AutoCount.Document.DocumentType.ARCN, _srcPayment.INVOICENUMBER, 0);
                                    }

                                }
                                else
                                {
                                    return Ok();
                                }

                            }
                            try
                            {
        
                                arRefundDA.SaveARRefund(_arRefund, SettingsMain.DefaultAutoCountID);
                          
                              

                            }
                            catch (Exception ex)
                            {
                                return InternalServerError(ex);
                            }
                            finally
                            {
                          
                            }


                        }
                    }
                }

                return Ok();
            }

            catch (Exception ex)
            {

                SettingsMain.MLogger.Error(ex.Message + ex.StackTrace);
                return InternalServerError(ex);
            }

        }


        [Route("arap/knockoffcndelete")]
        [HttpPost]
        public IHttpActionResult knockoffcndelete(cnKnockOff DocNo)
        {
            try
            {
                if (!SettingsMain.MyDbSession.IsLogin)
                {
                    SettingsMain.MyDbSession.Login(SettingsMain.DefaultAutoCountID, SettingsMain.DefaultAutoCountPWD);
                }
                ARCNDataAccess arCnDa = ARCNDataAccess.Create(SettingsMain.MyDbSession, SettingsMain.MydbSetting);
                ARCNEntity toDelete = null;
                try
                {
                   toDelete = arCnDa.GetARCN(DocNo.CREDITNUMBER);
                   toDelete.ClearKnockOff();
                    try
                    {
 
                        arCnDa.SaveARCN(toDelete, SettingsMain.DefaultAutoCountID);
                    }
                    catch (Exception ex)
                    {

                        return InternalServerError(ex);
                    }
                    finally
                    {
                
                    }
                }
                catch (Exception ex)
                {

                    SettingsMain.MLogger.Error(ex);
                }

                return Ok();
    
         
            }

            catch (Exception ex)
            {

                SettingsMain.MLogger.Error(ex.Message + ex.StackTrace);
                return InternalServerError(ex);
            }

        }

        [Route("arap/knockoffcn")]
        [HttpPost]
        public IHttpActionResult knockoff(cnKnockOff[] srcpayment)
        {
            try
            {
                //SettingsMain.MLogger.Info(DocNo.INVOICENUMBER);
                if (srcpayment.Count() > 0)
                {
                    if (!SettingsMain.MyDbSession.IsLogin)
                    {
                        SettingsMain.MyDbSession.Login(SettingsMain.DefaultAutoCountID, SettingsMain.DefaultAutoCountPWD);
                    }
                    cnKnockOff _srcPayment = srcpayment[0];
                    ARCNDataAccess arCnDa = ARCNDataAccess.Create(SettingsMain.MyDbSession, SettingsMain.MydbSetting);
                    ARCNEntity toApply = null;
                    try
                    {
                        toApply = arCnDa.GetARCN(_srcPayment.CREDITNUMBER);
                        foreach (var item in srcpayment)
                        {
                            //if (string.IsNullOrWhiteSpace(item.CURRENCYCODE))
                            //{

                            //}
                            //else
                            //{

                            //}
                            toApply.KnockOff(AutoCount.Document.DocumentType.ARInvoice, item.INVOICENUMBER, item.APPLIEDAMOUNT);
                        }
                    }

                    catch (Exception ex)
                    {

                        SettingsMain.MLogger.Error(ex);
                    }

                    try
                    {
 
                        arCnDa.SaveARCN(toApply, SettingsMain.DefaultAutoCountID);

                    }
                    catch (Exception ex)
                    {

                        return InternalServerError(ex);
                    }
                    finally
                    {
                     
                    }
                }
                    return Ok();
            }

            catch (Exception ex)
            {

                SettingsMain.MLogger.Error(ex.Message + ex.StackTrace);
                return InternalServerError(ex);
            }

        }


        [Route("arap/payment")]
        [HttpPost]
        public IHttpActionResult arpayment(cpayment[] srcpayment)
        {
            try
            {
                if (srcpayment.Count() > 0)
                {
                    if (!SettingsMain.MyDbSession.IsLogin)
                    {
                        SettingsMain.MyDbSession.Login(SettingsMain.DefaultAutoCountID, SettingsMain.DefaultAutoCountPWD);
                    }
                    cpayment _srcPayment = srcpayment[0];
          
                    bool executeCancel = false;
                    bool executeUncancel = false;
                    bool isInsertDoc2Refund = false;

                    using (SqlConnection cnn = new SqlConnection(SettingsMain.MydbSetting.ConnectionString))
                    {
                      
                        if (_srcPayment.PAYMENTAMOUNT > 0)
                        {
                            var myPayment = cnn.Query<cpayment>("select * from ARPayment where DocNo2 = @DocNo2", new { DocNo2 = _srcPayment.DocNo });
                            bool isExistPayment = myPayment.Count() > 0;
                            ARPaymentDataAccess arPaymentDA = ARPaymentDataAccess.Create(SettingsMain.MyDbSession, SettingsMain.MydbSetting);
                            ARPaymentEntity _arPayment = null;
                            if (!isExistPayment)
                            {
                                //Payment Does not Exist
                                _arPayment = arPaymentDA.NewARPayment();
                                _arPayment.DocNo2 = _srcPayment.DocNo;
                                _arPayment.DebtorCode = _srcPayment.DebtorCode;
                                _arPayment.DocDate = _srcPayment.DocDate;
                                _arPayment.Description = _srcPayment.Description;

                                if (!string.IsNullOrWhiteSpace(_srcPayment.CURRENCYCODE))
                                {
                                    _arPayment.CurrencyCode = _srcPayment.CURRENCYCODE;
                                    _arPayment.ToHomeRate = _srcPayment.EXCHANGERATE;
                                }


                                ARPaymentDTLEntity arPaymentDetail = _arPayment.NewDetail();
                                arPaymentDetail.ChequeNo = _srcPayment.ChequeNo;
                            var PaymentMethod = getPaymentMethod(_srcPayment.DebitAcc);
                                if (!string.IsNullOrWhiteSpace(PaymentMethod))
                                {
                                    arPaymentDetail.PaymentMethod = PaymentMethod;
                                
                                    arPaymentDetail.PaymentAmt = _srcPayment.PAYMENTAMOUNT;
                                }
                              
                            }
                            else
                            {
                                //Payment Exist
                                _arPayment = arPaymentDA.GetARPayment(myPayment.First().DocNo);
                                if (myPayment.First().Cancelled == "T")
                                {
                                    //Uncancel document and update the latest amount
                                
                                    executeUncancel = true;
                                    _arPayment.ClearDetails();
                                    _arPayment.DocNo2 = _srcPayment.DocNo;
                                    _arPayment.DebtorCode = _srcPayment.DebtorCode;
                                    _arPayment.DocDate = _srcPayment.DocDate;
                                  
                                    if (!string.IsNullOrWhiteSpace(_srcPayment.CURRENCYCODE))
                                    {
                                        _arPayment.CurrencyCode = _srcPayment.CURRENCYCODE;
                                        _arPayment.ToHomeRate = _srcPayment.EXCHANGERATE;
                                    }


                                    ARPaymentDTLEntity arPaymentDetail = _arPayment.NewDetail();
                                    //     arPaymentDetail.PaymentMethod = _srcPayment.PAYMENTMETHOD.ToUpper();
                                    arPaymentDetail.ChequeNo = _srcPayment.ChequeNo;
                                    var PaymentMethod = getPaymentMethod(_srcPayment.DebitAcc);
                                    if (!string.IsNullOrWhiteSpace(PaymentMethod))
                                    {
                                        arPaymentDetail.PaymentMethod = PaymentMethod;
                                        arPaymentDetail.PaymentAmt = _srcPayment.PAYMENTAMOUNT;
                                    }
                                    arPaymentDetail.PaymentAmt = _srcPayment.PAYMENTAMOUNT;

                                }
                                else
                                {
                                    executeCancel = true;
                             
                                }
                            }
                            try
                            {
             
                                arPaymentDA.SaveARPayment(_arPayment, SettingsMain.DefaultAutoCountID);
                                if (executeUncancel)
                                {

                                //    arPaymentDA.UncancelARPayment(_arPayment.DocNo, SettingsMain.DefaultAutoCountID);
                                }
                                if (executeCancel)
                                {
                                    arPaymentDA.CancelARPayment(_arPayment.DocNo, SettingsMain.DefaultAutoCountID);

                                }


                            }
                            catch (Exception ex)
                            {

                                return InternalServerError(ex);
                            }
                            finally
                            {
                          
                            }
                        }
                        else
                        {
                            var myRefund = cnn.Query<cpayment>("select * from ARRefund where DocNo2 = @DocNo2", new { DocNo2 = _srcPayment.DocNo });
                            bool isExistPayment = myRefund.Count() > 0;
                            ARRefundDataAccess arRefundDA = ARRefundDataAccess.Create(SettingsMain.MyDbSession, SettingsMain.MydbSetting);
                            ARRefundEntity _arRefund = null;
                            if (!isExistPayment)
                            {
                                //Payment Does not Exist
                                _arRefund = arRefundDA.NewARRefund();
                                // _arRefund.DocNo = _srcPayment.DocNo;
                                isInsertDoc2Refund = true;
                                _arRefund.DebtorCode = _srcPayment.DebtorCode;
                                _arRefund.DocDate = _srcPayment.DocDate;

                                if (!string.IsNullOrWhiteSpace(_srcPayment.CURRENCYCODE))
                                {
                                    _arRefund.CurrencyCode = _srcPayment.CURRENCYCODE;
                                    _arRefund.ToHomeRate = _srcPayment.EXCHANGERATE;
                                }


                                ARRefundDTLEntity arRefundDetail = _arRefund.NewDetail();
                                //   arRefundDetail.PaymentMethod = _srcPayment.PAYMENTMETHOD.ToUpper();
                                arRefundDetail.ChequeNo = _srcPayment.ChequeNo;
                                var PaymentMethod = getPaymentMethod(_srcPayment.CreditAcc);
                                if (!string.IsNullOrWhiteSpace(PaymentMethod))
                                {
                                    arRefundDetail.PaymentMethod = PaymentMethod;
                                  //  arRefundDetail.PaymentAmt = _srcPayment.PAYMENTAMOUNT;
                                }
                                arRefundDetail.PaymentAmt = _srcPayment.FXVALUE;
                            }
                            else
                            {
                                _arRefund = arRefundDA.GetARRefund(myRefund.First().DocNo);
                                if (myRefund.First().Cancelled == "T")
                                {
                                    executeUncancel = true;
                                    _arRefund.ClearDetails();
                                    _arRefund.DebtorCode = _srcPayment.DebtorCode;
                                    _arRefund.DocDate = _srcPayment.DocDate;

                                    if (!string.IsNullOrWhiteSpace(_srcPayment.CURRENCYCODE))
                                    {
                                        _arRefund.CurrencyCode = _srcPayment.CURRENCYCODE;
                                        _arRefund.ToHomeRate = _srcPayment.EXCHANGERATE;
                                    }


                                    ARRefundDTLEntity arRefundDetail = _arRefund.NewDetail();
                                    arRefundDetail.ChequeNo = _srcPayment.ChequeNo;
                                    var PaymentMethod = getPaymentMethod(_srcPayment.CreditAcc);
                                    if (!string.IsNullOrWhiteSpace(PaymentMethod))
                                    {
                                        arRefundDetail.PaymentMethod = PaymentMethod;
                                        //  arRefundDetail.PaymentAmt = _srcPayment.PAYMENTAMOUNT;
                                    }
                                    arRefundDetail.PaymentAmt = _srcPayment.FXVALUE;

                                }
                                else
                                {
                                    executeCancel = true;
                                }

                            }
                            try
                            {
                  
                                arRefundDA.SaveARRefund(_arRefund, SettingsMain.DefaultAutoCountID);
                                if (executeUncancel)
                                {
                                   // arRefundDA.UncancelARRefund(_arRefund.DocNo, SettingsMain.DefaultAutoCountID);
                                }
                                if (executeCancel)
                                {
                                    arRefundDA.CancelARRefund(_arRefund.DocNo, SettingsMain.DefaultAutoCountID);
                                }
                                if (isInsertDoc2Refund)
                                {
                                    cnn.Execute(@"Update ARRefund Set DocNo2=@DocNo2 where DocNo = @DocNo", new { DocNo2 = _srcPayment.DocNo, DocNo = _arRefund.DocNo });
                                }

                            }
                            catch (Exception ex)
                            {
                                return InternalServerError(ex);
                            }
                            finally
                            {
                     
                            }


                        }
                    }
                }

                return Ok();
            }

            catch (Exception ex)
            {

                SettingsMain.MLogger.Error(ex.Message + ex.StackTrace);
                return InternalServerError(ex);
            }

        }

        [Route("arap/arinvoicejson")]
        [HttpPost]
        public IHttpActionResult arinvoicejson(arinvoice[] invoice)
        {

          //  var abc = invoice.ToObject<Dictionary<string, object>>();
           //System.Text.Decode
            return Ok("abc");
            //try
            //{
            //    if(invoice.Count()>0)
            //    {

            //        List<string> disInv = invoice.Select(c => c.DocNo).Distinct().ToList();
            //        foreach (var dis in disInv)
            //        {
            //            List<arinvoice> toProcess = invoice.Where(c => c.DocNo == dis).ToList();
            //            ARInvoiceDataAccess arInvoiceDA = ARInvoiceDataAccess.Create(SettingsMain.MyDbSession, SettingsMain.MydbSetting);
            //            ARInvoiceEntity _invoiceToAdd = arInvoiceDA.NewARInvoice();
            //            arinvoice _srcInvoice = toProcess.First();
            //            _invoiceToAdd.DocNo = _srcInvoice.DocNo;
            //            _invoiceToAdd.DebtorCode = _srcInvoice.DebtorCode;
            //            _invoiceToAdd.DisplayTerm = _srcInvoice.DisplayTerm;
            //            _invoiceToAdd.DocDate = _srcInvoice.DocDate;

            //            if(!string.IsNullOrWhiteSpace(_srcInvoice.CURRENCYCODE))
            //            {
            //                _invoiceToAdd.CurrencyCode = _srcInvoice.CURRENCYCODE;
            //                _invoiceToAdd.CurrencyRate = _srcInvoice.EXCHANGERATE;
            //            }
            //            if (!string.IsNullOrWhiteSpace(_srcInvoice.SalesAgent))
            //            {
            //                _invoiceToAdd.SalesAgent = _srcInvoice.SalesAgent;
            //            }
            //            else
            //            {
            //                _invoiceToAdd.SalesAgent = null;
            //            }



            //            _invoiceToAdd.JournalType = _srcInvoice.JournalType;
            //            _invoiceToAdd.RefNo2 = _srcInvoice.BATCHFILENO;
            //            _invoiceToAdd.Description = "Batch No:" + _srcInvoice.BATCHFILENO;

            //            foreach (var item in toProcess)
            //            {
            //                ARInvoiceDTLEntity arInvoiceDetail = _invoiceToAdd.NewDetail();
            //                arInvoiceDetail.AccNo = item.AccNo;
            //                arInvoiceDetail.Description = item.Description;
            //                arInvoiceDetail.TaxType = item.TaxType;
            //                arInvoiceDetail.TaxRate = item.TaxRate;
            //       //         arInvoiceDetail.Tax = item.Tax;
            //                arInvoiceDetail.Amount = item.Amount;
            //            }
            //            arInvoiceDA.SaveARInvoice(_invoiceToAdd, SettingsMain.DefaultAutoCountID);
            //        }

            //    }


            //    return Ok();

            //}



            //catch (Exception ex)
            //{

            //    SettingsMain.MLogger.Error(ex.Message + ex.StackTrace);
            //    return InternalServerError(ex);
            //}

        }


        [Route("arap/arinvoicesql")]
        [HttpPost]
        public IHttpActionResult arinvoicesql([FromBody] JToken myData)
        {



            try
            {
                var toPost = myData.ToObject<Dictionary<string, object>>();

                using (FbConnection cnn = new FbConnection(SettingsMain.OstendoCnn.ConnectionString))
                {
                    List<arinvoice> invoice = SqlMapper.Query<arinvoice>(cnn, toPost["sql"].ToString()).ToList();


                    if (invoice.Count() > 0)
                    {

                        List<string> disInv = invoice.Select(c => c.DocNo).Distinct().ToList();
                        foreach (var dis in disInv)
                        {
                            List<arinvoice> toProcess = invoice.Where(c => c.DocNo == dis).ToList();
                            ARInvoiceDataAccess arInvoiceDA = ARInvoiceDataAccess.Create(SettingsMain.MyDbSession, SettingsMain.MydbSetting);
                            ARInvoiceEntity _invoiceToAdd = arInvoiceDA.NewARInvoice();
                            arinvoice _srcInvoice = toProcess.First();
                            _invoiceToAdd.DocNo = _srcInvoice.DocNo;
                            _invoiceToAdd.DebtorCode = _srcInvoice.DebtorCode;
                            _invoiceToAdd.DisplayTerm = _srcInvoice.DisplayTerm;
                            _invoiceToAdd.DocDate = _srcInvoice.DocDate;

                            if (!string.IsNullOrWhiteSpace(_srcInvoice.CURRENCYCODE))
                            {
                                _invoiceToAdd.CurrencyCode = _srcInvoice.CURRENCYCODE;
                                _invoiceToAdd.CurrencyRate = _srcInvoice.EXCHANGERATE;
                            }
                            if (!string.IsNullOrWhiteSpace(_srcInvoice.SalesAgent))
                            {
                                _invoiceToAdd.SalesAgent = _srcInvoice.SalesAgent;
                            }
                            else
                            {
                                _invoiceToAdd.SalesAgent = null;
                            }



                            _invoiceToAdd.JournalType = _srcInvoice.JournalType;
                            _invoiceToAdd.RefNo2 = _srcInvoice.BATCHFILENO;
                            _invoiceToAdd.Description = "Batch No:" + _srcInvoice.BATCHFILENO;

                            foreach (var item in toProcess)
                            {
                                ARInvoiceDTLEntity arInvoiceDetail = _invoiceToAdd.NewDetail();
                                arInvoiceDetail.AccNo = item.AccNo;
                                arInvoiceDetail.Description = item.Description;
                                arInvoiceDetail.TaxCode = item.TaxType;
                                arInvoiceDetail.TaxRate = item.TaxRate;
                                //         arInvoiceDetail.Tax = item.Tax;
                                arInvoiceDetail.Amount = item.Amount;
                            }
                            arInvoiceDA.SaveARInvoice(_invoiceToAdd, SettingsMain.DefaultAutoCountID);
                        }

                    }


                    return Ok();

                }

            }

            catch (Exception ex)
            {

                SettingsMain.MLogger.Error(ex.Message + ex.StackTrace);
                return InternalServerError(ex);
            }

        }


        [Route("arap/arinvoice")]
        [HttpPost]
        public IHttpActionResult arinvoice(arinvoice[] invoice)
        {
            try
            {
                if (invoice.Count() > 0)
                {
                    if (!SettingsMain.MyDbSession.IsLogin)
                    {
                        SettingsMain.MyDbSession.Login(SettingsMain.DefaultAutoCountID, SettingsMain.DefaultAutoCountPWD);
                    }

                    List<string> disInv = invoice.Select(c => c.DocNo).Distinct().ToList();
                    foreach (var dis in disInv)
                    {
                        List<arinvoice> toProcess = invoice.Where(c => c.DocNo == dis).ToList();
                        ARInvoiceDataAccess arInvoiceDA = ARInvoiceDataAccess.Create(SettingsMain.MyDbSession, SettingsMain.MydbSetting);
                        ARInvoiceEntity _invoiceToAdd = arInvoiceDA.NewARInvoice();
                        arinvoice _srcInvoice = toProcess.First();
                        _invoiceToAdd.DocNo = _srcInvoice.DocNo;
                        _invoiceToAdd.DebtorCode = _srcInvoice.DebtorCode;
                        _invoiceToAdd.DisplayTerm = _srcInvoice.DisplayTerm;
                        _invoiceToAdd.DocDate = _srcInvoice.DocDate;

                        if (!string.IsNullOrWhiteSpace(_srcInvoice.CURRENCYCODE))
                        {
                            _invoiceToAdd.CurrencyCode = _srcInvoice.CURRENCYCODE;
                            _invoiceToAdd.CurrencyRate = _srcInvoice.EXCHANGERATE;
                        }
                        if (!string.IsNullOrWhiteSpace(_srcInvoice.SalesAgent))
                        {
                            _invoiceToAdd.SalesAgent = _srcInvoice.SalesAgent;
                        }
                        else
                        {
                            _invoiceToAdd.SalesAgent = null;
                        }



                        _invoiceToAdd.JournalType = _srcInvoice.JournalType;
                        _invoiceToAdd.RefNo2 = _srcInvoice.BATCHFILENO;
                        //_invoiceToAdd.Description = "Batch No:" + _srcInvoice.BATCHFILENO;
                        _invoiceToAdd.Description = "Invoice";

                        foreach (var item in toProcess)
                        {
                            ARInvoiceDTLEntity arInvoiceDetail = _invoiceToAdd.NewDetail();
                    
                            arInvoiceDetail.AccNo = item.AccNo;
                            arInvoiceDetail.ProjNo = item.ProjNo;
                            arInvoiceDetail.Description = item.Description;
                            arInvoiceDetail.TaxCode = item.TaxType;
                            arInvoiceDetail.TaxRate = item.TaxRate;
                            //         arInvoiceDetail.Tax = item.Tax;
                            arInvoiceDetail.Amount = item.Amount;
                        }
                        arInvoiceDA.SaveARInvoice(_invoiceToAdd, SettingsMain.DefaultAutoCountID);
                    }

                }


                return Ok();

            }



            catch (Exception ex)
            {

                SettingsMain.MLogger.Error(ex.Message + ex.StackTrace);
                return InternalServerError(ex);
            }

        }


        [Route("arap/arcn")]
        [HttpPost]
        public IHttpActionResult arcn(arinvoice[] invoice)
        {
            try
            {
                if (invoice.Count() > 0)
                {
                    if (!SettingsMain.MyDbSession.IsLogin)
                    {
                        SettingsMain.MyDbSession.Login(SettingsMain.DefaultAutoCountID, SettingsMain.DefaultAutoCountPWD);
                    }
                    List<string> disInv = invoice.Select(c => c.DocNo).Distinct().ToList();

                    foreach (var dis in disInv)
                    {
                        List<arinvoice> toProcess = invoice.Where(c => c.DocNo == dis).ToList();
                        ARCNDataAccess arCnDa = ARCNDataAccess.Create(SettingsMain.MyDbSession, SettingsMain.MydbSetting);
                       
                        ARCNEntity _invoiceToAdd = arCnDa.NewARCN();
                        arinvoice _srcInvoice = toProcess.First();
                        _invoiceToAdd.DocNo = _srcInvoice.DocNo;
                        _invoiceToAdd.DebtorCode = _srcInvoice.DebtorCode;
                      //  _invoiceToAdd. = _srcInvoice.DisplayTerm;
                        _invoiceToAdd.DocDate = _srcInvoice.DocDate;
                      //  _invoiceToAdd.SalesAgent = _srcInvoice.SalesAgent;
                        _invoiceToAdd.JournalType = _srcInvoice.JournalType;
                        _invoiceToAdd.RefNo2 = _srcInvoice.BATCHFILENO;
                        //_invoiceToAdd.Description = "Batch No:" + _srcInvoice.BATCHFILENO;
                        _invoiceToAdd.Description = "Credit Note";
                        if (!string.IsNullOrWhiteSpace(_srcInvoice.CURRENCYCODE))
                        {
                            _invoiceToAdd.CurrencyCode = _srcInvoice.CURRENCYCODE;
                            _invoiceToAdd.CurrencyRate = _srcInvoice.EXCHANGERATE;
                        }
                        foreach (var item in toProcess)
                        {
                            ARCNDTLEntity arInvoiceDetail = _invoiceToAdd.NewDetail();
                            arInvoiceDetail.AccNo = item.AccNo;
                            arInvoiceDetail.Description = item.Description;
                            arInvoiceDetail.ProjNo = item.ProjNo;
                            arInvoiceDetail.TaxCode = item.TaxType;
                            arInvoiceDetail.TaxRate = item.TaxRate;
                   //         arInvoiceDetail.Tax = item.Tax;
                            arInvoiceDetail.Amount = item.Amount;
                        }

                        try
                        {
           
                            arCnDa.SaveARCN(_invoiceToAdd, UserSession.CurrentUserSession.LoginUserID);
                        }
                        catch (Exception ex)
                        {

                            return InternalServerError(ex);
                        }
                        finally
                        {
                    
                        }
                     
                    }

                }


                return Ok();

            }



            catch (Exception ex)
            {

                SettingsMain.MLogger.Error(ex.Message + ex.StackTrace);
                return InternalServerError(ex);
            }

        }
    }
}
    

