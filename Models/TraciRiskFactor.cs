using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Moldtrax.Models
{
    public class TraciRiskFactor
    {
        public int MoldDataID { get; set; }
        public string MoldName { get; set; }
        public string MoldDesc { get; set; }
        public double PF { get; set; }
        public double SF { get; set; }
        public double LF { get; set; }
        public long MoldYears { get; set; }
        public long ShotCount { get; set; }
        [Column("Part1")]
        public double PartOne { get; set; }
        [Column("Part1.5")]
        public double PartOnePointFive { get; set; }
        [Column("Part1.6")]
        public double PartOnePointSix { get; set; }
        [Column("Part2")]
        public double PartTwo { get; set; }
        [Column("Part2.5")]
        public double PartTwoPointFive { get; set; }
        public double RiskFactor { get; set; }

    }
}