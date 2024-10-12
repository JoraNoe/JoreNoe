using JoreNoe.Middleware;

namespace TestNET6Project
{
    public class ip:IJoreNoeAPIGlobalVisitRecordIpAddressMiddleware
    {

        public async Task VisitRecordIpAddress(GlobalVisitRecord Context)
        {
            Console.WriteLine(Context.IpAddress);
            
        }
    }
}
