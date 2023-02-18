namespace BoricaNet;

public class BoricaNetParams
{
    public BoricaNetParams(
        string pathToPublicKeyPath,
        string pathToPrivateKeyPath, 
        string privateKeyPassword, 
        string email,
        string terminalId,
        string description,
        string merchantUrl,
        string merchantName,
        string merchant,
        string amount,
        string transactionCode="1",
        string language = "BG",
        string country = "BG",
        string currency = "BGN"
        )
    {
        Validate(
            pathToPublicKeyPath,
            pathToPrivateKeyPath, 
            privateKeyPassword, 
            email,
            terminalId, 
            transactionCode, 
            description,
            merchantUrl,
            merchantName,
            merchant,
            language,
            country,
            amount,
            currency);
        
        PathToPublicKeyPath = pathToPublicKeyPath;
        PathToPrivateKeyPath = pathToPrivateKeyPath;
        PrivateKeyPassword = privateKeyPassword;
        Email = email;
        TerminalId = terminalId;
        TransactionCode = transactionCode;
        Description = description;
        MerchantUrl = merchantUrl;
        MerchantName = merchantName;
        Merchant = merchant;
        Language = language;
        Country = country;
        Amount = amount;
        Currency = currency;
    }

    public string PathToPublicKeyPath { get; private set; }
    
    public string PathToPrivateKeyPath { get; private set; }
    
    public string PrivateKeyPassword { get; private set; }
    
    public string Email { get; private set; }
    
    public string TerminalId { get; private set; }
    
    public string TransactionCode { get; private set; }
    
    public string Description { get; private set; }
    
    public string MerchantUrl { get; private set; }
    
    public string MerchantName { get; private set; }
    
    public string Merchant { get; private set; }
    
    public string Language { get; private set; }
    
    public string Country { get; private set; }
    
    public string Amount { get; private set; }
    
    public string Currency { get; private set; }

    public void SetPathToPublicKeyPath(string pathToPublicKeyPath)
    {
        ValidatePathToPublicKeyPath(pathToPublicKeyPath);
        PathToPublicKeyPath = pathToPublicKeyPath;
    }

    public void SetPathToPrivateKeyPath(string pathToPrivateKeyPath)
    {
        ValidatePathToPrivateKeyPath(pathToPrivateKeyPath);
        PathToPrivateKeyPath = pathToPrivateKeyPath;
    }

    public void SetPrivateKeyPassword(string privateKeyPassword)
    {
        ValidatePrivateKeyPassword(privateKeyPassword);
        PrivateKeyPassword = privateKeyPassword;
    }

    public void SetEmail(string email)
    {
        ValidateEmail(email);
        Email = email;
    }

    public void SetTerminalId(string terminalId)
    {
        ValidateTerminalId(terminalId);
        TerminalId = terminalId;
    }

    public void SetTransactionCode(string transactionCode)
    {
        ValidateTransactionCode(transactionCode);
        TransactionCode = transactionCode;
    }

    public void SetDescription(string description)
    {
        ValidateDescription(description);
        Description = description;
    }

    public void SetMerchantUrl(string merchantUrl)
    {
        ValidateMerchantUrl(merchantUrl);
        MerchantUrl = merchantUrl;
    }

    public void SetMerchantName(string merchantName)
    {
        ValidateMerchantName(merchantName);
        MerchantName = merchantName;
    }

    public void SetMerchant(string merchant)
    {
        ValidateMerchant(merchant);
        Merchant = merchant;
    }

    public void SetLanguage(string language)
    {
        ValidateLanguage(language);
        Language = language;
    }

    public void SetCountry(string country)
    {
        ValidateCountry(country);
        Country = country;
    }
    
    public void SetAmount(string amount)
    {
        ValidateAmount(amount);
        Amount = amount;
    }
    
    public void SetCurrency(string currency)
    {
        ValidateCurrency(currency);
        Currency = currency;
    }

    private void ValidatePathToPublicKeyPath(string pathToPublicKeyPath)
    {
        if (string.IsNullOrWhiteSpace(pathToPublicKeyPath))
        {
            throw new BoricaNetException($"Invalid {nameof(PathToPublicKeyPath)}");
        }
    }

    private void ValidatePathToPrivateKeyPath(string pathToPrivateKeyPath)
    {
        if (string.IsNullOrWhiteSpace(pathToPrivateKeyPath))
        {
            throw new BoricaNetException($"Invalid {nameof(PathToPrivateKeyPath)}");
        }
    }

    private void ValidatePrivateKeyPassword(string privateKeyPassword)
    {
        if (string.IsNullOrWhiteSpace(privateKeyPassword))
        {
            throw new BoricaNetException($"Invalid {nameof(PrivateKeyPassword)}");
        }
    }

    private void ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new BoricaNetException($"Invalid {nameof(Email)}");
        }
    }

    private void ValidateTerminalId(string terminalId)
    {
        if (string.IsNullOrWhiteSpace(terminalId))
        {
            throw new BoricaNetException($"Invalid {nameof(TerminalId)}");
        }
    }

    private void ValidateTransactionCode(string transactionCode)
    {
        if (string.IsNullOrWhiteSpace(transactionCode))
        {
            throw new BoricaNetException($"Invalid {nameof(TransactionCode)}");
        }
    }

    private void ValidateDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            throw new BoricaNetException($"Invalid {nameof(Description)}");
        }
    }
    
    private void ValidateMerchantUrl(string merchantUrl)
    {
        if (string.IsNullOrWhiteSpace(merchantUrl))
        {
            throw new BoricaNetException($"Invalid {nameof(MerchantUrl)}");
        }
    }
    
    private void ValidateMerchantName(string merchantName)
    {
        if (string.IsNullOrWhiteSpace(merchantName))
        {
            throw new BoricaNetException($"Invalid {nameof(MerchantName)}");
        }
    }
    
    private void ValidateMerchant(string merchant)
    {
        if (string.IsNullOrWhiteSpace(merchant))
        {
            throw new BoricaNetException($"Invalid {nameof(Merchant)}");
        }
    }
    
    private void ValidateLanguage(string language)
    {
        if (string.IsNullOrWhiteSpace(language))
        {
            throw new BoricaNetException($"Invalid {nameof(Language)}");
        }
    }
    
    private void ValidateCountry(string country)
    {
        if (string.IsNullOrWhiteSpace(country))
        {
            throw new BoricaNetException($"Invalid {nameof(Country)}");
        }
    }
    
    private void ValidateAmount(string amount)
    {
        if (string.IsNullOrWhiteSpace(amount))
        {
            throw new BoricaNetException($"Invalid {nameof(Amount)}");
        }
    }
    
    private void ValidateCurrency(string currency)
    {
        if (string.IsNullOrWhiteSpace(currency))
        {
            throw new BoricaNetException($"Invalid {nameof(Currency)}");
        }
    }

    private void Validate(string pathToPublicKeyPath,
        string pathToPrivateKeyPath, 
        string privateKeyPassword, 
        string email,
        string terminalId,
        string transactionCode,
        string description,
        string merchantUrl,
        string merchantName,
        string merchant,
        string language,
        string country,
        string amount,
        string currency)
    {
        ValidatePathToPublicKeyPath(pathToPublicKeyPath);
        ValidatePathToPrivateKeyPath(pathToPrivateKeyPath);
        ValidatePrivateKeyPassword(privateKeyPassword);
        ValidateEmail(email);
        ValidateTerminalId(terminalId);
        ValidateTransactionCode(transactionCode);
        ValidateDescription(description);
        ValidateMerchantUrl(merchantUrl);
        ValidateMerchantName(merchantName);
        ValidateMerchant(merchant);
        ValidateLanguage(language);
        ValidateCountry(country);
        ValidateAmount(amount);
        ValidateCurrency(currency);
    }
}