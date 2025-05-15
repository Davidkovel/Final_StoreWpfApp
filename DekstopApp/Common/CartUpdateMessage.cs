using CommunityToolkit.Mvvm.Messaging.Messages;

namespace DekstopApp.Common;

public class CartUpdatedMessage : ValueChangedMessage<bool>
{
    public CartUpdatedMessage() : base(true) { }
}