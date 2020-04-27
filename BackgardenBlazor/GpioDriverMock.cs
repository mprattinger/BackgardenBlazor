using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BackgardenBlazor
{
    public class GpioDriverMock : GpioDriver
    {
        private Dictionary<int, PinValue> _gpioStatus = new Dictionary<int, PinValue>();
        private Dictionary<int, PinMode> _gpioPinMode = new Dictionary<int, PinMode>();

        protected override int PinCount => 0;

        protected override void AddCallbackForPinValueChangedEvent(int pinNumber, PinEventTypes eventTypes, PinChangeEventHandler callback) { }

        protected override void ClosePin(int pinNumber) { }

        protected override int ConvertPinNumberToLogicalNumberingScheme(int pinNumber) => 0;

        protected override PinMode GetPinMode(int pinNumber)
        {
            if(_gpioPinMode.ContainsKey(pinNumber))
            {
                return _gpioPinMode[pinNumber];
            } else
            {
                return default(PinMode);
            }
        }

        protected override bool IsPinModeSupported(int pinNumber, PinMode mode) => true;

        protected override void OpenPin(int pinNumber) 
        {
            if (!_gpioPinMode.ContainsKey(pinNumber))
            {
                _gpioPinMode.Add(pinNumber, default(PinMode));
            }
        }

        protected override PinValue Read(int pinNumber)
        {
            if(_gpioStatus.ContainsKey(pinNumber))
            {
                return _gpioStatus[pinNumber];
            } else
            {
                return PinValue.Low;
            }
        }

        protected override void RemoveCallbackForPinValueChangedEvent(int pinNumber, PinChangeEventHandler callback) { }

        protected override void SetPinMode(int pinNumber, PinMode mode) 
        {
            if (_gpioPinMode.ContainsKey(pinNumber))
            {
                _gpioPinMode[pinNumber] = mode;
            } else
            {
                _gpioPinMode.Add(pinNumber, mode);
            }
        }

        protected override WaitForEventResult WaitForEvent(int pinNumber, PinEventTypes eventTypes, CancellationToken cancellationToken) => default(WaitForEventResult);

        protected override void Write(int pinNumber, PinValue value) {
            if (_gpioStatus.ContainsKey(pinNumber))
            {
                _gpioStatus[pinNumber] = value;
            } else
            {
                _gpioStatus.Add(pinNumber, value);
            }
        }
    }
}
