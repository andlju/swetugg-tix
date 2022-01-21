import { Observable, of } from 'rxjs';
import { ajax, AjaxResponse } from 'rxjs/ajax';
import { catchError, delay, filter, map, repeatWhen, take, tap, timeoutWith } from 'rxjs/operators';

import { buildUrl } from '../url-utils';

export enum CommandLogSeverity {
  Verbose = 0,
  Debug = 1,
  Information = 2,
  Warning = 3,
  Error = 4,
}

export interface CommandStatusMessage {
  code: string;
  message: string;
  severity: CommandLogSeverity;
}

export interface CommandStatus {
  commandId: string;
  aggregateId?: string;
  revision?: number;
  status: string;
  jsonBody?: string;
  body?: unknown;
  messages?: CommandStatusMessage[];
}

interface SendCommandOptions {
  method?: string;
  token?: string;
}

export function waitForCommandResult$(commandId: string, token?: string): Observable<CommandStatus> {
  const url = buildUrl(`/activities/commands/${commandId}`);

  return ajax.getJSON<CommandStatus>(url, { Authorization: `Bearer ${token}` }).pipe(
    catchError((err) => {
      console.log('Error getting CommandStatus', err);
      return of({
        commandId: commandId,
        status: 'Created',
        body: null,
        jsonBody: '',
      });
    }),
    filter((commandStatus) => commandStatus.status !== 'Created'),
    map((status) => ({ ...status, body: status.jsonBody && JSON.parse(status.jsonBody) })),
    repeatWhen((obs) => obs.pipe(delay(1000))),
    take(1),
    timeoutWith(
      10000,
      of({
        commandId: commandId,
        status: 'Failed',
        body: null,
        jsonBody: '',
        messages: [
          {
            code: 'Timeout',
            message: 'Command timed out',
            severity: CommandLogSeverity.Error,
          },
        ],
      })
    )
  );
}

export function sendActivityCommand$<TBody>(url: string, body: TBody, options: SendCommandOptions): Observable<CommandStatus> {
  const headers = { Authorization: `Bearer ${options.token}`, 'Content-Type': 'application/json' };
  const commandUrl = buildUrl(url);

  const sendCommand = (): Observable<AjaxResponse> => {
    switch (options.method) {
      case 'PUT':
        return ajax.put(commandUrl, body, headers);
      case 'POST':
        return ajax.post(commandUrl, body, headers);
      case 'DELETE':
        return ajax.delete(commandUrl, headers);
      default:
        return ajax.post(commandUrl, body, headers);
    }
  };

  return sendCommand().pipe(map((resp) => resp.response as CommandStatus));
}
