import React from 'react';
import { useSelector } from 'react-redux';
import { GetServerSideProps } from 'next';
import { makeStyles } from '@material-ui/core';
import clsx from 'clsx';
import Typography from '@material-ui/core/Typography';
import Paper from '@material-ui/core/Paper';
import Container from '@material-ui/core/Container';
import Grid from '@material-ui/core/Grid';
import { END } from 'redux-saga';
import { RootState } from '../../store/root.reducer';
import { ActivitiesState } from '../../components/activities/store/activities.reducer';
import { ActivityList } from '../../components';
import wrapper, { SagaStore } from '../../store/store';
import { LOAD_ACTIVITIES } from '../../components/activities/store/activities.actions';
import { getSession } from 'next-auth/client';

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

function IndexPage() {
  const classes = useStyles();

  const activitiesState = useSelector<RootState, ActivitiesState>(state => state.activities);

  const activities = activitiesState.visibleActivities.ids.map(id => activitiesState.activities[id]);

  return (
    <Container maxWidth={false} className={classes.container}>
      <Typography variant="h4" component="h1" gutterBottom>
        Activities
        </Typography>
      <Grid container spacing={3}>
        {/* List of activities */}
        <Grid item xs={12}>
          <Paper className={clsx(classes.paper, classes.activityList)}>
            <ActivityList activities={activities} />
          </Paper>
        </Grid>
      </Grid>
    </Container>
  );
}

export default IndexPage;

export const getServerSideProps: GetServerSideProps = wrapper.getServerSideProps(async (ctx) => {
  const { store} = ctx;

  store.dispatch({ type: LOAD_ACTIVITIES });
  store.dispatch(END);
  await (store as SagaStore).sagaTask.toPromise();

  return {
    props: {
      session: await getSession(ctx),
      activities: [] //data
    }
  };
});