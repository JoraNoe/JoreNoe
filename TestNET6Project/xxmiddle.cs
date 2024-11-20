using JoreNoe.Queue.RBMQ;

namespace TestNET6Project
{
    public static class xxmiddle
    {
        
        public static void usexxmiddle(this IApplicationBuilder biilder)
        {
            //var con = biilder.ApplicationServices.GetRequiredService<IQueueManger>();
            //// 注册使用
            //con.Receive(new reci(),"test");
        }
    }

    public class reci : ICustome<string>
    {
        public async Task<string> ConSume(CustomeContent<string> Context)
        {
            Console.WriteLine(Context.Context);
            return Context.Context;
        }
    }
}
