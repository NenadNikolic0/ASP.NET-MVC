using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Pdf_project.Database;
using Pdf_project.Models;
using Microsoft.Office.Interop.Word;
using Spire;
using System.IO;
using System.Net.Mail;
using System.Net;

namespace Pdf_project.Controllers
{
    public class DashboardController : Controller
    {
        // GET: Dashboard
        public ActionResult Index()
        {
            //Check if user is logged, if is open dashboard
            if (Convert.ToInt32(Session["isLoggedIn"]) == 1 && Session["userZip"].ToString().Trim() == Request["zip"].ToString().Trim())
            {
                ViewBag.Zip = Request["zip"].ToString().Trim();
                ViewBag.Email = Request["email"].ToString().Trim();

                //Get data from db for current user 
                DSGVOEntities db = new DSGVOEntities();
                string zip = Request["zip"].ToString().Trim();

                kunden CurrentUser = db.kundens.Where(t => t.plz.Trim() == zip).First();

            
                //Adding user data to viewbag
                if (CurrentUser.name1!=null)
                {
                    ViewBag.AgName1 = CurrentUser.name1.ToString().Trim();
                }

                if (CurrentUser.name2 != null)
                {
                    ViewBag.AgName2 = CurrentUser.name2.ToString().Trim();
                }

                if (CurrentUser.strasse != null)
                {
                    ViewBag.Street = CurrentUser.strasse.ToString().Trim();
                }

                if (CurrentUser.plz != null)
                {
                    ViewBag.Zip = CurrentUser.plz.ToString().Trim();
                }

                if (CurrentUser.ort != null)
                {
                    ViewBag.City = CurrentUser.ort.ToString().Trim();
                }

                if (CurrentUser.land!= null)
                {
                    //Set country 
                    switch (CurrentUser.land.ToString().Trim())
                    {
                        case "D":
                            CurrentUser.land = "Deutschland";
                            break;
                        case "A":
                            CurrentUser.land = "Österreich";
                            break;
                        case "CH":
                            CurrentUser.land = "Schweiz";
                            break;
                        default:                          
                            break;
                    }





                    ViewBag.Country = CurrentUser.land.ToString().Trim();
                }

                if (CurrentUser.email != null)
                {
                    ViewBag.AgContact = CurrentUser.email.ToString().Trim();
                }

                //Generating strong name as word and pdf name
                string HashName = UserInfo.CalculateMD5Hash(Request["serialno"].ToString().Trim() + "-" + Request["zip"].ToString().Trim());

                string pdfFile = Server.MapPath("~/Pdf/").ToString() + HashName + ".pdf";

                if (System.IO.File.Exists(pdfFile))
                {
                    ViewBag.ExistingPdf = true;
                    ViewBag.PdfName = HashName;
                }

                else
                {
                    ViewBag.PdfName = "#";
                    ViewBag.ExistingPdf = false;
                }



                return View();
            }

            //If not open login page
            else
            {
                return RedirectToAction("index", "home");
            }
            
        }

        public ActionResult CreateDocumentsFromTemplate(ContractDetails details)
        {

            //Get data from db for current user 
            DSGVOEntities db = new DSGVOEntities();
            

            kunden CurrentUser = db.kundens.Where( t => t.plz == details.Zip.ToString().Trim() && t.seriennr.Substring(15).Trim() == details.SerialNo.ToString().Trim()).First();


            //Adding user data to viewbag
            if (CurrentUser.name1 != null)
            {
                details.Name1 = CurrentUser.name1.ToString().Trim();
            }

            if (CurrentUser.name2 != null)
            {
                details.Name2 = CurrentUser.name2.ToString().Trim();
            }

            if (CurrentUser.strasse != null)
            {
                details.Street= CurrentUser.strasse.ToString().Trim() + " " ;
            }

            

            if (CurrentUser.ort != null)
            {
                details.City= CurrentUser.ort.ToString().Trim();
            }

            if (CurrentUser.land != null)
            {
                //Set country 
                switch (CurrentUser.land.ToString().Trim())
                {
                    case "D":
                        CurrentUser.land = "Deutschland";
                        break;
                    case "A":
                        CurrentUser.land = "Österreich";
                        break;
                    case "CH":
                        CurrentUser.land = "Schweiz";
                        break;
                    default:
                        break;
                }


                details.Country = CurrentUser.land.ToString().Trim();
            }

            if (CurrentUser.email != null)
            {
                details.Contact = CurrentUser.email.ToString().Trim();
            }


            string Result, Name;

            //Generating strong name as word and pdf name
            string HashName = UserInfo.CalculateMD5Hash(details.SerialNo + "-" + details.Zip);


            try
            {
                Spire.Doc.Document document = new Spire.Doc.Document();
                
                document.LoadFromFile(Server.MapPath("~/Template/template.docx").ToString());

               
                if(details.Name1!=null && details.Name1.ToString().Trim() != "")
                {
                    document.Replace("##AGName1##", details.Name1, false, true);
                }

                else
                {
                    document.Replace("##AGName1##", "", false, true);
                }

                if (details.Name2 != null && details.Name2.ToString().Trim() != "")
                {
                    document.Replace("##AGName2##", details.Name2, false, true);
                }

                else
                {
                    document.Replace("##AGName2##", "", false, true);
                }

                if (details.Street != null && details.Street.ToString().Trim() != "")
                {
                    document.Replace("##AGStreet##", details.Street, false, true);
                }

                else
                {
                    document.Replace("##AGStreet##", "", false, true);
                }


                if (details.Zip != null && details.Zip.ToString().Trim() != "")
                {
                    document.Replace("##AGZIP##", details.Zip, false, true);
                }

                else
                {
                    document.Replace("##AGZIP##", "", false, true);
                }


                if (details.City != null && details.City.ToString().Trim() != "")
                {
                    document.Replace("##AGCITY##", details.City, false, true);
                    document.Replace("##City##", details.City, false, true);
                }

                else
                {
                    document.Replace("##AGCITY##", "", false, true);
                    document.Replace("##City##", "", false, true);
                }

                if (details.Country != null && details.Country.ToString().Trim() != "")
                {
                    document.Replace("##AGCountry##", details.Country, false, true);
                }

                else
                {
                    document.Replace("##AGCountry##", "", false, true);
                }

                if (details.Contact != null && details.Contact.ToString().Trim() != "")
                {
                    document.Replace("##AGCONTACT##", details.Contact, false, true);
                }

                else
                {
                    document.Replace("##AGCONTACT##", "", false, true);
                }


                document.Replace("##DayDate##", DateTime.Now.ToString("dd.MM.yyyy"), false, true);

                document.SaveToFile(Server.MapPath("~/Word/").ToString() + HashName + ".docx", Spire.Doc.FileFormat.Docx);


                //Code for making connection with existing word template 
                Microsoft.Office.Interop.Word._Application wApp = new Microsoft.Office.Interop.Word.Application();
                Microsoft.Office.Interop.Word.Documents wDocs = wApp.Documents;
                Microsoft.Office.Interop.Word._Document wDoc = wDocs.Open(Server.MapPath("~/Word/").ToString() + HashName + ".docx", ReadOnly: false, Visible:false);
                wDoc.Activate();

                string pdfName = @Server.MapPath("~/Pdf/").ToString() + HashName + ".pdf";

                wDoc.ExportAsFixedFormat(OutputFileName: pdfName, ExportFormat: WdExportFormat.wdExportFormatPDF, UseISO19005_1: true);

                wDoc.Close();


                ////Send email 
                //SmtpClient smtpClient = new SmtpClient();
                //NetworkCredential basicCredential =
                //    new NetworkCredential("dsgvo@hope-software.com", "hopeDSVGO");
                //System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();
                //MailAddress fromAddress = new MailAddress("office@hope-software.com");

                //smtpClient.Host = "smtp.1und1.de";
                //smtpClient.UseDefaultCredentials = false;
                //smtpClient.Credentials = basicCredential;

                //smtpClient.Port = 587;
                //smtpClient.EnableSsl = true;

                //message.From = fromAddress;
                //message.Subject = "your subject";
                ////Set IsBodyHtml to true means you can send HTML email.
                //message.IsBodyHtml = true;
                //message.Body = "<h1>your message body</h1>";
                //message.To.Add("nikolic_n@hotmail.com");

                //smtpClient.Send(message);


                //Call webservice action 






                return Json( new { Result = "true", Name = HashName });


            }

            catch (Exception ex)
            {
                Console.WriteLine("Error occured:", ex.ToString());
                return Json(Result = ex.ToString());
            }

            finally
            {
                Dispose();
            }

        }

        public FileResult DownloadPdf()
        {
            string Name = Request["name"];
            byte[] fileBytes = System.IO.File.ReadAllBytes(Server.MapPath("~/Pdf/").ToString() + Name + ".pdf");
            string fileName = Name +".pdf";
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }


    }
}