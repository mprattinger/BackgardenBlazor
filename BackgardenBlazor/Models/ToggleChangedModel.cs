using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackgardenBlazor.Models
{
    public enum ToggleType
    {
        SPRINKLER,
        POWER,
        PUMP,
        VALVE
    }

    public class ToggleChangedModel
    {
        public ToggleType ToggleType { get; set; } = ToggleType.SPRINKLER;

        public int SprinklerId { get; set; }

        public bool NewValue { get; set; }
    }
}
