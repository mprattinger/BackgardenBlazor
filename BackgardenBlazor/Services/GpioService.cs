﻿using BackgardenBlazor.Data;
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
        private readonly GpioController _gpioController;
        private bool _setupDone = false;

        public GpioService(AppState appState, GpioSettingsConfiguration gpioSettings, GpioController gpioController)
        {
            _appState = appState;
            _appState.OnToggleGpio += _appState_OnToggleGpio;

            //_ctx = sprinklerContext;
            _gpioSettings = gpioSettings;
            _gpioController = gpioController;

            setupGpio();
        }

        public void Dispose()
        {
            _appState.OnToggleGpio -= _appState_OnToggleGpio;
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
            await Task.Run(async () =>
            {
                Console.WriteLine($"Change Sprinkler {arg.SprinklerId}");
                switch (arg.ToggleType)
                {
                    case ToggleType.SPRINKLER:
#if Linux
                        _gpioController.Write(arg.SprinklerId, arg.NewValue ? PinValue.High : PinValue.Low);
#endif
                        await _appState.GpioValueChanged(arg);
                        break;
                    case ToggleType.POWER:
                        #if Linux
                        _gpioController.Write(_gpioSettings.PowerPin, arg.NewValue ? PinValue.High : PinValue.Low);
                        #endif
                        await _appState.GpioValueChanged(arg);
                        break;
                    case ToggleType.PUMP:
#if Linux
                            _gpioController.Write(_gpioSettings.PumpPin, arg.NewValue ? PinValue.High : PinValue.Low);
#endif
                        await _appState.GpioValueChanged(arg);
                        break;
                    case ToggleType.VALVE:
#if Linux
                            _gpioController.Write(_gpioSettings.ValvePin, arg.NewValue ? PinValue.High : PinValue.Low);
#endif
                        await _appState.GpioValueChanged(arg);
                        break;
                    default:
                        break;
                }
            });
        }
    }
}
