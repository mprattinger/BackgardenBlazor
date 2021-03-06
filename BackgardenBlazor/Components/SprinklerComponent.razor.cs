﻿using BackgardenBlazor.Models;
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
        public SprinklerService SprinklerService { get; set; }

        [Parameter]
        public ToggleChangedModel Sprinkler { get; set; }

        public string Description { get; set; } = string.Empty;

        public bool Enabled { get; set; }

        public string Message { get; set; } = string.Empty;

        public bool IsSequenceRunning { get; set; } = false;

        protected override void OnInitialized()
        {
            //AppState.OnGpioValueChangedAsync += appState_OnGpioValueChanged;
            AppState.OnSprinklerMessage += appState_OnSprinklerMessage;

            switch (Sprinkler.ToggleType)
            {
                case ToggleType.WERFER:
                    Description = "Werfer";
                    break;
                case ToggleType.SPRUEHER:
                    Description = "Sprüher";
                    break;
                case ToggleType.TROPFER:
                    Description = "Tropfer";
                    break;
                case ToggleType.POWER:
                    Description = "Einschalten";
                    break;
                case ToggleType.PUMP:
                    Description = "Pumpe";
                    break;
                case ToggleType.VALVE:
                    Description = "Ventil";
                    break;
                case ToggleType.WATERLEVEL:
                    Description = "Wasser Level";
                    break;
                case ToggleType.UNKNOWN:
                    Description = "Unbekannt";
                    break;
                default:
                    break;
            }
        }

        private async Task appState_OnSprinklerMessage(ToggleType toogleType, string msg)
        {
            await InvokeAsync(() =>
            {
                if (toogleType == Sprinkler.ToggleType)
                {
                    Message = msg;
                    StateHasChanged();
                }
            });
        }

        private async Task appState_OnGpioValueChanged(ToggleChangedModel data)
        {
            await InvokeAsync(() =>
            {
                if (data.ToggleType == Sprinkler.ToggleType)
                {
                    Enabled = data.NewValue;
                    StateHasChanged();
                }
            });
        }

        public async Task ToggleChangedAsync(bool newValue)
        {
            Sprinkler.NewValue = newValue;
            IsSequenceRunning = true;
            StateHasChanged();
            await SprinklerService.RunSprinklerSequence(Sprinkler);
            Enabled = Sprinkler.NewValue;
            IsSequenceRunning = false;
            StateHasChanged();

            //await AppState.ToggleGpioAsync(Sprinkler);
        }

        
        public void Dispose()
        {
            //AppState.OnGpioValueChangedAsync -= appState_OnGpioValueChanged;
            AppState.OnSprinklerMessage -= appState_OnSprinklerMessage;
        }
    }
}
