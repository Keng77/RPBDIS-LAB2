using System;
using System.Collections.Generic;

namespace RPBDISLAB2.Models;

public partial class ViolationType
{
    public int ViolationTypeId { get; set; }

    public string Name { get; set; } = null!;

    public decimal PenaltyAmount { get; set; }

    public int CorrectionPeriodDays { get; set; }

    public virtual ICollection<Inspection> Inspections { get; set; } = new List<Inspection>();
}
