using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using InterMescyt.Data;

namespace InterMescyt.Controllers
{
    public class ExecutionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ExecutionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Executions
        public async Task<IActionResult> Index()
        {
            return View(await _context.Executions.ToListAsync());
        }

        // GET: Executions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var execution = await _context.Executions.Include(e=>e.ExecutionLines)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (execution == null)
            {
                return NotFound();
            }

            return View(execution);
        }

        // GET: Executions/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Executions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,EndDate,Executed")] Execution execution)
        {
            if (ModelState.IsValid)
            {
                _context.Add(execution);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(execution);
        }

        // GET: Executions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var execution = await _context.Executions.FindAsync(id);
            if (execution == null)
            {
                return NotFound();
            }
            return View(execution);
        }

        // POST: Executions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,EndDate,Executed")] Execution execution)
        {
            if (id != execution.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(execution);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExecutionExists(execution.Id))
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
            return View(execution);
        }

        // GET: Executions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var execution = await _context.Executions
                .FirstOrDefaultAsync(m => m.Id == id);
            if (execution == null)
            {
                return NotFound();
            }

            return View(execution);
        }

        // POST: Executions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var execution = await _context.Executions.FindAsync(id);
            _context.Executions.Remove(execution);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ExecutionExists(int id)
        {
            return _context.Executions.Any(e => e.Id == id);
        }
    }
}
