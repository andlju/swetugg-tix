import { Fab, makeStyles, Toolbar } from '@material-ui/core';
import Container from '@material-ui/core/Container';
import Grid from '@material-ui/core/Grid';
import Paper from '@material-ui/core/Paper';
import Typography from '@material-ui/core/Typography';
import AddIcon from '@material-ui/icons/Add';
import { GetServerSideProps, NextPage } from 'next';
import React, { useEffect, useState } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { AddUserRoleModal } from '../../../../components/roles/add-user-role-modal';

import { UserRoleList } from '../../../../components/roles/user-role-list';
import { useAuthenticatedUser } from '../../../../src/use-authenticated-user.hook';
import { loadActivities } from '../../../../store/activities/activities.actions';
import { listVisible } from '../../../../store/common/list-state.models';
import { loadOrganization, loadOrganizationUsers } from '../../../../store/organizations/organizations.actions';
import { loadRoles } from '../../../../store/roles/roles.actions';
import { RootState } from '../../../../store/store';
import { loadUserRoles, UserRole } from '../../../../store/users/users.actions';

interface OrganizationUserProps {
  organizationId: string;
  userId: string;
}

const useStyles = makeStyles((theme) => ({
  container: {
    paddingTop: theme.spacing(4),
    paddingBottom: theme.spacing(4),
  },
  paper: {
    padding: theme.spacing(2),
  },
  organizationUsersList: {
    minHeight: theme.spacing(30),
    maxHeight: theme.spacing(60),
    overflow: 'auto',
  },
  organizationUsersListTitle: {
    flex: '1 1 100%',
  },
  organizationUsersListToolbar: {
    paddingLeft: theme.spacing(2),
    paddingRight: theme.spacing(2),
  },
}));

const OrganizationUserPage: NextPage<OrganizationUserProps> = ({ userId, organizationId }) => {
  const classes = useStyles();
  const dispatch = useDispatch();

  const { user } = useAuthenticatedUser(['https://swetuggtixdev.onmicrosoft.com/tix-api/access_as_backoffice']);

  useEffect(() => {
    if (user.current?.userId) {
      dispatch(loadOrganization(organizationId));
      dispatch(loadOrganizationUsers(organizationId));
      dispatch(loadUserRoles(userId, organizationId));
      dispatch(loadActivities());
      dispatch(loadRoles());
    }
  }, [user.current, organizationId, userId]);

  const [addRoleModalOpen, setAddRoleModalOpen] = useState(false);

  const onAddRoleButtonClick = () => {
    setAddRoleModalOpen(true);
  };

  const organizations = useSelector((r: RootState) => r.organizations);
  const roles = useSelector((r: RootState) => r.roles);
  const users = useSelector((r: RootState) => r.users);

  const organization = organizations.organizations.models[organizationId];
  const orgUser = organizations.organizationUsers.models[userId];

  const visibleUserRoles = listVisible(users.userRoles);
  const visibleRoles = listVisible(roles.roles);

  return (
    <React.Fragment>
      {orgUser && (
        <Container maxWidth={false} className={classes.container}>
          <Typography variant="h4" component="h1" gutterBottom>
            {orgUser.name}
          </Typography>
          <Grid container spacing={3}>
            <Grid item xs={12} md={5}>
              <Grid container spacing={2}>
                <Grid item xs={12}>
                  <Paper className={classes.paper}></Paper>
                </Grid>
              </Grid>
            </Grid>
            <Grid item xs={12}>
              <Paper className={classes.paper}>
                <Toolbar className={classes.organizationUsersListToolbar}>
                  <Typography className={classes.organizationUsersListTitle} variant="h6" component="div">
                    Roles
                  </Typography>
                  <Fab size="small" color="primary" onClick={onAddRoleButtonClick}>
                    <AddIcon />
                  </Fab>
                  {
                    <AddUserRoleModal
                      roles={visibleRoles}
                      userId={userId}
                      open={addRoleModalOpen}
                      setOpen={setAddRoleModalOpen}
                    />
                  }
                </Toolbar>
                {<UserRoleList organization={organization} user={orgUser} userRoles={visibleUserRoles} />}
              </Paper>
            </Grid>
          </Grid>
        </Container>
      )}
    </React.Fragment>
  );
};

export default OrganizationUserPage;

export const getServerSideProps: GetServerSideProps = async ({ params, query }) => {
  const organizationId = params?.organizationId;
  const userId = params?.userId;

  return {
    props: {
      organizationId: organizationId,
      userId: userId,
    },
  };
};
