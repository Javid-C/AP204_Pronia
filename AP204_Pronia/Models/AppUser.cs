﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AP204_Pronia.Models
{
    public class AppUser: IdentityUser
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string ConnetionId { get; set; }
    }
}
