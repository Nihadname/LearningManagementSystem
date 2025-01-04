﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningManagementSystem.Application.Dtos.Course
{
    public record CourseListItemDto
    {
        public Guid Id  { get; set; }
        public string Name { get; init; }
        public string Description { get; init; }
        public string ImageUrl  { get; init; }
    }
}
