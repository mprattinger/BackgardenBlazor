using BackgardenBlazor.Models;
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

        public GpioService(AppState appState, GpioSettingsConfiguration gpioSettings, GpioController gpioController)
        {
            _appState = appState;
            _appState.OnToggleGpioAsnc += appState_OnToggleGpio;

            //_ctx = sprinklerContext;
            _gpioSettings = gpioSettings;
            _gpioController = gpioController;

            setupGpio();

            _gpioController.RegisterCallbackForPinValueChangedEvent(_gpioSettings.WaterLevelPin, PinEventTypes.Rising, waterLevelOn);
            _gpioController.RegisterCallbackForPinValueChangedEvent(_gpioSettings.WaterLevelPin, PinEventTypes.Falling, waterLevelOff);
        }

        public void Dispose()
        {
            _appState.OnToggleGpioAsnc -= appState_OnToggleGpio;
            _gpioController.UnregisterCallbackForPinValueChangedEvent(_gpioSettings.WaterLevelPin, waterLevelOn);
            _gpioController.UnregisterCallbackForPinValueChangedEvent(_gpioSettings.WaterLevelPin, waterLevelOff);
        }

        private void setupGpio()
        {
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
                _gpioController.OpenPin(_gpioSettings.WaterLevelPin, PinMode.Input);
                var val = _gpioController.Read(_gpioSettings.WaterLevelPin);
                _appState.GpioValueChanged(
                    new ToggleChangedModel { 
                        GpioPin = _gpioSettings.WaterLevelPin, 
                        ToggleType = ToggleType.WATERLEVEL, 
                        NewValue = val == PinValue.High ? true : false });
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
        }

        private async Task appState_OnToggleGpio(ToggleChangedModel arg)
        {
            await Task.Run(async () =>
            {
                Console.WriteLine($"Change GpioPin {arg.GpioPin}");

#if Linux
                        _gpioController.Write(_gpioSettings.GpioPin, arg.NewValue ? PinValue.High : PinValue.Low);
#endif
                await _appState.GpioValueChangedAsync(arg);

                //                switch (arg.ToggleType)
                //                {
                //                    case ToggleType.WERFER:
                //#if Linux
                //                        _gpioController.Write(arg.SprinklerId, arg.NewValue ? PinValue.High : PinValue.Low);
                //#endif
                //                        await _appState.GpioValueChangedAsync(arg);
                //                        break;
                //                    case ToggleType.POWER:
                //#if Linux
                //                        _gpioController.Write(_gpioSettings.PowerPin, arg.NewValue ? PinValue.High : PinValue.Low);
                //#endif
                //                        await _appState.GpioValueChangedAsync(arg);
                //                        break;
                //                    case ToggleType.PUMP:
                //#if Linux
                //                            _gpioController.Write(_gpioSettings.PumpPin, arg.NewValue ? PinValue.High : PinValue.Low);
                //#endif
                //                        await _appState.GpioValueChangedAsync(arg);
                //                        break;
                //                    case ToggleType.VALVE:
                //#if Linux
                //                            _gpioController.Write(_gpioSettings.ValvePin, arg.NewValue ? PinValue.High : PinValue.Low);
                //#endif
                //                        await _appState.GpioValueChangedAsync(arg);
                //                        break;
                //                    default:
                //                        break;
                //                }
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
