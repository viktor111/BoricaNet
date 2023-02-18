namespace BoricaNet;

public class BoricaResponse
{
    public BoricaResponsePayload Payload { get; set; }
    
    public string Message { get; set; }
    
    public bool IsError { get; set; }
    
    public BoricaPaymentResponseType ResponseType { get; set; }
}