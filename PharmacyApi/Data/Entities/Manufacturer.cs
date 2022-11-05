using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PharmacyApi.Data.Entities
{
    public class Manufacturer
    {
        [Required]
        public int ManufacturerId { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string Phone { get; set; }
        public string Email { get; set; }

        public ICollection<Medicine> Medicines { get; set; }
    }

}
