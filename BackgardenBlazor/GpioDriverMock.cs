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
        protected override int PinCount => 0;

        protected override void AddCallbackForPinValueChangedEvent(int pinNumber, PinEventTypes eventTypes, PinChangeEventHandler callback) { }

        protected override void ClosePin(int pinNumber) { }

        protected override int ConvertPinNumberToLogicalNumberingScheme(int pinNumber) => 0;

        protected override PinMode GetPinMode(int pinNumber) => default(PinMode);

        protected override bool IsPinModeSupported(int pinNumber, PinMode mode) => true;

        protected override void OpenPin(int pinNumber) { }

        protected override PinValue Read(int pinNumber) => default(PinValue);

        protected override void RemoveCallbackForPinValueChangedEvent(int pinNumber, PinChangeEventHandler callback) { }

        protected override void SetPinMode(int pinNumber, PinMode mode) { }

        protected override WaitForEventResult WaitForEvent(int pinNumber, PinEventTypes eventTypes, CancellationToken cancellationToken) => default(WaitForEventResult);

        protected override void Write(int pinNumber, PinValue value) {
            Console.WriteLine("Test");
        }
    }
}
