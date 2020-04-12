using BackgardenBlazor.Models;
using System;
using System.Threading.Tasks;

namespace BackgardenBlazor.Services
{
    public class AppState
    {
        public event Action<ToggleChangedModel> OnToggleGpio;
        public event Func<ToggleChangedModel, Task> OnToggleGpioAsync;

        public event Action<ToggleChangedModel> OnGpioValueChanged;
        public event Func<ToggleChangedModel, Task> OnGpioValueChangedAsync;

        public event Func<ToggleType, string, Task> OnSprinklerMessage;

        public void ToggleGpio(ToggleChangedModel data)
        {
            if (OnToggleGpio != null)
            {
                OnToggleGpio.Invoke(data);
            }
        }

        public async Task ToggleGpioAsync(ToggleChangedModel data)
        {
            if (OnToggleGpioAsync != null)
            {
                await OnToggleGpioAsync.Invoke(data);
            }
        }

        public void GpioValueChanged(ToggleChangedModel data)
        {
            if (OnGpioValueChanged != null)
            {
                OnGpioValueChanged.Invoke(data);
            }
        }

        public async Task GpioValueChangedAsync(ToggleChangedModel data)
        {
            if (OnGpioValueChangedAsync != null)
            {
                await OnGpioValueChangedAsync.Invoke(data);
            }
        }

        public async Task SprinklerMessageAsync(ToggleType toggleType, string message)
        {
            if(OnSprinklerMessage != null)
            {
                await OnSprinklerMessage.Invoke(toggleType, message);
            }
        }
    }
}
