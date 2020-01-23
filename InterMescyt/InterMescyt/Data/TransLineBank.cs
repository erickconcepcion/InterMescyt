using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InterMescyt.Data
{
    public class TransLineBank
    {
        public int Id { get; set; }

        [Display(Name = "Cédula o No. Pasaporte")]
        [StringLength(11)]
        public string Cedula { get; set; }

        [Display(Name = "Cuenta Bancaria")]
        [StringLength(10)]
        public string BankAccount { get; set; }

        [Display(Name = "Sueldo Neto")]
        public decimal NetSalary { get; set; }

        [Display(Name = "Fecha de Deposito")]
        public DateTime TransDate { get; set; }

        public int HeaderBankId { get; set; }
        [Display(Name = "Encabezado")]
        public virtual HeaderBank HeaderBank { get; set; }
    }
}
