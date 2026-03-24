
using AutoCount.Authentication;
using AutoCount.GL.JournalEntry;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatchSBSAccount
{
    class Program
    {
        static void Main(string[] args)
        {

          
            if (args.Count() > 0)
            {
                if (args[0] == "Check")
                {
                    Check();
                }
                if (args[0] == "Add")
                {
                    InsertDocument(args[1]);
                }
                if (args[0] == "Insert")
                {
                    Console.WriteLine("Inserting");
                    try
                    {

                        var toList = SBSDocuments();
                        foreach (var item in toList)
                        {
                            Console.WriteLine("Inserting " + item);
                            InsertDocument(item);
                            Console.WriteLine("Inserting Done");
                        }

                    }
                    catch (Exception ex)
                    {

                        Console.WriteLine(ex.Message);
                    }
                }

                if (args[0] == "Delete")
                {
                    Console.WriteLine("Deleting");
                    try
                    {

                        var toDelete = SBSDocuments();
                        foreach (var item in toDelete)
                        {
                            Console.WriteLine("Deleting " + item);
                            DeleteDocument(item);
                            Console.WriteLine(item + " Deleted");
                        }
                    }
                    catch (Exception ex)
                    {

                        Console.WriteLine(ex.Message);
                    }
                }

                if (args[0] == "Cancel")
                {
                    Console.WriteLine("Cancel");
                    try
                    {

                        var toDelete = SBSDocuments();
                        foreach (var item in toDelete)
                        {
                            Console.WriteLine("Cancelling " + item);
                            CancelDocument(item);
                            Console.WriteLine(item + " Cancelled");
                        }
                    }
                    catch (Exception ex)
                    {

                        Console.WriteLine(ex.Message);
                    }
                }
            }
          

            Console.WriteLine("Done");
          
        }

       static void Check()
        {
            DataSet dt = new DataSet();
            SqlConnectionStringBuilder cmb = new SqlConnectionStringBuilder();
            cmb.UserID = ConfigurationManager.AppSettings["SQLDBUser"];
            cmb.Password = ConfigurationManager.AppSettings["SQLDBPWD"];
            cmb.DataSource = ConfigurationManager.AppSettings["SQLServer"];
            cmb.InitialCatalog = ConfigurationManager.AppSettings["SQLDBName"];


            SqlConnection cnn = new SqlConnection(cmb.ConnectionString);
       
       
            List<string> toUse = ConfigurationManager.AppSettings["MappingAccount"].Split(',').ToList<string>();
            foreach (var item in toUse)
            {
                List<string> toFind = item.Split('|').ToList<string>();

                SqlDataAdapter da = new SqlDataAdapter($"select * from GLDTL G where G.Accno = '{toFind[0]}' and G.RefNo2 not in (select k.RefNo2 from gldtl k where k.Accno = '{toFind[1]}')", cnn);
                da.Fill(dt);
                dt.WriteXml($"{toFind[0]}.log");
                dt.Clear();
            }
        }

        static List<string> SBSDocuments()
        {
            Console.WriteLine("Retrieving Documents");
            List<string> toReturn = new List<string>();
            SqlConnectionStringBuilder cmb = new SqlConnectionStringBuilder();
            cmb.UserID = ConfigurationManager.AppSettings["SQLDBUser"];
            cmb.Password = ConfigurationManager.AppSettings["SQLDBPWD"];
            cmb.DataSource = ConfigurationManager.AppSettings["SQLServer"];
            cmb.InitialCatalog = ConfigurationManager.AppSettings["SQLDBName"];


            SqlConnection cnn = new SqlConnection(cmb.ConnectionString);
            SqlDataAdapter da = new SqlDataAdapter("SELECT DISTINCT(G.RefNo1) FROM.[dbo].[GLDTL] G where G.AccNo in (SELECT AccNo FROM [GLMast] G where G.SpecialAccType = 'SBS') or G.DEAccNo in (SELECT AccNo FROM [GLMast] G where G.SpecialAccType ='SBS')", cnn);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if(dt.Rows.Count > 0)
            {
                foreach (DataRow item in dt.Rows)
                {
                    
                    toReturn.Add(item[0].ToString());
                }
            }
            return toReturn;
        
        }

        static string DeleteDocument(string docNumber)
        {
            try
            {
                if (!UserSession.CurrentUserSession.IsLogin)
                {
                    UserSession.CurrentUserSession.Login(SettingsMain.DefaultAutoCountID, SettingsMain.DefaultAutoCountPWD);
                }
                JournalEntryCommand cmd = JournalEntryCommand.Create(SettingsMain.MyDbSession, SettingsMain.MydbSetting);
                cmd.Delete(docNumber);
                return "Deleted " + docNumber;
            }
            catch (Exception ex)
            {

                return "Failed to Delete" + docNumber;
            }
        }

        static string CancelDocument(string docNumber)
        {
            try
            {
                if (!UserSession.CurrentUserSession.IsLogin)
                {
                    UserSession.CurrentUserSession.Login(SettingsMain.DefaultAutoCountID, SettingsMain.DefaultAutoCountPWD);
                }
                JournalEntryCommand cmd = JournalEntryCommand.Create(SettingsMain.MyDbSession, SettingsMain.MydbSetting);
                //cmd.CancelDocument(docNumber, ConfigurationManager.AppSettings["DefaultAutoCountID"]);
                cmd.CancelDocument(docNumber);
                return "Cancelled " + docNumber;
            }
            catch (Exception ex)
            {

                return "Failed to Cancel" + docNumber;
            }
       
        }

        static string InsertDocument(string docNumber)
        {
            //JV-000543
            DataTable dt = new DataTable();
            try
            {
                Console.WriteLine("Inserting GL " + docNumber);
                SqlConnectionStringBuilder cmb = new SqlConnectionStringBuilder();
                cmb.UserID = ConfigurationManager.AppSettings["SQLDBUser"];
                cmb.Password = ConfigurationManager.AppSettings["SQLDBPWD"];
                cmb.DataSource = ConfigurationManager.AppSettings["SQLServer"];
                cmb.InitialCatalog = ConfigurationManager.AppSettings["SQLDBName"];


                SqlConnection cnn = new SqlConnection(cmb.ConnectionString);
                SqlDataAdapter da = new SqlDataAdapter($"SELECT * FROM GLDTL G WHERE G.RefNo1 = '{docNumber}' order by G.GLDtlKey", cnn);
          
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    List<glTran> ToInsert = new List<glTran>();

                    //InsertGL(ToInsert.ToArray());
                    for (int i = 0; i < dt.Rows.Count; i += 2)
                    {
                        var item = dt.Rows[i];
                        //Console.WriteLine(item["AccNo"].ToString());

                        var toUse = replaceAccNo(item);
                        ToInsert.Add(new glTran { DocNo = item["RefNo1"].ToString(), DocNo2 = item["RefNo1"].ToString(), Description = item["Description"].ToString(), DocDate = (System.DateTime)item["TransDate"], DebitAcc = (decimal)item["HomeCR"] == 0 ? item["AccNo"].ToString() : item["DEAccNo"].ToString(), FINANCIALCOST = (decimal)item["HomeDR"] == 0 ? (decimal)item["HomeCR"] : (decimal)item["HomeDR"], CreditAcc = (decimal)item["HomeCR"] == 0 ? item["DEAccNo"].ToString() : item["AccNo"].ToString(), UNIQUEID = int.Parse(item["RefNo2"].ToString()) });

                    }
                    InsertGL(ToInsert.ToArray());

                }

                return dt.Rows.Count.ToString();
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
                return dt.Rows.Count.ToString();
            }
          

        }

       static DataRow replaceAccNo( DataRow toReplace)
        {
            DataRow toReturn = toReplace;
            List<string> toUse = ConfigurationManager.AppSettings["MappingAccount"].Split(',').ToList<string>();
            foreach (var item in toUse)
            {
                List<string> toFind = item.Split('|').ToList<string>();
                if(toReturn["AccNo"].ToString() == toFind[0])
                {
                    toReturn["AccNo"] = toFind[1];
                }
                if (toReturn["DEAccNo"].ToString() == toFind[0])
                {
                    toReturn["DEAccNo"] = toFind[1];
                }
            }
            return toReturn;

        }

        static string InsertGL(glTran[] trans)
        {
            try
            {
                if (trans.Count() > 0)
                {
                    if (!UserSession.CurrentUserSession.IsLogin)
                    {
                        UserSession.CurrentUserSession.Login(SettingsMain.DefaultAutoCountID, SettingsMain.DefaultAutoCountPWD);
                    }
                    JournalEntryCommand cmd = JournalEntryCommand.Create(SettingsMain.MyDbSession, SettingsMain.MydbSetting);
                    JournalEntry doc = cmd.AddNew();

                    doc.Description = trans[0].Description;
                    doc.DocDate = trans[0].DocDate;
                    doc.DocNo2 = trans[0].DocNo;
                    doc.JournalType = "GENERAL";

                    foreach (var item in trans)
                    {
                        JournalEntryDetail detail = doc.AddDetail();
                        detail.AccNo = item.DebitAcc;
                        detail.DR = item.FINANCIALCOST;
                        detail.RefNo2 = item.UNIQUEID.ToString();
                        detail = doc.AddDetail();
                        detail.AccNo = item.CreditAcc;
                        detail.CR = item.FINANCIALCOST;
                        detail.RefNo2 = item.UNIQUEID.ToString();

                    }

                    doc.Save();
                }

                return "OK";

            }

            catch (Exception ex)
            {

                return ex.ToString();
            }
            finally
            {
                UserSession.CurrentUserSession.Logout();
            }
        }
    }
}
