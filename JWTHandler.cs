using JWT.Algorithms;
using JWT.Builder;
using JWT.Exceptions;
using System.Security.Cryptography;

namespace EIV_Common;

public class JWTHandler
{
    public static void CreateRSA()
    {
        if (!Directory.Exists("cert"))
            Directory.CreateDirectory("cert");
        if (!File.Exists("cert/jwt_rsa.xml"))
        {
            RSA rsa = RSA.Create();
            File.WriteAllText("cert/jwt_rsa.xml", rsa.ToXmlString(true));
        }
    }

    public static RSA GetRSA()
    {
        RSA rsa = RSA.Create();
        if (!File.Exists("cert/jwt_rsa.xml"))
        {
            File.WriteAllText("cert/jwt_rsa.xml", rsa.ToXmlString(true));
            return rsa;
        }
        else
        {
            rsa.FromXmlString(File.ReadAllText("cert/jwt_rsa.xml"));
            return rsa;
        }
    }

    public static RSA Parse3DPartyRSA(string XML)
    {
        RSA rsa = RSA.Create();
        rsa.FromXmlString(XML);
        return rsa;
    }


    public static string GetJWTJson(string token, string RSAXML = "")
    {
        RSA rsa = RSAXML == "" ? GetRSA() : Parse3DPartyRSA(RSAXML);
        var alg = RSAXML == "" ? new RS256Algorithm(rsa, rsa) : new RS256Algorithm(rsa);
        var json = JwtBuilder.Create()
            .WithAlgorithm(alg)
            .Decode(token);

        return json;
    }

    public static string Create<T>(T claim, string claim_name, string ISSUER = "EIV_Common", double exp_hourTime = 1)
    {
        var now = DateTime.Now;
        RSA rsa = GetRSA();
        var token = JwtBuilder.Create()
        .WithAlgorithm(new RS256Algorithm(rsa, rsa))
        .AddClaim<T>(claim_name, claim)
        .ExpirationTime(now.AddHours(exp_hourTime))
        .IssuedAt(now)
        .Issuer(ISSUER)
        .Encode();

        return token;
    }

    public static JwtBuilder CreateBuilder()
    {
        var now = DateTime.Now;
        RSA rsa = GetRSA();
        return JwtBuilder.Create()
        .WithAlgorithm(new RS256Algorithm(rsa, rsa));
    }

    public static bool Validate(string token, string RSAXML = "")
    {
        RSA rsa = RSAXML == "" ? GetRSA() : Parse3DPartyRSA(RSAXML);
        var alg = RSAXML == "" ? new RS256Algorithm(rsa, rsa) : new RS256Algorithm(rsa);
        try
        {
            var json = JwtBuilder.Create()
                                 .WithAlgorithm(alg)
                                 .MustVerifySignature()
                                 .Decode(token);
        }
        catch (TokenNotYetValidException)
        {
            Console.WriteLine("Token is not valid yet");
            return false;
        }
        catch (TokenExpiredException)
        {
            Console.WriteLine("Token has expired");
            return false;
        }
        catch (SignatureVerificationException)
        {
            Console.WriteLine("Token has invalid signature");
            return false;
        }
        return true;
    }
}
