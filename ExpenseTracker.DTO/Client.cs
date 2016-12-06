using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.DTO
{
    public class Client
    {
        public int Id { get; set; }
        [Required]
        public string Description { get; set; }
        [Display(Name = "Address 1")]
        public string Address1 { get; set; }
        [Display(Name = "Address 2")]
        public string Address2 { get; set; }
        [Display(Name = "Address 3")]
        public string Address3 { get; set; }
        [Display(Name = "Address 4")]
        public string Postcode { get; set; }
        [Required]
        [Display(Name = "Cuscom's Admin Email")]
        [DataType(DataType.EmailAddress)]
        public string AdminEmail { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email Provider Id")]
        public string Email { get; set; }
        public bool IsNonOrganisationlClient { get; set; }
        [Display(Name = "Active")]
        public bool IsActive { get; set; }
    }
}
