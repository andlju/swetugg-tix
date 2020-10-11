import React from 'react';
import { GetServerSideProps } from 'next';
import { makeStyles } from '@material-ui/core';
import clsx from 'clsx';
import Typography from '@material-ui/core/Typography';
import Paper from '@material-ui/core/Paper';
import Container from '@material-ui/core/Container';
import Grid from '@material-ui/core/Grid';
import Link from '../../components/Link';
import Layout from '../../components/layout/main-layout';
import { buildUrl } from '../../src/url-utils';
import { Activity } from '../../components/activities/activity.models';
import ActivityDetails from '../../components/activities/activity-details';
import TicketTypeList from '../../components/ticket-types/ticket-type-list';
import { TicketType } from '../../components/ticket-types/ticket-type.models';
import { getView } from '../../src/services/view-fetcher.service';

interface ActivityProps {
  activity: Activity,
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

export default function ActivityPage({ activity, ticketTypes }: ActivityProps) {
  const classes = useStyles();

  return (
    <Layout>
      <Container maxWidth={false} className={classes.container}>
        <Typography variant="h4" component="h1" gutterBottom>
          { activity.name }
        </Typography>
        <Grid container spacing={3}>
          <Grid item xs={12} md={4}>
            <ActivityDetails activity={activity} />
          </Grid>
          <Grid item xs={12} md={8}>
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
    getView<TicketType[]>(buildUrl(`/activities/${activityId}/ticket-types`))
  ]);

  return {
    props: {
      activity: activityResp,
      ticketTypes: ticketTypesResp
    }
  }
}