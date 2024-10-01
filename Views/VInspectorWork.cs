using System;
using System.Collections.Generic;

namespace RPBDISLAB2.Views;

public partial class VInspectorWork
{
    public string InspectorName { get; set; } = null!;

    public string Department { get; set; } = null!;

    public int? TotalInspections { get; set; }

    public decimal? TotalPenaltyAmount { get; set; }

    public string? InspectedEnterprises { get; set; }
}
