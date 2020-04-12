using BackgardenBlazor.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Linq;
using System.Threading.Tasks;

namespace BackgardenBlazor.Services
{
    public class SprinklerService
    {
        private readonly ILogger<SprinklerService> _logger;
        private readonly GpioSettingsConfiguration _gpioSettings;
        private readonly GpioController _gpioController;
        private readonly AppState _appState;

        public SprinklerService(ILogger<SprinklerService> logger, GpioSettingsConfiguration gpioSettings, GpioController gpioController, AppState appState)
        {
            _logger = logger;
            _gpioSettings = gpioSettings;
            _gpioController = gpioController;
            _appState = appState;
        }

        public async Task RunSprinklerSequence(ToggleChangedModel toggleChangedModel)
        {
            _logger.LogDebug($"Running sequence for sprinkler {toggleChangedModel.ToggleType}...");
            if(toggleChangedModel.NewValue)
            {
                //Einschalten
                _logger.LogDebug($"Sequence is for activating {toggleChangedModel.ToggleType}!");
                await activationSequence(toggleChangedModel);
            } else
            {
                //Ausschalten
                _logger.LogDebug($"Sequence is for deactivating {toggleChangedModel.ToggleType}!");
                await deactivationSequence(toggleChangedModel);
            }
        }

        private async Task activationSequence(ToggleChangedModel toggleChanged)
        {
            try
            {
                var sPin = GpioService.getGpioPin(_gpioSettings, toggleChanged.ToggleType);
                //Sequence is;
                // Power on -> Valve in correct position -> Sprinkler on -> Pump on
                _logger.LogDebug("Checking if ports are ready...");
                if(_gpioController.IsPinOpen(_gpioSettings.PowerPin) &&
                   _gpioController.IsPinOpen(_gpioSettings.ValvePin) &&
                   _gpioController.IsPinOpen(sPin) &&
                   _gpioController.IsPinOpen(_gpioSettings.PumpPin))
                {
                    _logger.LogDebug("Power on...");
#if Linux
                    _gpioController.Write(_gpioSettings.PowerPin, PinValue.High);
#endif
                    await notifyFrontend(ToggleType.POWER, true);
                    _logger.LogDebug("Power is on! Checking for valve...");
                    await _appState.SprinklerMessageAsync(toggleChanged.ToggleType, "Power on");
                    var val = _gpioController.Read(_gpioSettings.WaterLevelPin);
                    if(val == PinValue.High)
                    {
                        _logger.LogDebug("Waterlevel is good! Switching to reservoir...");
#if Linux
                        _gpioController.Write(_gpioSettings.ValvePin, PinValue.High);
#endif
                        await notifyFrontend(ToggleType.VALVE, true);
                        _logger.LogDebug("Valve switched! Waiting until valve is in position...");
                        await _appState.SprinklerMessageAsync(toggleChanged.ToggleType, "Switching valve...");
                        await Task.Delay(Convert.ToInt32(TimeSpan.FromSeconds(Convert.ToDouble(_gpioSettings.ValveDelay)).TotalMilliseconds));
                        await _appState.SprinklerMessageAsync(toggleChanged.ToggleType, "Valve on");
                        _logger.LogDebug("Valve is in position! Switching sprinkler on...");
#if Linux
                        _gpioController.Write(sPin, PinValue.High);
#endif
                        await notifyFrontend(toggleChanged.ToggleType, true);
                        await _appState.SprinklerMessageAsync(toggleChanged.ToggleType, $"Sprinkler {toggleChanged.ToggleType} on");
                        _logger.LogDebug("Sprinkler is on! Activating pump...");
#if Linux
                        _gpioController.Write(_gpioSettings.PumpPin, PinValue.High);
#endif
                        await notifyFrontend(ToggleType.PUMP, true);
                        await _appState.SprinklerMessageAsync(toggleChanged.ToggleType, "Pump on");
                        _logger.LogDebug("Pump is on!");
                    } else
                    {
                        _logger.LogDebug("Waterlevel is bad! Switching to water pipe...");
#if Linux
                        _gpioController.Write(_gpioSettings.ValvePin, PinValue.Low);
#endif
                        _logger.LogDebug("Valve switched! Waiting until valve is in position...");
                        await _appState.SprinklerMessageAsync(toggleChanged.ToggleType, "Switching valve...");
                        await notifyFrontend(ToggleType.VALVE, false);
                        await Task.Delay(Convert.ToInt32(TimeSpan.FromSeconds(Convert.ToDouble(_gpioSettings.ValveDelay)).TotalMilliseconds));
                        await _appState.SprinklerMessageAsync(toggleChanged.ToggleType, "Valve off");
                        _logger.LogDebug("Valve is in position! Switching sprinkler on...");
#if Linux
                        _gpioController.Write(sPin, PinValue.High);
#endif
                        await notifyFrontend(toggleChanged.ToggleType, true);
                        await _appState.SprinklerMessageAsync(toggleChanged.ToggleType, $"Sprinkler {toggleChanged.ToggleType} on");
                        _logger.LogDebug("Sprinkler is on!");
                    }
                } else
                {
                    var msg = $"Error a pin is not open: \n\tPower: {_gpioController.IsPinOpen(_gpioSettings.PowerPin)}";
                    msg = $"{msg}\n\tValve: {_gpioController.IsPinOpen(_gpioSettings.ValvePin)}";
                    msg = $"{msg}\n\tSprinkler: {_gpioController.IsPinOpen(sPin)}";
                    msg = $"{msg}\n\tPumpe: {_gpioController.IsPinOpen(_gpioSettings.PumpPin)}";
                    throw new Exception(msg);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error activating sequence for {toggleChanged.ToggleType}: {ex.Message}!");
            }
        }

        private async Task deactivationSequence(ToggleChangedModel toggleChanged)
        {
            try
            {
                var sPin = GpioService.getGpioPin(_gpioSettings, toggleChanged.ToggleType);
                //Sequence is;
                // Pump off and wait until pressure is gone -> Sprinkler off -> Valve to water pipe and wait until in position -> Power off
                _logger.LogDebug("Checking if ports are ready...");
                if (_gpioController.IsPinOpen(_gpioSettings.PowerPin) &&
                   _gpioController.IsPinOpen(_gpioSettings.ValvePin) &&
                   _gpioController.IsPinOpen(sPin) &&
                   _gpioController.IsPinOpen(_gpioSettings.PumpPin))
                {
                    _logger.LogDebug("Pump off and wait until pressure is gone...");
#if Linux
                    _gpioController.Write(_gpioSettings.PumpPin, PinValue.Low);
#endif
                    await _appState.SprinklerMessageAsync(toggleChanged.ToggleType, "Pump off! Waiting...");
                    await Task.Delay(Convert.ToInt32(TimeSpan.FromSeconds(Convert.ToDouble(_gpioSettings.PumpDelay)).TotalMilliseconds));
                    await notifyFrontend(ToggleType.PUMP, false);
                    await _appState.SprinklerMessageAsync(toggleChanged.ToggleType, "Pump off");
                    _logger.LogDebug("Pump is off! Setting valve to water pipe...");
#if Linux
                    _gpioController.Write(_gpioSettings.ValvePin, PinValue.Low);
#endif
                    await notifyFrontend(toggleChanged.ToggleType, false);
                    await _appState.SprinklerMessageAsync(toggleChanged.ToggleType, "Switching valve...");
                    _logger.LogDebug("Valve switched! Waiting until valve is in position...");
                    await Task.Delay(Convert.ToInt32(TimeSpan.FromSeconds(Convert.ToDouble(_gpioSettings.ValveDelay)).TotalMilliseconds));
                    await notifyFrontend(ToggleType.VALVE, false);
                    await _appState.SprinklerMessageAsync(toggleChanged.ToggleType, "Valve off");
                    _logger.LogDebug("Valve is in position! Switching power off...");
#if Linux
                    _gpioController.Write(_gpioSettings.PowerPin, PinValue.Low);
#endif
                    await notifyFrontend(ToggleType.POWER, false);
                    await _appState.SprinklerMessageAsync(toggleChanged.ToggleType, "Power off");
                    _logger.LogDebug("Sprinkler is off!");
                }
                else
                {
                    var msg = $"Error a pin is not open: \n\tPower: {_gpioController.IsPinOpen(_gpioSettings.PowerPin)}";
                    msg = $"{msg}\n\tValve: {_gpioController.IsPinOpen(_gpioSettings.ValvePin)}";
                    msg = $"{msg}\n\tSprinkler: {_gpioController.IsPinOpen(sPin)}";
                    msg = $"{msg}\n\tPumpe: {_gpioController.IsPinOpen(_gpioSettings.PumpPin)}";
                    throw new Exception(msg);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deactivating sequence for {toggleChanged.ToggleType}: {ex.Message}!");
            }
        }

        private async Task notifyFrontend(ToggleType toggleType, bool onoff)
        {
            var model = new ToggleChangedModel { ToggleType = toggleType, NewValue = onoff };
            await _appState.GpioValueChangedAsync(model);
        }
    }
}
