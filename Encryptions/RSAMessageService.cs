using System.Security.Cryptography;

namespace EIV_Common.Encryptions
{
    public class RSAMessageService
    {
        static RSA private_rsa; // this user private key.
        static RsaService privateService; //this user private service
        static Dictionary<string, RsaService> public_RSAServices;

        static RSAMessageService()
        {
            private_rsa = RSA.Create(1024);
            if (!Directory.Exists("cert"))
                Directory.CreateDirectory("cert");
            if (File.Exists("cert/rsa_private.xml"))
                private_rsa.FromXmlString(File.ReadAllText("cert/rsa_private.xml"));
            File.WriteAllText("cert/rsa_private.xml", private_rsa.ToXmlString(true));
            privateService = new(private_rsa);
            public_RSAServices = [];
        }

        /// <summary>
        /// Encrypt message to the UserId
        /// </summary>
        /// <param name="msg">Message as Bytes</param>
        /// <param name="userId">UserId</param>
        /// <returns>Encrypted message</returns>
        public static byte[] EncryptMessage(byte[] msg, string userId)
        {
            if (!public_RSAServices.ContainsKey(userId))
                return [];
            return public_RSAServices[userId].Encrypt(msg);
        }

        /// <summary>
        /// Adding new user into collection.
        /// </summary>
        /// <param name="userId">UserID</param>
        /// <param name="publicKey">RSA PublicKey</param>
        public static void AddRSA(string userId, byte[] publicKey)
        {
            RSA rsa = RSA.Create(1024);
            rsa.ImportRSAPublicKey(publicKey, out _);
            AddRSA(userId, rsa);
        }

        /// <summary>
        /// Adding new user into collection.
        /// </summary>
        /// <param name="userId">UserID</param>
        /// <param name="xml">RSA exported as XML format</param>
        public static void AddRSA(string userId, string xml)
        {
            RSA rsa = RSA.Create(1024);
            rsa.FromXmlString(xml);
            AddRSA(userId, rsa);
        }

        /// <summary>
        /// Adding new user into collection.
        /// </summary>
        /// <param name="userId">UserID</param>
        /// <param name="rsa">The RSA</param>
        public static void AddRSA(string userId, RSA rsa)
        {
            public_RSAServices[userId] = new(rsa);
        }

        /// <summary>
        /// Decrypt message that sent to current user.
        /// </summary>
        /// <param name="encoded_msg">Encoded message from other users "EncryptMessage" value</param>
        /// <returns>Decrypted message</returns>
        public static byte[] DecryptMessage(byte[] encoded_msg)
        {
            return privateService.Decrypt(encoded_msg);
        }

        /// <summary>
        /// Getting RSA public Key
        /// </summary>
        /// <returns>The public key in bytes</returns>
        public static byte[] GetPublicKey()
        {
            return private_rsa.ExportRSAPublicKey();
        }
    }
}
