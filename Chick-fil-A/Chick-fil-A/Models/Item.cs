using System;
using System.Collections.Generic;

namespace Chick_fil_A.Models
{
    public class Item
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public string Description { get; set; }
        public List<dynamic> Properties { get; set; }
    }
}