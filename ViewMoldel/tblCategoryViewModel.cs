using Moldtrax.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Moldtrax.ViewMoldel
{
    public class tblCategoryViewModel
    {
        public string CategoryName { get; set; }
        public List<FinalChecklstResult> ChecksheetData { get; set; }
    }
}