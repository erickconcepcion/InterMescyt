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
    public class HeaderBanksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HeaderBanksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: HeaderBanks
        public async Task<IActionResult> Index()
        {
            return View(await _context.HeaderBanks.ToListAsync());
        }

        // GET: HeaderBanks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var headerBank = await _context.HeaderBanks.Include(h=>h.TransLineBanks)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (headerBank == null)
            {
                return NotFound();
            }

            return View(headerBank);
        }

        // GET: HeaderBanks/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: HeaderBanks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Rnc,TransDate,InputDate")] HeaderBank headerBank)
        {
            if (ModelState.IsValid)
            {
                _context.Add(headerBank);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(headerBank);
        }

        // GET: HeaderBanks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var headerBank = await _context.HeaderBanks.FindAsync(id);
            if (headerBank == null)
            {
                return NotFound();
            }
            return View(headerBank);
        }

        // POST: HeaderBanks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Rnc,TransDate,InputDate")] HeaderBank headerBank)
        {
            if (id != headerBank.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(headerBank);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HeaderBankExists(headerBank.Id))
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
            return View(headerBank);
        }

        // GET: HeaderBanks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var headerBank = await _context.HeaderBanks
                .FirstOrDefaultAsync(m => m.Id == id);
            if (headerBank == null)
            {
                return NotFound();
            }

            return View(headerBank);
        }

        // POST: HeaderBanks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var headerBank = await _context.HeaderBanks.FindAsync(id);
            _context.HeaderBanks.Remove(headerBank);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HeaderBankExists(int id)
        {
            return _context.HeaderBanks.Any(e => e.Id == id);
        }
    }
}
