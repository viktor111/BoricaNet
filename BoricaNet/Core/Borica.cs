using System.Text;
using BoricaNet.Dto;
using BoricaNet.Exceptions;
using BoricaNet.Helpers;
using BoricaNet.Types;
using BoriscaNet.Dto;
using Newtonsoft.Json;
using static BoricaNet.Constants;

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
    /// <returns>BoricaPaymentPayload</returns>
    public BoricaPaymentPayload GeneratePayload()
    {
        var payload = GenerateBoricaPayloadData();
        return payload;
    }
    
    /// <summary>
    /// Generates payload with data for Borica payment with custom orderId
    /// </summary>
    /// <param name="orderId">Custom orderId</param>
    /// <returns>BoricaPaymentPayload</returns>
    public BoricaPaymentPayload GeneratePayload(int orderId)
    {
        var payload = GenerateBoricaPayloadData(orderId);
        return payload;
    }
    
    /// <summary>
    /// Generates form filled with data for Borica payment as string
    /// </summary>
    /// <param name="isDev">Decides to use dev url or prod url for borica</param>
    /// <returns>string</returns>
    public string GenerateForm(bool isDev)
    {
        var payload = GenerateBoricaPayloadData();
        
        var payloadToJson = JsonConvert.SerializeObject(payload);
        var form = GenerateHtmlForm.GenerateHTMLForm(payloadToJson, isDev);

        return form;
    }
    
    /// <summary>
    /// Generates form filled with data for Borica payment as string
    /// </summary>
    /// <param name="isDev">Decides to use dev url or prod url for borica</param>
    /// <param name="orderId">Custom orderId</param>
    /// <returns>string</returns>
    public string GenerateForm(bool isDev, int orderId)
    {
        var payload = GenerateBoricaPayloadData(orderId);
        
        var payloadToJson = JsonConvert.SerializeObject(payload);
        var form = GenerateHtmlForm.GenerateHTMLForm(payloadToJson, isDev);

        return form;
    }
    
    /// <summary>
    /// Generates form filled with data for Borica payment as string
    /// </summary>
    /// <param name="httpClient">The HttpClient used to make the request to borica API</param>
    /// <param name="statusParams">The parameters needed to send to borica</param>
    /// <param name="isDev">Decides to use dev url or prod url for borica default will be prod</param>
    /// <returns>string</returns>
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

        var url = isDev ? "https://3dsgate-dev.borica.bg/cgi-bin/cgi_lin" : "https://3dsgate.borica.bg/cgi-bin/cgi_lin";
        
        var data = new FormUrlEncodedContent(new[] {
            new KeyValuePair<string, string>("TERMINAL", payload.TerminalId),
            new KeyValuePair<string, string>("TRTYPE", payload.TransactionCode),
            new KeyValuePair<string, string>("ORDER", payload.OrderId),
            new KeyValuePair<string, string>("TRAN_TRTYPE", payload.OriginalTransactionCode),
            new KeyValuePair<string, string>("NONCE", payload.Nonce),
            new KeyValuePair<string, string>("P_SIGN", payload.PSign)
        });
        
        var response = await httpClient.PostAsync(url, data);
        
        var responseString = await response.Content.ReadAsStringAsync();
        
        var responsePayload = JsonConvert.DeserializeObject<BoricaStatusCheckResponse>(responseString);
        
        return responsePayload ?? throw new BoricaNetException("Null response from Borica");
    }

    /// <summary>
    /// Generates form filled with data for Borica payment as string
    /// </summary>
    /// <param name="formBody">The data submitted from boricas custom form to yor backend</param>
    /// <example>
    /// Use this example to get the formBody from the request in Post action method
    /// <code>
    /// [FromForm] Dictionary string, string> formBody
    /// </code>
    /// </example>
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
            response.MessageEn = "Invalid signature";
            response.MessageBg = "Невалиден подпис";
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
        var response = BoricaPaymentResponseType.Error;
        switch (action)
        {
            case "0":
                response = BoricaPaymentResponseType.Success;
                break;
            case "1":
                response = BoricaPaymentResponseType.Duplicate;
                break;
            case "2":
                response = BoricaPaymentResponseType.Refused;
                break;
            case "3":
                response = BoricaPaymentResponseType.Error;
                break;
            case "7":
                response = BoricaPaymentResponseType.DuplicateWithBadAuth;
                break;
            case "21":
                response = BoricaPaymentResponseType.SoftDecline;
                break;
            default:
                response = BoricaPaymentResponseType.Error;
                break;
        }

        return response;
    }
    
    public BoricaMessageContent GetMessageByRc(string rc)
    {
        var response = new BoricaMessageContent();
        
        switch (rc)
        {
            case "00":
                response.MessageEn = "Success";
                response.MessageBg = "Успешна транзакция";
                break;
            case "-1":
                response.MessageEn = "A mandatory request field is not filled in";
                response.MessageBg = "Заявката съдържа поле с некоректно име";
                
                break;
            case "-2":
                response.MessageEn = "CGI request validation failed";
                response.MessageBg = "Aвторизационният хост не отговаря или форматът на отговора е неправилен";
                break;
            case "-4":
                response.MessageEn = "No connection to the acquirer host (TS)";
                response.MessageBg = "Няма връзка с авторизационния хост";
                break;
            case "-5":
                response.MessageEn = "Acquirer host (TS) does not respond or wrong format of e-gateway response template file";
                response.MessageBg = "Грешка във връзката с авторизационния хост";
                break;
            case "-6":
                response.MessageEn = "e-Gateway configuration error";
                response.MessageBg = "Грешка в конфигурацията на APGW";
                break;
            case "-7":
                response.MessageEn = "The acquirer host (TS) response is invalid, e.g. mandatory fields missing";
                response.MessageBg = "Форматът на отговора от авторизационният хост е неправилен";
                break;
            case "-10":
                response.MessageEn = "Error in the \"Amount\" request field";
                response.MessageBg = "Грешка в поле \"Сума\" в заявката";
                break;
            case "-11":
                response.MessageEn = "Error in the \"Currency\" request field";
                response.MessageBg = "Грешка в поле \"Валута\" в заявката";
                break;
            case "-12":
                response.MessageEn = "Error in the \"Merchant ID\" request field";
                response.MessageBg = "Грешка в поле \"Идентификатор на търговеца\" в заявката";
                break;
            case "-13":
                response.MessageEn = "The referrer IP address (usually the merchant's IP) is not the one expected";
                response.MessageBg = "Неправилен IP адрес на търговеца";
                break;
            case "-15":
                response.MessageEn = "Error in the \"RRN\" request field";
                response.MessageBg = "Грешка в поле \"RRN\" в заявката";
                break;
            case "-16":
                response.MessageEn = "Another transaction is being performed on the terminal";
                response.MessageBg = "В момента се изпълнява друга трансакция на терминала";
                break;
            case "-17":
                response.MessageEn = "The terminal is denied access to e-Gateway";
                response.MessageBg = "Отказан достъп до платежния сървър (напр. грешка при проверка на P_SIGN)";
                break;
            case "-19":
                response.MessageEn = "Error in the authentication information request or authentication failed.";
                response.MessageBg = "Грешка в искането за автентикация или неуспешна автентикация";
                break;
            case "-20":
                response.MessageEn = "The permitted time interval (1 hour by default) between the transaction timestamp request field and the e-Gateway time was exceeded";
                response.MessageBg = "Разрешената разлика между времето на сървъра на търговеца и e-Gateway сървъра е надвишена";
                break;
            case "-21":
                response.MessageEn = "The transaction has already been executed";
                response.MessageBg = "Трансакцията вече е била изпълнена";
                break;
            case "-22":
                response.MessageEn = "Transaction contains invalid authentication information";
                response.MessageBg = "Транзакцията съдържа невалидни данни за аутентикация";
                break;
            case "-23":
                response.MessageEn = "Invalid transaction context";
                response.MessageBg = "Невалиден контекст на транзакцията";
                break;
            case "-24":
                response.MessageEn = "Transaction context data mismatch";
                response.MessageBg = "Заявката съдържа стойности за полета, които не могат да бъдат обработени. Например валутата е различна от валутата на терминала или транзакцията е по-стара от 24 часа";
                break;
            case "-25":
                response.MessageEn = "Transaction confirmation state was canceled by user";
                response.MessageBg = "Допълнителното потвърждение на трансакцията е отказано от картодържателя";
                break;
            case "-26":
                response.MessageEn = "Invalid action BIN";
                response.MessageBg = "Невалиден BIN на картата";
                break;
            case "-27":
                response.MessageEn = "Invalid merchant name";
                response.MessageBg = "Невалидно име на търговеца";
                break;
            case "-28":
                response.MessageEn = "Invalid incoming addendum(s)";
                response.MessageBg = "Невалидно допълнително поле (например AD.CUST_BOR_ORDER_ID)";
                break;
            case "-29":
                response.MessageEn = "Invalid/duplicate authentication reference";
                response.MessageBg = "Невалиден отговор от ACS на издателя на картат";
                break;
            case "-30":
                response.MessageEn = "Transaction was declined as fraud";
                response.MessageBg = "Трансакцията е отказана";
                break;
            case "-31":
                response.MessageEn = "Transaction already in progress";
                response.MessageBg = "Трансакцията е в процес на обрбаотка";
                break;
            case "-32":
                response.MessageEn = "Duplicate declined transaction";
                response.MessageBg = "Дублирана отказана трансакция";
                break;
            case "-33":
                response.MessageEn = "Customer authentication by random amount or verify one-time code in progress";
                response.MessageBg = "Трансакцията е в процес на аутентикация на картодържателя";
                break;
            default:
                response.MessageEn = "Error in the transaction";
                response.MessageBg = "Грешка в транзакцията";
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
    
    private BoricaPaymentPayload GenerateBoricaPayloadData(int orderId)
    {
        var date = DateHelper.FormatDateForBorica(DateTime.UtcNow);
        var nonce = GenerateNonce();
        var adBorCustOrderId = orderId+"ORDER";
        
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