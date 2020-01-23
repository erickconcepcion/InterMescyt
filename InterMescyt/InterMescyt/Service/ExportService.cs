using InterMescyt.Data;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace InterMescyt.Service
{
    public interface IExportService
    {
        MemoryStream ExportFile(int headerId);
        string SetFileSummary(int lineCount);
        MemoryStream ExportJsonFile(int headerId);
        MemoryStream ExportBankFile(int headerId);
    }
    public class ExportService : IExportService
    {
        private readonly IFormatService _formatService;
        private readonly ApplicationDbContext _context;
        public ExportService(ApplicationDbContext context, IFormatService formatService)
        {
            _formatService = formatService;
            _context = context;
        }
        public MemoryStream ExportFile(int headerId)
        {
            var header = _context.Headers.Include(h => h.TransLines).FirstOrDefault(h=>h.Id==headerId);
            MemoryStream stream = new MemoryStream();
            TextWriter sw = new StreamWriter(stream);
            sw.WriteLine(_formatService.HeaderToFileLine(header));
            foreach (var item in header.TransLines)
            {
                sw.WriteLine(_formatService.TransLineToFileLine(item));
            }
            sw.WriteLine(SetFileSummary(header.TransLines.Count));
            sw.Flush();
            sw.Close();
            return stream;
        }
        public void ConfigureToBank()
        {
            _formatService.MapHeader = new int[] { 0, 1, 10 };
            _formatService.MapLine = new int[] { 0, 1, 12, 22, 28 };
            _formatService.MaxHeader = 20;
            _formatService.MaxDetail = 38;
        }
        public MemoryStream ExportBankFile(int headerId)
        {
            ConfigureToBank();
            var header = _context.HeaderBanks.Include(h => h.TransLineBanks).FirstOrDefault(h => h.Id == headerId);
            MemoryStream stream = new MemoryStream();
            TextWriter sw = new StreamWriter(stream);
            sw.WriteLine(_formatService.HeaderBankToFileLine(header));
            foreach (var item in header.TransLineBanks)
            {
                sw.WriteLine(_formatService.TransLineBankToFileLine(item));
            }
            sw.WriteLine(SetFileSummary(header.TransLineBanks.Count));
            sw.Flush();
            sw.Close();
            return stream;
        }
        public MemoryStream ExportJsonFile(int headerId)
        {
            var header = _context.Headers.Include(h => h.TransLines).FirstOrDefault(h => h.Id == headerId);
            foreach (var item in header.TransLines)
            {
                item.Header = null;
            }
            MemoryStream stream = new MemoryStream();
            TextWriter sw = new StreamWriter(stream);
            sw.WriteLine(JsonConvert.SerializeObject(header));
            sw.Flush();
            sw.Close();
            return stream;
        }
        public string SetFileSummary(int lineCount) 
            => $"{_formatService.SummaryId}{lineCount.ToString().PadRight(6).Substring(0,6)}";
    }
}
