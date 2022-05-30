using AP204_Pronia.DAL;
using AP204_Pronia.Extensions;
using AP204_Pronia.Models;
using AP204_Pronia.Utilities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AP204_Pronia.Areas.ProniaAdmin.Controllers
{
    [Area("ProniaAdmin")]
    public class PlantController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public PlantController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task<IActionResult> Index()
        {
            List<Plant> plants = await _context.Plants.Include(p => p.PlantImages).ToListAsync();
            return View(plants);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Sizes = await _context.Sizes.ToListAsync();
            ViewBag.Colors = await _context.Colors.ToListAsync();
            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Create(Plant plant)
        {
            ViewBag.Sizes = await _context.Sizes.ToListAsync();
            ViewBag.Colors = await _context.Colors.ToListAsync();
            ViewBag.Categories = await _context.Categories.ToListAsync();
            if (!ModelState.IsValid) return View();
            if (plant.MainImage == null || plant.AnotherImages == null)
            {
                ModelState.AddModelError("", "Please choose main image or another image");
                return View();
            }
            if (!plant.MainImage.IsOkay(1))
            {
                ModelState.AddModelError("MainImage", "Please choose image file and max 1MB");
                return View();
            }
            foreach (var image in plant.AnotherImages)
            {
                if (!image.IsOkay(1))
                {
                    ModelState.AddModelError("AnotherImages", "Please choose image file and max 1MB");
                    return View();
                }
            }

            plant.PlantImages = new List<PlantImage>();

            PlantImage mainImage = new PlantImage
            {
                ImagePath = await plant.MainImage.FileCreate(_env.WebRootPath, @"assets\images\website-images"),
                IsMain = true,
                Plant = plant
            };



            plant.PlantImages.Add(mainImage);


            foreach (var image in plant.AnotherImages)
            {
                PlantImage another = new PlantImage
                {
                    ImagePath = await image.FileCreate(_env.WebRootPath, @"assets\images\website-images"),
                    IsMain = false,
                    Plant = plant
                };

                plant.PlantImages.Add(another);
            }
            plant.PlantCategories = new List<PlantCategory>();

            foreach (var id in plant.CategoryIds)
            {
                PlantCategory plantCategory = new PlantCategory
                {
                    Plant = plant,
                    CategoryId = id
                };
                plant.PlantCategories.Add(plantCategory);
            }

            await _context.Plants.AddAsync(plant);
            await _context.SaveChangesAsync();





            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.Sizes = await _context.Sizes.ToListAsync();
            ViewBag.Colors = await _context.Colors.ToListAsync();
            ViewBag.Categories = await _context.Categories.ToListAsync();
            Plant plant = await _context.Plants.Include(p => p.PlantImages).Include(p => p.PlantCategories).FirstOrDefaultAsync(p => p.Id == id);
            if (plant == null) return NotFound();
            return View(plant);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Edit(int id, Plant plant)
        {
            ViewBag.Sizes = await _context.Sizes.ToListAsync();
            ViewBag.Colors = await _context.Colors.ToListAsync();
            ViewBag.Categories = await _context.Categories.ToListAsync();
            Plant existed = await _context.Plants.Include(p => p.PlantImages).Include(p => p.PlantCategories).FirstOrDefaultAsync(p => p.Id == id);
            if (existed == null) return NotFound();


            if (plant.ImageIds == null && plant.AnotherImages == null)
            {
                ModelState.AddModelError("", "You can not delete all images without adding another image");
                return View(existed);
            }

            List<PlantImage> removableImages = existed.PlantImages.Where(p => p.IsMain == false && !plant.ImageIds.Contains(p.Id)).ToList();

            existed.PlantImages.RemoveAll(p => removableImages.Any(ri => ri.Id == p.Id));

            List<PlantCategory> removableCategories = existed.PlantCategories.Where(pc => !plant.CategoryIds.Contains(pc.CategoryId)).ToList();

            existed.PlantCategories.RemoveAll(pc => removableCategories.Any(rc => rc.Id == pc.Id));

            foreach (var item in plant.CategoryIds)
            {
                PlantCategory existedCategory = existed.PlantCategories.FirstOrDefault(pc => pc.CategoryId == item);
                if (existedCategory == null)
                {
                    PlantCategory plantCategory = new PlantCategory
                    {
                        PlantId = existed.Id,
                        CategoryId = item
                    };
                    existed.PlantCategories.Add(plantCategory);
                }
            }

            foreach (var image in removableImages)
            {
                FileUtilities.FileDelete(_env.WebRootPath, @"assets\images\website-images", image.ImagePath);
            }

            if (plant.AnotherImages != null)
            {
                foreach (var image in plant.AnotherImages)
                {
                    PlantImage plantImage = new PlantImage
                    {
                        ImagePath = await image.FileCreate(_env.WebRootPath, @"assets\images\website-images"),
                        IsMain = false,
                        PlantId = existed.Id
                    };
                    existed.PlantImages.Add(plantImage);
                }
            }


            _context.Entry(existed).CurrentValues.SetValues(plant);
            await _context.SaveChangesAsync();


            return RedirectToAction(nameof(Index));
        }
    }
}
