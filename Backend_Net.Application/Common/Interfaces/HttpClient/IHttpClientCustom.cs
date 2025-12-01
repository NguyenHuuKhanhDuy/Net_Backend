namespace Backend_Net.Application.Common.Interfaces.HttpClient;

public interface IHttpClientCustom<T>
{
    Task<TResult?> GetAsync<TResult>(string path, CancellationToken cancellationToken);
    Task<TResult?> GetAsync<TResult>(string host, string path, CancellationToken cancellationToken);
    Task<TResult?> PostAsync<TResult>(string path, string jsonContent, CancellationToken cancellationToken);
    Task<TResult?> PostAsync<TResult>(string host, string path, string jsonContent, CancellationToken cancellationToken);
    Task<TResult?> PostAsync<TResult>(string path, string jsonContent, Dictionary<string, string> headers, CancellationToken cancellationToken);
}