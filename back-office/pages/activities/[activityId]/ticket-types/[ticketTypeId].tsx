import React from 'react';
import { GetServerSideProps } from 'next';
import { END } from 'redux-saga';
import { makeStyles } from '@material-ui/core';
import Typography from '@material-ui/core/Typography';
import Paper from '@material-ui/core/Paper';
import Container from '@material-ui/core/Container';
import Grid from '@material-ui/core/Grid';
import { useSelector } from 'react-redux';
import { RootState } from '../../../../store/root.reducer';
import { ActivitiesState } from '../../../../components/activities/store/activities.reducer';
import { ModifyLimits, TicketTypeDetails } from '../../../../components';
import wrapper, { SagaStore } from '../../../../store/store';
import { LOAD_ACTIVITY } from '../../../../components/activities/store/activities.actions';

interface TicketTypeProps {
  activityId: string,
  ticketTypeId: string,
}

const useStyles = makeStyles((theme) => ({
  container: {
    paddingTop: theme.spacing(4),
    paddingBottom: theme.spacing(4),
  },
  paper: {
    padding: theme.spacing(2),
  },
}))

export default function TicketTypePage({ activityId, ticketTypeId }: TicketTypeProps) {
  const classes = useStyles();

  const activities = useSelector<RootState, ActivitiesState>(store => store.activities);

  const activity = activities.activities[activityId];
  const ticketType = activity.ticketTypes.find(t => t.ticketTypeId === ticketTypeId);

  if (!ticketType) {
    return (<Container>
      <Typography variant="h4" component="h1" gutterBottom>
        Unknown ticket type
      </Typography>
    </Container>);
  }

  return (
    <Container maxWidth={false} className={classes.container}>
      <Typography variant="h4" component="h1" gutterBottom>
        {activity.name} - {ticketType.name}
      </Typography>
      <Grid container spacing={3}>
        <Grid item xs={12} md={5}>
          <Grid container spacing={2}>
            <Grid item xs={12}>
              <Paper className={classes.paper}>
                <TicketTypeDetails activity={activity} ticketType={ticketType} />
              </Paper>
            </Grid>
            <Grid item xs={12}>
              <Paper className={classes.paper}>
                <ModifyLimits ticketType={ticketType} />
              </Paper>
            </Grid>
          </Grid>
        </Grid>
      </Grid>
    </Container>
  );
}

export const getServerSideProps: GetServerSideProps = wrapper.getServerSideProps(async ({ store, params }) => {
  if (!params) {
    return {
      props: {
      }
    }
  }
  const activityId = params.activityId;
  const ticketTypeId = params.ticketTypeId;
  store.dispatch({ type: LOAD_ACTIVITY, payload: { activityId } });
  store.dispatch(END);

  await (store as SagaStore).sagaTask.toPromise();

  return {
    props: {
      activityId: activityId,
      ticketTypeId: ticketTypeId
    }
  };
});