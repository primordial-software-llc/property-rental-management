using System;
using System.Collections.Generic;
using System.Linq;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Api.Routes;
using Newtonsoft.Json.Linq;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]
namespace Api
{
    public class Function
    {
        public APIGatewayProxyResponse FunctionHandler(APIGatewayProxyRequest request, ILambdaContext context)
        {
            var clientDomain = "https://www.primordial-software.com";
            var response = new APIGatewayProxyResponse
            {
                Headers = new Dictionary<string, string>
                {
                    {"access-control-allow-origin", clientDomain},
                    {"Access-Control-Allow-Credentials", "true" }
                },
                StatusCode = 200
            };

            try
            {
                List<IRoute> routes = new List<IRoute>
                {
                };

                var matchedRoute = routes.FirstOrDefault(route => string.Equals(request.HttpMethod, route.HttpMethod, StringComparison.OrdinalIgnoreCase) &&
                                                                  string.Equals(request.Path, route.Path, StringComparison.OrdinalIgnoreCase));
                if (matchedRoute != null)
                {
                    matchedRoute.Run(request, response);
                }
                else
                {
                    response.StatusCode = 404;
                    response.Body = PropertyRentalManagement. Constants.JSON_EMPTY;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                response.StatusCode = 500;
                response.Body = new JObject {{"error", exception.ToString()}}.ToString();
            }
            return response;
        }

    }
}
