import { useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { CommandOptions, sendActivityCommand } from "../store/activities/activities.actions";
import { ActivityCommandState } from "../store/activities/activities.reducer";
import { RootState } from "../store/store";

export function useActivityCommand<TBody>(url: string, options?: CommandOptions) : [(data: TBody) => void, ActivityCommandState | undefined ] {
  const dispatch = useDispatch();
  const [uiId, setUiId] = useState('');
  
  const command = useSelector((s: RootState) => s.activities.commands.find(c => c.uiId == uiId));
  
  const _sendCommand = (body: TBody) => {
    const cmd = sendActivityCommand(url, body, options);
    setUiId(cmd.payload.uiId);
    dispatch(cmd);
  }

  return [ _sendCommand, command ];
}