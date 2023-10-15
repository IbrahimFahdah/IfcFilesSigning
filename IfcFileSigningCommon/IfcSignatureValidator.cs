using System.Security.Cryptography;

namespace IfcFileSigningCommon
{
    public class IfcSignatureValidator
    {
        /// <summary>
        /// This is a key that can be shared with the public to validate signatures created by the private key.
        /// </summary>
        const string publicKey = "<RSAKeyValue><Modulus>w7xfeGEb7y8/dSXDKyohwXl8tgDJW0Fr/9gTpkgVwJtKlGVh6uN7BtrieJHCGTizNGGueSqQoN9t531cMQXPadg/b684sABermIOhbRq4MmEtjX2knxSJKU7SDKM3pAGmDRlbxhgrzHJL8sfe//TezZxp1NDT+Vwegs0jGFs2KU=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

        public static (IfcSignatureStatus status, string singedBy) ValidateFile(string path)
        {
            var data = File.ReadLines(path).First();
            var indx = data.IndexOf("Signature:");
            if (indx>0)
            {
                // read the signature
                var signedBy = data.Substring(data.IndexOf("SignedBy:")+ "SignedBy:".Length, indx- data.IndexOf("SignedBy:")- "SignedBy:".Length-1);
                var cryptedSignature = data.Substring(indx + "Signature:".Length);
                cryptedSignature= cryptedSignature.Substring(0,cryptedSignature.Length-2);

                // create the crypto-service provider:
                RSACryptoServiceProvider dsa = new RSACryptoServiceProvider();

                // setup the provider from the public key:
                dsa.FromXmlString(publicKey);

                // get the signature data:
                var signature = Convert.FromBase64String(cryptedSignature);

                var ifcData=GetIFCData(path);

                var current=Shared.CalculateIfcFileChecksum(ifcData);

                if (dsa.VerifyData(current, CryptoConfig.MapNameToOID("SHA512"), signature))
                {
                    return (IfcSignatureStatus.Valid, signedBy);
                }

                return (IfcSignatureStatus.Invalid, null);
            }

            return (IfcSignatureStatus.NotFound, null);
        }

        public static byte[]  GetIFCData(string filePath)
        {
            // Read all bytes from the file
            byte[] fileBytes = File.ReadAllBytes(filePath);

            // Find the position of the first newline character
            int firstNewlinePosition = Array.IndexOf(fileBytes, (byte)'\n');

            if (firstNewlinePosition >= 0)
            {
                // Calculate the length of the byte array without the first line
                int newLength = fileBytes.Length - (firstNewlinePosition + 1);

                // Create a new byte array to store the content without the first line
                byte[] newBytes = new byte[newLength];

                // Copy the bytes after the first line to the new array
                Array.Copy(fileBytes, firstNewlinePosition + 1, newBytes, 0, newLength);

                return newBytes;
            }

            return null;
        }
    }
}