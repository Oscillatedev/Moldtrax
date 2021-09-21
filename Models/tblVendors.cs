using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Moldtrax.Models
{
    public class tblVendors
    {
        [Key]
        public int VendorID { get; set; }
        public int CompanyID { get; set; }
        public string VendorName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string StateOrProvince { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string PhoneNumber { get; set; }
        public string FaxNumber { get; set; }
        public string VendorURL { get; set; }
        public string Notes { get; set; }
        public string ServiceProvided { get; set; }
        public string VENCont1FirstName { get; set; }
        public string VENCont1LastName { get; set; }
        public string VENCont1Phone { get; set; }
        public string VENCont1Fax { get; set; }
        public string VENCont1Mobile { get; set; }
        public string VENCont1EMail { get; set; }
        public string VENCont2FirstName { get; set; }
        public string VENCont2LastName { get; set; }
        public string VENCont2Phone { get; set; }
        public string VENCont2Fax { get; set; }
        public string VENCont2Mobile { get; set; }
        public string VENCont2EMail { get; set; }
        public string VENCont3FirstName { get; set; }
        public string VENCont3LastName { get; set; }
        public string VENCont3Phone { get; set; }
        public string VENCont3Fax { get; set; }
        public string VENCont3Mobile { get; set; }
        public string VENCont3EMail { get; set; }

    }

    public class TBLVendorMain
    {
        public tblVendors TblVendors { get; set; }
        public IEnumerable<tblVendors> TblVendorsList { get; set; }
    }
}