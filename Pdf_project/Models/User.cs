using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pdf_project.Models
{
    //Class user that will hold db result, user zip and email
    public class User
    {
        public String Result { get; set; }
        public String UserZip { get; set; }
        public String UserEmail { get; set; }
    }
}