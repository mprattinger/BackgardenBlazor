using BackgardenBlazor.Models;
using BackgardenBlazor.Services;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackgardenBlazor.Pages
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1063:Implement IDisposable Correctly", Justification = "<Pending>")]
    public partial class Index : IDisposable
    {
        [Inject]
        public AppState AppState { get; set; }

        public bool PumpEnabled { get; set; }

        public List<ToggleChangedModel> Sprinklers { get; set; } = new List<ToggleChangedModel>();

        protected override void OnInitialized()
        {
            Sprinklers.Add(new ToggleChangedModel { ToggleType = ToggleType.WERFER });
            Sprinklers.Add(new ToggleChangedModel { ToggleType = ToggleType.SPRUEHER });
            Sprinklers.Add(new ToggleChangedModel { ToggleType = ToggleType.TROPFER });
            AppState.OnGpioValueChangedAsync += appState_OnGpioValueChanged;
        }

        private async Task appState_OnGpioValueChanged(ToggleChangedModel data)
        {
            await InvokeAsync(() =>
            {
                if (data.ToggleType == ToggleType.PUMP)
                {
                    PumpEnabled = data.NewValue;
                    StateHasChanged();
                }
            });
        }

        public async Task ToggleChangedAsync(bool newValue)
        {
            await AppState.ToggleGpioAsync(new ToggleChangedModel { NewValue = newValue, ToggleType = ToggleType.PUMP });
        }

        public void Dispose()
        {
            AppState.OnGpioValueChangedAsync -= appState_OnGpioValueChanged;
        }
    }
}
