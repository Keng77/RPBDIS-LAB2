using System;
using System.Collections.Generic;

namespace RPBDISLAB2.Models;

public partial class Inspector
{
    public int InspectorId { get; set; }

    public string FullName { get; set; } = null!;

    public string Department { get; set; } = null!;

    public virtual ICollection<Inspection> Inspections { get; set; } = new List<Inspection>();
}
