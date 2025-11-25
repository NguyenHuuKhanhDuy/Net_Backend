using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Backend_Net.Application.Services.Signature;

public interface ISignatureService
{
   string CreateSignature(Dictionary<string, string> parameters, string secretKey);
}