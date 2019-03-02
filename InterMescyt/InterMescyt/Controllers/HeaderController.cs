using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InterMescyt.Data;

namespace InterMescyt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HeaderController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public HeaderController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Header
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Header>>> GetHeaders()
        {
            var headers = await _context.Headers.Include(h => h.TransLines).ToListAsync();
            foreach (var header in headers)
            {
                foreach (var line in header.TransLines)
                {
                    line.Header = null;
                }
            }
            return headers;
        }

        // GET: api/Header/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Header>> GetHeader(int id)
        {
            var header = await _context.Headers.Include(h=>h.TransLines).FirstOrDefaultAsync(h=>h.Id==id);

            if (header == null)
            {
                return NotFound();
            }
            foreach (var item in header.TransLines)
            {

            }
            return header;
        }

        // PUT: api/Header/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutHeader(int id, Header header)
        {
            if (id != header.Id)
            {
                return BadRequest();
            }

            _context.Entry(header).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HeaderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Header
        [HttpPost]
        public async Task<ActionResult<Header>> PostHeader(Header header)
        {
            _context.Headers.Add(header);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetHeader", new { id = header.Id }, header);
        }

        // DELETE: api/Header/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Header>> DeleteHeader(int id)
        {
            var header = await _context.Headers.FindAsync(id);
            if (header == null)
            {
                return NotFound();
            }

            _context.Headers.Remove(header);
            await _context.SaveChangesAsync();

            return header;
        }

        private bool HeaderExists(int id)
        {
            return _context.Headers.Any(e => e.Id == id);
        }
    }
}
