import React from 'react';
import { GetServerSideProps } from 'next';
import { END } from 'redux-saga';
import { makeStyles } from '@material-ui/core';
import Typography from '@material-ui/core/Typography';
import Paper from '@material-ui/core/Paper';
import Container from '@material-ui/core/Container';
import Grid from '@material-ui/core/Grid';
import BackOfficeLayout from '../../../../../layout/back-office/main-layout';
import { ModifyLimits, TicketTypeDetails } from '../../../../../src/back-office';
import { LOAD_ACTIVITY } from '../../../../../src/back-office/store/activities.actions';
import wrapper, { SagaStore, State } from '../../../../../src/back-office/store/store';
import { useSelector } from 'react-redux';
import { ActivitiesState } from '../../../../../src/back-office/store/activities.reducer';

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

  const activities = useSelector<State, ActivitiesState>(store => store.activities);
  
  const activity = activities.activities[activityId];
  const ticketType = activity.ticketTypes.find(t => t.ticketTypeId === ticketTypeId);

  if (!ticketType) {
    return (<BackOfficeLayout>Unknown ticket type</BackOfficeLayout>);
  }

  return (
    <BackOfficeLayout>
      <Container maxWidth={false} className={classes.container}>
        <Typography variant="h4" component="h1" gutterBottom>
          {activity.name}
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
    </BackOfficeLayout>
  );
}

export const getServerSideProps: GetServerSideProps = wrapper.getServerSideProps(async ({store, params}) => {
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