using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InterMescyt.Data
{
    public class Execution
    {
        public Execution()
        {
            ExecutionLines = new HashSet<ExecutionLine>();
        }
        public int Id { get; set; }
        public DateTime EndDate { get; set; }
        public bool Executed { get; set; }
        public int? TransactionNumber { get; set; }
        public int? TransactionBankNumber { get; set; }
        public ICollection<ExecutionLine> ExecutionLines { get; set; }
    }
}
