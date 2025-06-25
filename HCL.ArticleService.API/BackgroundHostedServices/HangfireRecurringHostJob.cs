using Hangfire;
using HCL.ArticleService.API.BLL.Interfaces;

namespace HCL.ArticleService.API.BackgroundHostedServices
{
    public class HangfireRecurringHostJob : BackgroundService
    {
        private IArticleControllService _articleControllService;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public HangfireRecurringHostJob(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            _articleControllService = scope.ServiceProvider.GetRequiredService<IArticleControllService>();

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(5);
                RecurringJob.AddOrUpdate(() => _articleControllService.UpdateArticlesActualState(), Cron.Daily);
            }
        }
    }
}
