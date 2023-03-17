using BoricaNet.Types;

namespace BoricaNet.Dto;

public class BoricaResponse
{
    public BoricaResponsePayload Payload { get; set; }
    
    public string MessageEn { get; set; }
    
    public string MessageBg { get; set; }
    
    public BoricaPaymentResponseType ResponseType { get; set; }
}