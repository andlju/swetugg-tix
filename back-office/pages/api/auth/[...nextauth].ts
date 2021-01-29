import { NextApiRequest } from 'next';
import NextAuth, { InitOptions } from 'next-auth';
import { Profile } from 'next-auth/adapters';

const tenantName = process.env.NEXT_AUTH_TENANT_NAME
const tenantGuid = process.env.NEXT_AUTH_TENANT_GUID
const userFlow = process.env.NEXT_USER_FLOW

console.log('ClientId', process.env.AUTH_CLIENT_ID);
console.log('Client secret', process.env.AUTH_CLIENT_SECRET);

const options : InitOptions = {
  session: {
    jwt: true,
  },
  secret: process.env.JWT_SECRET,
  pages: {
    signOut: '/auth/signout',
  },
  providers: [
    {
      id: 'azureb2c',
      name: 'Azure B2C',
      type: 'oauth',
      version: '2.0',
      debug: true,
      scope: 'offline_access openid',
      params: {
        grant_type: 'authorization_code',
      },
      accessTokenUrl: `https://${tenantName}.b2clogin.com/${tenantName}.onmicrosoft.com/${userFlow}/oauth2/v2.0/token`,
      // requestTokenUrl: `https://login.microsoftonline.com/${process.env.AUTH_TENANT_GUID}/oauth2/v2.0/token`,
      authorizationUrl: `https://${tenantName}.b2clogin.com/${tenantName}.onmicrosoft.com/${userFlow}/oauth2/v2.0/authorize?response_type=code+id_token&response_mode=form_post`,
      profileUrl: 'https://graph.microsoft.com/oidc/userinfo',
      profile: (profile: Profile | any) => {
        console.log('THE PROFILE', profile)

        return {
          id: profile.oid,
          firstName: profile.given_name,
          lastName: profile.family_name,
          email: profile.emails.length ? profile.emails[0] : null,
        }
      },
      clientId: process.env.AUTH_CLIENT_ID,
      clientSecret: process.env.AUTH_CLIENT_SECRET,
      idToken: true,
      state: false,
    },
  ],
}

export default (req: NextApiRequest, res: NextApiResponse<any>) => NextAuth(req, res, options)