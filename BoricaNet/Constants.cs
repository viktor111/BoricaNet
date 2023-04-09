namespace BoricaNet;

internal static class Constants
{
    public const string ADTD = "AD,TD";
    
    public const string TransactionCode1 = "1";
    public const string TransactionCode12 = "12";
    public const string TransactionCode21 = "21";
    public const string TransactionCode22 = "22";
    public const string TransactionCode24 = "24";
    public const string TransactionCode90 = "90";

    public const string BoricaDevUrl = "https://3dsgate-dev.borica.bg/cgi-bin/cgi_lin";
    public const string BoricaProdUrl = "https://3dsgate.borica.bg/cgi-bin/cgi_lin";

    public const string Order = "ORDER";

    internal static class BoricaParamNames
    {
        public const string Terminal = "TERMINAL";
        public const string TrType = "TRTYPE";
        public const string Order = "ORDER";
        public const string TranTrType = "TRAN_TRTYPE";
        public const string Nonce = "NONCE";
        public const string PSign = "P_SIGN";
    }

    internal static class BoricaErrorMessages
    {
        public const string InvalidSignatureEn = "Invalid signature";
        public const string InvalidSignatureBg = "Невалиден подпис";

        public const string NullResponseFromBorica = "Null response from Borica";

        public const string MessageNotHexEncoded = "Message not hex encoded";

        public const string TransactionCodeNotValidResponse = "Transaction code not valid. Cannot create signature for response";
        public const string TransactionCodeNotValidRequest = "Transaction code not valid. Cannot create signature for request";
    }

    internal static class SignerErrorMessages
    {
        public const string PrivateKeyNotFound = "Private key not found";
        public const string PublicKeyNotFound = "Public key not found";
        public const string PrivateKeyFilePathError = "Private key file path is required";
        public const string PrivateKeyPasswordError = "Private key password is required";
        public const string PublicKeyFilePathError = "Public key file path is required";
    }

    internal static class ResponseCodeMessages
    {
        public const string Code00En = "Success";
        public const string Code00Bg = "Успешна транзакция";

        public const string Code_1En = "A mandatory request field is not filled";
        public const string Code_1Bg = "Заявката съдържа поле с некоректно име";

        public const string Code_2En = "CGI request validation failed";
        public const string Code_2Bg = "Aвторизационният хост не отговаря или форматът на отговора е неправилен";

        public const string Code_4En = "No connection to the acquirer host (TS)";
        public const string Code_4Bg = "Няма връзка с авторизационния хост";

        public const string Code_5En = "Acquirer host (TS) does not respond or wrong format of e-gateway response template file";
        public const string Code_5Bg = "Грешка във връзката с авторизационния хост";

        public const string Code_6En = "e-Gateway configuration error";
        public const string Code_6Bg = "Грешка в конфигурацията на APGW";

        public const string Code_7En = "The acquirer host (TS) response is invalid, e.g. mandatory fields missing";
        public const string Code_7Bg = "Форматът на отговора от авторизационният хост е неправилен";

        public const string Code_10En = "Error in the \"Amount\" request field";
        public const string Code_10Bg = "Грешка в поле \"Сума\" в заявката";

        public const string Code_11En = "Error in the \"Currency\" request field";
        public const string Code_11Bg = "Грешка в поле \"Валута\" в заявката";

        public const string Code_12En = "Error in the \"Merchant ID\" request field";
        public const string Code_12Bg = "Грешка в поле \"Идентификатор на търговеца\" в заявката";

        public const string Code_13En = "The referrer IP address (usually the merchant's IP) is not the one expected";
        public const string Code_13Bg = "Неправилен IP адрес на търговеца";

        public const string Code_15En = "Error in the \"RRN\" request field";
        public const string Code_15Bg = "Грешка в поле \"RRN\" в заявката";

        public const string Code_16En = "Another transaction is being performed on the terminal";
        public const string Code_16Bg = "В момента се изпълнява друга трансакция на терминала";

        public const string Code_17En = "The terminal is denied access to e-Gateway";
        public const string Code_17Bg = "Отказан достъп до платежния сървър (напр. грешка при проверка на P_SIGN)";

        public const string Code_19En = "Error in the authentication information request or authentication failed.";
        public const string Code_19Bg = "Грешка в искането за автентикация или неуспешна автентикация";

        public const string Code_20En = "The permitted time interval (1 hour by default) between the transaction timestamp request field and the e-Gateway time was exceeded";
        public const string Code_20Bg = "Разрешената разлика между времето на сървъра на търговеца и e-Gateway сървъра е надвишена";

        public const string Code_21En = "The transaction has already been executed";
        public const string Code_21Bg = "Трансакцията вече е била изпълнена";

        public const string Code_22En = "Transaction contains invalid authentication information";
        public const string Code_22Bg = "Транзакцията съдържа невалидни данни за аутентикация";

        public const string Code_23En = "Invalid transaction context";
        public const string Code_23Bg = "Невалиден контекст на транзакцията";

        public const string Code_24En = "Transaction context data mismatch";
        public const string Code_24Bg = "Заявката съдържа стойности за полета, които не могат да бъдат обработени. Например валутата е различна от валутата на терминала или транзакцията е по-стара от 24 часа";

        public const string Code_25En = "Transaction confirmation state was canceled by user";
        public const string Code_25Bg = "Допълнителното потвърждение на трансакцията е отказано от картодържателя";

        public const string Code_26En = "Invalid action BIN";
        public const string Code_26Bg = "Невалиден BIN на картата";

        public const string Code_27En = "Invalid merchant name";
        public const string Code_27Bg = "Невалидно име на търговеца";

        public const string Code_28En = "Invalid incoming addendum(s)";
        public const string Code_28Bg = "Невалидно допълнително поле (например AD.CUST_BOR_ORDER_ID)";

        public const string Code_29En = "Invalid/duplicate authentication reference";
        public const string Code_29Bg = "Невалиден отговор от ACS на издателя на картат";

        public const string Code_30En = "Transaction was declined as fraud";
        public const string Code_30Bg = "Трансакцията е отказана";

        public const string Code_31En = "Transaction already in progress";
        public const string Code_31Bg = "Трансакцията е в процес на обрбаотка";

        public const string Code_32En = "Duplicate declined transaction";
        public const string Code_32Bg = "Дублирана отказана трансакция";

        public const string Code_33En = "Customer authentication by random amount or verify one-time code in progress";
        public const string Code_33Bg = "Трансакцията е в процес на аутентикация на картодържателя";

        public const string CodeDefaultEn = "Error in the transaction";
        public const string CodeDefaultBg = "Грешка в транзакцията";
    }

    internal static class ResponseCodes
    {
        public const string RC00 = "00";
        public const string RC_1 = "1";
        public const string RC_2 = "2";
        public const string RC_4 = "4";
        public const string RC_5 = "5";
        public const string RC_6 = "6";
        public const string RC_7 = "7";
        public const string RC_10 = "10";
        public const string RC_11 = "11";
        public const string RC_12 = "12";
        public const string RC_13 = "13";
        public const string RC_15 = "15";
        public const string RC_16 = "16";
        public const string RC_17 = "17";
        public const string RC_19 = "19";
        public const string RC_20 = "20";
        public const string RC_21 = "21";
        public const string RC_22 = "22";
        public const string RC_23 = "23";
        public const string RC_24 = "24";
        public const string RC_25 = "25";
        public const string RC_26 = "26";
        public const string RC_27 = "27";
        public const string RC_28 = "28";
        public const string RC_29 = "29";
        public const string RC_30 = "30";
        public const string RC_31 = "31";
        public const string RC_32 = "32";
        public const string RC_33 = "33";
    }

    internal static class ActionCodes
    {
        public const string AC0 = "0";
        public const string AC1 = "1";
        public const string AC2 = "2";
        public const string AC3 = "3";
        public const string AC7 = "7";
        public const string AC21 = "21";
    }
}