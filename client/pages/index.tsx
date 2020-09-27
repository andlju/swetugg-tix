import React from 'react';
import { GetServerSideProps } from 'next';
import { makeStyles } from '@material-ui/core';
import clsx from 'clsx';
import Typography from '@material-ui/core/Typography';
import Paper from '@material-ui/core/Paper';
import Container from '@material-ui/core/Container';
import Grid from '@material-ui/core/Grid';
import Link from '../components/Link';
import Layout from '../components/layout/main-layout';
import { buildUrl } from '../src/url-utils';
import ActivityList from '../components/activities/activity-list';
import { Activity } from '../components/activities/activity.models';

interface IndexProps {
  activities: Activity[]
}

const useStyles = makeStyles((theme) => ({
  container: {
    paddingTop: theme.spacing(4),
    paddingBottom: theme.spacing(4),
  },
  paper: {
    padding: theme.spacing(2),
  },
  activityList: {
    maxHeight: theme.spacing(60),
    overflow: 'auto',
  }
}))

export default function Index({ activities }: IndexProps) {
  const classes = useStyles();

  return (
    <Layout>
      <Container maxWidth={false} className={classes.container}>
        <Typography variant="h4" component="h1" gutterBottom>
          Swetugg Tix Dashboard
        </Typography>
        <Grid container spacing={3}>
          {/* List of activities */}
          <Grid item xs={12}>
            <Paper className={clsx(classes.paper, classes.activityList)}>
              <ActivityList activities={activities} ></ActivityList>
            </Paper>
          </Grid>
        </Grid>

        <Link href="/about" color="secondary">
          Go to the about page
        </Link>
      </Container>
    </Layout>
  );
}

export const getServerSideProps: GetServerSideProps = async () => {
  const resp = await fetch(buildUrl('/activities'));
  const data = await resp.json() as Activity[];

  return {
    props: {
      activities: data
    }
  }
}