using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace BoricaNet.Core;

internal class Signer
{
    private readonly RSA _privateKey;
    private readonly RSA _publicKey;
    
    public Signer(string privateKeyFilePath, string privateKeyPassword, string publicKeyFilePath)
    {
        var pfx = new X509Certificate2(privateKeyFilePath, privateKeyPassword, X509KeyStorageFlags.Exportable);
        _privateKey = pfx.GetRSAPrivateKey() ?? throw new Exception("Private key not found");
        
        var certificate = new X509Certificate2(publicKeyFilePath);
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