using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Euroins.Payment.Services.Shared;

public class Signer : ISigner
{
    private const string PathToPfx = "Config/keystore_test.pfx";
    private const string PathToCer = "Config/V6200275-euroins.com-T.cer";
    
    private readonly RSA _privateKey;
    private readonly RSA _publicKey;
    
    public Signer()
    {
        var pfx = new X509Certificate2(PathToPfx, "viktor11", X509KeyStorageFlags.Exportable);
        _privateKey = pfx.GetRSAPrivateKey() ?? throw new Exception("Private key not found");
        
        var certificate = new X509Certificate2(PathToCer);
        _publicKey = certificate.GetRSAPublicKey() ?? throw new Exception("Public key not found");
    }

    public byte[] CreateSignature(byte[] data)
    {
       return _privateKey.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
    }

    public bool VerifySignature(byte[] messageData, byte[] signature)
    {
        return _publicKey.VerifyData(messageData, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
    }
}