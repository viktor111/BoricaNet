using System.Text;
using BoricaNet.Dto;
using BoricaNet.Exceptions;
using BoricaNet.Helpers;
using BoricaNet.Types;
using BoriscaNet.Dto;
using Newtonsoft.Json;

using static BoricaNet.Constants;
using static BoricaNet.Constants.ResponseCodeMessages;
using static BoricaNet.Constants.ResponseCodes;
using static BoricaNet.Constants.ActionCodes;
using static BoricaNet.Constants.BoricaErrorMessages;

namespace BoricaNet.Core;

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

    /// <summary>
    /// Generates payload with data for Borica payment
    /// </summary>
    /// <returns>The borica payment paload containing the data needed to start a payment to borica</returns>
    public BoricaPaymentPayload GeneratePayload()
    {
        var payload = GenerateBoricaPayloadData();
        return payload;
    }

    /// <summary>
    /// Generates payload with data for Borica payment with custom orderId
    /// </summary>
    /// <param name="orderId">Custom orderId</param>
    /// <returns>The borica payment paload containing the data needed to start a payment to borica</returns>
    public BoricaPaymentPayload GeneratePayload(int orderId)
    {
        var payload = GenerateBoricaPayloadData(orderId);
        return payload;
    }

    /// <summary>
    /// Generates form filled with data for Borica payment as string
    /// </summary>
    /// <param name="isDev">Decides to use dev url or prod url for borica</param>
    /// <returns>The borica payment form as string with all the inputs hidden and filled with data needed to start a payment to borica</returns>
    public string GenerateForm(bool isDev)
    {
        var payload = GenerateBoricaPayloadData();
        
        var payloadToJson = JsonConvert.SerializeObject(payload);
        var form = FormGenerator.GenerateHTMLForm(payloadToJson, isDev);

        return form;
    }

    /// <summary>
    /// Generates form filled with data for Borica payment as string
    /// </summary>
    /// <param name="isDev">Decides to use dev url or prod url for borica</param>
    /// <param name="orderId">Custom orderId</param>
    /// <returns>The borica payment form as string with all the inputs hidden and filled with data needed to start a payment to borica</returns>
    public string GenerateForm(bool isDev, int orderId)
    {
        var payload = GenerateBoricaPayloadData(orderId);
        
        var payloadToJson = JsonConvert.SerializeObject(payload);
        var form = FormGenerator.GenerateHTMLForm(payloadToJson, isDev);

        return form;
    }
    
    /// <summary>
    /// Querries borica for the status of the given order
    /// </summary>
    /// <param name="httpClient">The HttpClient used to make the request to borica API</param>
    /// <param name="statusParams">The parameters needed to send to borica</param>
    /// <param name="isDev">Decides to use dev url or prod url for borica default will be prod</param>
    /// <returns>The data from borica of the given order</returns>
    public async Task<BoricaStatusCheckResponse> CheckStatusForOrder(HttpClient httpClient, BoricaCheckStatusParams statusParams, bool isDev = false)
    {
        var nonce = GenerateNonce();
        
        var payload = new BoricaStatusRequest{
            TerminalId = statusParams.TerminalId,
            OrderId = statusParams.OrderId,
            Nonce = nonce,
            OriginalTransactionCode = statusParams.OriginalTransactionCode,
            TransactionCode = TransactionCode90
        };

        SignPayloadStatusCheck(payload);

        var url = isDev ? BoricaDevUrl : BoricaProdUrl;
        
        var data = new FormUrlEncodedContent(new[] {
            new KeyValuePair<string, string>(BoricaParamNames.Terminal, payload.TerminalId),
            new KeyValuePair<string, string>(BoricaParamNames.TrType, payload.TransactionCode),
            new KeyValuePair<string, string>(BoricaParamNames.Order, payload.OrderId),
            new KeyValuePair<string, string>(BoricaParamNames.TranTrType, payload.OriginalTransactionCode),
            new KeyValuePair<string, string>(BoricaParamNames.Nonce, payload.Nonce),
            new KeyValuePair<string, string>(BoricaParamNames.PSign, payload.PSign)
        });
        
        var response = await httpClient.PostAsync(url, data);
        
        var responseString = await response.Content.ReadAsStringAsync();
        
        var responsePayload = JsonConvert.DeserializeObject<BoricaStatusCheckResponse>(responseString);
        
        return responsePayload ?? throw new BoricaNetException(NullResponseFromBorica);
    }

    /// <summary>
    /// Generates form filled with data for Borica payment as string
    /// To get the formBody from the request in Post action method
    /// [FromForm] Dictionary string, string> formBody
    /// </summary>
    /// <param name="formBody">The data submitted from boricas custom form to yor backend</param>
    /// <returns>string</returns>
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
            response.MessageEn = InvalidSignatureEn;
            response.MessageBg = InvalidSignatureBg;
            return response;
        }

        var responseType = GetResponseTypeByAction(responsePayload.Action);
        response.ResponseType = responseType;
        
        var message = GetMessageByRc(responsePayload.Rc);
        response.MessageEn = message.MessageEn;
        response.MessageBg = message.MessageBg;
        
        return response;
    }

    public BoricaPaymentResponseType GetResponseTypeByAction(string action)
    {
        BoricaPaymentResponseType response;
        switch (action)
        {
            case AC0:
                response = BoricaPaymentResponseType.Success;
                break;
            case AC1:
                response = BoricaPaymentResponseType.Duplicate;
                break;
            case AC2:
                response = BoricaPaymentResponseType.Refused;
                break;
            case AC3:
                response = BoricaPaymentResponseType.Error;
                break;
            case AC7:
                response = BoricaPaymentResponseType.DuplicateWithBadAuth;
                break;
            case AC21:
                response = BoricaPaymentResponseType.SoftDecline;
                break;
            default:
                response = BoricaPaymentResponseType.Error;
                break;
        }

        return response;
    }
    
    /// <summary>
    /// Helper method that gives message in English and Bulgarian based on the RC(Response Code) returned from Borica
    /// </summary>
    /// <param name="rc"></param>
    /// <returns>Object with message values in Engilsh and Bulgarian</returns>
    public BoricaMessageContent GetMessageByRc(string rc)
    {
        var response = new BoricaMessageContent();
        
        switch (rc)
        {
            case RC00:
                response.MessageEn = Code00En;
                response.MessageBg = Code00Bg;
                break;
            case RC_1:
                response.MessageEn = Code_1En;
                response.MessageBg = Code_1Bg;
                
                break;
            case RC_2:
                response.MessageEn = Code_2En;
                response.MessageBg = Code_2Bg;
                break;
            case RC_4:
                response.MessageEn = Code_4En;
                response.MessageBg = Code_4Bg;
                break;
            case RC_5:
                response.MessageEn = Code_5En;
                response.MessageBg = Code_5Bg;
                break;
            case RC_6:
                response.MessageEn = Code_6En;
                response.MessageBg = Code_6Bg;
                break;
            case RC_7:
                response.MessageEn = Code_7En;
                response.MessageBg = Code_7Bg;
                break;
            case RC_10:
                response.MessageEn = Code_10En;
                response.MessageBg = Code_10Bg;
                break;
            case RC_11:
                response.MessageEn = Code_11En;
                response.MessageBg = Code_11Bg;
                break;
            case RC_12:
                response.MessageEn = Code_12En;
                response.MessageBg = Code_12Bg;
                break;
            case RC_13:
                response.MessageEn = Code_13En;
                response.MessageBg = Code_13Bg;
                break;
            case RC_15:
                response.MessageEn = Code_15En;
                response.MessageBg = Code_15Bg;
                break;
            case RC_16:
                response.MessageEn = Code_16En;
                response.MessageBg = Code_16Bg;
                break;
            case RC_17:
                response.MessageEn = Code_17En;
                response.MessageBg = Code_17Bg;
                break;
            case RC_19:
                response.MessageEn = Code_19En;
                response.MessageBg = Code_19Bg;
                break;
            case RC_20:
                response.MessageEn = Code_20En;
                response.MessageBg = Code_20Bg;
                break;
            case RC_21:
                response.MessageEn = Code_21En;
                response.MessageBg = Code_21Bg;
                break;
            case RC_22:
                response.MessageEn = Code_22En;
                response.MessageBg = Code_22Bg;
                break;
            case RC_23:
                response.MessageEn = Code_23En;
                response.MessageBg = Code_23Bg;
                break;
            case RC_24:
                response.MessageEn = Code_24En;
                response.MessageBg = Code_24Bg;
                break;
            case RC_25:
                response.MessageEn = Code_25En;
                response.MessageBg = Code_25Bg;
                break;
            case RC_26:
                response.MessageEn = Code_26En;
                response.MessageBg = Code_26Bg;
                break;
            case RC_27:
                response.MessageEn = Code_27En;
                response.MessageBg = Code_27Bg;
                break;
            case RC_28:
                response.MessageEn = Code_28En;
                response.MessageBg = Code_28Bg;
                break;
            case RC_29:
                response.MessageEn = Code_29En;
                response.MessageBg = Code_29Bg;
                break;
            case RC_30:
                response.MessageEn = Code_30En;
                response.MessageBg = Code_30Bg;
                break;
            case RC_31:
                response.MessageEn = Code_31En;
                response.MessageBg = Code_31Bg;
                break;
            case RC_32:
                response.MessageEn = Code_32En;
                response.MessageBg = Code_32Bg;
                break;
            case RC_33:
                response.MessageEn = Code_33En;
                response.MessageBg = Code_33Bg;
                break;
            default:
                response.MessageEn = CodeDefaultEn;
                response.MessageBg = CodeDefaultBg;
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
        var adBorCustOrderId = orderId + Order;
        
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
    
    private BoricaPaymentPayload GenerateBoricaPayloadData(int orderId)
    {
        var date = DateHelper.FormatDateForBorica(DateTime.UtcNow);
        var nonce = GenerateNonce();
        var adBorCustOrderId = orderId + Order;
        
        var boricaPaymentPayload = new BoricaPaymentPayload(
            amount: _boricaNetParams.Amount,
            currency: _boricaNetParams.Currency,
            orderDescription: _boricaNetParams.Description,
            orderId: orderId.ToString(),
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
        var nonce = Generator.ByteArrayToHexString(nonceBytes);
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
        return Generator.ByteArrayToHexString(sign);
    }
    
    private byte[] DecodeMessage(string encodedMsg)
    {
        var len = encodedMsg.Length;
        
        if (len % 2 != 0)
        {
            throw new BoricaNetException(MessageNotHexEncoded);
        }
        
        var data = new byte[len / 2];
        
        for (var i = 0; i < len; i += 2)
        {
            data[i / 2] = (byte)((Convert.ToInt32(encodedMsg[i].ToString(), 16) << 4) + Convert.ToInt32(encodedMsg[i + 1].ToString(), 16));
        }

        return data;
    }
    
    private void SignPayloadStatusCheck(BoricaStatusRequest payload)
    {
        var signatureParameters = new List<string>()
        {
            payload.TerminalId,
            payload.TransactionCode,
            payload.OrderId,
            payload.Nonce
        };

        var test = GenerateSignature(signatureParameters);
        payload.PSign = test;
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
                throw new BoricaNetException(TransactionCodeNotValidResponse);
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
                throw new BoricaNetException(TransactionCodeNotValidRequest);
        }

        return signatureParameters;
    }

    
}