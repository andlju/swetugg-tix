import { useSnackbar } from "notistack";
import { useState } from "react";
import { CommandStatus, sendCommand } from "./services/command.service";

interface CommandOptions {
  method?: string
}

export function useCommand(commandName: string, options?: CommandOptions): [(commandUrl: string, data: any) => Promise<CommandStatus>, boolean] {
  const { enqueueSnackbar } = useSnackbar();
  const [sending, setSending] = useState(false);
  const method = options?.method ?? "POST";

  const _sendCommand = async (commandUrl: string, data: any) => {
    try {
      setSending(true);
      const result = await sendCommand(commandUrl, data, { method });
      enqueueSnackbar(`${commandName} completed`, { variant: "success" });
      return result;
    } catch (err) {
      enqueueSnackbar(err.message, { variant: "error" });
      throw err;
    } finally {
      setSending(false);
    }
  };

  return [
    _sendCommand,
    sending
  ]
}
