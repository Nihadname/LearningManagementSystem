﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningManagementSystem.Application.Dtos.Course
{
    public record CourseSelectItemDto
    {
        public Guid Id { get; init; }
        public string   Name { get; init; }
    }
}
