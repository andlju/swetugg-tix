export function buildUrl(url: string) {
  const baseUrl = 'http://localhost:4711/api';
  // const baseUrl = 'https://tix-dev-apieee8332a.azurewebsites.net/api'
  return `${baseUrl}${url}`;
}