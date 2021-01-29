import { NextPage } from 'next';
import { signOut } from 'next-auth/client'

const Signout: NextPage = () => {
  if (typeof window !== 'undefined') {
    console.log('Is client - signing out')
    console.log('*** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***')

    signOut({ callbackUrl: process.env.NEXTAUTH_URL })
  }
  else {
    console.log('Not client - not signing out')
    console.log('*** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***')
  }

  return null;
}

export default Signout
