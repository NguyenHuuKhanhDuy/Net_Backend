using Backend_Net.Api.Validation;
using Newtonsoft.Json;

namespace Backend_Net.Api.Handler
{
    public class ErrorHandler
    {
        public bool success { get; set; }
        public string errorMessage { get; set; }
        public string errorMessageCode { get; set; }
        public List<ValidationError> errors { get; set; }
        public object data { get; set; }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
