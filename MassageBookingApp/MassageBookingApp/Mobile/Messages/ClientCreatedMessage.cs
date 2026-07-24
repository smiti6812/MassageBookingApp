using CommunityToolkit.Mvvm.Messaging.Messages;
using MassageBookingApp.Mobile.Models.Clients;

namespace MassageBookingApp.Mobile.Messages
{
    public class ClientCreatedMessage(ClientDto value) : ValueChangedMessage<ClientDto>(value)
    {
    }
}
