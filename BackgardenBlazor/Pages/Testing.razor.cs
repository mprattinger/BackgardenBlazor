using BackgardenBlazor.Models;
using BackgardenBlazor.Services;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
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

        public bool WaterLevelOk { get; set; } = false;

        public bool WerferEnabled { get; set; }

        public bool SprueherEnabled { get; set; }

        public bool TropferEnabled { get; set; }

        public List<ToggleChangedModel> Sprinklers { get; set; } = new List<ToggleChangedModel>();

        protected override void OnInitialized()
        {
            AppState.OnGpioValueChangedAsync += appState_OnGpioValueChangedAsync;
            Sprinklers.Add(new ToggleChangedModel { ToggleType = ToggleType.WERFER });
            Sprinklers.Add(new ToggleChangedModel { ToggleType = ToggleType.SPRUEHER });
            Sprinklers.Add(new ToggleChangedModel { ToggleType = ToggleType.TROPFER });
        }

        private async Task appState_OnGpioValueChangedAsync(ToggleChangedModel arg)
        {
            await InvokeAsync(() =>
            {
                switch (arg.ToggleType)
                {
                    case ToggleType.WERFER:
                        WerferEnabled = arg.NewValue;
                        break;
                    case ToggleType.SPRUEHER:
                        SprueherEnabled = arg.NewValue;
                        break;
                    case ToggleType.TROPFER:
                        TropferEnabled = arg.NewValue;
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
                    case ToggleType.WATERLEVEL:
                        WaterLevelOk = arg.NewValue;
                        break;
                    default:
                        break;
                }
                StateHasChanged();
            });
        }

        public async Task PowerToggled(bool newValue)
        {
            await AppState.ToggleGpioAsync(new ToggleChangedModel { NewValue = newValue, ToggleType = ToggleType.POWER });
        }

        public async Task ValveToggled(bool newValue)
        {
            await AppState.ToggleGpioAsync(new ToggleChangedModel { NewValue = newValue, ToggleType = ToggleType.VALVE });
        }

        public async Task WerferToggled(bool newValue)
        {
            await AppState.ToggleGpioAsync(new ToggleChangedModel { NewValue = newValue, ToggleType = ToggleType.WERFER });
        }

        public async Task SprueherToggled(bool newValue)
        {
            await AppState.ToggleGpioAsync(new ToggleChangedModel { NewValue = newValue, ToggleType = ToggleType.SPRUEHER });
        }

        public async Task TropferToggled(bool newValue)
        {
            await AppState.ToggleGpioAsync(new ToggleChangedModel { NewValue = newValue, ToggleType = ToggleType.TROPFER });
        }

        public async Task PumpToggled(bool newValue)
        {
            await AppState.ToggleGpioAsync(new ToggleChangedModel { NewValue = newValue, ToggleType = ToggleType.PUMP });
        }

        public void Dispose()
        {
            AppState.OnGpioValueChangedAsync -= appState_OnGpioValueChangedAsync;
        }
    }
}
