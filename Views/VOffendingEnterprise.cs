using System;
using System.Collections.Generic;

namespace RPBDISLAB2.Views;

public partial class VOffendingEnterprise
{
    public string EnterpriseName { get; set; } = null!;

    public string DirectorName { get; set; } = null!;

    public string DirectorPhone { get; set; } = null!;

    public string? ViolationList { get; set; }

    public string CorrectionStatus { get; set; } = null!;
}
