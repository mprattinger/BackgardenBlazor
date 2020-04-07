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
        public ISprinklerService SprinklerService { get; set; }

        [Inject]
        public AppState AppState { get; set; }

        public bool Enabled { get; set; }

        public List<SprinklerModel> Sprinklers { get; set; } = new List<SprinklerModel>();

        protected override async Task OnInitializedAsync()
        {
            Sprinklers = await SprinklerService.LoadSprinklersAsync();
            AppState.OnSprinklerStateChanged += AppState_OnSprinklerStateChanged;
        }

        public async Task ToggleChangedAsync(bool newValue)
        {
            Enabled = newValue;
            await AppState.SprinklerStateChanged(new ToggleChangedModel { NewValue = newValue, ToggleType = ToggleType.PUMP });
        }

        private async Task AppState_OnSprinklerStateChanged(ToggleChangedModel data)
        {
            await InvokeAsync(() =>
            {
                if (data.ToggleType == ToggleType.PUMP)
                {
                    Enabled = data.NewValue;
                    StateHasChanged();
                }
            });
        }

        public void Dispose()
        {
            AppState.OnSprinklerStateChanged -= AppState_OnSprinklerStateChanged;
        }
    }
}
