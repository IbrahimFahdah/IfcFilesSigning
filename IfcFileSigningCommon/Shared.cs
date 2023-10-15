using System.Security.Cryptography;

namespace IfcFileSigningCommon
{
    public class Shared
    {
        public static byte[] CalculateIfcFileChecksum(string filename)
        {
            return CalculateIfcFileChecksum(File.ReadAllBytes(filename));
        }

        public static byte[] CalculateIfcFileChecksum(byte[] data)
        {
            using (var md5 = MD5.Create())
            {
                return md5.ComputeHash(data);
            }
        }
    }
}