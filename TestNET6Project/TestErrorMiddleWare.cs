using JoreNoe.Middleware;

namespace TestNET6Project
{
    public class TestErrorMiddleWare : IJoreNoeGlobalErrorHandling
    {
        public async Task GlobalErrorHandling(Exception Ex)
        {
            await Console.Out.WriteLineAsync(JoreNoeRequestCommonTools.FormatError(Ex));
        }
    }
}
