﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSideData.Models
{
    public class User
    {
        public int Id { get; set; }
        public string username { get; set; }
        public string hashPassword { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }

    }
}
