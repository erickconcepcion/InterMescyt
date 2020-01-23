using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InterMescyt.Data
{
    public class HeaderBank
    {
        public HeaderBank()
        {
            TransLineBanks = new HashSet<TransLineBank>();
        }
        public int Id { get; set; }
        [Display(Name = "RNC")]
        [StringLength(11)]
        public string Rnc { get; set; }
        [Display(Name = "Fecha Transmision")]
        public DateTime TransDate { get; set; }
        [Display(Name = "Fecha de Creacion")]
        public DateTime InputDate { get; set; } = DateTime.Now;
        public ICollection<TransLineBank> TransLineBanks { get; set; }
    }
}
