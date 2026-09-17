namespace MonitorApp.Models;

public class SiteMonitor
{
    public int Id { get; set; }
    public string Url { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public bool StatusOnline { get; set; }
    public long UltimaLatenciaMs { get; set; }
    public DateTime UltimaVerificacao { get; set; }
}

public class ChecagemResultado
{
    public int SiteId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public bool Online { get; set; }
    public long LatenciaMs { get; set; }
    public DateTime Timestamp { get; set; }
}