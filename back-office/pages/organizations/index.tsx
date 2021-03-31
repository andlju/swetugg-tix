import React, { useEffect, useMemo } from 'react';
import { GetServerSideProps } from 'next';
import clsx from 'clsx';
import { Fab, makeStyles, Toolbar } from '@material-ui/core';
import AddIcon from '@material-ui/icons/Add';
import Typography from '@material-ui/core/Typography';
import Paper from '@material-ui/core/Paper';
import Container from '@material-ui/core/Container';
import Grid from '@material-ui/core/Grid';
import { useAuthenticatedUser } from '../../src/use-authenticated-user.hook';
import { useDispatch, useSelector } from 'react-redux';
import { loadOrganizations } from '../../store/organizations/organizations.actions';
import { RootState } from '../../store/store';
import { OrganizationList } from '../../components/organizations/organization-list';
import { useState } from 'react';
import { CreateOrganizationModal } from '../../components/organizations/create-organization-modal';

const useStyles = makeStyles((theme) => ({
  container: {
    paddingTop: theme.spacing(4),
    paddingBottom: theme.spacing(4),
  },
  paper: {
    padding: theme.spacing(1),
  },
  organizationList: {
    minHeight: theme.spacing(30),
    maxHeight: theme.spacing(60),
    overflow: 'auto',
  },
  organizationListTitle: {
    flex: '1 1 100%'
  },
  organizationListToolbar: {
    paddingLeft: theme.spacing(2),
    paddingRight: theme.spacing(2)
  }
}));

export default function Index() {
  const classes = useStyles();

  const organizations = useSelector((r: RootState) => r.organizations);

  const orgs = useMemo(() => organizations.visibleOrganizations.ids.map(orgId => organizations.organizations[orgId]), [organizations]);

  const { user } = useAuthenticatedUser(["https://swetuggtixlocal.onmicrosoft.com/tix-api/access_as_admin"]);

  const [addModalOpen, setAddModalOpen] = useState(false);

  const onAddButtonClick = () => {
    setAddModalOpen(true);
  };

  const dispatch = useDispatch();
  useEffect(() => {
    dispatch(loadOrganizations());
  }, []);

  return (
    <Container maxWidth={false} className={classes.container}>
      <Typography variant="h4" component="h1" gutterBottom>
        Organizations
      </Typography>
      <Grid container spacing={3}>
        {/* List of organizations for current user */}
        <Grid item xs={12}>
          <Paper className={clsx(classes.paper, classes.organizationList)}>
            <Toolbar className={classes.organizationListToolbar}>
              <Typography className={classes.organizationListTitle} variant="h6" component="div">
                Organizations
              </Typography>
              <Fab size="small" color="primary" onClick={onAddButtonClick}>
                <AddIcon />
              </Fab>
              {user.current && <CreateOrganizationModal organizations={organizations} open={addModalOpen} setOpen={setAddModalOpen} />}
            </Toolbar>
            {orgs && <OrganizationList organizations={orgs} />}
          </Paper>
        </Grid>
      </Grid >

    </Container >
  );
}

export const getServerSideProps: GetServerSideProps = async () => {
  return {
    props: {

    }
  };
};