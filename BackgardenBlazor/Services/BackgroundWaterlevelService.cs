using BackgardenBlazor.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Device.Gpio;
using System.Threading;
using System.Threading.Tasks;

namespace BackgardenBlazor.Services
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1063:Implement IDisposable Correctly", Justification = "<Pending>")]
    public class BackgroundWaterlevelService : IHostedService, IDisposable
    {
        private readonly ILogger<BackgroundWaterlevelService> _logger;
        private readonly AppState _appState;
        private readonly GpioController _gpioController;
        private readonly GpioSettingsConfiguration _gpioSettings;
        private Timer _timer;

        public BackgroundWaterlevelService(ILogger<BackgroundWaterlevelService> logger, AppState appState, GpioController gpioController, GpioSettingsConfiguration gpioSettings)
        {
            _logger = logger;
            _appState = appState;
            _gpioController = gpioController;
            _gpioSettings = gpioSettings;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"BackgroundWaterlevelService started");
            _timer = new Timer(doWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"BackgroundWaterlevelService stopped");
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        private void doWork(object state)
        {
            try
            {
                if (_gpioController.IsPinOpen(_gpioSettings.WaterLevelPin))
                {
                    var val = _gpioController.Read(_gpioSettings.WaterLevelPin);
                    _logger.LogDebug($"Waterlevel is: {val}");

                    _appState.GpioValueChanged(
                        new ToggleChangedModel
                        {
                            GpioPin = _gpioSettings.WaterLevelPin,
                            ToggleType = ToggleType.WATERLEVEL,
                            NewValue = val == PinValue.High ? true : false
                        });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error reading WaterLevel: {ex.Message}");
            }
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
