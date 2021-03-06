import React from 'react';
import { GetServerSideProps } from 'next';
import { Button, makeStyles } from '@material-ui/core';
import Typography from '@material-ui/core/Typography';
import Container from '@material-ui/core/Container';
import Grid from '@material-ui/core/Grid';
import Layout from '../../../layout/public-layout';
import { useSelector } from 'react-redux';
import { RootState } from '../../../store/root.reducer';
import { ActivityState } from '../../../store/activity.reducer';
import { TicketTypeCard } from '../../../components/ticket-types/ticket-type-card';
import wrapper, { SagaStore } from '../../../store/store';
import { LOAD_ACTIVITY } from '../../../store/activity.actions';
import { END } from 'redux-saga';
import { OrderState } from '../../../store/order.reducer';

interface ActivityProps {
  activityId: string,
  ownerId: string
}

const useStyles = makeStyles((theme) => ({
  icon: {
    marginRight: theme.spacing(2),
  },
  heroContent: {
    backgroundColor: theme.palette.background.paper,
    padding: theme.spacing(8, 0, 6),
  },
  heroButtons: {
    marginTop: theme.spacing(4),
  },
}));

const PublicActivityPage: React.FC<ActivityProps> = ({ activityId, ownerId }) => {
  const classes = useStyles();

  const { currentActivity: activity } = useSelector<RootState, ActivityState>(state => state.activity);
  const { currentOrder: order } = useSelector<RootState, OrderState>(state => state.order);

  if (!activity) {
    return (<Layout title="Unknown activity">
      <Container maxWidth="md">
        <Typography component="h1" variant="h2" align="center" color="textPrimary" gutterBottom>
          Unknown Activity
          </Typography>
      </Container>
    </Layout>);
  }

  return (
    <Layout title={activity.name}>
      {/* Hero unit */}
      <div className={classes.heroContent}>
        <Container maxWidth="md">
          <Typography component="h1" variant="h2" align="center" color="textPrimary" gutterBottom>
            Tickets
          </Typography>
          <Typography variant="h5" align="center" color="textSecondary" paragraph>
            Something short and leading about the collection below—its contents, the creator, etc.
            Make it short and sweet, but not too short so folks don&apos;t simply skip over it
            entirely.
          </Typography>
          <div className={classes.heroButtons}>
            <Grid container spacing={2} justify="center">
              <Grid item>
                <Button variant="contained" color="primary">
                  Buy now
                </Button>
              </Grid>
              <Grid item>
                <Button variant="outlined" color="primary">
                  Find order
                </Button>
              </Grid>
            </Grid>
          </div>
        </Container>
      </div>
      <Container maxWidth="md">
        <Grid container spacing={4}>
          {activity.ticketTypes.map((tt) =>
            <Grid item key={tt.ticketTypeId} xs={12} sm={6} md={4}>
              <TicketTypeCard ticketType={tt} orderId={order?.orderId} />
            </Grid>
          )}
        </Grid>
      </Container>
    </Layout>
  );
};

export default PublicActivityPage;

export const getServerSideProps: GetServerSideProps = wrapper.getServerSideProps(async ({ store, params }) => {
  if (!params) {
    return {
      props: {
      }
    }
  }
  const activityId = params?.activityId;
  const ownerId = params?.ownerId;

  store.dispatch({ type: LOAD_ACTIVITY, payload: { activityId, ownerId } });
  store.dispatch(END);

  await (store as SagaStore).sagaTask.toPromise();

  console.log(store.getState());
  return {
    props: {
      activityId: activityId,
      ownerId: ownerId
    }
  };
});
