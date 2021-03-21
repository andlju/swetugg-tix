import React, { useEffect } from 'react';
import { GetServerSideProps } from 'next';
import { makeStyles } from '@material-ui/core';
import Typography from '@material-ui/core/Typography';
import Paper from '@material-ui/core/Paper';
import Container from '@material-ui/core/Container';
import Grid from '@material-ui/core/Grid';
import { useDispatch, useSelector } from 'react-redux';
import { ActivitiesState } from '../../../../store/activities/activities.reducer';
import { ModifyLimits, TicketTypeDetails } from '../../../../components';
import { loadActivity } from '../../../../store/activities/activities.actions';
import { RootState } from '../../../../store/store';
import { useAuthenticatedUser } from '../../../../src/use-authenticated-user.hook';

interface TicketTypeProps {
  activityId: string,
  ticketTypeId: string,
  ownerId: string,
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

export default function TicketTypePage({ activityId, ownerId, ticketTypeId }: TicketTypeProps) {
  const classes = useStyles();
  const dispatch = useDispatch();

  const { user } = useAuthenticatedUser(["https://swetuggtixlocal.onmicrosoft.com/tix-api/access_as_admin"]);

  useEffect(() => {
    dispatch(loadActivity(activityId, ownerId));
  }, []);

  const activities = useSelector<RootState, ActivitiesState>(store => store.activities);

  const activity = activities.activities && activities.activities[activityId];
  const ticketType = activity?.ticketTypes.find(t => t.ticketTypeId === ticketTypeId);

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

export const getServerSideProps: GetServerSideProps = async ({ params, query }) => {

  if (!params) {
    return {
      props: {
      }
    };
  }

  const ownerId = query?.ownerId;

  const activityId = params.activityId;
  const ticketTypeId = params.ticketTypeId;

  return {
    props: {
      activityId: activityId,
      ticketTypeId: ticketTypeId,
      ownerId: ownerId,
    }
  };
};