using System;
using NServiceBus;
using SimpleCqrs.Commanding;

namespace SimpleCqrs.NServiceBus.Commanding
{
    [Serializable]
    public class CommandWithReturnValueMessage : IMessage
    {
        public SimpleCqrs.Commanding.ICommand Command { get; set; }
    }
}