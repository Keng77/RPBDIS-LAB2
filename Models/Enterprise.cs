using System;
using System.Collections.Generic;

namespace RPBDISLAB2.Models
{
    public partial class Enterprise
    {
        public int EnterpriseId { get; set; }
        public string Name { get; set; } = null!;
        public string OwnershipType { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string DirectorName { get; set; } = null!;
        public string DirectorPhone { get; set; } = null!;

        public virtual ICollection<Inspection> Inspections { get; set; } = new List<Inspection>();

        public override string ToString()
        {
            return $"Enterprise ID: {EnterpriseId}\n" +
                   $"Name: {Name}\n" +
                   $"Ownership Type: {OwnershipType}\n" +
                   $"Address: {Address}\n" +
                   $"Director Name: {DirectorName}\n" +
                   $"Director Phone: {DirectorPhone}";
        }
    }
}