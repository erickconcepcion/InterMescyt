using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InterMescyt.Data
{
    public class Header
    {
        public Header()
        {
            TransLines = new HashSet<TransLine>();
        }
        public int Id { get; set; }
        [Display(Name = "RNC")]
        [StringLength(11)]
        public string Rnc { get; set; }
        [Display(Name = "Entidad")]
        [StringLength(15)]
        public string Name { get; set; }
        [Display(Name = "Localidad Entidad")]
        [StringLength(30)]
        public string Location { get; set; }
        [Display(Name = "Fecha Transmisión")]
        public DateTime TransDate { get; set; }
        [Display(Name = "Fecha de Creacion")]
        public DateTime InputDate { get; set; } = DateTime.Now;
        public ICollection<TransLine> TransLines { get; set; }
    }
}
