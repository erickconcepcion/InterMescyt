using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InterMescyt.Data
{
    public class ExecutionLine
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string ValidationMessage { get; set; }
        public bool Suscess { get; set; }

        public int ExecutionId { get; set; }
        public virtual Execution Execution { get; set; }
    }
}
