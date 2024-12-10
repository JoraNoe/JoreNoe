using JoreNoe.Middleware;

namespace Test
{
    public class WeatherForecast : IJorenoeRuningRequestLogging
    {
        public Task RunningRequestLogging(JorenoeRuningRequestLoggingModel Context)
        {
            throw new NotImplementedException();
        }
    }
}
