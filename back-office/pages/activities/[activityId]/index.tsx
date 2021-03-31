import { makeStyles } from '@material-ui/core';
import Container from '@material-ui/core/Container';
import Grid from '@material-ui/core/Grid';
import Paper from '@material-ui/core/Paper';
import Typography from '@material-ui/core/Typography';
import { GetServerSideProps, NextPage } from 'next';
import React, { useEffect } from 'react';
import { useDispatch, useSelector } from 'react-redux';

import { ActivityDetails, ModifySeats, TicketTypeList } from '../../../components';
import { useAuthenticatedUser } from '../../../src/use-authenticated-user.hook';
import { loadActivity } from '../../../store/activities/activities.actions';
import { ActivitiesState } from '../../../store/activities/activities.reducer';
import { RootState } from '../../../store/store';

interface ActivityProps {
  activityId: string;
  ownerId?: string;
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

const ActivityPage: NextPage<ActivityProps> = ({ activityId, ownerId }) => {
  const classes = useStyles();
  const dispatch = useDispatch();

  const { user } = useAuthenticatedUser([
    'https://swetuggtixlocal.onmicrosoft.com/tix-api/access_as_admin',
  ]);

  useEffect(() => {
    if (user.current?.userId) {
      dispatch(loadActivity(activityId, ownerId ?? user.current.userId));
    }
  }, [user]);

  const activities = useSelector<RootState, ActivitiesState>((state) => state.activities);

  const activity = activities.activities[activityId];
  const ticketTypes = activity?.ticketTypes;

  return (
    <React.Fragment>
      {activity && (
        <Container maxWidth={false} className={classes.container}>
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
              <TicketTypeList
                ticketTypes={ticketTypes}
                loading={activities.visibleActivities.loading}
                activityId={activity.activityId}
                ownerId={activity.ownerId}
              />
            </Grid>
          </Grid>
        </Container>
      )}
    </React.Fragment>
  );
};

export default ActivityPage;

export const getServerSideProps: GetServerSideProps = async ({ params, query }) => {
  const activityId = params?.activityId;
  const ownerId = query?.ownerId;

  return {
    props: {
      activityId: activityId,
      ownerId: ownerId,
    },
  };
};
