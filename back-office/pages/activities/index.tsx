import React, { useEffect, useMemo, useState } from 'react';
import { useSelector, useDispatch } from 'react-redux';
import { Fab, Link, makeStyles, Toolbar } from '@material-ui/core';
import AddIcon from '@material-ui/icons/Add';
import clsx from 'clsx';
import Typography from '@material-ui/core/Typography';
import Paper from '@material-ui/core/Paper';
import Container from '@material-ui/core/Container';
import Grid from '@material-ui/core/Grid';
import { ActivitiesState } from '../../store/activities/activities.reducer';
import { ActivityList } from '../../components';
import { loadActivities } from '../../store/activities/activities.actions';
import { RootState } from '../../store/store';
import { useAuthenticatedUser } from '../../src/use-authenticated-user.hook';
import { loadOrganizations } from '../../store/organizations/organizations.actions';
import { CreateActivityModal } from '../../components/activities/create-activity-modal';

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
  },
  activityListTitle: {
    flex: '1 1 100%'
  },
  activityListToolbar: {
    paddingLeft: theme.spacing(2),
    paddingRight: theme.spacing(2)
  }
}));

function IndexPage() {
  const classes = useStyles();

  const { user } = useAuthenticatedUser(["https://swetuggtixlocal.onmicrosoft.com/tix-api/access_as_admin"]);

  const [ addModalOpen, setAddModalOpen ] = useState(false);

  const dispatch = useDispatch();
  useEffect(() => {
    dispatch(loadActivities());
    dispatch(loadOrganizations());
  }, []);

  const onAddButtonClick = () => {
    setAddModalOpen(true);
  }
  
  const { organizations, visibleOrganizations } = useSelector((r: RootState) => r.organizations);

  const orgs = useMemo(() => visibleOrganizations.ids.map(oId => organizations[oId]), [organizations, visibleOrganizations]);

  const activitiesState = useSelector<RootState, ActivitiesState>(state => state.activities);

  const activities = activitiesState.visibleActivities.ids.map(id => activitiesState.activities[id]);

  return (
    <Container maxWidth={false} className={classes.container}>
      <Typography variant="h4" component="h1" gutterBottom>
        Activities
        </Typography>
      <Grid container spacing={3}>
        {/* List of activities */}
        <Grid item xs={12}>
          <Paper className={clsx(classes.paper, classes.activityList)}>
            <Toolbar className={classes.activityListToolbar}>
              <Typography className={classes.activityListTitle} variant="h6" component="div">
                Activities
              </Typography>
              <Fab size="small" color="primary" onClick={onAddButtonClick}>
                <AddIcon />
              </Fab>
              {user.current && <CreateActivityModal user={user.current} organizations={orgs} open={addModalOpen} setOpen={setAddModalOpen}/> }
            </Toolbar>
            <ActivityList activities={activities} />
          </Paper>
        </Grid>
      </Grid>
    </Container>
  );
}

export default IndexPage;
