using MassTransit;
using MongoDB.Bson;

using Reclamos.Domain.Events;

using Reclamos.Infrastructure.Interfaces;
using RestSharp;

namespace Reclamos.Infrastructure.Consumer
{
    public class SendNotificationConsumer(IServiceProvider serviceProvider, IRestClient restClient) : IConsumer<NotificationSendEvent>
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider;
        private readonly IRestClient RestClient = restClient;

        public async Task Consume(ConsumeContext<NotificationSendEvent> @event)
        {
            try
            {
                var message = @event.Message;
                var APIRequest = new RestRequest(Environment.GetEnvironmentVariable("NOTIFICACION_MS_URL") + "/enviar", Method.Post);
                APIRequest.AddJsonBody(new
                {
                    IdsUsuarios = message.IdsUsuarios,
                    Motivo = message.Motivo,
                    Cuerpo = message.Cuerpo
                });
                var APIResponse = await RestClient.ExecuteAsync(APIRequest);
                if (!APIResponse.IsSuccessful)
                {
                    throw new Exception("Error al obtener la información del usuario.");
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}