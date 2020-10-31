import React, { useEffect, useState } from 'react';
import { GetServerSideProps } from 'next';
import { Button, makeStyles } from '@material-ui/core';
import Typography from '@material-ui/core/Typography';
import Container from '@material-ui/core/Container';
import Grid from '@material-ui/core/Grid';
import Layout from '../layout/public-layout';
import { buildUrl } from '../../../src/url-utils';
import { Activity } from '../../../components/activities/activity.models';
import { TicketType, TicketTypesView } from '../../../components/ticket-types/ticket-type.models';
import { getView } from '../../../src/services/view-fetcher.service';
import TicketTypeCard from '../../../components/public/ticket-type/ticket-type';

interface ActivityProps {
  initialActivity: Activity,
  ticketTypes: TicketType[]
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
}))

export default function PublicActivityPage({ initialActivity, ticketTypes }: ActivityProps) {
  const classes = useStyles();
  const [activity, setActivity] = useState(initialActivity);
  const [refreshActivityRevision, setRefreshActivityRevision] = useState(initialActivity.revision);
  
  useEffect(() => {
    const fetchData = async () => {
      if (refreshActivityRevision > activity.revision) {
        const resp = await getView<Activity>(
          buildUrl(`/activities/${activity.activityId}`),
          { revision: refreshActivityRevision });
        setActivity(resp);
        setRefreshActivityRevision(resp.revision);
      }
    };
    fetchData();
  }, [refreshActivityRevision]);

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
          {ticketTypes.map((tt) =>
            <Grid item key={tt.ticketTypeId} xs={12} sm={6} md={4}>
              <TicketTypeCard ticketType={tt} />
            </Grid>
          )}
        </Grid>
      </Container>
    </Layout>
  );
}

export const getServerSideProps: GetServerSideProps = async (context) => {
  const activityId = context.params?.activityId;

  const [activityResp, ticketTypesResp] = await Promise.all([
    getView<Activity>(buildUrl(`/activities/${activityId}`)),
    getView<TicketTypesView>(buildUrl(`/activities/${activityId}/ticket-types`))
  ]);

  return {
    props: {
      initialActivity: activityResp,
      ticketTypes: ticketTypesResp.ticketTypes
    }
  }
}