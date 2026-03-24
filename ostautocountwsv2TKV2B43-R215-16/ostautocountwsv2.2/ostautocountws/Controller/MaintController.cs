
using AutoCount.ARAP.Creditor;
using AutoCount.GL;
using Dapper;
using Encrypt_a_string;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ostautocountws.Model;
using ostautocountws.Model.sqlacc;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.OData;

namespace ostautocountws
{
    public class MaintController : ApiController
    {


        [Route("Maint/license")]
        [HttpPost]
        public IHttpActionResult check(licenseMaint license)
        {
            var filePath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "AutocountLink.lic";
            if (!File.Exists(filePath))
            {
                return BadRequest("License Not Found");
            }
            try
            {
                Encrypt_a_string.EncryptionAlgorithm alg = (Encrypt_a_string.EncryptionAlgorithm)3;
                byte[] IV = Convert.FromBase64String(license.ProductKey);
                var LicenseFile = File.ReadAllText(filePath);
                Decryptor dec = new Decryptor(alg, IV);
                var decryptedData = dec.Decrypt(LicenseFile, "accstreamelink2u");
                license.EncryptedData = LicenseFile;
                license.DecryptedData = decryptedData;
                return Ok(license);
            }
            catch (Exception ex)
            {

                SettingsMain.MLogger.Error(ex.Message + ex.StackTrace);
                return InternalServerError(ex);
            }

        }


        public string GetNewDebtorCode(AutoCount.Authentication.UserSession userSession, string controlAccNo, string companyName)
        {
            try
            {
                return AutoCount.GL.AccountCodeHelper.Create(userSession.DBSetting)
                    .GetNextDebtorCode(controlAccNo, companyName);
            }
            catch (AutoCount.GL.InvalidAutoDebtorCodeFormatException ex)
            {
                //Log error ex.Message;
            }
            catch (AutoCount.Data.DataAccessException ex)
            {
                //Log error ex.Message;
            }

            //If the catch throw out exception, then return null is not necessary.
            return null;
        }

        [Route("Maint/debtor")]
        [HttpPost]
        public IHttpActionResult debtor(debtorMaint[] debtor)
        {
            try
            {



               
                bool isInserting = false;
                string newDebtorAccountNo = string.Empty;
                string CompanyName = string.Empty;
                AutoCount.ARAP.Debtor.DebtorDataAccess debtorDA;
                string userId = SettingsMain.MyDbSession.LoginUserID;
                using (SqlConnection cnn = new SqlConnection(SettingsMain.MydbSetting.ConnectionString))
                {
                    foreach (var item in debtor)
                    {

                        AutoCount.ARAP.Debtor.DebtorEntity toAddOrUpdate;
                        //if (item.INSERTEDORUPDATED == "Inserted")
                        //{

                  
                        var myDebtor = cnn.Query<debtorMaint>("select * from debtor where AccNo = @AccNo", new { AccNo = item.AccNo });
                  
                        if (myDebtor.Count() > 0)
                            {
                            debtorDA = AutoCount.ARAP.Debtor.DebtorDataAccess.Create(SettingsMain.MyDbSession, SettingsMain.MydbSetting);
                            toAddOrUpdate = debtorDA.GetDebtor(item.AccNo);
                            }
                            else
                             {
                      
                             newDebtorAccountNo = GetNewDebtorCode(SettingsMain.MyDbSession, item.ControlAccount, item.CompanyName);
                             debtorDA = AutoCount.ARAP.Debtor.DebtorDataAccess.Create(SettingsMain.MyDbSession, SettingsMain.MydbSetting);
                         
                            toAddOrUpdate = debtorDA.NewDebtor();
                            //AutoCount.GL.AccountCodeHelper.Create(SettingsMain.MydbSetting).GetNextDebtorCode(item.ControlAccount, item.CompanyName);

                            toAddOrUpdate.AccNo = newDebtorAccountNo;


                                isInserting = true;
                                CompanyName = item.CompanyName;
                                

                            }
                        //}
                        //else
                        //{
                        //    toAddOrUpdate = debtorDA.GetDebtor(item.AccNo);
                        //}

                        //    toAddOrUpdate.AccNo = item.AccNo;
                        toAddOrUpdate.ControlAccount = item.ControlAccount;
                        toAddOrUpdate.CompanyName = item.CompanyName;
                        toAddOrUpdate.Address1 = item.Address1.Length > 40 ? item.Address1.Substring(0,39):item.Address1;
                        toAddOrUpdate.Address2 = item.Address2.Length > 40 ? item.Address2.Substring(0, 39) : item.Address2;
                        toAddOrUpdate.Address3 = item.Address3.Length > 40 ? item.Address3.Substring(0, 39) : item.Address3;
                        toAddOrUpdate.Address4 = item.Address4.Length > 40 ? item.Address4.Substring(0, 39) : item.Address4;
                        toAddOrUpdate.PostCode = item.PostCode;
                        toAddOrUpdate.Phone1 = item.Phone1;
                        toAddOrUpdate.Fax1 = item.Fax1;
                        if (item.FOREIGNCURRENCY)
                        {
                   //         toAddOrUpdate.CurrencyCode = item.CurrencyCode;
                        }
                        if (!string.IsNullOrEmpty(item.AreaCode))
                        {
                            toAddOrUpdate.AreaCode = item.AreaCode;
                        }
                        else
                        {
                            toAddOrUpdate.AreaCode = null;
                        }
                        if (!string.IsNullOrEmpty(item.SalesAgent))
                        {
                            toAddOrUpdate.SalesAgent = item.SalesAgent;
                        }
                        else
                        {
                            toAddOrUpdate.SalesAgent = null;
                        }
                        toAddOrUpdate.WebURL = item.WebURL;
                        toAddOrUpdate.EmailAddress = item.EmailAddress;
                        toAddOrUpdate.DisplayTerm = item.DisplayTerm;
                        toAddOrUpdate.CreditLimit = item.CreditLimit;
                        toAddOrUpdate.CurrencyCode = item.CurrencyCode;
                        toAddOrUpdate.Note = item.Note;
                        //   toAddOrUpdate.TaxType = item.TaxType;
           
                        debtorDA.SaveDebtor(toAddOrUpdate, SettingsMain.DefaultAutoCountID);
                        

                    }

                }

                if (isInserting)
                {
                    return Ok(new { Message = "0", AccNo = newDebtorAccountNo, CompanyName = CompanyName });
                }

                else
                {
                    return Ok(new { Message = "0", AccNo = "" });
                }
            }
            catch (Exception ex)
            {

                SettingsMain.MLogger.Error(ex.Message + ex.StackTrace);
                return InternalServerError(ex);
            }

        }



        [Route("Maint/creditor")]
        [HttpPost]
        public IHttpActionResult creditor(creditorMaint[] creditor)
        {
            try
            {
                CreditorDataAccess creditorDA = CreditorDataAccess.Create(SettingsMain.MyDbSession, SettingsMain.MydbSetting);
                bool isInserting = false;
                string newCreditorAccountNo = string.Empty;
                string CompanyName = string.Empty;
                using (SqlConnection cnn = new SqlConnection(SettingsMain.MydbSetting.ConnectionString))
                {
                    foreach (var item in creditor)
                    {
                        var myCreditor = cnn.Query<creditorMaint>("select * from creditor where AccNo = @AccNo", new { AccNo = item.AccNo });
                        CreditorEntity toAddOrUpdate;
                        if (item.INSERTEDORUPDATED == "Inserted")
                        {
                            if (myCreditor.Count() > 0)
                            {
                                toAddOrUpdate = creditorDA.GetCreditor(item.AccNo);
                            }
                            else
                            {
                                toAddOrUpdate = creditorDA.NewCreditor();
                                newCreditorAccountNo = AccountCodeHelper.Create(SettingsMain.MydbSetting).GetNextCreditorCode(item.ControlAccount, item.CompanyName);
                                toAddOrUpdate.AccNo = newCreditorAccountNo;
                                CompanyName = item.CompanyName;
                                isInserting = true;
                            }
                        }
                        else
                        {
                            toAddOrUpdate = creditorDA.GetCreditor(item.AccNo);
                        }


                        toAddOrUpdate.ControlAccount = item.ControlAccount;
                        toAddOrUpdate.CompanyName = item.CompanyName;
                        toAddOrUpdate.Address1 = item.Address1.Length > 40 ? item.Address1.Substring(0, 39) : item.Address1;
                        toAddOrUpdate.Address2 = item.Address2.Length > 40 ? item.Address2.Substring(0, 39) : item.Address2;
                        toAddOrUpdate.Address3 = item.Address3.Length > 40 ? item.Address3.Substring(0, 39) : item.Address3;
                        toAddOrUpdate.Address4 = item.Address4.Length > 40 ? item.Address4.Substring(0, 39) : item.Address4;
                        toAddOrUpdate.PostCode = item.PostCode;
                        toAddOrUpdate.Phone1 = item.Phone1;
                        toAddOrUpdate.Fax1 = item.Fax1;

                        if (!string.IsNullOrEmpty(item.AreaCode))
                        {
                            toAddOrUpdate.AreaCode = item.AreaCode;
                        }
                        else
                        {
                            toAddOrUpdate.AreaCode = null;
                        }


                        toAddOrUpdate.WebURL = item.WebURL;
                        toAddOrUpdate.EmailAddress = item.EmailAddress;
                        toAddOrUpdate.DisplayTerm = item.DisplayTerm;
                        toAddOrUpdate.CreditLimit = item.CreditLimit;
                        toAddOrUpdate.CurrencyCode = item.CurrencyCode;
                        toAddOrUpdate.Note = item.Note;
                        //   toAddOrUpdate.TaxType = item.TaxType;

                        creditorDA.SaveCreditor(toAddOrUpdate, SettingsMain.DefaultAutoCountID);

                    }

                }

                if (isInserting)
                {
                    return Ok(new { Message = "0", AccNo = newCreditorAccountNo, CompanyName = CompanyName });
                }
                else
                {
                    return Ok(new { Message = "0", AccNo = "" });
                }

            }
            catch (Exception ex)
            {

                SettingsMain.MLogger.Error(ex.Message + ex.StackTrace);
                return InternalServerError(ex);
            }

        }

        [Route("Maint/area")]
        [HttpPost]
        public IHttpActionResult area(areaMaint[] area)
        {
            try
            {
                using (SqlConnection cnn = new SqlConnection(SettingsMain.MydbSetting.ConnectionString))
                {
                    foreach (var item in area)
                    {
                        var myAgetn = cnn.Query<areaMaint>("select * from area where AreaCode = @AreaCode", new { AreaCode = item.AreaCode });
                        if (myAgetn.Count() > 0)
                        {
                            cnn.Execute(@"Update Area Set Description=@Description,LastUpdate = @LastUpdate where AreaCode=@AreaCode", new areaMaint { Description = item.Description, LastUpdate = myAgetn.First().LastUpdate + 1 });
                        }
                        else
                        {
                            cnn.Execute(@"Insert Area(AreaCode,Description,LastUpdate,Guid) values (@AreaCode, @Description,@LastUpdate,@Guid)", new areaMaint { AreaCode = item.AreaCode, Description = item.Description, LastUpdate = 1,Guid=Guid.NewGuid() });
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



        [Route("Maint/agent")]
        [HttpPost]
        public IHttpActionResult agent(agentMaint[] agent)
        {
            try
            {
                using (SqlConnection cnn = new SqlConnection(SettingsMain.MydbSetting.ConnectionString))
                {
                    foreach (var item in agent)
                    {
                        var myAgetn = cnn.Query<agentMaint>("select * from SalesAgent where SalesAgent = @SalesAgent", new { SalesAgent = item.SalesAgent });
                        if (myAgetn.Count() > 0)
                        {
                            //cnn.Execute(@"Update SalesAgent Set Description=@Description,IsActive=@IsActive,LastUpdate = @LastUpdate where SalesAgent=@SalesAgent", new agentMaint {SalesAgent=item.SalesAgent, Description = item.Description, IsActive = item.IsActive, LastUpdate = myAgetn.First().LastUpdate + 1 });
                        }
                        else
                        {
                            cnn.Execute(@"Insert SalesAgent(SalesAgent,Description,IsActive,LastUpdate,Guid) values (@SalesAgent, @Description,@IsActive,@LastUpdate,@Guid)", new agentMaint { SalesAgent = item.SalesAgent, Description = item.Description, IsActive = item.IsActive, LastUpdate = 1,Guid = Guid.NewGuid() });
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

        [Route("Maint/terms")]
        [HttpPost]
        public IHttpActionResult terms(termMaint[] terms)
        {
            try
            {
                using (SqlConnection cnn = new SqlConnection(SettingsMain.MydbSetting.ConnectionString))
                {
                    foreach (var item in terms)
                    {
                        var myAgetn = cnn.Query<termMaint>("select * from terms where DisplayTerm = @DisplayTerm", new { DisplayTerm = item.DisplayTerm });
                        if (myAgetn.Count() > 0)
                        {
                            //     cnn.Execute(@"Update Terms Set Description=@Description,LastUpdate = @LastUpdate where AreaCode=@AreaCode", new areaMaint { Description = new String(item.Description), LastUpdate = myAgetn.First().LastUpdate + 1 });
                        }
                        else
                        {
                            //
                            int TermDays = item.TermDays;
                            string TermDaysFrom = item.TermDaysFrom;
                            string TermsToCreate = string.Empty;

                            if (TermDays > 0)
                            {
                                switch (TermDaysFrom)
                                {
                                    case "Invoice Date":
                                        {
                                            TermsToCreate = string.Format("Net {0} days", TermDays);
                                            break;
                                        }
                                    case "End of Month":
                                        {
                                            TermsToCreate = string.Format("Net {0}th Next 1 Month", TermDays);
                                            break;
                                        }
                                    case "End of Next Month":
                                        {
                                            TermsToCreate = string.Format("Net {0}th Next 2 Month", TermDays);
                                            break;
                                        }
                                    default:
                                        TermsToCreate = "C.O.D.";
                                        break;
                                }
                            }
                            else
                            {
                                TermsToCreate = "Cash";
                            }
                            cnn.Execute(@"Insert Terms(DisplayTerm,Terms,LastUpdate) values (@DisplayTerm,@Terms,@LastUpdate)", new termMaint { DisplayTerm = item.DisplayTerm, Terms = TermsToCreate, LastUpdate = 1 });
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

        [Route("Maint/tax")]
        [HttpGet]
        [EnableQuery]
        public IHttpActionResult tax()
        {
            try
            {
                using (SqlConnection cnn = new SqlConnection(SettingsMain.MydbSetting.ConnectionString))
                {
                    List<taxMaint> resulst = SqlMapper.Query<taxMaint>(cnn, "select taxcode,  cast(description as char(50)) as description, taxrate from taxcode").ToList();
                    return Ok(resulst);
                }
            }
            catch (Exception ex)
            {

                SettingsMain.MLogger.Error(ex.Message + ex.StackTrace);
                return InternalServerError(ex);
            }
        }

        [Route("Maint/currency")]
        [HttpGet]
        [EnableQuery]
        public IHttpActionResult currency()
        {
            try
            {
                using (SqlConnection cnn = new SqlConnection(SettingsMain.MydbSetting.ConnectionString))
                {
                    List<currencyMaint> resulst = SqlMapper.Query<currencyMaint>(cnn, "select CurrencyCode, CurrencyWord, CurrencySymbol from CURRENCY").ToList();
                    return Ok(resulst);
                }
            }
            catch (Exception ex)
            {

                SettingsMain.MLogger.Error(ex.Message + ex.StackTrace);
                return InternalServerError(ex);
            }
        }

        [Route("Maint/gl")]
        [HttpGet]
        [EnableQuery]
        public IHttpActionResult gl()
        {
            try
            {
                using (SqlConnection cnn = new SqlConnection(SettingsMain.MydbSetting.ConnectionString))
                {
                    List<glMast> resulst = SqlMapper.Query<glMast>(cnn, "select AccNo, Description, Coalesce(SpecialAccType, '') SpecialAccType from glmast where coalesce(SpecialAccType,'') not in ('SDR','SCR')").ToList();
                    return Ok(resulst);
                }
            }
            catch (Exception ex)
            {
                SettingsMain.MLogger.Error(ex.Message + ex.StackTrace);

                return InternalServerError(ex);
            }


        }
    }
}
