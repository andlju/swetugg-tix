using System;
using System.Collections.Generic;

namespace Swetugg.Tix.Infrastructure
{
    public class CommandDispatcher : ICommandDispatcher
    {
        private readonly IDictionary<Type, Action<object>> _handlers = new Dictionary<Type, Action<object>>();

        public void Register<T>(Func<ICommandHandler<T>> commandHandlerFunc)
        {
            Action<object> handleAction = (cmd) =>
            {
                var handler = commandHandlerFunc();
                handler.Handle((T)cmd);
            };
            _handlers[typeof(T)] = handleAction;
        }

        /// <summary>
        /// Dispatch this command
        /// </summary>
        /// <remarks>
        /// Finds the correct command handler for this message, creates an instance
        /// and uses it to handle the command
        /// </remarks>
        /// <exception cref="Swetugg.Tix.Activity.Domain.CommandHandlerException">
        /// If no handler is found, a <see cref="Swetugg.Tix.Activity.Domain.CommandHandlerException" /> is thrown
        /// </exception>
        /// <param name="cmd">Command to dispatch</param>
        public void Dispatch(object cmd)
        {
            Action<object> handler;
            var commandType = cmd.GetType();
            if (!_handlers.TryGetValue(commandType, out handler))
                throw new CommandHandlerException($"No handler found for {commandType}");
            handler(cmd);
        }
    }
}