import React, { useEffect, useState } from 'react';
import { GetServerSideProps } from 'next';
import { makeStyles } from '@material-ui/core';
import Typography from '@material-ui/core/Typography';
import Paper from '@material-ui/core/Paper';
import Container from '@material-ui/core/Container';
import Grid from '@material-ui/core/Grid';
import ModifyLimits from '../../../../components/ticket-types/modify-limits';
import { getView } from '../../../../src/services/view-fetcher.service';
import { buildUrl } from '../../../../src/url-utils';
import { TicketType, TicketTypesView } from '../../../../components/ticket-types/ticket-type.models';
import Layout from '../../../../components/layout/main-layout';
import { Activity } from '../../../../components/activities/activity.models';
import TicketTypeDetails from '../../../../components/ticket-types/ticket-type-details';

interface TicketTypeProps {
  activity: Activity,
  initialTicketType: TicketType
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

export default function TicketTypePage({ activity, initialTicketType }: TicketTypeProps) {
  const classes = useStyles();
  const [ticketType, setTicketType] = useState(initialTicketType);
  const [refreshTicketTypeRevision, setRefreshTicketTypeRevision] = useState(initialTicketType.revision);

  useEffect(() => {
    const fetchData = async () => {
      if (refreshTicketTypeRevision > ticketType.revision) {
        const resp = await getView<TicketTypesView>(
          buildUrl(`/activities/${activity.activityId}/ticket-types`),
          { revision : refreshTicketTypeRevision });
          const selectedTicketType = resp.ticketTypes.find(tt => tt.ticketTypeId === ticketType.ticketTypeId);
          if (selectedTicketType) {
             setTicketType(selectedTicketType);
          }
          setRefreshTicketTypeRevision(resp.revision);
      }
    };
    fetchData();
  }, [refreshTicketTypeRevision]);

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
                  <TicketTypeDetails ticketType={ticketType} />
                </Paper>
              </Grid>
              <Grid item xs={12}>
                <Paper className={classes.paper}>
                  <ModifyLimits ticketType={ticketType} refreshTicketTypesRevision={setRefreshTicketTypeRevision}/>
                </Paper>
              </Grid>
            </Grid>
          </Grid>
        </Grid>
      </Container>
    </Layout>
  );
}

export const getServerSideProps: GetServerSideProps = async (context) => {
  const activityId = context.params?.activityId;
  const ticketTypeId = context.params?.ticketTypeId;

  const [activityResp, ticketTypesResp] = await Promise.all([
    getView<Activity>(buildUrl(`/activities/${activityId}`)), 
    getView<TicketTypesView>(buildUrl(`/activities/${activityId}/ticket-types`))
  ]);

  const selectedTicketType = ticketTypesResp.ticketTypes.find(tt => tt.ticketTypeId === ticketTypeId)
  return {
    props: {
      activity: activityResp,
      initialTicketType: selectedTicketType
    }
  }
}