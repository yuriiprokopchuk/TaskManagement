using System.Text;

namespace TaskManagement.Bus.Infrastructure.Extensions
{
    public static class BusExtensions
    {
        public static byte[] ToBytes(this string str)
        {
            return Encoding.UTF8.GetBytes(str);
        }

        public static string ToContent(this byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes);
        }

        public static string CreateName(this Type type)
        {
            if (string.IsNullOrWhiteSpace(type.FullName))
                throw (new Exception("Unable to define queue name"));

            var queueName = type.FullName.Replace(".Commands", string.Empty);

            return queueName;
        }
    }
}
