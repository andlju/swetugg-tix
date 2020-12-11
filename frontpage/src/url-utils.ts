export function buildUrl(url: string) : string {
  const rootUrl = process.env.NEXT_PUBLIC_API_ROOT ?? 'http://localhost:4711/api';
  return `${rootUrl}${url}`;
}
