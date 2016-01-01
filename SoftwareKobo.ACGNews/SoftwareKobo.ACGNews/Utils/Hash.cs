using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;

namespace SoftwareKobo.ACGNews.Utils
{
    public static class Hash
    {
        public static string GetHash(string algoritm, string str)
        {
            var alg = HashAlgorithmProvider.OpenAlgorithm(algoritm);
            var buff = CryptographicBuffer.ConvertStringToBinary(str, BinaryStringEncoding.Utf8);
            var hashed = alg.HashData(buff);
            var res = CryptographicBuffer.EncodeToHexString(hashed);
            return res;
        }

        public static string GetMd5(string str)
        {
            return GetHash(HashAlgorithmNames.Md5, str);
        }
    }
}