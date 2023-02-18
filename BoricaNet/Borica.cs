using System.Globalization;
using System.Text;
using Newtonsoft.Json;
using static BoricaNet.Constants;

namespace BoricaNet;

public class Borica
{
    private readonly Signer _signer;
    private readonly BoricaNetParams _boricaNetParams;

    public Borica(BoricaNetParams boricaNetParams)
    {
        _boricaNetParams = boricaNetParams;
        _signer = new Signer(
            privateKeyFilePath: boricaNetParams.PathToPrivateKeyPath,
            privateKeyPassword: boricaNetParams.PrivateKeyPassword, 
            publicKeyFilePath: boricaNetParams.PathToPublicKeyPath);
    }

    public BoricaPaymentPayload GeneratePayload()
    {
        var payload = GenerateBoricaPayloadData();
        return payload;
    }
    
    public string GenerateForm(bool isDev)
    {
        var payload = GenerateBoricaPayloadData();
        
        var payloadToJson = JsonConvert.SerializeObject(payload);
        var form = GenerateHtmlForm.GenerateHTMLForm(payloadToJson, isDev);
        
        return form;
    }

    public BoricaResponse HandleResponse(Dictionary<string, string> formBody)
    {
        var response = new BoricaResponse();
        
        var responsePayload = BoricaResponsePayload.FromBodyForm(formBody);
        
        response.Payload = responsePayload;
        
        var signatureData = GenerateSignatureDataForResponse(responsePayload);
        var signatureBytes = Encoding.UTF8.GetBytes(signatureData);
        
        var validSignature = VerifyBoricaResponse(signatureBytes, responsePayload.Signature);
        
        if (!validSignature)
        {
            response.ResponseType = BoricaPaymentResponseType.Error;
            response.Message = "Invalid signature";
            return response;
        }
        
        switch (responsePayload.Rc)
        {
            case "65":
            case "1A":
                response.ResponseType = BoricaPaymentResponseType.SoftDecline;
                break;
            case "00":
                response.ResponseType = BoricaPaymentResponseType.Success;
                response.Message = "Successful payment";
                break;
            default:
                response.ResponseType = BoricaPaymentResponseType.Error;
                response.Message = "Invalid payment";
                break;
        }
        
        return response;
    }
    
    private bool VerifyBoricaResponse(byte[] messageData, string signature)
    {
        var signatureData = DecodeMessage(signature);

        var signatureFlag = _signer.VerifySignature(messageData, signatureData);
        return signatureFlag;
    }

    private BoricaPaymentPayload GenerateBoricaPayloadData()
    {
        var date = DateHelper.FormatDateForBorica(DateTime.UtcNow);
        var nonce = GenerateNonce();
        var orderId = Generator.GenerateOrderNumber(nonce).ToString();
        var adBorCustOrderId = orderId+"ORDER";
        
        var boricaPaymentPayload = new BoricaPaymentPayload(
            amount: _boricaNetParams.Amount,
            currency: _boricaNetParams.Currency,
            orderDescription: _boricaNetParams.Description,
            orderId: orderId,
            country: _boricaNetParams.Country,
            email: _boricaNetParams.Email,
            merchantId: _boricaNetParams.Merchant,
            merchantName: _boricaNetParams.MerchantName,
            merchantUrl: _boricaNetParams.MerchantUrl,
            terminalId: _boricaNetParams.TerminalId,
            transactionCode: _boricaNetParams.TransactionCode,
            timezone: date,
            transactionDate: date,
            nonce: nonce,adBorCustOrderId: adBorCustOrderId
        );
        
        boricaPaymentPayload.PSign = GetSignatureFromParams(boricaPaymentPayload);
        
        return boricaPaymentPayload;
    }

    private string GenerateNonce()
    {
        var nonceBytes = Generator.GenerateRandomByteArray(16, 16);
        var nonce = Generator.GenerateHexStringFromByteArray(nonceBytes);

        return nonce;
    }
    
    private string GeneratePayload(List<string> parameters)
    {
        var sb = new StringBuilder();
        foreach (var parameter in parameters)
        {
            if (string.IsNullOrEmpty(parameter))
            {
                sb.Append("-");
            }
            else
            {
                var bytes = Encoding.UTF8.GetBytes(parameter);
                var len = bytes.Length;
                sb.Append(len + parameter);
            }
        }

        return sb.ToString();
    }

    private string GetSignatureFromParams(BoricaPaymentPayload boricaPaymentPayload)
    {
        var signatureParameters = GetParametersForSignatureRequest(boricaPaymentPayload, boricaPaymentPayload.Timezone);

        var signature = GenerateSignature(signatureParameters);

        return signature;
    }
    
    private string GenerateSignature(List<string> parameters) { 
        var parametersJoined = GeneratePayload(parameters);
        
        var sign = _signer.CreateSignature(Encoding.UTF8.GetBytes(parametersJoined));
        return Generator.GenerateHexStringFromByteArray(sign);
    }
    
    private byte[] DecodeMessage(string encodedMsg)
    {
        var len = encodedMsg.Length;
        
        if (len % 2 != 0)
        {
            throw new Exception("Message not hex encoded");
        }
        
        var data = new byte[len / 2];
        
        for (var i = 0; i < len; i += 2)
        {
            data[i / 2] = (byte)((Convert.ToInt32(encodedMsg[i].ToString(), 16) << 4) + Convert.ToInt32(encodedMsg[i + 1].ToString(), 16));
        }

        return data;
    }
    
    private string GenerateSignatureDataForResponse(BoricaResponsePayload response) {
        switch (response.TransactionCode) {
            case TransactionCode1:
            case TransactionCode12:
            case TransactionCode21:
            case TransactionCode22:
            case TransactionCode24:
            case TransactionCode90:
                return GeneratePayload(new List<string>{
                    response.Action, 
                    response.Rc, 
                    response.Approval,
                    response.TerminalId, 
                    response.TransactionCode,
                    response.Amount,
                    response.Currency,
                    response.OrderID,
                    response.Rrn, 
                    response.IntRef,
                    response.ParesStatus,
                    response.Eci, 
                    response.TransactionTime,
                    response.Nonce,
                    response.Rfu
                });
            default:
                throw new BoricaNetException("Transaction code not valid. Cannot create signature for response");
        }
    }
    
    private List<string> GetParametersForSignatureRequest(BoricaPaymentPayload boricaPaymentPayload, string transactionDate)
    {
        List<string> signatureParameters;
        switch (boricaPaymentPayload.TransactionCode)
        {
            case TransactionCode1:
            case TransactionCode12:
            case TransactionCode21:
            case TransactionCode22:
            case TransactionCode24:
                signatureParameters = new List<string>()
                {
                    boricaPaymentPayload.TerminalId,
                    boricaPaymentPayload.TransactionCode,
                    boricaPaymentPayload.Amount,
                    boricaPaymentPayload.Currency,
                    boricaPaymentPayload.OrderId, 
                    transactionDate,
                    boricaPaymentPayload.Nonce,
                    boricaPaymentPayload.Rfu
                };
                break;
            case TransactionCode90:
                signatureParameters = new List<string>()
                {
                    boricaPaymentPayload.TerminalId,
                    boricaPaymentPayload.TransactionCode,
                    boricaPaymentPayload.OrderId, 
                    boricaPaymentPayload.Nonce
                };
                break;
            default:
                throw new BoricaNetException("Transaction code not valid. Cannot create signature for request");
        }

        return signatureParameters;
    }
}