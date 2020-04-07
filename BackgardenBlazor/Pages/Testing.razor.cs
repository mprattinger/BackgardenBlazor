using BackgardenBlazor.Models;
using BackgardenBlazor.Services;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace BackgardenBlazor.Pages
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1063:Implement IDisposable Correctly", Justification = "<Pending>")]
    public partial class Testing : IDisposable
    {
        [Inject]
        public AppState AppState { get; set; }

        [Inject]
        public GpioSettingsConfiguration GpioSettings { get; set; }

        public bool PowerEnabled { get; set; }

        public bool ValveEnabled { get; set; }

        public bool PumpOn { get; set; }

        protected override void OnInitialized()
        {
            AppState.OnGpioValueChanged += AppState_OnGpioValueChanged;
        }

        private async Task AppState_OnGpioValueChanged(ToggleChangedModel arg)
        {
            await InvokeAsync(() =>
            {
                switch (arg.ToggleType)
                {
                    case ToggleType.SPRINKLER:
                        break;
                    case ToggleType.POWER:
                        PowerEnabled = arg.NewValue;
                        break;
                    case ToggleType.PUMP:
                        PumpOn = arg.NewValue;
                        break;
                    case ToggleType.VALVE:
                        ValveEnabled = arg.NewValue;
                        break;
                    default:
                        break;
                }
            });
        }

        public async Task PowerToggled(bool newValue)
        {
            await AppState.ToggleGpio(new ToggleChangedModel { SprinklerId = GpioSettings.PowerPin, NewValue = newValue, ToggleType = ToggleType.POWER });
        }

        public async Task ValveToggled(bool newValue)
        {
            await AppState.ToggleGpio(new ToggleChangedModel { SprinklerId = GpioSettings.ValvePin, NewValue = newValue, ToggleType = ToggleType.VALVE });
        }

        public async Task PumpToggled(bool newValue)
        {
            await AppState.ToggleGpio(new ToggleChangedModel { SprinklerId = GpioSettings.PumpPin, NewValue = newValue, ToggleType = ToggleType.PUMP });
        }

        public void Dispose()
        {
            AppState.OnGpioValueChanged -= AppState_OnGpioValueChanged;
        }
    }
}
