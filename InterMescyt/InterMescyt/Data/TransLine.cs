using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InterMescyt.Data
{
    public class TransLine
    {
        public int Id { get; set; }
        [Display(Name = "Cédula o No. Pasaporte")]
        [StringLength(11)]
        public string Cedula { get; set; }
        [Display(Name = "Nombre")]
        [StringLength(50)]
        public string Name { get; set; }
        [Display(Name = "Matrícula")]
        [StringLength(11)]
        public string EnrollNumber { get; set; }
        [Display(Name = "Carrera o Especialidad")]
        [StringLength(31)]
        public string Career { get; set; }
        [Display(Name = "Índice Académico")]
        [Range(0.00, 4.00)]
        public decimal AcademicIndex { get; set; }
        [Display(Name = "Periodo Académico")]
        [Range(0,99)]
        public int Period { get; set; }
        [Display(Name = "Titulación")]
        [StringLength(10)]
        public string Title { get; set; }

        public int HeaderId { get; set; }
        [Display(Name = "Encabezado")]
        public virtual Header Header { get; set; }
    }
}
