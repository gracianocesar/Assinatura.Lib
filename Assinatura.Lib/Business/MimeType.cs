using org.apache.tika;

namespace Assinador.Lib.Business
{
    public static class MimeType
    {
        public static string GetMimeFromFile(byte[] data)
        {
            try
            {
                var tika = new Tika();
                var result = tika.detect(data);

                if ("audio/opus".Equals(result))
                {
                    return "audio/ogg";
                }
                else if ("video/opus".Equals(result))
                {
                    return "video/ogg";
                }

                return result;
            }
            catch
            {
                return "unknown/unknown";
            }
        }
    }
}
