


namespace Entities.Helpers
{
    public static class StringHelper
    {

        public static string ToLowerCamelCase(this string source)
        {
            if (string.IsNullOrWhiteSpace(source))
                return source;

            var charArray = source.ToCharArray();
            charArray[0] = char.ToLower(charArray[0]);
            return new string(charArray);
        }
    }
}
