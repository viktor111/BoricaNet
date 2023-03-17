using BoricaNet.Exceptions;
using Newtonsoft.Json;

namespace BoricaNet.Dto;

public class BoricaResponsePayload
{
    [JsonProperty("TERMINAL")] 
    public string TerminalId;

    [JsonProperty("TRTYPE")]
    public string TransactionCode { get; set; }

    [JsonProperty("ORDER")]
    public string OrderID { get; set; }

    [JsonProperty("AMOUNT")]
    public string Amount { get; set; }

    [JsonProperty("CURRENCY")]
    public string Currency { get; set; }

    [JsonProperty("ACTION")]
    public string Action { get; set; }

    [JsonProperty("RC")]
    public string Rc { get; set; }

    [JsonProperty("APPROVAL")]
    public string Approval { get; set; }

    [JsonProperty("RRN")]
    public string Rrn { get; set; }

    [JsonProperty("INT_REF")]
    public string IntRef { get; set; }

    [JsonProperty("PARES_STATUS")]
    public string ParesStatus { get; set; }

    [JsonProperty("ECI")]
    public string Eci { get; set; }

    [JsonProperty("TIMESTAMP")]
    public string TransactionTime { get; set; }

    [JsonProperty("NONCE")]
    public string Nonce { get; set; }

    [JsonProperty("P_SIGN")]
    public string Signature { get; set; }
   
    [JsonProperty("RFU")] 
    public string Rfu { get; set; }
    
    public static BoricaResponsePayload FromBodyForm(Dictionary<string, string> body)
    {
        if(body is null)
            throw new BoricaNetException("Borica response body is null.");
        
        var json = JsonConvert.SerializeObject(body);
        var boricaResponsePayload = JsonConvert.DeserializeObject<BoricaResponsePayload>(json);
        
        if(boricaResponsePayload is null)
            throw new BoricaNetException("Borica response payload is null.");
        
        return boricaResponsePayload;
    }
}