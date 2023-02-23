using Newtonsoft.Json;

namespace BoricaNet;

internal class BoricaStatusRequest
{
    [JsonProperty("TERMINAL")]
    public string TerminalId { get; set; }

    [JsonProperty("TRTYPE")]
    public string TransactionCode { get; set; } = "90";

    [JsonProperty("ORDER")]
    public string OrderId { get; set; }

    [JsonProperty("TRAN_TRTYPE")]
    public string OriginalTransactionCode { get; set; } = "1";

    [JsonProperty("NONCE")]
    public string Nonce { get; set; }

    [JsonProperty("P_SIGN")]
    public string PSign { get; set; }
}