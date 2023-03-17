namespace BoricaNet.Types;

public enum BoricaPaymentResponseType
{
    Success,
    Duplicate,
    Refused,
    Error,
    DuplicateWithBadAuth,
    SoftDecline,
}