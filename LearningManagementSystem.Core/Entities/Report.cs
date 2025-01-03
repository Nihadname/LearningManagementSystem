﻿using LearningManagementSystem.Core.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningManagementSystem.Core.Entities
{
    public class Report:BaseEntity
    {
        public string Description { get; set; }
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }
        public string ReportedUserId { get; set; }  
        public AppUser ReportedUser { get; set; }
        public Guid ReportOptionId { get; set; }
        public ReportOption ReportOption { get; set; }
        public bool IsVerified { get; set; }
    }
}
