import React from 'react';
import { GetServerSideProps, NextPage } from 'next';
import { makeStyles } from '@material-ui/core';
import Typography from '@material-ui/core/Typography';
import Paper from '@material-ui/core/Paper';
import Container from '@material-ui/core/Container';
import Grid from '@material-ui/core/Grid';
import { useSelector } from 'react-redux';
import { END } from 'redux-saga';
import { RootState } from '../../../store/root.reducer';
import { ActivityDetails, ModifySeats, TicketTypeList } from '../../../components';
import wrapper, { SagaStore } from '../../../store/store';
import { LOAD_ACTIVITY } from '../../../components/activities/store/activities.actions';
import { ActivitiesState } from '../../../components/activities/store/activities.reducer';

interface ActivityProps {
  activityId: string
}

const useStyles = makeStyles((theme) => ({
  container: {
    paddingTop: theme.spacing(4),
    paddingBottom: theme.spacing(4),
  },
  paper: {
    padding: theme.spacing(2),
  },
}));

const ActivityPage: NextPage<ActivityProps> = ({ activityId }) => {
  const classes = useStyles();

  const activities = useSelector<RootState, ActivitiesState>(state => state.activities);

  const activity = activities.activities[activityId];
  const ticketTypes = activity?.ticketTypes;

  return (
    <React.Fragment>
      {activity &&
        (<Container maxWidth={false} className={classes.container}>
          <Typography variant="h4" component="h1" gutterBottom>
            {activity.name}
          </Typography>
          <Grid container spacing={3}>
            <Grid item xs={12} md={5}>
              <Grid container spacing={2}>
                <Grid item xs={12}>
                  <Paper className={classes.paper}>
                    <ActivityDetails activity={activity} />
                  </Paper>
                </Grid>
                <Grid item xs={12}>
                  <Paper className={classes.paper}>
                    <ModifySeats activity={activity} />
                  </Paper>
                </Grid>
              </Grid>
            </Grid>
            <Grid item xs={12} md={7}>
              <TicketTypeList ticketTypes={ticketTypes} loading={activities.visibleActivities.loading} activityId={activity.activityId} />
            </Grid>
          </Grid>
        </Container>)}
    </React.Fragment>
  );
}

export default ActivityPage;

export const getServerSideProps: GetServerSideProps = wrapper.getServerSideProps(async ({ store, params }) => {
  const activityId = params?.activityId;
  store.dispatch({ type: LOAD_ACTIVITY, payload: { activityId } });
  store.dispatch(END);

  await (store as SagaStore).sagaTask.toPromise();
  return {
    props: {
      activityId: activityId
    }
  };
});