using BackgardenBlazor.Data;
using BackgardenBlazor.Models;
using BackgardenBlazor.Services;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace BackgardenBlazor.Components
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1063:Implement IDisposable Correctly", Justification = "<Pending>")]
    public partial class SprinklerComponent : IDisposable
    {
        [Inject]
        public AppState AppState { get; set; }

        [Inject]
        public SprinklerContext Context { get; set; }

        [Parameter]
        public SprinklerModel Sprinkler { get; set; }

        public bool Enabled { get; set; }

        protected override void OnInitialized()
        {
            AppState.OnGpioValueChanged += AppState_OnGpioValueChanged; 
        }

        private async Task AppState_OnGpioValueChanged(ToggleChangedModel data)
        {
            await InvokeAsync(() =>
            {
                if (data.SprinklerId == Sprinkler.Id)
                {
                    Enabled = data.NewValue;
                    StateHasChanged();
                }
            });
        }

        public async Task ToggleChangedAsync(bool newValue)
        {
            await AppState.ToggleGpio(new ToggleChangedModel { SprinklerId = Sprinkler.Id, NewValue = newValue, ToggleType = ToggleType.SPRINKLER });
        }

        
        public void Dispose()
        {
            AppState.OnGpioValueChanged += AppState_OnGpioValueChanged;
        }
    }
}
