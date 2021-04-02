import { makeStyles } from '@material-ui/core';
import Container from '@material-ui/core/Container';
import Grid from '@material-ui/core/Grid';
import Paper from '@material-ui/core/Paper';
import Typography from '@material-ui/core/Typography';
import { GetServerSideProps, NextPage } from 'next';
import React, { useEffect } from 'react';
import { useDispatch, useSelector } from 'react-redux';

import { OrganizationDetails } from '../../../components/organizations/organization-details';
import { UserList } from '../../../components/organizations/user-list';
import { useAuthenticatedUser } from '../../../src/use-authenticated-user.hook';
import { loadOrganization, loadOrganizationUsers } from '../../../store/organizations/organizations.actions';
import { RootState } from '../../../store/store';

interface OrganizationProps {
  organizationId: string;
}

const useStyles = makeStyles((theme) => ({
  container: {
    paddingTop: theme.spacing(4),
    paddingBottom: theme.spacing(4),
  },
  paper: {
    padding: theme.spacing(2),
  },
}));

const OrganizationPage: NextPage<OrganizationProps> = ({ organizationId }) => {
  const classes = useStyles();
  const dispatch = useDispatch();

  const { user } = useAuthenticatedUser(['https://swetuggtixlocal.onmicrosoft.com/tix-api/access_as_admin']);

  useEffect(() => {
    if (user.current?.userId) {
      dispatch(loadOrganization(organizationId));
      dispatch(loadOrganizationUsers(organizationId));
    }
  }, [user, organizationId]);

  const organizations = useSelector((r: RootState) => r.organizations);

  const organization = organizations.organizations[organizationId];

  return (
    <React.Fragment>
      {organization && (
        <Container maxWidth={false} className={classes.container}>
          <Typography variant="h4" component="h1" gutterBottom>
            {organization.name}
          </Typography>
          <Grid container spacing={3}>
            <Grid item xs={12} md={5}>
              <Grid container spacing={2}>
                <Grid item xs={12}>
                  <Paper className={classes.paper}>
                    <OrganizationDetails organization={organization} />
                  </Paper>
                </Grid>
                <Grid item xs={12}>
                  <Paper className={classes.paper}></Paper>
                </Grid>
              </Grid>
            </Grid>
            <Grid item xs={12}>
              <UserList organization={organization} users={[]} />
            </Grid>
          </Grid>
        </Container>
      )}
    </React.Fragment>
  );
};

export default OrganizationPage;

export const getServerSideProps: GetServerSideProps = async ({ params, query }) => {
  const organizationId = params?.organizationId;

  return {
    props: {
      organizationId: organizationId,
    },
  };
};
