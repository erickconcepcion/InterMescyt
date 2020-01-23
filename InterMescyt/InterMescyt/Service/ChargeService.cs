using InterMescyt.Data;
using InterMescyt.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterMescyt.Service
{
    public interface IChargeService
    {
        Execution UploadFile(Stream file, bool isbank = false);
        ValidateResult ValidateLine(string line, bool isbank = false);
        ExecutionLine ExecutionLineCreate(ValidateResult validatedLine);
        Execution ExecutionCreate(IEnumerable<ExecutionLine>Lines);
        Header ExecuteImport(int execId);
        Execution UploadJsonFile(Stream file);
        void ConfigureToBank();
        HeaderBank ExecuteBankImport(int execId);
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
            header.InputDate = DateTime.Now;
            _context.Headers.Add(header);
            _context.SaveChanges();
            foreach (var item in exec.ExecutionLines.Where(el=>el.Text.FirstOrDefault() == _formatService.DetailId))
            {
                var trans = _formatService.FileLineToTransLine(item.Text);
                trans.HeaderId = header.Id;
                _context.TransLines.Add(trans);
            }
            exec.Executed = true;
            exec.TransactionNumber = header.Id;
            _context.SaveChanges();
            return header;
        }
        public HeaderBank ExecuteBankImport(int execId)
        {
            HeaderBank header = null;
            Execution exec = _context.Executions.Where(e => e.Id == execId).Include(e => e.ExecutionLines).FirstOrDefault();
            if (exec.ExecutionLines.Any(el => !el.Suscess))
            {
                return header;
            }

            header = _formatService.FileLineToHeaderBank(
                exec.ExecutionLines
                .Where(el => el.Text.FirstOrDefault() == _formatService.HeaderId)
                .FirstOrDefault().Text);
            header.InputDate = DateTime.Now;
            _context.HeaderBanks.Add(header);
            _context.SaveChanges();
            foreach (var item in exec.ExecutionLines.Where(el => el.Text.FirstOrDefault() == _formatService.DetailId))
            {
                var trans = _formatService.FileLineToTransLineBank(item.Text);
                trans.HeaderBankId = header.Id;
                _context.TransLineBanks.Add(trans);
            }
            exec.Executed = true;
            exec.TransactionBankNumber = header.Id;
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
        public Execution UploadJsonFile(Stream file)
        {
            string totalLine = "";
            using (StreamReader sr = new StreamReader(file))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    totalLine = $"{totalLine}{line}";
                }
            }
            var ret = new Execution
            {
                EndDate = DateTime.Now,
                Executed = false
            };
            _context.Executions.Add(ret);
            _context.SaveChanges();
            ExecutionLine item = new ExecutionLine();
            item.ExecutionId = ret.Id;
            item.Text = totalLine;
            try
            {
                var header = JsonConvert.DeserializeObject<Header>(totalLine);
                if (header.Id==0 && header.TransLines.Sum(l=>l.Id)==0 && header.TransLines.Sum(l => l.HeaderId) == 0)
                {
                    _context.Headers.Add(header);
                    _context.SaveChanges();
                    ret.Executed = true;
                    ret.TransactionNumber = header.Id;
                }
                else
                {
                    item.ValidationMessage = "No debe incluir el campo Id en ningun caso o por lo menos colocarlos en 0";
                }
            }
            catch (Exception ex)
            {
                item.ValidationMessage = ex.Message;
            }
            _context.ExecutionLines.Add(item);
            _context.SaveChanges();
            return ret;
        }
        public void ConfigureToBank()
        {
            _formatService.MapHeader = new int[] { 0, 1, 10 };
            _formatService.MapLine = new int[] { 0, 1, 12, 22, 28 };
            _formatService.MaxHeader = 20;
            _formatService.MaxDetail = 38;
        }
        public Execution UploadFile(Stream file, bool isbank =false)
        {
            var execLines = new List<ExecutionLine>();
            using (StreamReader sr = new StreamReader(file))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    execLines.Add( ExecutionLineCreate( ValidateLine(line, isbank) ) );
                }
            }
            return ExecutionCreate(execLines);
        }

        public ValidateResult ValidateLine(string line, bool isbank = false)
        {
            var init = new ValidateResult
            {
                Input = line,
                Suscess = true
            };
            try
            {
                _formatService.ValidateLineStructure(line);
                if (!isbank)
                {
                    ValidateStandard(line);
                }
                else
                {
                    ValidateBank(line);
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
        private void ValidateStandard(string line)
        {
            if (line.FirstOrDefault() == _formatService.HeaderId)
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
        }
        private void ValidateBank(string line)
        {
            if (line.FirstOrDefault() == _formatService.HeaderId)
            {
                _formatService.FileLineToHeaderBank(line);
            }
            else if (line.FirstOrDefault() == _formatService.DetailId)
            {
                _formatService.FileLineToTransLineBank(line);
            }
            else if (line.FirstOrDefault() == _formatService.SummaryId)
            {
                int.Parse(_formatService.GetSummaryField(1, line));
            }
        }

    }
}
