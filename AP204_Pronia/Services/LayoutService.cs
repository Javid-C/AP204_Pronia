using AP204_Pronia.DAL;
using AP204_Pronia.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AP204_Pronia.Services
{
    public class LayoutService
    {
        private readonly AppDbContext _context;

        public LayoutService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Setting> GetDatas()
        {
            Setting setting = await _context.Settings.FirstOrDefaultAsync();
            return setting;
        }
    }
}
