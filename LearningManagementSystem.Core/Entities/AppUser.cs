﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningManagementSystem.Core.Entities
{
    public class AppUser:IdentityUser
    {
        public string fullName { get; set; }
        public string? GoogleId { get; set; }
        public string? Image { get; set; }
        public bool IsBlocked { get; set; }
        public DateTime? CreatedTime { get; set; }
        public DateTime? UpdatedTime { get;  set; }
        public DateTime? BlockedUntil { get; set; }

    }
}
