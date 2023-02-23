using Newtonsoft.Json;

namespace BoricaNet;

public class BoricaStatusCheckResponse
{
    [JsonProperty("ACTION")]
    public string Action { get; set; }

    [JsonProperty("RC")]
    public string Rc { get; set; }

    [JsonProperty("STATUSMSG")]
    public string StatusMsg { get; set; }

    [JsonProperty("TERMINAL")]
    public string Terminal { get; set; }

    [JsonProperty("TRTYPE")]
    public string TrType { get; set; }

    [JsonProperty("AMOUNT")]
    public string Amount { get; set; }
    [JsonProperty("CURRENCY")]
    public string Currency { get; set; }

    [JsonProperty("ORDER")]
    public string Order { get; set; }

    [JsonProperty("TIMESTAMP")]
    public string TimeStamp { get; set; }

    [JsonProperty("TRAN_DATE")]
    public string TranDate { get; set; }

    [JsonProperty("TRAN_TRTYPE")]
    public string TranTrType { get; set; }

    [JsonProperty("APPROVAL")]
    public string Approval { get; set; }

    [JsonProperty("RRN")]
    public string Rrn { get; set; }

    [JsonProperty("INT_REF")]
    public string IntRef { get; set; }

    [JsonProperty("PARES_STATUS")]
    public string ParesStatus { get; set; }

    [JsonProperty("AUTH_STEP_RES")]
    public string AuthStepRes { get; set; }

    [JsonProperty("CARDHOLDERINFO")]
    public string CardHolderInfo { get; set; }

    [JsonProperty("ECI")]
    public string Eci { get; set; }

    [JsonProperty("CARD")]
    public string Card { get; set; }

    [JsonProperty("CARD_BRAND")]
    public string CardBrand { get; set; }

    [JsonProperty("NONCE")]
    public string Nonce { get; set; }

    [JsonProperty("P_SIGN")]
    public string PSign { get; set; }
}