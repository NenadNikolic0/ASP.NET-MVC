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
using System.Text;

namespace Pdf_project.Controllers
{
    public class DashboardController : Controller
    {
        // GET: Dashboard
        public ActionResult Index()
        {
            //Check if user is logged, if is open dashboard
            if (Convert.ToInt32(Session["isLoggedIn"]) == 1 && Session["userZip"].ToString().Trim() == Request["zip"].ToString().Trim() && Session["serialNo"].ToString().Trim() == Request["serialno"].ToString().Trim())
            {
                ViewBag.Zip = Request["zip"].ToString().Trim();
                ViewBag.Email = Request["email"].ToString().Trim();

                //Get data from db for current user 
                DSGVOEntities db = new DSGVOEntities();
                string zip = Request["zip"].ToString().Trim();
                string serialno = Request["serialno"].ToString().Trim();

                kunden CurrentUser = db.kundens.Where(t => t.plz == zip && t.seriennr.Substring(15).Trim() == serialno).First();


                //Adding user data to viewbag
                if (CurrentUser.name1 != null)
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


                //Set  serial no 
                ViewBag.SerialNo = Session["serialNo"].ToString().Trim();

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


            kunden CurrentUser = db.kundens.Where(t => t.plz == details.UserZip.ToString().Trim() && t.seriennr.Substring(15).Trim() == details.SerialNo.ToString().Trim()).First();

            ////Update contractuser in db 
            //CurrentUser.contractuser = details.ContractUser.ToString().Trim();

            ////Change email if it is changed 
            //if (CurrentUser.email.ToString().Trim() != details.Email.ToString().Trim())
            //{
            //    CurrentUser.email = details.Email.ToString().Trim();
            //}

            //db.SaveChanges();


            //Check if changes in data exist and send it in email 
            StringBuilder dataChanges = new StringBuilder();

            bool ChangesExist = false;

            dataChanges.Append("<table>");
            dataChanges.Append(" <tr><th>Previous data</th><th>Changed data from user</th></tr>");


            if (CurrentUser.name1.ToString().Trim() != details.Name1.ToString().Trim())
            {
                ChangesExist = true;
                dataChanges.Append(" <tr><td>" + CurrentUser.name1.ToString().Trim() + " </td><td>" + details.Name1.ToString().Trim() + "</td></tr>");

            }

            if (CurrentUser.name2.ToString().Trim() != details.Name2.ToString().Trim())
            {
                ChangesExist = true;
                dataChanges.Append(" <tr><td>" + CurrentUser.name2.ToString().Trim() + " </td><td>" + details.Name2.ToString().Trim() + "</td></tr>");

            }

            if (CurrentUser.strasse.ToString().Trim() != details.Street.ToString().Trim())
            {
                ChangesExist = true;
                dataChanges.Append(" <tr><td>" + CurrentUser.strasse.ToString().Trim() + " </td><td>" + details.Street.ToString().Trim() + "</td></tr>");

            }

            if (CurrentUser.plz.ToString().Trim() != details.Zip.ToString().Trim())
            {
                ChangesExist = true;
                dataChanges.Append(" <tr><td>" + CurrentUser.plz.ToString().Trim() + " </td><td>" + details.Zip.ToString().Trim() + "</td></tr>");

            }

            if (CurrentUser.ort.ToString().Trim() != details.City.ToString().Trim())
            {
                ChangesExist = true;
                dataChanges.Append(" <tr><td>" + CurrentUser.ort.ToString().Trim() + " </td><td>" + details.City.ToString().Trim() + "</td></tr>");

            }

            if (CurrentUser.land.ToString().Trim() != details.Country.ToString().Trim())
            {
                ChangesExist = true;
                dataChanges.Append(" <tr><td>" + CurrentUser.land.ToString().Trim() + " </td><td>" + details.Country.ToString().Trim() + "</td></tr>");

            }

            if (CurrentUser.email.ToString().Trim() != details.Email.ToString().Trim())
            {
                ChangesExist = true;
                dataChanges.Append(" <tr><td>" + CurrentUser.email.ToString().Trim() + " </td><td>" + details.Email.ToString().Trim() + "</td></tr>");

            }

            if (CurrentUser.contractuser == null)
            {
                ChangesExist = true;
                dataChanges.Append(" <tr><td>Empty</td><td>" + details.ContractUser.ToString().Trim() + "</td></tr>");

            }





            ////Adding user data to viewbag
            //if (CurrentUser.name1 != null)
            //{
            //    details.Name1 = CurrentUser.name1.ToString().Trim();
            //}

            //if (CurrentUser.name2 != null)
            //{
            //    details.Name2 = CurrentUser.name2.ToString().Trim();
            //}

            //if (CurrentUser.strasse != null)
            //{
            //    details.Street= CurrentUser.strasse.ToString().Trim() + " " ;
            //}



            //if (CurrentUser.ort != null)
            //{
            //    details.City= CurrentUser.ort.ToString().Trim();
            //}

            //if (CurrentUser.land != null)
            //{
            //    //Set country 
            //    switch (CurrentUser.land.ToString().Trim())
            //    {
            //        case "D":
            //            CurrentUser.land = "Deutschland";
            //            break;
            //        case "A":
            //            CurrentUser.land = "Österreich";
            //            break;
            //        case "CH":
            //            CurrentUser.land = "Schweiz";
            //            break;
            //        default:
            //            break;
            //    }


            //    details.Country = CurrentUser.land.ToString().Trim();
            //}

            //if (CurrentUser.email != null)
            //{
            //    details.Contact = CurrentUser.email.ToString().Trim();
            //}


            // Set country
            switch (details.Country.ToString().Trim())
            {
                case "D":
                    details.Country = "Deutschland";
                    break;
                case "A":
                    details.Country = "Österreich";
                    break;
                case "CH":
                    details.Country = "Schweiz";
                    break;
                default:
                    break;
            }


            string Result, Name;

            //Generating strong name as word and pdf name
            string HashName = UserInfo.CalculateMD5Hash(details.SerialNo + "-" + details.UserZip);


            try
            {
                Spire.Doc.Document document = new Spire.Doc.Document();

                document.LoadFromFile(Server.MapPath("~/Template/template.docx").ToString());


                if (details.Name1 != null && details.Name1.ToString().Trim() != "")
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

                if (details.ContractUser != null && details.ContractUser.ToString().Trim() != "")
                {
                    document.Replace("##ContractUser##", details.ContractUser, false, true);
                }


                if (details.Email != null && details.Email.ToString().Trim() != "")
                {
                    document.Replace("##AGCONTACT##", details.Email, false, true);
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
                Microsoft.Office.Interop.Word._Document wDoc = wDocs.Open(Server.MapPath("~/Word/").ToString() + HashName + ".docx", ReadOnly: false, Visible: false);
                wDoc.Activate();

                string pdfName = @Server.MapPath("~/Pdf/").ToString() + HashName + ".pdf";

                wDoc.ExportAsFixedFormat(OutputFileName: pdfName, ExportFormat: WdExportFormat.wdExportFormatPDF, UseISO19005_1: true);

                wDoc.Close();


                //Send email with pdf as attachemnt 
                SmtpClient smtpClient = new SmtpClient();
                NetworkCredential basicCredential =
                    new NetworkCredential("dsgvo@hope-software.com", "hopeDSGVO");
                System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();
                MailAddress fromAddress = new MailAddress("dsgvo@hope-software.com");

                smtpClient.Host = "smtp.1und1.de";
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = basicCredential;

                smtpClient.Port = 587;
                smtpClient.EnableSsl = true;

                message.From = fromAddress;
                message.Subject = "hope-DSGVO - AV-Vertrag, Kunde: " + details.UserZip + " - hotel: " + details.Name1;

                message.Attachments.Add(new System.Net.Mail.Attachment(Server.MapPath("~/Pdf/").ToString() + HashName + ".pdf"));

                //office@hope-software.com

                message.To.Add("office@hope-software.com");

                smtpClient.Send(message);


                //Send email if user changed data on html form info@hope-software.com

                if (ChangesExist)
                {
                    message.From = fromAddress;
                    message.Subject = "hope-DSGVO - Kundendaten geändert: " + details.UserZip + " - hotel: " + details.Name1;


                    message.IsBodyHtml = true;

                    message.Body = dataChanges.ToString();
                    //office@hope-software.com

                    message.To.Add("office@hope-software.com");

                    smtpClient.Send(message);
                }



                //Call webservice action 






                return Json(new { Result = "true", Name = HashName });


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
            string fileName = Name + ".pdf";
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }


    }
}