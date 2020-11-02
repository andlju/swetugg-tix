import React from 'react';
import { GetServerSideProps } from 'next';
import { makeStyles } from '@material-ui/core';
import Typography from '@material-ui/core/Typography';
import Paper from '@material-ui/core/Paper';
import Container from '@material-ui/core/Container';
import Grid from '@material-ui/core/Grid';
import Layout from '../../components/layout/main-layout';
import RefreshActivityView from '../../components/admin/refresh-activity-view';
import RefreshOrderView from '../../components/admin/refresh-order-view';

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

  return (
    <Layout>
      <Container maxWidth={false} className={classes.container}>
        <Typography variant="h4" component="h1" gutterBottom>
          Administration
        </Typography>
        <Grid container spacing={3}>
          <Grid item xs={6}>
            <Paper className={classes.paper} >
              <RefreshActivityView initialActivityId={""} />
            </Paper>
          </Grid>
          <Grid item xs={6}>
            <Paper className={classes.paper} >
              <RefreshOrderView initialOrderId={""} />
            </Paper>
          </Grid>
        </Grid>

      </Container>
    </Layout>
  );
}

export const getServerSideProps: GetServerSideProps = async () => {
  return {
    props: {
    }
  }
}