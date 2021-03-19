import React from 'react';
import { GetServerSideProps } from 'next';
import { makeStyles, Paper } from '@material-ui/core';
import {
  Container,
  Grid,
  Typography,
} from '@material-ui/core';
import { useAuthenticatedUser } from '../../src/use-authenticated-user.hook';
import { CreateOrganization } from '../../components/organizations/create-organization';


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

  return (
    <Container maxWidth={false} className={classes.container}>
      <Typography variant="h4" component="h1" gutterBottom>
        Create Organization
        </Typography>
      <Grid container spacing={3}>
        <Grid item xs={12} md={6}>
          <Paper className={classes.paper}>
            <CreateOrganization />
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
  }
}