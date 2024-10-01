using System;
using System.Collections.Generic;

namespace RPBDISLAB2.Models;

public partial class Inspection
{
    public int InspectionId { get; set; }

    public int InspectorId { get; set; }

    public int EnterpriseId { get; set; }

    public DateOnly InspectionDate { get; set; }

    public string ProtocolNumber { get; set; } = null!;

    public int ViolationTypeId { get; set; }

    public string ResponsiblePerson { get; set; } = null!;

    public decimal PenaltyAmount { get; set; }

    public DateOnly PaymentDeadline { get; set; }

    public DateOnly CorrectionDeadline { get; set; }

    public string? PaymentStatus { get; set; }

    public string? CorrectionStatus { get; set; }

    public virtual Enterprise Enterprise { get; set; } = null!;

    public virtual Inspector Inspector { get; set; } = null!;

    public virtual ViolationType ViolationType { get; set; } = null!;

    public override string ToString()
    {
        return $"Inspection ID: {InspectionId}\n" +
               $"Inspector ID: {InspectorId}\n" +
               $"Enterprise ID: {EnterpriseId}\n" +
               $"Inspection Date: {InspectionDate}\n" +
               $"Protocol Number: {ProtocolNumber}\n" +
               $"Violation Type ID: {ViolationTypeId}\n" +
               $"Responsible Person: {ResponsiblePerson}\n" +
               $"Penalty Amount: {PenaltyAmount}\n" +
               $"Payment Deadline: {PaymentDeadline}\n" +
               $"Correction Deadline: {CorrectionDeadline}\n" +
               $"Payment Status: {PaymentStatus}\n" +
               $"Correction Status: {CorrectionStatus}";
    }
}
