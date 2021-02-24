using System;
using Meadow.Foundation.Web.Maple.Server;
using Meadow.Foundation.Web.Maple.Server.Routing;
using Therm;

namespace Therm.MapleRequestHandlers
{
    public class TemperatureRequestHandlers : RequestHandlerBase
    {
        [HttpGet]
        public void SetTemp()
        {
            Console.WriteLine("GET::SetTemp");

            var temp = float.Parse(base.QueryString["temp"] as string);

            Console.WriteLine($"New desired temp: {temp}");

            var desiredTemp = new ClimateModel() { DesiredTemperature = temp };
            ClimateModelManager.Instance.UpdateDesiredClimate(desiredTemp);

            this.Context.Response.ContentType = ContentTypes.Application_Text;
            this.Context.Response.StatusCode = 200;
            _ = this.Send($"{temp} received");

        }
    }
}
