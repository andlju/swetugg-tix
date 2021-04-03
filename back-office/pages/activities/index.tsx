import { Fab, Link, makeStyles, Toolbar } from '@material-ui/core';
import Container from '@material-ui/core/Container';
import Grid from '@material-ui/core/Grid';
import Paper from '@material-ui/core/Paper';
import Typography from '@material-ui/core/Typography';
import AddIcon from '@material-ui/icons/Add';
import clsx from 'clsx';
import React, { useEffect, useMemo, useState } from 'react';
import { useDispatch, useSelector } from 'react-redux';

import { ActivityList } from '../../components';
import { CreateActivityModal } from '../../components/activities/create-activity-modal';
import { useAuthenticatedUser } from '../../src/use-authenticated-user.hook';
import { loadActivities } from '../../store/activities/activities.actions';
import { ActivitiesState } from '../../store/activities/activities.reducer';
import { listVisible } from '../../store/common/list-state.models';
import { loadOrganizations } from '../../store/organizations/organizations.actions';
import { RootState } from '../../store/store';

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
    flex: '1 1 100%',
  },
  activityListToolbar: {
    paddingLeft: theme.spacing(2),
    paddingRight: theme.spacing(2),
  },
}));

function IndexPage() {
  const classes = useStyles();

  const { user } = useAuthenticatedUser(['https://swetuggtixlocal.onmicrosoft.com/tix-api/access_as_admin']);

  const [addModalOpen, setAddModalOpen] = useState(false);

  const dispatch = useDispatch();
  useEffect(() => {
    dispatch(loadActivities());
    dispatch(loadOrganizations());
  }, []);

  const onAddButtonClick = () => {
    setAddModalOpen(true);
  };

  const { organizations } = useSelector((r: RootState) => r.organizations);

  const orgs = useMemo(() => listVisible(organizations), [organizations]);

  const activities = useSelector((r: RootState) => listVisible(r.activities.activities));

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
              {user.current && (
                <CreateActivityModal user={user.current} organizations={orgs} open={addModalOpen} setOpen={setAddModalOpen} />
              )}
            </Toolbar>
            <ActivityList activities={activities} />
          </Paper>
        </Grid>
      </Grid>
    </Container>
  );
}

export default IndexPage;
