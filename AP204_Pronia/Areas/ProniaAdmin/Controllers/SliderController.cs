using AP204_Pronia.DAL;
using AP204_Pronia.Extensions;
using AP204_Pronia.Models;
using AP204_Pronia.Utilities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AP204_Pronia.Areas.ProniaAdmin.Controllers
{
    [Area("ProniaAdmin")]
    public class SliderController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public SliderController(AppDbContext context,IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task<IActionResult> Index()
        {
            List<Slider> sliders = await _context.Sliders.ToListAsync();
            return View(sliders);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Create(Slider slider)
        {
            if (!ModelState.IsValid) return View();

            if (slider.Photo != null)
            {
                //if (!slider.Photo.IsImage())
                //{
                //    ModelState.AddModelError("Photo", "Please choose supported file");
                //    return View();
                //}
                //if (slider.Photo.IsGreater(1))
                //{
                //    ModelState.AddModelError("Photo", "Please choose image file which size under 1 MB");
                //    return View();
                //}
                if (!slider.Photo.IsOkay(1))
                {
                    ModelState.AddModelError("Photo", "Please choose supported file");
                    return View();
                }

                
                slider.Image = await slider.Photo.FileCreate(_env.WebRootPath,@"assets\images\website-images");
                await _context.Sliders.AddAsync(slider);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ModelState.AddModelError("Photo", "Please choose file");
                return View();
            }          
        }
    }
}
