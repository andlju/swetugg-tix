import { buildUrl } from "../url-utils";

enum CommandLogSeverity {
  Verbose = 0,
  Debug = 1,
  Information = 2,
  Warning = 3,
  Error = 4
}

interface CommandStatusMessage {
  code: string,
  message: string,
  severity: CommandLogSeverity
}

export interface CommandStatus {
  commandId: string,
  aggregateId?: string,
  revision?: number,
  status: string,
  jsonBody: string,
  body: any,
  messages?: CommandStatusMessage[]
}

interface SendCommandOptions {
  method?: string
}

function waitForResult(commandId: string): Promise<CommandStatus> {

  let attempts = 0;
  const pollStatus = async (resolve: any, reject: any) => {
    attempts++;
    console.log(`Polling for command ${commandId}. Attempt ${attempts}`);
    const res = await fetch(buildUrl(`/orders/commands/${commandId}`));
    if (res.status == 200) {
      const commandStatus = await res.json() as CommandStatus;
      if (commandStatus.jsonBody) {
        commandStatus.body = JSON.parse(commandStatus.jsonBody);
      }
      if (commandStatus.status === 'Completed') {
        resolve(commandStatus);
        return;
      }
      if (commandStatus.status === 'Failed') {
        const message = commandStatus.messages?.find(_ => true) ?? { code: "UnknownError", message: "Unknown command failure" };
        reject(message);
        return;
      }
    }
    if (attempts >= 20) {
      reject({
        code: 'Timeout',
        message: 'Command timed out'
      });
      return;
    }
    setTimeout(() => pollStatus(resolve, reject), 1000);
  };

  const promise = new Promise<CommandStatus>((resolve, reject) => {
    pollStatus(resolve, reject);
  });
  return promise;
}

export async function sendOrderCommand(url: string, body: any, options? : SendCommandOptions): Promise<CommandStatus> {
  const res = await fetch(buildUrl(url), {
    method: options?.method ?? "POST",
    headers: {
      'Content-Type': 'application/json'
    },
    body: JSON.stringify(body)
  });
  if (res.status === 200) {
    const commandStatus = await res.json() as CommandStatus;
    console.log(`Command sent`, commandStatus);
    return await waitForResult(commandStatus.commandId);
  }
  throw { code: "CommandSendFailed", message: "Failed when sending command" }
}

