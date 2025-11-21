using DbUp.Engine;
using DbUp.Engine.Transactions;

namespace Backend_Net.Migration;

public class CustomScriptProvider : IScriptProvider
{
    private readonly IScriptProvider _innerProvider;
    private readonly Func<string, string> _rename;

    public CustomScriptProvider(
        IScriptProvider innerProvider,
        Func<string, string> rename)
    {
        _innerProvider = innerProvider;
        _rename = rename;
    }

    public IEnumerable<SqlScript> GetScripts(IConnectionManager connectionManager)
    {
        var scripts = _innerProvider.GetScripts(connectionManager);

        foreach (var s in scripts)
        {
            var newName = _rename(s.Name);

            yield return new SqlScript(newName, s.Contents);
        }
    }
}