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
        public ActionResult JsonImport()
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

        [HttpPost("SaveUploadedJsonFile")]
        public IActionResult SaveUploadedJsonFile(IFormFile file)
        {
            if (file.Length == 0)
            {
                return BadRequest("No se permiten archivos vacios");
            }
            Execution exec;
            exec = _chargeService.UploadJsonFile(file.OpenReadStream());
            var header = _chargeService.ExecuteImport(exec.Id);
            return Ok(exec.Id);
        }


        
    }
}