using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackgardenBlazor.Models
{
    public class SprinklerModel
    {
        public int Id { get; set; } = int.MinValue;
        public string Description { get; set; } = string.Empty;

        public int GpioPort { get; set; } = int.MinValue;
    }
}
