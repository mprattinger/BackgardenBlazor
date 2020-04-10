using BackgardenBlazor.Models;
using System;
using System.Threading.Tasks;

namespace BackgardenBlazor.Services
{
    public class AppState
    {
        public event Action<ToggleChangedModel> OnToggleGpio;
        public event Func<ToggleChangedModel, Task> OnToggleGpioAsnc;

        public event Action<ToggleChangedModel> OnGpioValueChanged;
        public event Func<ToggleChangedModel, Task> OnGpioValueChangedAsync;
       
        public void ToggleGpio(ToggleChangedModel data)
        {
            if (OnToggleGpio != null)
            {
                OnToggleGpio.Invoke(data);
            }
        }

        public async Task ToggleGpioAsync(ToggleChangedModel data)
        {
            if (OnToggleGpioAsnc != null)
            {
                await OnToggleGpioAsnc.Invoke(data);
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
    }
}
