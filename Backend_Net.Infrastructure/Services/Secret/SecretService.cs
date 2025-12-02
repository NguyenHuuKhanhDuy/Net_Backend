using System.Collections.Concurrent;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Backend_Net.Application.Common.Interfaces.Azure;
using Backend_Net.Infrastructure.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.Extensions;

namespace Backend_Net.Infrastructure.Services.Secret;

public class SecretService : ISecretService
{
    private readonly SecretClient _secretClient;
    private readonly ILogger<SecretService> _logger;
    private readonly AzureOptions _azureOptions;

    private static readonly ConcurrentDictionary<string, string> _secretCache = new();

    public SecretService
    (
        ILogger<SecretService> logger,
        IOptions<AzureOptions> azureOptions
    )
    {
        _logger = logger;
        _azureOptions = azureOptions.Value;
        
        var keyVaultSetting = _azureOptions.KeyVault;

        var credential = new ClientSecretCredential(
            tenantId: keyVaultSetting.TenantId,
            clientId: keyVaultSetting.ClientId,
            clientSecret: keyVaultSetting.ClientSecret
        );

        _secretClient = new SecretClient(
            new Uri(keyVaultSetting.Url),
            credential
        );
    }

    public async Task<string> GetSecretAsync(string keyId, CancellationToken cancellationToken)
    {
        var functionName = $"{nameof(SecretService)} {nameof(GetSecretAsync)} KeyId = {keyId} => ";
        _logger.LogInformation(functionName);

        if (_secretCache.TryGetValue(keyId, out var cachedSecret))
        {
            return cachedSecret;
        }

        try
        {
            KeyVaultSecret secret = await _secretClient.GetSecretAsync(keyId, cancellationToken: cancellationToken);
            var value = secret.Value;

            _secretCache[keyId] = value;
            return value;
        }
        catch (Exception ex)
        {
            ex.LogError(_logger, functionName);
            return string.Empty;
        }
    }
    
    public async Task<bool> SetSecretAsync(string keyId, string secretValue, CancellationToken cancellationToken)
    {
        var functionName = $"{nameof(SecretService)} {nameof(SetSecretAsync)} KeyId = {keyId} => ";
        _logger.LogInformation($"{functionName} START");

        try
        {
            KeyVaultSecret secret = new(keyId, secretValue);
            await _secretClient.SetSecretAsync(secret, cancellationToken);
            _secretCache[keyId] = secretValue;
            
            return true;
        }
        catch (Exception ex)
        {
            ex.LogError(_logger, functionName);
            return false;
        }
    }
}
