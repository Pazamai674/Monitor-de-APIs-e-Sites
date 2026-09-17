using Microsoft.AspNetCore.Mvc;
using MonitorApp.Models;
using MonitorApp.Services;

namespace MonitorApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SitesController : ControllerBase
{
    [HttpGet]
    public IActionResult GetSites()
    {
        return Ok(HealthCheckWorker.Sites);
    }

    [HttpPost]
    public IActionResult AddSite([FromBody] SiteMonitor novoSite)
    {
        novoSite.Id = HealthCheckWorker.Sites.Count + 1;
        HealthCheckWorker.Sites.Add(novoSite);
        return CreatedAtAction(nameof(GetSites), new { id = novoSite.Id }, novoSite);
    }
}