using AP204_Pronia.DAL;
using AP204_Pronia.Models;
using AP204_Pronia.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AP204_Pronia.Services
{
    public class LayoutService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContext;

        public LayoutService(AppDbContext context, IHttpContextAccessor _httpContext)
        {
            _context = context;
            this._httpContext = _httpContext;
        }

        public async Task<Setting> GetDatas()
        {
            Setting setting = await _context.Settings.FirstOrDefaultAsync();
            return setting;
        }

        public BasketVM GetBasket()
        {
            string basketStr = _httpContext.HttpContext.Request.Cookies["Basket"];
            //BasketVM basket = new BasketVM();
            if (!string.IsNullOrEmpty(basketStr))
            {
                BasketVM basketData = JsonConvert.DeserializeObject<BasketVM>(basketStr);
                return basketData;

            }
            else
            {
                return null;
            }
        }
    }
}
