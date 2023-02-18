# BoricaNet
Client libary for borica in .Net

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

3. Or generate html form as string directly with hiddeen input
- Using `isDev: true` will generate with borica's dev url
- Using `isDev: false` will generate with borica production url
```csharp
var paymentPayload = borica.GenerateForm(isDev: true);
```

## Example response
The formBody should is the response submited from borica after payment on your endpoint
```csharp
var paymentResponse = borica.HandleResponse(formBody: new Dictionary<string, string>());
```
- `paymentResponse` will cointain the payload data inside if you want to handle the error checks yourself
