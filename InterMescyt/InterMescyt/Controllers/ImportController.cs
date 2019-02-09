using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using InterMescyt.Data;
using InterMescyt.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;

namespace InterMescyt.Controllers
{
    public class ImportController : Controller
    {
        private readonly IChargeService _chargeService;

        public ImportController(IChargeService chargeService)
        {
            _chargeService = chargeService;
        }
        // GET: Import
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost("SaveUploadedFile")]
        public IActionResult SaveUploadedFile(IFormFile file)
        {
            if (file.Length == 0)
            {
                return BadRequest("No se permiten archivos vacios");
            }
            Execution exec;
            exec = _chargeService.UploadFile(file.OpenReadStream());
            var header = _chargeService.ExecuteImport(exec.Id);
            return Ok(exec.Id);
        }


        // GET: Import/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Import/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Import/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Import/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Import/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Import/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Import/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}