using InterMescyt.Data;
using InterMescyt.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace InterMescyt.Service
{
    public interface IChargeService
    {
        Execution UploadFile(Stream file);
        IEnumerable<ValidateResult> ValidateLine(string line);
        ExecutionLine ExecutionLineCreate(IEnumerable<ValidateResult> validatedLine);
        Execution ExecutionCreate(IEnumerable<ExecutionLine>Lines);

    }
    public class ChargeService: IChargeService
    {
        public ChargeService(ApplicationDbContext context)
        {

        }

        public Execution ExecutionCreate(IEnumerable<ExecutionLine> Lines)
        {
            throw new NotImplementedException();
        }

        public ExecutionLine ExecutionLineCreate(IEnumerable<ValidateResult> validatedLine)
        {
            throw new NotImplementedException();
        }

        public Execution UploadFile(Stream file)
        {
            var execLines = new List<ExecutionLine>();
            using (StreamReader sr = new StreamReader(file))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    execLines.Add( ExecutionLineCreate( ValidateLine(line) ) );
                }
            }
            return ExecutionCreate(execLines);
        }

        public IEnumerable<ValidateResult> ValidateLine(string line)
        {
            throw new NotImplementedException();
        }
    }
}
