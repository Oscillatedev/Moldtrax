using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Moldtrax.Models
{
    public class tblDDMoldCategoryID
    {
        public int ID { get; set; }
        public string MoldCategoryID { get; set; }
        public string MoldCategoryIDDesc { get; set; }
        public int? CompanyID { get; set; }
        //public byte[] SSMA_TimeStamp { get; set; }
    }

    public class tblDDDepartmentID
    {
        public int ID { get; set; }
        public string DepartmentID { get; set; }
        public string DepartmentIDDesc { get; set; }
        public int? CompanyID { get; set; }
        //public byte[] SSMA_TimeStamp { get; set; }
    }

    public class tblDDProductLine
    {
        public int ID { get; set; }
        public string ProductLine { get; set; }
        public string ProductLineDesc { get; set; }
        public int? CompanyID { get; set; }
        //public byte[] SSMA_TimeStamp { get; set; }
    }


    public class tblDDProductPart
    {
        public int ID { get; set; }
        public string ProductPart { get; set; }
        public string ProductPartDesc { get; set; }
        public int? CompanyID { get; set; }

        //public byte[] SSMA_TimeStamp { get; set; }
    }
    public class tblDDMoldResinType
    {
        public int ID { get; set; }
        public string MoldResinType { get; set; }
        public string MoldResinTypeDesc { get; set; }
        public int? CompanyID { get; set; }

        //public byte[] SSMA_TimeStamp { get; set; }
    }


    public class tblDDMoldCav
    {
        public int ID { get; set; }
        public string MoldCav { get; set; }
        public string MoldCavDesc { get; set; }
        public int? CompanyID { get; set; }

        //public byte[] SSMA_TimeStamp { get; set; }

    }

    public class tblCustomer
    {
        [Key]
        public int CustomerID { get; set; }
        public int CompanyID { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerCity { get; set; }
        public string CustomerState { get; set; }
        public string CustomerZipCode { get; set; }
        public string CustomerCountry { get; set; }
        public string CustomerMainPhone { get; set; }
        public string CustomerFax { get; set; }
        public string CustomerWebsite { get; set; }
        public string CustCustNotes { get; set; }
        public string CustMoldList { get; set; }
        public string CustCont1FirstName { get; set; }
        public string CustCont1LastName { get; set; }
        public string CustCont1Phone { get; set; }
        public string CustCont1Fax { get; set; }
        public string CustCont1Mobile { get; set; }
        public string CustCont1EMail { get; set; }
        public string CustCont2FirstName { get; set; }
        public string CustCont2LastName { get; set; }
        public string CustCont2Phone { get; set; }
        public string CustCont2Fax { get; set; }
        public string CustCont2Mobile { get; set; }
        public string CustCont2EMail { get; set; }
        public string CustCont3FirstName { get; set; }
        public string CustCont3LastName { get; set; }
        public string CustCont3Phone { get; set; }
        public string CustCont3Fax { get; set; }
        public string CustCont3Mobile { get; set; }
        public string CustCont3EMail { get; set; }
        //public byte[] SSMA_TimeStamp { get; set; }

    }

    public class TblCustomerMain
    {
        public tblCustomer TBLCustomer { get; set; }
        public IEnumerable<tblCustomer> TblCustomerList { get; set; }
    }
}