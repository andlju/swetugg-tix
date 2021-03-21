import React, { useEffect, useMemo } from 'react';
import { GetServerSideProps } from 'next';
import { makeStyles, Paper } from '@material-ui/core';
import {
  Container,
  Grid,
  Typography,
} from '@material-ui/core';
import { CreateActivity } from '../../components';
import { useAuthenticatedUser } from '../../src/use-authenticated-user.hook';
import { RootState } from '../../store/store';
import { useDispatch, useSelector } from 'react-redux';
import { loadOrganizations } from '../../store/organizations/organizations.actions';

const useStyles = makeStyles((theme) => ({
  container: {
    paddingTop: theme.spacing(4),
    paddingBottom: theme.spacing(4),
  },
  paper: {
    padding: theme.spacing(2)
  }
}));


export default function CreatePage() {
  const classes = useStyles();

  const { user } = useAuthenticatedUser(["https://swetuggtixlocal.onmicrosoft.com/tix-api/access_as_admin"]);

  const { organizations, visibleOrganizations } = useSelector((r: RootState) => r.organizations);

  const orgOptions = useMemo(() => visibleOrganizations.ids.map(oId => organizations[oId]), [organizations, visibleOrganizations]);

  const dispatch = useDispatch();
  
  useEffect(() => {
    dispatch(loadOrganizations());
  },[])

  return (
    <Container maxWidth={false} className={classes.container}>
      <Typography variant="h4" component="h1" gutterBottom>
        Create Activity
        </Typography>
      <Grid container spacing={3}>
        <Grid item xs={12} md={6}>
          <Paper className={classes.paper}>
            {user.current && <CreateActivity user={user.current} organizations={ orgOptions }/>}
          </Paper>
        </Grid>
        <Grid item xs={12} md={6}>

        </Grid>
      </Grid>
    </Container>
  );
}

export const getServerSideProps: GetServerSideProps = async () => {

  return {
    props: {
    }
  };
};