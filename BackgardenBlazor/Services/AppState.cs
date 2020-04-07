using BackgardenBlazor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackgardenBlazor.Services
{
    public class AppState
    {
        public event Func<ToggleChangedModel, Task> OnToggleGpio;
        public event Func<ToggleChangedModel, Task> OnGpioValueChanged;

        public async Task ToggleGpio(ToggleChangedModel data)
        {
            if(OnToggleGpio != null)
            {
                await OnToggleGpio.Invoke(data);
            }
        }

        public async Task GpioValueChanged(ToggleChangedModel data)
        {
            if (OnGpioValueChanged != null)
            {
                await OnGpioValueChanged.Invoke(data);
            }
        }
    }
}
