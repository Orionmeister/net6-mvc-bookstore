using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Models
{
    public class CoverType
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("Cover Type")]
        [Required]
        [MaxLength(50,ErrorMessage = "Cover Type Name lentgh can not exceed 50 characters.")]
        public string Name { get; set; }
    }
}
