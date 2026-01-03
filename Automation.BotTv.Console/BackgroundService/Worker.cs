using Microsoft.Extensions.Hosting;

namespace Automation.BotTv.Console
{
    public class Worker : IHostedService
    {
        #region Services
        private readonly AutomationService _automationService;
        private Timer? _timer;
        #endregion

        #region Constructor
        public Worker(AutomationService automationService)
        {
            _automationService = automationService;
        }
        #endregion

        #region Methods

        #region StartAsync
        public Task StartAsync(CancellationToken cancellationToken)
        {
            var intervaloEmSegundos = 30;
            _timer = new Timer(Execute, null, TimeSpan.Zero, TimeSpan.FromSeconds(intervaloEmSegundos));

            System.Console.WriteLine("Serviço de Automação iniciado. Verificando painéis...");
            System.Console.WriteLine();
            return Task.CompletedTask;
        }
        #endregion

        #region Execute
        private void Execute(object? state)
        {
            Task.Run(() => _automationService.Execute());
        }
        #endregion

        #region StopAsync
        public Task StopAsync(CancellationToken cancellationToken)
        {
            System.Console.WriteLine("Serviço de automação sendo encerrado...");
            _timer?.Change(Timeout.Infinite, 0);
            _automationService.QuitBrowser();
            return Task.CompletedTask;
        }
        #endregion

        #region Dispose
        public void Dispose()
        {
            _timer?.Dispose();
        }
        #endregion

        #endregion
    }
}