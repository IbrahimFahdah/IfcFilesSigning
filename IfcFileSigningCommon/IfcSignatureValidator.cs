using System.Security.Cryptography;
using System.Text;

namespace IfcFileSigningCommon
{
    public class IfcSignatureValidator
    {
        /// <summary>
        /// This is a key that can be shared with the public to validate signatures created by the private key.
        /// </summary>
        public const string PublicKey = "<RSAKeyValue><Modulus>w7xfeGEb7y8/dSXDKyohwXl8tgDJW0Fr/9gTpkgVwJtKlGVh6uN7BtrieJHCGTizNGGueSqQoN9t531cMQXPadg/b684sABermIOhbRq4MmEtjX2knxSJKU7SDKM3pAGmDRlbxhgrzHJL8sfe//TezZxp1NDT+Vwegs0jGFs2KU=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

        public static (IfcSignatureStatus status, string singedBy) ValidateFile(string filename)
        {
            var sigData = new List<string>();

            using (var reader = new StreamReader(filename))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (line != null && line.StartsWith("SIGNATURE;"))
                    {
                        while (!reader.EndOfStream)
                        {
                            line = reader.ReadLine();
                            if (line != null && line.StartsWith("ENDSEC;"))
                                break;
                            sigData.Add(line);
                        }
                    }
                }
            }

            var singedByLine = sigData.FirstOrDefault(x => x.StartsWith("SignedBy"));
            var signatureLine = sigData.FirstOrDefault(x => x.StartsWith("SIGNATURE_DATA"));
            if (!string.IsNullOrWhiteSpace(singedByLine))
            {
                var signedBy = singedByLine.Substring(singedByLine.IndexOf('(') + 1, singedByLine.Length - singedByLine.IndexOf('(') - 3);
                if (signatureLine != null)
                {
                    var cryptedSignature = signatureLine.Substring(signatureLine.IndexOf('(') + 1, signatureLine.Length - signatureLine.IndexOf('(') - 3);

                    // create the crypto-service provider:
                    RSACryptoServiceProvider dsa = new RSACryptoServiceProvider();

                    // setup the provider from the public key:
                    dsa.FromXmlString(PublicKey);

                    // get the signature data:
                    var signature = Convert.FromBase64String(cryptedSignature);

                    var ifcData = GetIFCDataWithoutSignature(filename);

                    var current = Shared.CalculateIfcFileChecksum(ifcData);

                    if (dsa.VerifyData(current, CryptoConfig.MapNameToOID("SHA512"), signature))
                    {
                        return (IfcSignatureStatus.Valid, signedBy);
                    }

                    return (IfcSignatureStatus.Invalid, null);
                }
            }

            return (IfcSignatureStatus.NotFound, null);
        }

        public static byte[] GetIFCDataWithoutSignature(string filename)
        {
            var data = File.ReadAllText(filename, Encoding.UTF8);
            var sec1 = data.Substring(0, data.IndexOf("SIGNATURE;") - 2);
            var sec2 = data.Substring(data.IndexOf("SIGNATURE;"));
            var signSectionEndIndex = sec2.IndexOf("ENDSEC;");
            var str2 = sec2.Substring("ENDSEC;".Length + signSectionEndIndex);
            var originalData = sec1 + str2;

            var buffer = Encoding.ASCII.GetBytes(originalData);

            return buffer;
        }
    }
}