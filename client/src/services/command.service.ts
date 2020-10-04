import { buildUrl } from "../url-utils";

export interface CommandStatus {
  commandId: string,
  activityId: string,
  status: string,
}

export async function sendCommand(url: string, body: any) : Promise<CommandStatus> {
  const res = await fetch(buildUrl(url), {
    method: "POST",
    headers: {
      'Content-Type': 'application/json'
    },
    body: JSON.stringify(body)
  });
  const commandStatus = await res.json() as CommandStatus;
  console.log(`Command sent`, commandStatus);
  
  return await waitForResult(commandStatus.commandId);
}

export function waitForResult(commandId: string) : Promise<CommandStatus> {

  let attempts = 0;
  const pollStatus = async (resolve: any, reject: any) => {
    attempts++;
    console.log(`Polling for command ${commandId}. Attempt ${attempts}`);
    const res = await fetch(buildUrl(`/activities/commands/${commandId}`));
    if (res.status == 200) {
      const commandStatus = await res.json() as CommandStatus;
      if (commandStatus.status === 'Completed' || commandStatus.status === 'Failed') {
        resolve(commandStatus);
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