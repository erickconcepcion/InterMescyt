using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InterMescyt.Data
{
    public class Header
    {
        public int Id { get; set; }
        [Display(Name = "RNC")]
        public string Rnc { get; set; }
        [Display(Name = "Entidad")]
        public string Name { get; set; }
        [Display(Name = "Localidad Entidad")]
        public string Location { get; set; }
        [Display(Name = "Fecha Transmisión")]
        public DateTime TransDate { get; set; }
        [Display(Name = "Fecha de Creacion")]
        public DateTime InputDate { get; set; }
        public ICollection<TransLine> TransLines { get; set; }
    }
}
