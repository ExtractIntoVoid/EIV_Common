using JWT.Algorithms;
using JWT.Builder;
using JWT.Exceptions;
using System.Security.Cryptography;

namespace EIV_Common
{
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


        /// <summary>
        /// Get JWT Token as JSON
        /// </summary>
        /// <param name="token">The Token</param>
        /// <returns>JSON String</returns>
        public static string GetJWTJson(string token)
        {
            RSA rsa = GetRSA();
            var json = JwtBuilder.Create()
                                 .WithAlgorithm(new RS256Algorithm(rsa, rsa))
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

        /// <summary>
        /// Validating any jwt Token
        /// </summary>
        /// <param name="token">The Token</param>
        /// <returns>True | False</returns>
        public static bool Validate(string token)
        {
            RSA rsa = GetRSA();
            try
            {
                var json = JwtBuilder.Create()
                                     .WithAlgorithm(new RS256Algorithm(rsa, rsa))
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
}
