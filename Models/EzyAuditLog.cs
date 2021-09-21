using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Moldtrax.Models
{
    [Table("ezy_AuditLog")]
    public class EzyAuditLog
    {
        [Key]
        public int ID { get; set; }
        public DateTime DateTime { get; set; }
        public string User { get; set; }
        public string Action { get; set; }
        public string DataKey { get; set; }
        public string PageName { get; set; }
        public int CompanyID { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public string TableName { get; set; }
        public string LabelName { get; set; }
        [NotMapped]
        public string CompanyName { get; set; }
    }
}