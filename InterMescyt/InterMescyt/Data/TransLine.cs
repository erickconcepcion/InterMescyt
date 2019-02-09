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
        public string Cedula { get; set; }
        [Display(Name = "Nombre")]
        public string Name { get; set; }
        [Display(Name = "Matrícula")]
        public string EnrollNumber { get; set; }
        [Display(Name = "Carrera o Especialidad")]
        public string Career { get; set; }
        [Display(Name = "Índice Académico")]
        public decimal AcademicIndex { get; set; }
        [Display(Name = "Periodo Académico")]
        public int Period { get; set; }
        [Display(Name = "Titulación")]
        public string Title { get; set; }

        public int HeadrId { get; set; }
        public virtual Header Header { get; set; }
    }
}
