using BackgardenBlazor.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Device.Gpio;
using System.Threading.Tasks;

namespace BackgardenBlazor.Services
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1063:Implement IDisposable Correctly", Justification = "<Pending>")]
    public class GpioService : IDisposable
    {
        private readonly AppState _appState;
        //private readonly SprinklerContext _ctx;
        private readonly GpioSettingsConfiguration _gpioSettings;
        private readonly GpioController _gpioController;
        private readonly ILogger<GpioService> _logger;

        public GpioService(ILogger<GpioService> logger, AppState appState, GpioSettingsConfiguration gpioSettings, GpioController gpioController)
        {
            _appState = appState;
            _appState.OnToggleGpioAsync += appState_OnToggleGpioAsync;

            //_ctx = sprinklerContext;
            _gpioSettings = gpioSettings;
            _gpioController = gpioController;
            _logger = logger;

            SetupGpio();

            _logger.LogDebug($"Register Waterlevel callbacks...");
            try
            {
                System.Threading.Thread.Sleep(1000);
                _gpioController.RegisterCallbackForPinValueChangedEvent(_gpioSettings.WaterLevelPin, PinEventTypes.Rising, waterLevelOn);
                _gpioController.RegisterCallbackForPinValueChangedEvent(_gpioSettings.WaterLevelPin, PinEventTypes.Falling, waterLevelOff);
                _logger.LogDebug($"Waterlevel callbacks registered!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error when registering waterlevel callback events: {ex.Message}!");
            }
        }

        public void Dispose()
        {
            try
            {
                _appState.OnToggleGpioAsync -= appState_OnToggleGpioAsync;
                _gpioController.UnregisterCallbackForPinValueChangedEvent(_gpioSettings.WaterLevelPin, waterLevelOn);
                _gpioController.UnregisterCallbackForPinValueChangedEvent(_gpioSettings.WaterLevelPin, waterLevelOff);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error when disposing events: {ex.Message}");
            }
        }

        public void SetupGpio()
        {
            _logger.LogDebug($"Setting-Up Gpio's...");
            if (!_gpioController.IsPinOpen(_gpioSettings.PowerPin))
            {
                _gpioController.OpenPin(_gpioSettings.PowerPin, PinMode.Output);
            }
            if (!_gpioController.IsPinOpen(_gpioSettings.ValvePin))
            {
                _gpioController.OpenPin(_gpioSettings.ValvePin, PinMode.Output);
            }
            if (!_gpioController.IsPinOpen(_gpioSettings.PumpPin))
            {
                _gpioController.OpenPin(_gpioSettings.PumpPin, PinMode.Output);
            }
            if (!_gpioController.IsPinOpen(_gpioSettings.WaterLevelPin))
            {
                _logger.LogDebug($"Opening Waterlevel pin {_gpioSettings.WaterLevelPin}...");
                _gpioController.OpenPin(_gpioSettings.WaterLevelPin);
                _logger.LogDebug("Waterlevel pin is open! Set mode to input...");
                _gpioController.SetPinMode(_gpioSettings.WaterLevelPin, PinMode.InputPullDown);
                _logger.LogDebug("Waterlevel pin mode is input! Reading value...");
                //_gpioController.OpenPin(_gpioSettings.WaterLevelPin, PinMode.InputPullDown);
                var val = _gpioController.Read(_gpioSettings.WaterLevelPin);
                _logger.LogDebug($"Waterlevel value is {val}!");
                _appState.GpioValueChanged(
                    new ToggleChangedModel
                    {
                        GpioPin = _gpioSettings.WaterLevelPin,
                        ToggleType = ToggleType.WATERLEVEL,
                        NewValue = val == PinValue.High ? true : false
                    });
            }
            if (!_gpioController.IsPinOpen(_gpioSettings.WerferPin))
            {
                _gpioController.OpenPin(_gpioSettings.WerferPin, PinMode.Output);
            }
            if (!_gpioController.IsPinOpen(_gpioSettings.SprueherPin))
            {
                _gpioController.OpenPin(_gpioSettings.SprueherPin, PinMode.Output);
            }
            if (!_gpioController.IsPinOpen(_gpioSettings.TropferPin))
            {
                _gpioController.OpenPin(_gpioSettings.TropferPin, PinMode.Output);
            }
            _logger.LogDebug($"Gpio setup finished!");
        }

        private async Task appState_OnToggleGpioAsync(ToggleChangedModel arg)
        {
            await Task.Run(async () =>
            {
                _logger.LogDebug($"Change GpioPin {arg.GpioPin}");

#if Linux
                        _gpioController.Write(arg.GpioPin, arg.NewValue ? PinValue.High : PinValue.Low);
#endif
                await _appState.GpioValueChangedAsync(arg);
            });
        }

        private void waterLevelOn(object o, PinValueChangedEventArgs args)
        {
            _appState.GpioValueChanged(new ToggleChangedModel { GpioPin = _gpioSettings.WaterLevelPin, ToggleType = ToggleType.WATERLEVEL, NewValue = true });
        }
        private void waterLevelOff(object o, PinValueChangedEventArgs args)
        {
            _appState.GpioValueChanged(new ToggleChangedModel { GpioPin = _gpioSettings.WaterLevelPin, ToggleType = ToggleType.WATERLEVEL, NewValue = false });
        }
    }
}
