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
Sends a request to borica with the order id to check the status of the order
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

## Order generation
The order generation ensures that the order is unique for 24hrs and for the transaction by taking into account the current date and time and the nonce.
If you want to generate the order id yourself you can use the overloads of the `GeneratePayload` and `GenerateForm` methods
```csharp
var paymentPayload = borica.GeneratePayload(orderId: 123456);
var paymentPayloadForm = borica.GenerateForm(isDev: true, orderId: 123456);
```

## Resources 
- Documentation used 4.0
- [Borica resources](https://3dsgate-dev.borica.bg/)
- [Borica Documentation BG](https://3dsgate-dev.borica.bg/P-OM-41_BORICA_eCommerce_CGI_interface_v%204.0_BG.pdf)
- [Borica Documentation EN](https://3dsgate-dev.borica.bg/P-OM-41_BORICA_eCommerce_CGI_interface_v%204.0_EN.pdf)
- [Download Borica test public key](https://3dsgate-dev.borica.bg/MPI_OW_APGW_D.zip)
- [Download Borica production public key](https://3dsgate-dev.borica.bg/MPI_OW_APGW_Prod.zip)
- [Borica key generator for sign request](https://3dsgate-dev.borica.bg/generateCSR/)

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
