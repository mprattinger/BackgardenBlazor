using BackgardenBlazor.Data;
using BackgardenBlazor.Models;
using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Linq;
using System.Threading.Tasks;

namespace BackgardenBlazor.Services
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1063:Implement IDisposable Correctly", Justification = "<Pending>")]
    public class GpioService : IDisposable
    {
        private readonly AppState _appState;
        //private readonly SprinklerContext _ctx;
        private readonly GpioSettingsConfiguration _gpioSettings;

        private bool _setupDone = false;

        public GpioService(AppState appState, GpioSettingsConfiguration gpioSettings)
        {
             _appState = appState;
            _appState.OnToggleGpio += _appState_OnToggleGpio;

            //_ctx = sprinklerContext;
            _gpioSettings = gpioSettings;
        }

        public void Dispose()
        {
            _appState.OnToggleGpio -= _appState_OnToggleGpio;
        }

        public void SetupGpio()
        {
            if (_setupDone) return;
            //try
            //{
            //    _gpioController.OpenPin(_gpioSettings.PowerPin, PinMode.Output);
            //    _gpioController.OpenPin(_gpioSettings.PumpPin, PinMode.Output);
            //    _gpioController.OpenPin(_gpioSettings.ValvePin, PinMode.Output);

            //    _gpioController.OpenPin(_gpioSettings.WaterLevelPin, PinMode.Input);
            //    _gpioController.RegisterCallbackForPinValueChangedEvent(_gpioSettings.WaterLevelPin, PinEventTypes.None, (o, e) => {
            //        Console.WriteLine($"Waterlevel changed!");
            //    });

            //    _ctx.Sprinklers.ToList().ForEach(x => {
            //        _gpioController.OpenPin(x.GpioPort, PinMode.Output);
            //    });
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine($"Error setting up gpio controller: {ex.Message}");
            //}

#if IsWindows
#else
            //    using (var controller = new GpioController())
            //{
            //controller.OpenPin(arg.SprinklerId, PinMode.Output);
            //controller.OpenPin(arg.SprinklerId, PinMode.Output);
            //controller.OpenPin(arg.SprinklerId, PinMode.Output);
            //controller.OpenPin(arg.SprinklerId, PinMode.Output);
            //}
#endif
            _setupDone = true;
        }

        private async Task _appState_OnToggleGpio(ToggleChangedModel arg)
        {
            await Task.Run(async () => {
                Console.WriteLine($"Change Sprinkler {arg.SprinklerId}");
#if Windows
                using (var controller = new GpioController(PinNumberingScheme.Logical, new GpioDriverMock()))
#else
                using (var controller = new GpioController())
#endif
                {
                    switch (arg.ToggleType)
                    {
                        case ToggleType.SPRINKLER:
#if Linux
                            controller.OpenPin(arg.SprinklerId, PinMode.Output);
                            controller.Write(arg.SprinklerId, arg.NewValue ? PinValue.High : PinValue.Low);
#endif
                            await _appState.GpioValueChanged(arg);
                            break;
                        case ToggleType.POWER:
#if Linux
                            controller.OpenPin(arg.SprinklerId, PinMode.Output);
                            controller.Write(arg.SprinklerId, arg.NewValue ? PinValue.High : PinValue.Low);
#endif
                            await _appState.GpioValueChanged(arg);
                            break;
                        case ToggleType.PUMP:
#if Linux
                            controller.OpenPin(arg.SprinklerId, PinMode.Output);
                            controller.Write(arg.SprinklerId, arg.NewValue ? PinValue.High : PinValue.Low);
#endif
                            await _appState.GpioValueChanged(arg);
                            break;
                        case ToggleType.VALVE:
#if Linux
#else
                            controller.OpenPin(arg.SprinklerId, PinMode.Output);
                            controller.Write(arg.SprinklerId, arg.NewValue ? PinValue.High : PinValue.Low);
#endif
                            await _appState.GpioValueChanged(arg);
                            break;
                        default:
                            break;
                    }
                }
            });
        }
    }
}
