using System.Web.Script.Serialization;

namespace System.Runtime.Serialization
{
    public static class JsonSerializerUtility
    {
        public static T DeserializeFromString<T>(string text) => new JavaScriptSerializer() { MaxJsonLength = int.MaxValue }.Deserialize<T>(text);

        public static string SerializeToString(object item) => new JavaScriptSerializer() { MaxJsonLength = int.MaxValue }.Serialize(item);
    }
}