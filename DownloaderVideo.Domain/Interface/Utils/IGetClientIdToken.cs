using Microsoft.AspNetCore.Http;

namespace DownloaderVideo.Domain.Interface.Utils;

/// <summary>
/// Interface para obter o ID do cliente a partir do token.
/// </summary>
public interface IGetClientIdToken
{
    /// <summary>
    /// Obtém o ID do cliente a partir do token no contexto HTTP.
    /// </summary>
    /// <param name="context">O contexto HTTP que contém o token.</param>
    /// <returns>O ID do cliente extraído do token.</returns>
    string GetClientIdFromToken(HttpContext context);
}
