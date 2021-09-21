using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Moldtrax.Models
{
    public class tblMoldDataNotes
    {
        [Key]
        public int MoldDataNotesAutoID { get; set; }

        public int MoldDataID { get; set; }

        public DateTime? MoldDataNotesDate { get; set; }

        [AllowHtml]
        public string MoldDataNotesMemo { get; set; }

        public int? CompanyID { get; set; }

    }
}