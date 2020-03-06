using Amazon.Lambda.APIGatewayEvents;

namespace Api.Routes
{
    interface IRoute
    {
        string HttpMethod { get; }
        string Path { get; }
        void Run(
            APIGatewayProxyRequest request,
            APIGatewayProxyResponse response);
    }
}
