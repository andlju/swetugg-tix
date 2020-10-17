import React, { useEffect, useState } from 'react';
import { GetServerSideProps } from 'next';
import { makeStyles } from '@material-ui/core';
import clsx from 'clsx';
import Typography from '@material-ui/core/Typography';
import Paper from '@material-ui/core/Paper';
import Container from '@material-ui/core/Container';
import Grid from '@material-ui/core/Grid';
import Link from '../../../components/Link';
import Layout from '../../../components/layout/main-layout';
import { buildUrl } from '../../../src/url-utils';
import { Activity } from '../../../components/activities/activity.models';
import ActivityDetails from '../../../components/activities/activity-details';
import TicketTypeList from '../../../components/ticket-types/ticket-type-list';
import { TicketType, TicketTypesView } from '../../../components/ticket-types/ticket-type.models';
import { getView } from '../../../src/services/view-fetcher.service';
import ModifySeats from '../../../components/activities/modify-seats';

interface ActivityProps {
  initialActivity: Activity,
  ticketTypes: TicketType[]
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

export default function ActivityPage({ initialActivity, ticketTypes }: ActivityProps) {
  const classes = useStyles();
  const [activity, setActivity] = useState(initialActivity);
  const [refreshActivityRevision, setRefreshActivityRevision] = useState(initialActivity.revision);

  useEffect(() => {
    const fetchData = async () => {
      if (refreshActivityRevision > activity.revision) {
        const resp = await getView<Activity>(
          buildUrl(`/activities/${activity.activityId}`),
          {revision : refreshActivityRevision});
          setActivity(resp);
          setRefreshActivityRevision(resp.revision);
      }
    };
    fetchData();
  }, [refreshActivityRevision]);

  return (
    <Layout>
      <Container maxWidth={false} className={classes.container}>
        <Typography variant="h4" component="h1" gutterBottom>
          { activity.name }
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
                  <ModifySeats activity={activity} refreshActivityRevision={setRefreshActivityRevision}/>
                </Paper>
              </Grid>
            </Grid>
          </Grid>
          <Grid item xs={12} md={7}>
            <TicketTypeList initialTicketTypes={ticketTypes} activityId={activity.activityId} />
          </Grid>
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