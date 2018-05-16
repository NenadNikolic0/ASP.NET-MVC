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
                hopeCRMEntitiesSecond db = new hopeCRMEntitiesSecond();
                string zip = Request["zip"].ToString().Trim();

                Customer CurrentUser = db.Customers.Where(t => t.zip.Trim() == zip).First();

            
                //Adding user data to viewbag
                if (CurrentUser.name1!=null)
                {
                    ViewBag.AgName1 = CurrentUser.name1.ToString().Trim();
                }

                if (CurrentUser.name2 != null)
                {
                    ViewBag.AgName2 = CurrentUser.name2.ToString().Trim();
                }

                if (CurrentUser.street != null)
                {
                    ViewBag.Street = CurrentUser.street.ToString().Trim() + " " + CurrentUser.streetno.ToString().Trim();
                }

                if (CurrentUser.zip != null)
                {
                    ViewBag.Zip = CurrentUser.zip.ToString().Trim();
                }

                if (CurrentUser.city != null)
                {
                    ViewBag.City = CurrentUser.city.ToString().Trim();
                }

                if (CurrentUser.countryid != null)
                {
                    ViewBag.Country = CurrentUser.countryid.ToString().Trim();
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
            hopeCRMEntitiesSecond db = new hopeCRMEntitiesSecond();
            

            Customer CurrentUser = db.Customers.Where( t => t.zip == details.Zip.ToString().Trim() && t.serialno.Substring(15).Trim() == details.SerialNo.ToString().Trim()).First();


            //Adding user data to viewbag
            if (CurrentUser.name1 != null)
            {
                details.Name1 = CurrentUser.name1.ToString().Trim();
            }

            if (CurrentUser.name2 != null)
            {
                details.Name2 = CurrentUser.name2.ToString().Trim();
            }

            if (CurrentUser.street != null)
            {
                details.Street= CurrentUser.street.ToString().Trim() + " " + CurrentUser.streetno.ToString().Trim();
            }

            

            if (CurrentUser.city != null)
            {
                details.City= CurrentUser.city.ToString().Trim();
            }

            if (CurrentUser.countryid != null)
            {
                details.Country = CurrentUser.countryid.ToString().Trim();
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