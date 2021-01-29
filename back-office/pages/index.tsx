import React from 'react';
import { GetServerSideProps } from 'next';
import { makeStyles } from '@material-ui/core';
import clsx from 'clsx';
import Typography from '@material-ui/core/Typography';
import Paper from '@material-ui/core/Paper';
import Container from '@material-ui/core/Container';
import Grid from '@material-ui/core/Grid';
import { useSession } from 'next-auth/client';

const useStyles = makeStyles((theme) => ({
  container: {
    paddingTop: theme.spacing(4),
    paddingBottom: theme.spacing(4),
  },
  paper: {
    padding: theme.spacing(1),
  },
  activityList: {
    minHeight: theme.spacing(30),
    maxHeight: theme.spacing(60),
    overflow: 'auto',
  }
}));

export default function Index() {
  const classes = useStyles();

  const [session, loading] = useSession();

  return (
    <Container maxWidth={false} className={classes.container}>
      <Typography variant="h4" component="h1" gutterBottom>
        Swetugg Tix Dashboard
        </Typography>
      <Grid container spacing={3}>
        {/* List of activities */}
        <Grid item xs={12}>
          <Paper className={clsx(classes.paper, classes.activityList)}>
            {session ?
              <div>
                <p>You are signed in! You can also sign out if you like.</p>
                <ul>
                  <li>
                    <a style={{ color: 'blue' }} href="/api/auth/signout/azureb2c">Sign Out (API)</a>
                  </li>
                  <li>
                    <a style={{ color: 'blue' }} href={`https://${process.env.NEXT_AUTH_TENANT_NAME}.b2clogin.com/${process.env.NEXT_AUTH_TENANT_NAME}.onmicrosoft.com/${process.env.NEXT_USER_FLOW}/oauth2/v2.0/logout?post_logout_redirect_uri=${process.env.NEXTAUTH_URL}/auth/signout`}>Sign Out (FULL)</a>
                  </li>
                </ul>
              </div> :
              <div>
                You are not signed in! <a style={{ color: 'blue' }} href="/api/auth/signin">You must sign in to access documentation!</a>
              </div>
            }

          </Paper>
        </Grid>
      </Grid>

    </Container>
  );
}

export const getServerSideProps: GetServerSideProps = async () => {
  return {
    props: {

    }
  };
};