using Generators.GenerateCodeModels;

namespace Generators
{
    public class AppCore
    {
        public static void LogError(string msg)
        {
            System.IO.File.AppendAllText(Consts.ErrorFileName, msg);
        }
    }
}
