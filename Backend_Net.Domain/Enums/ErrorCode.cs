using System.ComponentModel;

namespace Backend_Net.Domain.Enums
{
    public enum ErrorCode
    {
        [Description("Data field is required")]
        DAT_ERR_001,
        
        [Description("Invalid data")]
        DAT_ERR_002,
        
        [Description("Tenant not found")]
        TEN_ERR_001,
        
        [Description("Tenant is inactive")]
        TEN_ERR_002,
        
        [Description("Reference ID already exists")]
        ORD_ERR_001,
        
        [Description("Invalid signature")]
        ORD_ERR_002
    }
}