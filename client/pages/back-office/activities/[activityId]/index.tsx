import React, { useEffect, useState, useContext } from 'react';
import { GetServerSideProps, NextPage } from 'next';
import { makeStyles } from '@material-ui/core';
import Typography from '@material-ui/core/Typography';
import Paper from '@material-ui/core/Paper';
import Container from '@material-ui/core/Container';
import Grid from '@material-ui/core/Grid';
import BackOfficeLayout from '../../../../layout/back-office/main-layout';
import { buildUrl } from '../../../../src/url-utils';
import { getView } from '../../../../src/services/view-fetcher.service';
import { Activity, ActivityDetails, ModifySeats, TicketType, TicketTypeList } from '../../../../src/back-office';
import { useSelector } from 'react-redux';
import { ActivitiesState } from '../../../../src/back-office/store/activities.reducer';
import wrapper, { SagaStore, State } from '../../../../src/back-office/store/store';
import { LOAD_ACTIVITY } from '../../../../src/back-office/store/activities.actions';
import { SortRounded } from '@material-ui/icons';
import { END } from 'redux-saga';

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

  const activities = useSelector<State, ActivitiesState>(state => state.activities);

  const activity = activities.activities[activityId];
  const ticketTypes = activity.ticketTypes;
  
  return (
    <BackOfficeLayout>
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
                    <ModifySeats activity={activity} refreshActivityRevision={(rev) => { }} />
                  </Paper>
                </Grid>
              </Grid>
            </Grid>
            <Grid item xs={12} md={7}>
              <TicketTypeList initialTicketTypes={ticketTypes} activityId={activity.activityId} />
            </Grid>
          </Grid>
        </Container>)}
    </BackOfficeLayout>
  );
}

export default ActivityPage;

export const getServerSideProps: GetServerSideProps = wrapper.getServerSideProps(async ({store, params}) => {
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