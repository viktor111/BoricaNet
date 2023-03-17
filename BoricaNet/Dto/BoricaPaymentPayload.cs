using Newtonsoft.Json;
using static BoricaNet.Constants;

namespace BoricaNet.Dto;

public class BoricaPaymentPayload
{
    public BoricaPaymentPayload(
        string transactionCode,
        string amount,
        string currency,
        string orderId,
        string country,
        string orderDescription,
        string email,
        string merchantUrl,
        string merchantId,
        string merchantName,
        string terminalId,
        string timezone, 
        string transactionDate, 
        string nonce, 
        string adBorCustOrderId
        )
    {
        TransactionCode = transactionCode;
        Amount = amount;
        Currency = currency;
        OrderId = orderId;
        Country = country;
        OrderDescription = orderDescription;
        Email = email;
        MerchantUrl = merchantUrl;
        MerchantId = merchantId;
        MerchantName = merchantName;
        TerminalId = terminalId;
        Timezone = timezone;
        TransactionDate = transactionDate;
        Nonce = nonce;
        AdBorCustOrderId = adBorCustOrderId;
    }

    [JsonProperty("TRTYPE")]
    public string TransactionCode { get; set; }
    
    [JsonProperty("TRAN_TRTYPE")]
    public string? OriginalTransactionCode { get; set; }

    [JsonProperty("AMOUNT")]   
    public string Amount { get; set; }

    [JsonProperty("CURRENCY")]  
    public string Currency { get; set; }

    [JsonProperty("ORDER")]  
    public string OrderId { get; set; }

    [JsonProperty("COUNTRY")]  
    public string Country { get; set; }  

    [JsonProperty("DESC")]  
    public string OrderDescription { get; set; }

    [JsonProperty("EMAIL")]  
    public string Email { get; set; }

    [JsonProperty("MERCH_URL")]  
    public string MerchantUrl { get; set; }

    [JsonProperty("MERCHANT")]  
    public string MerchantId { get; set; }

    [JsonProperty("MERCH_NAME")]  
    public string MerchantName { get; set; }

    [JsonProperty("TERMINAL")]  
    public string TerminalId { get; set; }

    [JsonProperty("MERCH_GMT")]  
    public string Timezone { get; set; }
    
    [JsonProperty("TIMESTAMP")]  
    public string TransactionDate { get; set; }

    [JsonProperty("NONCE")]  
    public string Nonce { get; set; }

    [JsonProperty("P_SIGN")]  
    public string PSign { get; set; } = string.Empty;

    [JsonProperty("RFU")] 
    public string Rfu { get; set; } = string.Empty;

    [JsonProperty("AD.CUST_BOR_ORDER_ID")]
    public string AdBorCustOrderId { get; set; }

    [JsonProperty("ADDENDUM")]  
    public string Addendum = ADTD;
}