using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Moldtrax.Models
{
    public class tblEmployees
    {
        [Key]
        public int EmployeeID { get; set; }
        public int CompanyID { get; set; }
        public string DepartmentID { get; set; }
        public string Shift { get; set; }
        public string EmployeeNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; }
        public string EmailName { get; set; }
        public string HomePhone { get; set; }
        public string WorkPhone { get; set; }
        public string Notes { get; set; }
        public string OfficeLocation { get; set; }
        public string EmplAddress { get; set; }
        public string EmplCity { get; set; }
        public string EmplState { get; set; }
        public string EmplCountry { get; set; }
        public string EmplZip { get; set; }
        public byte[] EmployeeMug { get; set; }
        public string EmplJobDescrip { get; set; }
        public DateTime? EmplHireDate { get; set; }
        public decimal? EmplHourlyRate { get; set; }
        public string EmpEmployeeNo { get; set; }

    }


    public class TbLEmployeeMain
    {
        public tblEmployees TblEmployees { get; set; }
        public IEnumerable<tblEmployees> TblEmployeesList { get; set; }

    }
}