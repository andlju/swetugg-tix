import React, { useEffect, useState } from 'react';
import { GetServerSideProps } from 'next';
import { makeStyles } from '@material-ui/core';
import Typography from '@material-ui/core/Typography';
import Paper from '@material-ui/core/Paper';
import Container from '@material-ui/core/Container';
import Grid from '@material-ui/core/Grid';
import Layout from '../../../../layout/back-office/main-layout';
import { buildUrl } from '../../../../src/url-utils';
import { getView } from '../../../../src/services/view-fetcher.service';
import { Activity, ActivityDetails, ModifySeats, TicketType, TicketTypeList } from '../../../../src/back-office';

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
  
  const [activityResp] = await Promise.all([
    getView<Activity>(buildUrl(`/activities/${activityId}`))
  ]);

  return {
    props: {
      initialActivity: activityResp,
      ticketTypes: activityResp.ticketTypes
    }
  }
}