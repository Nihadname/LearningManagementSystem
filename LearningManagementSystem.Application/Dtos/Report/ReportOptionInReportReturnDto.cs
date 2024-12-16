﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningManagementSystem.Application.Dtos.Report
{
    public record ReportOptionInReportReturnDto
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
    }
}
