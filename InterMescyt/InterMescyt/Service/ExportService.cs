using InterMescyt.Data;
using Microsoft.EntityFrameworkCore;
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
        public string SetFileSummary(int lineCount) 
            => $"{_formatService.SummaryId}{lineCount.ToString().PadRight(6).Substring(0,6)}";
    }
}
