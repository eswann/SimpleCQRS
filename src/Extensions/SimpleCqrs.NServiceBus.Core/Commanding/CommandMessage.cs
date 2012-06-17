using System;
using NServiceBus;
using SimpleCqrs.Commanding;

namespace SimpleCqrs.NServiceBus.Commanding
{
    [Serializable]
    public class CommandMessage : IMessage
    {
        public SimpleCqrs.Commanding.ICommand Command { get; set; }
    }
}