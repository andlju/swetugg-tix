import { useSnackbar } from "notistack";
import { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import { RootState } from "../store/store";
import { CommandStatus, sendActivityCommand } from "./services/activity-command.service";

interface CommandOptions {
  method?: string;
}

export function useActivityCommand<TBody>(commandName: string, options?: CommandOptions): [(commandUrl: string, data: TBody) => Promise<CommandStatus>, boolean] {
  const { enqueueSnackbar } = useSnackbar();
  const [ sending, setSending ] = useState(false);

  const token = useSelector((s: RootState) => s.auth.accessToken);

  const method = options?.method ?? "POST";

  const _sendCommand = async (commandUrl: string, data: TBody) => {
    try {
      setSending(true);
      const result = await sendActivityCommand(commandUrl, data, { method, token: token });
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
  ];
}
