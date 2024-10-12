using JoreNoe;

namespace TestNET6Project
{
    public class BaseService
    {
        private readonly IServiceProvider _serviceProvider;
        public BaseService(IServiceProvider ser)
        {
            this._serviceProvider = ser;
        }

        protected T getService<T>() => this._serviceProvider.GetRequiredService<T>();
    }
}
