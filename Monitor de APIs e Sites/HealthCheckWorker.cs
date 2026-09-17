using System.Diagnostics;
using Microsoft.AspNetCore.SignalR;
using MonitorApp.Hubs;
using MonitorApp.Models;

namespace MonitorApp.Services;

public class HealthCheckWorker : BackgroundService
{
    private readonly IHubContext<MonitorHub> _hubContext;
    private readonly IHttpClientFactory _httpClientFactory;

    // Lista de sites monitorados em memória
    public static readonly List<SiteMonitor> Sites = new()
    {
        new SiteMonitor { Id = 1, Nome = "Google", Url = "https://www.google.com" },
        new SiteMonitor { Id = 2, Nome = "GitHub", Url = "https://github.com" },
        new SiteMonitor { Id = 3, Nome = "API Inexistente (Teste Erro)", Url = "https://site-invalido-teste12345.com" }
    };

    public HealthCheckWorker(IHubContext<MonitorHub> hubContext, IHttpClientFactory httpClientFactory)
    {
        _hubContext = hubContext;
        _httpClientFactory = httpClientFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var client = _httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(3);

            foreach (var site in Sites)
            {
                var stopwatch = Stopwatch.StartNew();
                bool isOnline = false;

                try
                {
                    var response = await client.GetAsync(site.Url, stoppingToken);
                    isOnline = response.IsSuccessStatusCode;
                }
                catch
                {
                    isOnline = false;
                }
                finally
                {
                    stopwatch.Stop();
                }

                site.StatusOnline = isOnline;
                site.UltimaLatenciaMs = isOnline ? stopwatch.ElapsedMilliseconds : 0;
                site.UltimaVerificacao = DateTime.Now;

                var resultado = new ChecagemResultado
                {
                    SiteId = site.Id,
                    Nome = site.Nome,
                    Online = site.StatusOnline,
                    LatenciaMs = site.UltimaLatenciaMs,
                    Timestamp = site.UltimaVerificacao
                };

                
                await _hubContext.Clients.All.SendAsync("AtualizacaoSite", resultado, stoppingToken);
            }

   
            await Task.Delay(5000, stoppingToken);
        }
    }
}