namespace Generators.Extensions
{
    public static class StringGenExtension
    {
        public static string GetIfTrue(this string src, bool condition) => condition ? src : "";
    }
}
