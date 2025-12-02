namespace Backend_Net.Application.Common.Interfaces.Azure;

public interface ISecretService
{
    Task<string> GetSecretAsync(string keyId, CancellationToken cancellationToken);
    Task<bool> SetSecretAsync(string keyId, string secretValue, CancellationToken cancellationToken);
}