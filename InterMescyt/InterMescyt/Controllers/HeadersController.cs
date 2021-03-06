﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using InterMescyt.Data;
using InterMescyt.Service;
using System.IO;

namespace InterMescyt.Controllers
{
    public class HeadersController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly IExportService _exportService;

        public HeadersController(ApplicationDbContext context, IExportService exportService)
        {
            _context = context;
            _exportService = exportService;
        }

        // GET: Headers
        public async Task<IActionResult> Index()
        {
            return View(await _context.Headers.ToListAsync());
        }

        // GET: Headers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var header = await _context.Headers.Include(h=>h.TransLines)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (header == null)
            {
                return NotFound();
            }

            return View(header);
        }
        public async Task<IActionResult> DetailsLine(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var header = await _context.TransLines.Include(l=>l.Header)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (header == null)
            {
                return NotFound();
            }

            return View(header);
        }

        public IActionResult Download(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            MemoryStream stream = _exportService.ExportFile(id.Value);
            var content = stream.ToArray();
            stream.Flush();
            stream.Close();
            if (content == null)
                return NotFound();

            return File(content, "application/force-download", $"Transaction{id}.txt"); // returns a FileStreamResult


        }
        public IActionResult DownloadJson(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            MemoryStream stream = _exportService.ExportJsonFile(id.Value);
            var content = stream.ToArray();
            stream.Flush();
            stream.Close();
            if (content == null)
                return NotFound();

            return File(content, "application/force-download", $"Transaction{id}.txt"); // returns a FileStreamResult


        }

        // GET: Headers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Headers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Rnc,Name,Location,TransDate,InputDate")] Header header)
        {
            if (ModelState.IsValid)
            {
                _context.Add(header);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(header);
        }

        // GET: Headers/CreateLine
        public async Task<IActionResult> CreateLine(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var header = await _context.Headers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (header == null)
            {
                return NotFound();
            }
            var ret = new TransLine { Id = 0, HeaderId = header.Id };
            return View(ret);
        }

        // POST: Headers/CreateLine
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateLine([Bind("Id,Cedula,Name,EnrollNumber,Career,AcademicIndex,Period,Title,HeaderId")] TransLine line)
        {
            line.Id = 0;
            if (ModelState.IsValid)
            {
                _context.Add(line);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { id = line.HeaderId});
            }
            return View(line);
        }

        // GET: Headers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var header = await _context.Headers.FindAsync(id);
            if (header == null)
            {
                return NotFound();
            }
            return View(header);
        }

        // POST: Headers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Rnc,Name,Location,TransDate,InputDate")] Header header)
        {
            if (id != header.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(header);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HeaderExists(header.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(header);
        }

        // GET: Headers/EditLine/5
        public async Task<IActionResult> EditLine(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var line = await _context.TransLines.FindAsync(id);
            if (line == null)
            {
                return NotFound();
            }
            return View(line);
        }

        // POST: Headers/EditLine/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditLine(int id, [Bind("Id,Cedula,Name,EnrollNumber,Career,AcademicIndex,Period,Title,HeaderId")] TransLine line)
        {
            if (id != line.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(line);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HeaderExists(line.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Details), new { id = line.HeaderId});
            }
            return View(line);
        }

        // GET: Headers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var header = await _context.Headers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (header == null)
            {
                return NotFound();
            }

            return View(header);
        }

        // POST: Headers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var header = await _context.Headers.FindAsync(id);
            _context.Headers.Remove(header);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Headers/Delete/5
        public async Task<IActionResult> DeleteLine(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var line = await _context.TransLines
                .FirstOrDefaultAsync(m => m.Id == id);
            if (line == null)
            {
                return NotFound();
            }

            return View(line);
        }

        // POST: Headers/Delete/5
        [HttpPost, ActionName("DeleteLine")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteLineConfirmed(int id)
        {
            var line = await _context.TransLines.FindAsync(id);
            _context.TransLines.Remove(line);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = line.HeaderId });
        }

        private bool HeaderExists(int id)
        {
            return _context.Headers.Any(e => e.Id == id);
        }
    }
}
