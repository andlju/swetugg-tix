using Swetugg.Tix.Activity.Commands.Admin;
using Swetugg.Tix.Infrastructure;
using System;
using System.Threading.Tasks;

namespace Swetugg.Tix.Activity.Domain.Handlers.Admin
{
    public abstract class AdminCommandHandler<TCmd> :
        IMessageHandler<TCmd>
        where TCmd : IActivityAdminCommand
    {
        ICommandLog _commandLog;

        protected AdminCommandHandler(ICommandLog commandLog)
        {
            _commandLog = commandLog;
        }

        protected abstract void HandleCommand(TCmd command);

        public virtual async Task Handle(TCmd command)
        {
            await _commandLog.Store(command.CommandId, command);

            try
            {
                HandleCommand(command);
                await _commandLog.Complete(command.CommandId);
            }
            catch (ActivityException ex)
            {
                // This is a domain error and shouldn't be retried
                await _commandLog.Fail(command.CommandId, ex.ErrorCode, ex.Message);
            }
            catch (Exception ex)
            {
                await _commandLog.Fail(command.CommandId, "UnknownError", ex.ToString());
                throw;
            }
        }
    }
}