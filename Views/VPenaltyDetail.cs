using System;
using System.Collections.Generic;

namespace RPBDISLAB2.Views;

public partial class VPenaltyDetail
{
    public string EnterpriseName { get; set; } = null!;

    public DateOnly InspectionDate { get; set; }

    public string ViolationName { get; set; } = null!;

    public decimal PenaltyAmount { get; set; }

    public DateOnly PaymentDeadline { get; set; }

    public string PaymentStatus { get; set; } = null!;
}
