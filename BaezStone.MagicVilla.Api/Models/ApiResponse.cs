using System.Net;

namespace BaezStone.MagicVilla.Api.Models;

public class ApiResponse<T>
{
    public HttpStatusCode StatusCode { get; set; }
    public bool EsExitoso { get; set; } = true;
    public List<string> ErrorMensajes { get; set; } = new();
    public T? Resultado { get; set; }
}
