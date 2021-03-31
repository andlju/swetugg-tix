const authTenant = process.env['NEXT_PUBLIC_AUTH_TENANT_NAME'];

export const apiConfig = {
  b2cScopes: [`https://${authTenant}.onmicrosoft.com/helloapi/demo.read`],
};
