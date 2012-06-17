﻿using NServiceBus;
using SimpleCqrs.Commanding;

namespace SimpleCqrs.NServiceBus.Commanding
{
    public static class CommandBusExtensionMethods
    {
        public static ICallback ExecuteWithCallback<TCommand>(this ICommandBus commandBus, TCommand command) where TCommand : SimpleCqrs.Commanding.ICommand
        {
            var bus = (NsbCommandBus)commandBus;
            var destination = bus.GetDestinationForCommandType<TCommand>();
            return bus.InnerBus.Send<CommandWithReturnValueMessage>(destination, message => message.Command = command);
        }

        public static int ExecuteWeb<TCommand>(this ICommandBus commandBus, TCommand command) where TCommand : SimpleCqrs.Commanding.ICommand
        {
            var bus = (NsbCommandBus)commandBus;
            var destination = bus.GetDestinationForCommandType<TCommand>();
            var returnValue = 0;

            bus.InnerBus.Send<CommandWithReturnValueMessage>(destination, message => message.Command = command)
                .Register<int>(errorCode => returnValue = errorCode);

            return returnValue;
        }
    }
}