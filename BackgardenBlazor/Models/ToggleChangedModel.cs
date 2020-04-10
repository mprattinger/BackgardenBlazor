using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackgardenBlazor.Models
{
    public enum ToggleType
    {
        WERFER,
        SPRUEHER,
        TROPFER,
        POWER,
        PUMP,
        VALVE,
        WATERLEVEL,
        UNKNOWN
    }

    public class ToggleChangedModel
    {
        public ToggleType ToggleType { get; set; } = ToggleType.UNKNOWN;

        public int GpioPin { get; set; }

        public bool NewValue { get; set; }
    }
}
