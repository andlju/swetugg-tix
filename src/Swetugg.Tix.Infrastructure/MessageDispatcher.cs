﻿using System;
using System.Collections.Generic;

namespace Swetugg.Tix.Infrastructure
{
    public class MessageDispatcher : IMessageDispatcher
    {
        private readonly IDictionary<Type, Action<object>> _handlers = new Dictionary<Type, Action<object>>();

        /// <summary>
        /// Register a handler factory for a message. 
        /// </summary>
        /// <typeparam name="T">Message type</typeparam>
        /// <param name="commandHandlerFunc">The function that will be called to create a handler for the message</param>
        public void Register<T>(Func<IMessageHandler<T>> commandHandlerFunc)
        {
            Action<object> handleAction = (cmd) =>
            {
                var handler = commandHandlerFunc();
                handler.Handle((T)cmd);
            };
            _handlers[typeof(T)] = handleAction;
        }

        /// <summary>
        /// Dispatch this message
        /// </summary>
        /// <remarks>
        /// Finds the correct handler for this message, creates an instance
        /// and uses it to handle the message
        /// </remarks>
        /// <exception cref="MessageHandlerException">
        /// If no handler is found, a <see cref="MessageHandlerException" /> is thrown
        /// </exception>
        /// <param name="msg">Message to dispatch</param>
        public void Dispatch(object msg)
        {
            Action<object> handler;
            var messageType = msg.GetType();
            if (!_handlers.TryGetValue(messageType, out handler))
                throw new MessageHandlerException($"No handler found for {messageType}");
            handler(msg);
        }
    }
}