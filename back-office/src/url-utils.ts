export function buildUrl(url: string) : string {
  const rootUrl = process.env.NEXT_PUBLIC_API_ROOT;
  return `${rootUrl}${url}`;
}
