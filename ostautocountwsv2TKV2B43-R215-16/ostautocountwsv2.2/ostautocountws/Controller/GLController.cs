
using AutoCount.Authentication;
using AutoCount.GL.JournalEntry;
using ostautocountws.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;


namespace ostautocountws.Controller
{
    public class GLController : ApiController
    {
        [Route("gl/trans")]
        [HttpPost]
        public IHttpActionResult FglTrans(glTran[] trans)
        {
            try
            {
                if (trans.Count() > 0)
                {

                    if (!SettingsMain.MyDbSession.IsLogin)
                    {
                        SettingsMain.MyDbSession.Login(SettingsMain.DefaultAutoCountID, SettingsMain.DefaultAutoCountPWD);
                    }
                    JournalEntryCommand cmd = JournalEntryCommand.Create(SettingsMain.MyDbSession, SettingsMain.MydbSetting);
                    
                    
                    //Because of posting by batch if the transaction was using separate date it so we get distinct list of transaction date from the system

                    //separate by trans date
                    var listofDates = trans.Select(d => d.DocDate).Distinct();

                    foreach (var thedate in listofDates)
                    {
                        var toPost = trans.Where(c => c.DocDate == thedate.Date).ToList();

                        JournalEntry doc = cmd.AddNew();
                        
                        if(toPost[0].Description.StartsWith("FPRcpt Batch No:"))
                        {
                            doc.DocNo = toPost[0].DocNo;
                        }

                        doc.Description = toPost[0].Description;
                        doc.DocDate = toPost[0].DocDate;
                        doc.DocNo2 = toPost[0].DocNo;
                        doc.JournalType = "GENERAL";

                        foreach (var item in toPost)
                        {
                            JournalEntryDetail detail = doc.AddDetail();
                            detail.AccNo = item.DebitAcc;
                            detail.DR = item.FINANCIALCOST;
                            detail.RefNo2 = item.UNIQUEID.ToString();
                            detail.ProjNo = item.ProjNo;
                            detail = doc.AddDetail();
                            detail.AccNo = item.CreditAcc;
                            detail.CR = item.FINANCIALCOST;                     
                            detail.RefNo2 = item.UNIQUEID.ToString();
                            detail.ProjNo = item.ProjNo;

                        }

                        doc.Save();

                    }

                }

                return Ok();

            }

            catch (Exception ex)
            {
                
                SettingsMain.MLogger.Error(ex.Message + ex.StackTrace);
                return InternalServerError(ex);
            }
            finally
            {
                SettingsMain.MyDbSession.Logout();
            }

        }

       
    }
}
    

