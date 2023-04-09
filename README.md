![](https://github.com/viktor111/BoricaNet/actions/workflows/dotnet.yml/badge.svg)
# BoricaNet
Library for borica in .Net

## Download
`dotnet add package BoricaNet`

## Example request
1. Create params
```csharp
var boricaData = new BoricaNetParams(
    pathToPublicKeyPath: "borica-pub-key.cer",
    pathToPrivateKeyPath: "keystore-test.pfx",
    privateKeyPassword: "password",
    email: "myemail@gmail.com",
    terminalId: "V0000000",
    description: "Description shows at payment page",
    merchantUrl: "https://mywebsite.com",
    merchantName: "myCompany",
    merchant: "0000000000",
    amount: "31.00"
);
```
2. Initialize Borica class with params

```csharp
var borica = new Borica(boricaData);
```

3. Generate payload
```csharp
var paymentPayload = borica.GeneratePayload();
```

3. Or generate html form as string directly with hidden input
- Using `isDev: true` will generate with borica's dev url
- Using `isDev: false` will generate with borica production url
```csharp
var paymentPayload = borica.GenerateForm(isDev: true);
```

## Example response
The formBody is the response submited from borica after payment on your endpoint
```csharp
var paymentResponse = borica.HandleResponse(formBody: new Dictionary<string, string>());
```
- `paymentResponse` will cointain the payload data inside if you want to handle the error checks yourself

## Example order status check
Sends a request to borica with the order id to check the status of the order. Borica will save the order id for 24hrs only.

```csharp
 var statusCheckParams = new BoricaCheckStatusParams
 {
    OrderId = "123456",
    TerminalId = "V0000000",
    OriginalTransactionCode = "1"
};
        
 await CheckStatusForOrder(httpClient: new HttpClient(), statusParams: statusCheckParams);
```
By default it will send the request to borica's production url, if you want to send it to dev url use the overwrite default value `isDev: true`
```csharp
 await CheckStatusForOrder(httpClient: new HttpClient(), statusParams: statusCheckParams, isDev: true);
```

## Example get message by RC
```csharp
var message = borica.GetMessageByRc(rc: "00");
```
Will return `BoricaMessageContent` with the message and the description in english and bulgarian.

## Example get message by AC
```csharp
var message = borica.GetResponseTypeByAction(action: "0");
```
Will return `BoricaResponseType` with the response type.

## Order generation
The order generation ensures that the order is unique for 24hrs and for the transaction by taking into account the current date and time and the nonce.
If you want to generate the order id yourself you can use the overloads of the `GeneratePayload` and `GenerateForm` methods
```csharp
var paymentPayload = borica.GeneratePayload(orderId: 123456);
var paymentPayloadForm = borica.GenerateForm(isDev: true, orderId: 123456);
```

## RC Table
| RC | Description EN | Description BG |
| --- | --- | --- |
| 00 | Success | Успешна транзакция |
| -1 | A mandatory request field is not filled | Заявката съдържа поле с некоректно име |
| -2 | CGI request validation failed | Aвторизационният хост не отговаря или форматът на отговора е неправилен |
| -4 | No connection to the acquirer host (TS) | Няма връзка с авторизационния хост |
| -5 | Acquirer host (TS) does not respond or wrong format of e-gateway response template file | Грешка във връзката с авторизационния хост |
| -6 | e-Gateway configuration error | Грешка в конфигурацията на APGW |
| -7 | The acquirer host (TS) response is invalid, e.g. mandatory fields missing | Форматът на отговора от авторизационният хост е неправилен |
| -10 | Error in the "Amount" request field | Грешка в поле "Сума" в заявката" |
| -11 | Error in the "Currency" request field | Грешка в поле "Валута" в заявката |
| -12 | Error in the "Merchant ID" request field | Грешка в поле "Идентификатор на търговеца" в заявката |
| -13 | The referrer IP address (usually the merchant's IP) is not the one expected | Неправилен IP адрес на търговеца |
| -15 | "Error in the "RRN" request field | Грешка в поле "RRN" в заявката |
| -16 | Another transaction is being performed on the terminal | В момента се изпълнява друга трансакция на терминала |
| -17 | The terminal is denied access to e-Gateway | Отказан достъп до платежния сървър (напр. грешка при проверка на P_SIGN) |
| -19 | Error in the authentication information request or authentication failed. | Грешка в искането за автентикация или неуспешна автентикация |
| -20 | The permitted time interval (1 hour by default) between the transaction timestamp request field and the e-Gateway time was exceeded | Разрешената разлика между времето на сървъра на търговеца и e-Gateway сървъра е надвишена |
| -21 | The transaction has already been executed | Трансакцията вече е била изпълнена |
| -22 | Transaction contains invalid authentication information | Транзакцията съдържа невалидни данни за аутентикация |
| -23 | Invalid transaction context | Невалиден контекст на транзакцията |
| -24 | Transaction context data mismatch | Заявката съдържа стойности за полета, които не могат да бъдат обработени. Например валутата е различна от валутата на терминала или транзакцията е по-стара от 24 часа |
| -25 | Transaction confirmation state was canceled by user | Допълнителното потвърждение на трансакцията е отказано от картодържателя |
| -26 | Invalid action BIN | Невалиден BIN на картата |
| -27 | Invalid merchant name | Невалидно име на търговеца |
| -28 | Invalid incoming addendum(s) | Невалидно допълнително поле (например AD.CUST_BOR_ORDER_ID) |
| -29 | Invalid/duplicate authentication reference | Невалиден отговор от ACS на издателя на картат |
| -30 | Transaction was declined as fraud | Трансакцията е отказана |
| -31 | Transaction already in progress | Трансакцията е в процес на обрбаотка |
| -32 | Duplicate declined transaction | Дублирана отказана трансакция |
| -33 | Customer authentication by random amount or verify one-time code in progress | Трансакцията е в процес на аутентикация на картодържателя |

## Resources 
- Documentation used 4.0
- [Borica resources](https://3dsgate-dev.borica.bg/)
- [Borica Documentation BG](https://3dsgate-dev.borica.bg/P-OM-41_BORICA_eCommerce_CGI_interface_v%204.0_BG.pdf)
- [Borica Documentation EN](https://3dsgate-dev.borica.bg/P-OM-41_BORICA_eCommerce_CGI_interface_v%204.0_EN.pdf)
- [Download Borica test public key](https://3dsgate-dev.borica.bg/MPI_OW_APGW_D.zip)
- [Download Borica production public key](https://3dsgate-dev.borica.bg/MPI_OW_APGW_Prod.zip)
- [Borica key generator for sign request](https://3dsgate-dev.borica.bg/generateCSR/)

### Borica urls that process the requests
- Borica test url `https://3dsgate-dev.borica.bg/cgi-bin/cgi_link`
- Borica production url `https://3dsgate.borica.bg/cgi-bin/cgi_link`

## openssl commands to convert keys
1. Generate cer file form csr and private.key
   - ```openssl x509 -req -in [.csr] -signkey [.key] -out certificate.cer```
2. Generate pfx file from cer and private.key
   - ```openssl pkcs12 -export -inkey [.key] -in [.cer] -out key-store.pfx```

## Test cards with status codes

For testing purposes you can use the following cards with the corresponding status codes.
The card date and CVV can any numbers.

- `5100770000000022` - `Approved` - RC: `00`
- `5555000000070019` - `Declined` - RC: `04`
- `5555000000070027` - `Declined` - RC: `13`
- `5555000000070035` - `Declined` - RC: `91`
- `4341792000000044` - `Approved` - RC: `00`
   - Requires 3DSecure verification with sum greater than `30.00`
   - The 3DSecure password is `111111`
