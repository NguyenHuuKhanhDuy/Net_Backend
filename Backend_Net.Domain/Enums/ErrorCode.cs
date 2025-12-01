using System.ComponentModel;

namespace Backend_Net.Domain.Enums
{
    public enum ErrorCode
    {
        [Description("Internal server error")]
        EXH_ERR_001,
        
        [Description("Error when executing external service")]
        EXH_ERR_002,
        
        [Description("Token is invalid or expired")] 
        INV_ERR_001,

        [Description("Data field is required")]
        DAT_ERR_001,

        [Description("Invalid data")] DAT_ERR_002,

        [Description("Tenant not found")] TEN_ERR_001,

        [Description("Tenant is inactive")] TEN_ERR_002,

        #region Orders

        [Description("Reference ID already exists")]
        ORD_ERR_001,

        [Description("Invalid signature")] ORD_ERR_002,

        #endregion

        #region Payment Methods

        [Description("Unsupported payment method")]
        PMM_ERR_001,
        
        [Description("Can not get currency rate for the specified currency")]
        PMM_ERR_002,
        
        #endregion

        #region Payment transaction

        [Description("Payment transaction not found")]
        PMT_ERR_001,
        
        [Description("Payment transaction already processed")]
        PMT_ERR_002,

        #endregion

        #region Payment Currency

        [Description("Currency not supported")]
        CUR_ERR_001

        #endregion
    }
}