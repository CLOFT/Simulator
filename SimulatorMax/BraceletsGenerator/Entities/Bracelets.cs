using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BraceletsGenerator.Entities
{
    public class Bracelets
    {
        public Guid SerialNumber { get; set; }

        public string? Username { get; set; }

        public string Color { get; set; }
        public int? Serendipity { get; set; } 
    }
}
