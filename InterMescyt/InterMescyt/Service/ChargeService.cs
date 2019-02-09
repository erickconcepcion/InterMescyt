using InterMescyt.Data;
using InterMescyt.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterMescyt.Service
{
    public interface IChargeService
    {
        Execution UploadFile(Stream file);
        ValidateResult ValidateLine(string line);
        ExecutionLine ExecutionLineCreate(ValidateResult validatedLine);
        Execution ExecutionCreate(IEnumerable<ExecutionLine>Lines);
        Header ExecuteImport(int execId);
    }
    public class ChargeService: IChargeService
    {
        private readonly IFormatService _formatService;
        private readonly ApplicationDbContext _context;

        public ChargeService(ApplicationDbContext context, IFormatService formatService)
        {
            _formatService = formatService;
            _context = context;
        }

        public Header ExecuteImport(int execId)
        {
            Header header = null;
            Execution exec = _context.Executions.Where(e => e.Id == execId).Include(e=>e.ExecutionLines).FirstOrDefault();
            if (exec.ExecutionLines.Any(el=>!el.Suscess))
            {
                return header;
            }
            
            header = _formatService.FileLineToHeader(
                exec.ExecutionLines
                .Where(el=>el.Text.FirstOrDefault()==_formatService.HeaderId)
                .FirstOrDefault().Text);
            _context.Headers.Add(header);
            exec.Executed = true;
            _context.SaveChanges();
            foreach (var item in exec.ExecutionLines.Where(el=>el.Text.FirstOrDefault() == _formatService.DetailId))
            {
                _context.TransLines.Add(_formatService.FileLineToTransLine(item.Text));
            }
            _context.SaveChanges();
            return header;
        }

        public Execution ExecutionCreate(IEnumerable<ExecutionLine> Lines)
        {
            var ret = new Execution
            {
                EndDate = DateTime.Now,
                Executed = false
            };
            _context.Executions.Add(ret);
            _context.SaveChanges();
            foreach (var item in Lines)
            {
                item.ExecutionId = ret.Id;
                _context.ExecutionLines.Add(item);
            }
            var sum = Lines.Where(l => l.Text.FirstOrDefault() == _formatService.SummaryId);
            if (!Lines.Where(l => l.Text.FirstOrDefault() == _formatService.HeaderId).Any() &&
                    !Lines.Where(l => l.Text.FirstOrDefault() == _formatService.DetailId).Any() &&
                    !sum.Any())
            {
                _context.ExecutionLines.Add(new ExecutionLine
                {
                    ExecutionId = ret.Id,
                    Text = "Extra",
                    ValidationMessage = "Debe tener por lo menos un solo registro de cada tipo",
                    Suscess = false
                });
            }
            if (sum.Any())
            {
                var total = int.Parse(_formatService.GetSummaryField(1, sum.FirstOrDefault().Text));
                if (total != Lines.Where(l=>l.Text.FirstOrDefault() == _formatService.DetailId).Count())
                {
                    _context.ExecutionLines.Add(new ExecutionLine
                    {
                        ExecutionId = ret.Id,
                        Text = "Extra",
                        ValidationMessage = "La cantidad en el sumario difiere de las lineas procesadas",
                        Suscess = false
                    });
                }
            }
            _context.SaveChanges();
            return ret;
        }

        public ExecutionLine ExecutionLineCreate(ValidateResult validatedLine)
        => new ExecutionLine
        {
            Suscess = validatedLine.Suscess,
            Text = validatedLine.Input,
            ValidationMessage = validatedLine.Message
        };

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

        public ValidateResult ValidateLine(string line)
        {
            var init = new ValidateResult
            {
                Input = line,
                Suscess = true
            };
            try
            {
                _formatService.ValidateLineStructure(line);
                if (line.FirstOrDefault()==_formatService.HeaderId)
                {
                    _formatService.FileLineToHeader(line);
                }
                else if (line.FirstOrDefault() == _formatService.DetailId)
                {
                    _formatService.FileLineToTransLine(line);
                }
                else if (line.FirstOrDefault() == _formatService.SummaryId)
                {
                    int.Parse(_formatService.GetSummaryField(1, line));
                }
                //int.Parse(_formatService.GetSummaryField(1, sum.FirstOrDefault().Text))
                init.Message = "Estructura Valida";
            }
            catch (Exception ex)
            {
                init.Suscess = false;
                init.Message = ex.Message;
            }

            return init;
            
        }
    }
}
