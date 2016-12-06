using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTracker.Repository.Entities
{
    [Table("Clients")]
    public partial class Client
    {
        public int Id { get; set; }
        [Required]
        public string Description { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Postcode { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string AdminEmail { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        public bool IsNonOrganisationlClient { get; set; }
        public bool IsActive { get; set; }
    }
}
