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

            setupGpio();
        }

        public void Dispose()
        {
            try
            {
                _appState.OnToggleGpioAsync -= appState_OnToggleGpioAsync;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error when disposing events: {ex.Message}");
            }
        }

        private void setupGpio()
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
                Task.Run(async () =>
                {
                    await _appState.GpioValueChangedAsync(
                    new ToggleChangedModel
                    {
                        ToggleType = ToggleType.WATERLEVEL,
                        NewValue = val == PinValue.High ? true : false
                    });
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
                var pin = getGpioPin(_gpioSettings, arg.ToggleType);
                _logger.LogDebug($"Change {arg.ToggleType} to {arg.NewValue} on pin {pin}...");

#if Linux
                        _gpioController.Write(pin, arg.NewValue ? PinValue.High : PinValue.Low);
#endif
                await _appState.GpioValueChangedAsync(arg);
            });
        }

        public static int getGpioPin(GpioSettingsConfiguration config, ToggleType toggleType)
        {
            switch (toggleType)
            {
                case ToggleType.WERFER:
                    return config.WerferPin;
                case ToggleType.SPRUEHER:
                    return config.SprueherPin;
                case ToggleType.TROPFER:
                    return config.TropferPin;
                case ToggleType.POWER:
                    return config.PowerPin;
                case ToggleType.PUMP:
                    return config.PumpPin;
                case ToggleType.VALVE:
                    return config.ValvePin;
                case ToggleType.WATERLEVEL:
                    return config.WaterLevelPin;
                case ToggleType.UNKNOWN:
                    return -1;
                default:
                    return -1;
            }
        }
    }
}
