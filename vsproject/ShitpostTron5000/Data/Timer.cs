﻿using System;
using System.Collections.Generic;
using System.Text;

#nullable enable

namespace ShitpostTron5000.Data
{
    public class Timer
    {
        public int Id { get; set; }
        public DateTime DueTime { get; set; }
        public string? Description { get; set; }

    }
}
