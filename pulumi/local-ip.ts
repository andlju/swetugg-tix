import { v4 } from "public-ip";

export async function getPublicIp() {
  
  try {
    const myIp = await v4();
    console.log(`Found local IP: ${myIp}`);
    return myIp;
  } catch(e) {
    console.error(`Failed to get local IP`);
    return undefined;
  }
}
