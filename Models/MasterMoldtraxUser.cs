using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Moldtrax.Models
{
    public class MasterMoldtraxUser
    {
        [Key]
        public int ID { get; set; }
        public string UserID { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string OperatorStamp { get; set; }
        public string UserEmail { get; set; }
        public string Database { get; set; }

    }

    public class MasterUser
    {
        [Key]
        public int ID { get; set; }
        public string UserID { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string OperatorStamp { get; set; }
        public string UserEmail { get; set; }
        public string Database { get; set; }
    }
}