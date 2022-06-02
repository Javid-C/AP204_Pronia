using AP204_Pronia.DAL;
using AP204_Pronia.Models;
using AP204_Pronia.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AP204_Pronia.Controllers
{
    public class PlantController : Controller
    {
        private readonly AppDbContext _context;

        public PlantController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            var query= _context.Plants.AsQueryable();
            ViewBag.TotalPage = Math.Ceiling(((decimal)await query.CountAsync()) / 1);
            ViewBag.CurrentPage = page;
            List<Plant> plants = await query.Include(p => p.PlantImages).Skip((page - 1) * 1).Take(1).ToListAsync();
            return View(plants);
        }

        public async Task<IActionResult> AddBasket(int id)
        {
            Plant plant = await _context.Plants.FirstOrDefaultAsync(p=>p.Id == id);
            if (plant == null) return NotFound();
            string basketStr = HttpContext.Request.Cookies["Basket"];
            List<BasketCookieItemVM> basket;
            //BasketVM basket;
            //string itemsStr;
            if (string.IsNullOrEmpty(basketStr))
            {
                basket = new List<BasketCookieItemVM>();
                BasketCookieItemVM cookie = new BasketCookieItemVM
                {
                    Id =plant.Id,
                    Count =1
                };
                //BasketItemVM item = new BasketItemVM
                //{
                //    Plant = plant,
                //    Count = 1
                //};
                basket.Add(cookie);
                //basket.BasketItemVMs.Add(item);
                //basket.TotalPrice = item.Plant.Price;
                //basket.Count = 1;
                basketStr = JsonConvert.SerializeObject(basket);
               
            }
            else
            {
                basket = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(basketStr);
                //basket = JsonConvert.DeserializeObject<BasketVM>(basketStr);

                BasketCookieItemVM existedCookie = basket.FirstOrDefault(c => c.Id == plant.Id);
                //BasketItemVM existedItem = basket.BasketItemVMs.FirstOrDefault(i => i.Plant.Id == id);
                if(existedCookie == null)
                {
                    BasketCookieItemVM cookie = new BasketCookieItemVM
                    {
                        Id = plant.Id,
                        Count = 1
                    };
                    //BasketItemVM item = new BasketItemVM
                    //{
                    //    Plant = plant,
                    //    Count = 1
                    //};
                    basket.Add(cookie);
                    //basket.BasketItemVMs.Add(item);
                }
                else
                {
                    existedCookie.Count++;
                }

                //decimal total = default;
                //foreach (BasketItemVM item in basket.BasketItemVMs)
                //{
                //    total += item.Plant.Price * item.Count;
                //}
                //basket.TotalPrice = total;
                //basket.Count = basket.BasketItemVMs.Count;
                basketStr = JsonConvert.SerializeObject(basket);

            }
            
            HttpContext.Response.Cookies.Append("Basket", basketStr);
            return RedirectToAction("Index","Home");
        }

        public ActionResult ShowBasket()
        {
            return Content(HttpContext.Request.Cookies["Basket"]);
        }
    }
}
