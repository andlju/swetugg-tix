import React, { useContext, useEffect } from 'react';
import { useSelector, useDispatch } from 'react-redux';
import { GetServerSideProps } from 'next';
import { makeStyles } from '@material-ui/core';
import clsx from 'clsx';
import Typography from '@material-ui/core/Typography';
import Paper from '@material-ui/core/Paper';
import Container from '@material-ui/core/Container';
import Grid from '@material-ui/core/Grid';
import BackOfficeLayout from '../../../layout/back-office/main-layout';
import { buildUrl } from '../../../src/url-utils';
import { Activity, ActivityList } from '../../../src/back-office';
import { LOAD_ACTIVITIES, LOAD_ACTIVITIES_COMPLETE } from '../../../src/back-office/store/activities.actions';
import wrapper, { SagaStore, State } from '../../../src/back-office/store/store';
import { ActivitiesState } from '../../../src/back-office/store/activities.reducer';
import { END } from 'redux-saga';

interface ActivitiesProps {
  activities: Activity[];
}

const useStyles = makeStyles((theme) => ({
  container: {
    paddingTop: theme.spacing(4),
    paddingBottom: theme.spacing(4),
  },
  paper: {
    padding: theme.spacing(1),
  },
  activityList: {
    minHeight: theme.spacing(30),
    maxHeight: theme.spacing(60),
    overflow: 'auto',
  }
}));

function IndexPage() {
  const classes = useStyles();

  const dispatch = useDispatch();
/*
  useEffect(() => {
    dispatch({ type: LOAD_ACTIVITIES });
  }, []);
*/
  /*  useEffect(() => {
      dispatch({ type: LOAD_ACTIVITIES_COMPLETE, payload: { activities: activities } });
    }, []);*/

  return (
    <BackOfficeLayout>
      <Container maxWidth={false} className={classes.container}>
        <Typography variant="h4" component="h1" gutterBottom>
          Activities
        </Typography>
        <Grid container spacing={3}>
          {/* List of activities */}
          <Grid item xs={12}>
            <Paper className={clsx(classes.paper, classes.activityList)}>
              <ActivityList />
            </Paper>
          </Grid>
        </Grid>
      </Container>
    </BackOfficeLayout>
  );
}

export default IndexPage;

export const getServerSideProps: GetServerSideProps = wrapper.getServerSideProps(async ({store}) => {
  
  store.dispatch({type: LOAD_ACTIVITIES});
  store.dispatch(END);
  await (store as SagaStore).sagaTask.toPromise();

  return {
    props: {
      activities: [] //data
    }
  };
});