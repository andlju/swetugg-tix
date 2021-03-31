import { useEffect, useState } from 'react';
import { useDispatch, useSelector } from 'react-redux';

import { CommandOptions, sendActivityCommand } from '../store/activities/activities.actions';
import { CommandStatusState } from '../store/activities/activities.reducer';
import { RootState } from '../store/store';

interface QuickStatus {
  processing: boolean;
  completed: boolean;
  failed: boolean;
}

export function useActivityCommand<TBody>(
  url: string,
  options?: CommandOptions
): [(data: TBody) => void, QuickStatus, CommandStatusState | undefined] {
  const dispatch = useDispatch();
  const [uiId, setUiId] = useState('');
  const [status, setStatus] = useState<QuickStatus>({
    processing: false,
    completed: false,
    failed: false,
  });
  const command = useSelector((s: RootState) => s.activities.commands.find((c) => c.uiId == uiId));

  useEffect(() => {
    setStatus({
      processing: command?.status === 'Dispatched' || command?.status === 'Created',
      completed: command?.status === 'Completed',
      failed: command?.status === 'Failed',
    });
  }, [command?.status]);

  const _sendCommand = (body: TBody) => {
    const cmd = sendActivityCommand(url, body, options);
    setUiId(cmd.payload.uiId);
    dispatch(cmd);
  };

  return [_sendCommand, status, command];
}
