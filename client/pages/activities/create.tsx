import React from 'react';
import { GetServerSideProps } from 'next';
import { makeStyles, Paper } from '@material-ui/core';
import {
  Container,
  Grid,
  Typography,
} from '@material-ui/core';
import Layout from '../../components/layout/main-layout';
import CreateActivity from '../../components/activities/create-activity';


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

  return (
    <Layout>
      <Container maxWidth={false} className={classes.container}>
        <Typography variant="h4" component="h1" gutterBottom>
          Create Activity
        </Typography>
        <Grid container spacing={3}>
          <Grid item xs={12} md={6}>
            <Paper className={classes.paper}>
              <CreateActivity />
            </Paper>
          </Grid>
          <Grid item xs={12} md={6}>

          </Grid>
        </Grid>
      </Container>
    </Layout>
  );
}

export const getServerSideProps: GetServerSideProps = async () => {

  return {
    props: {
    }
  }
}