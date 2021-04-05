import { makeStyles } from '@material-ui/core';
import Container from '@material-ui/core/Container';
import Grid from '@material-ui/core/Grid';
import Paper from '@material-ui/core/Paper';
import Typography from '@material-ui/core/Typography';
import { GetServerSideProps } from 'next';
import React from 'react';

import { RefreshActivityView, RefreshOrderView } from '../../components';
import { useAuthenticatedUser } from '../../src/use-authenticated-user.hook';

const useStyles = makeStyles((theme) => ({
  container: {
    paddingTop: theme.spacing(4),
    paddingBottom: theme.spacing(4),
  },
  paper: {
    padding: theme.spacing(2),
  },
}));

export default function Index() {
  const classes = useStyles();

  const { user } = useAuthenticatedUser(['https://swetuggtixlocal.onmicrosoft.com/tix-api/access_as_admin']);

  return (
    <Container maxWidth={false} className={classes.container}>
      <Typography variant="h4" component="h1" gutterBottom>
        Administration
      </Typography>
      <Grid container spacing={3}>
        <Grid item xs={6}>
          <Paper className={classes.paper}>
            <RefreshActivityView initialActivityId={''} />
          </Paper>
        </Grid>
        <Grid item xs={6}>
          <Paper className={classes.paper}>
            <RefreshOrderView initialOrderId={''} />
          </Paper>
        </Grid>
      </Grid>
    </Container>
  );
}

export const getServerSideProps: GetServerSideProps = async () => {
  return {
    props: {},
  };
};
