﻿using LearningManagementSystem.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningManagementSystem.Application.Dtos.Fee
{
    public class FeeCreateDto
    {
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime PaidDate { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public Guid StudentId { get; set; }
    }
}
