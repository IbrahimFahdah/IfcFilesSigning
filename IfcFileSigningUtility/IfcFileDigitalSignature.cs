using IfcFileSigningCommon;
using System.Security.Cryptography;

namespace IfcFileSigningUtility
{
    public class IfcFileDigitalSignature
    {
        /// <summary>
        /// This is a secret key and should not be shared with the public. Only the signer knows this. private keys can be generated using <see cref="GeneratePrivateKey"/>
        /// </summary>
        const string privateKey = "<RSAKeyValue><Modulus>w7xfeGEb7y8/dSXDKyohwXl8tgDJW0Fr/9gTpkgVwJtKlGVh6uN7BtrieJHCGTizNGGueSqQoN9t531cMQXPadg/b684sABermIOhbRq4MmEtjX2knxSJKU7SDKM3pAGmDRlbxhgrzHJL8sfe//TezZxp1NDT+Vwegs0jGFs2KU=</Modulus><Exponent>AQAB</Exponent><P>14OQBBZfjtNTJE38uhvFukY8FUgk7Rks9Ca5PCLGd7KwYFzUkA3laG34Ys5L40LQlL+gg3jXohMpRJlHzb5oew==</P><Q>6IGl5Z98l0U1OLKDJoTTqb0wBtT7j4X/WpDt7OeH8EKVvMa26vSBi4ZxvSI2eH2vfz3Kr/wSrRcEuMar9RtJXw==</Q><DP>Y4whucGb4h07Ckn7svuhGanXlvz8EYjPevdoGJ73jdK8Jca7aM8CaHpjgUBJTXBPaGYbfp8S+4peRZGH2UFagQ==</DP><DQ>wGR3uHiuiiX0kkP1DmyfETfBhAW9W9gPowuGNaCo9gDDEwCD4AwPHjtT5qNm23F1RR8Gl3VIpv4DJDsRk7LOlQ==</DQ><InverseQ>Z3Lcx28aNgo5NPf9BPu7kO/GIXFMNF2VbkojSDzJBTHvyAAmWie3lGILyA88IikNGJT+dKNEqdZP/kcY1lxDPA==</InverseQ><D>mS9hoDqPvB9EEJCfL7bneB12BpKTA4It3arjpe0gaP6f3YeCnGuvquu+9hFM0KRZS5NvEpDHY7+4qcSoVA1yBst/Nhtnihl3cOXf1Zus4TeFedLFJLyy+jvrBCsHLX7OH2roPBS65trNHG4ec21/FWAfi7Zwt3xmQ7tHOLJSA7k=</D></RSAKeyValue>";

        /// <summary>
        /// Signs an IFC file by adding the signature to the file top.
        /// </summary>
        /// <param name="filename"></param>
        public static void SignIfcFile(string filename)
        {
            var hash = Shared.CalculateIfcFileChecksum(filename);
            var singatureInfo = CreateSignature(hash, "Signed By IXF");

            string tempfile = Path.GetTempFileName();
            using (var writer = new StreamWriter(tempfile))
            using (var reader = new StreamReader(filename))
            {
                writer.WriteLine($"/*SignedBy:{singatureInfo.SignedBy}, Signature:{singatureInfo.Signature}*\\");
                while (!reader.EndOfStream)
                    writer.WriteLine(reader.ReadLine());
            }
            File.Copy(tempfile, filename, true);
        }

        /// <summary>
        /// use a private key to generate a secure digital signature.
        /// The private key must match the public key accessible to the system validating the license.
        /// </summary>
        public static IfcFileDigitalSignatureInfo CreateSignature(byte[] ifcFileHash,
               string signedBy)
        {
            // create the crypto-service provider:
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();

            // setup the dsa from the private key:
            rsa.FromXmlString(privateKey);

            // get the signature:
            byte[] signature = rsa.SignData(ifcFileHash, CryptoConfig.MapNameToOID("SHA512"));

            // now create the digital signature.
            return new IfcFileDigitalSignatureInfo()
            {
                SignedBy = signedBy,
                Signature = Convert.ToBase64String(signature),
            };
        }

        /// <summary>
        /// generate a new, private key. This will be the master key for generating digital signatures.
        /// </summary>
        /// <returns></returns>
        public static string GeneratePrivateKey()
        {
            RSACryptoServiceProvider dsa = new RSACryptoServiceProvider();
            return dsa.ToXmlString(true);
        }

        /// <summary>
        /// get the public key from a private key. This key must be distributed with the application.
        /// </summary>
        /// <param name="privateKey"></param>
        /// <returns></returns>
        public static string GetPublicKey(string privateKey)
        {
            RSACryptoServiceProvider dsa = new RSACryptoServiceProvider();
            dsa.FromXmlString(privateKey);
            return dsa.ToXmlString(false);
        }

        /// <summary>
        /// A helper function to generate the public and private key files in the specified folder.
        /// </summary>
        /// <param name="outputFolder"></param>
        public static void GenerateKeys(String outputFolder)
        {
            // create the directory if it doesn't exist:
            if (!Directory.Exists(outputFolder))
            {
                Directory.CreateDirectory(outputFolder);
            }

            // generate the required key files:
            String publicKeyFile = outputFolder + "\\publicKey.xml";
            String privateKeyFile = outputFolder + "\\privateKey.xml";

            // create a new private key:
            String privateKey = GeneratePrivateKey();

            // extract the public part of the key:
            String publicKey = GetPublicKey(privateKey);

            // save them:
            File.WriteAllText(publicKeyFile, publicKey);
            File.WriteAllText(privateKeyFile, privateKey);
        }
    }
}