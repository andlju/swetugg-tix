import React from 'react';
import { GetServerSideProps } from 'next';
import { makeStyles } from '@material-ui/core';
import clsx from 'clsx';
import {
  Container,
  Grid,
  Typography,
} from '@material-ui/core';

import Link from '../../components/Link';
import Layout from '../../components/layout/main-layout';
import CreateActivity from '../../components/activities/create-activity';


const useStyles = makeStyles((theme) => ({
  container: {
    paddingTop: theme.spacing(4),
    paddingBottom: theme.spacing(4),
  },
  paper: {
    padding: theme.spacing(2),
    display: 'flex',
    flexDirection: 'column'
  },
  input: {
    flex: '1',
    marginTop: theme.spacing(2)
  },
  button: {
    marginTop: theme.spacing(2)
  }
}));

export default function ActivityPage() {
  const classes = useStyles();

  return (
    <Layout>
      <Container maxWidth={false} className={classes.container}>
        <Typography variant="h4" component="h1" gutterBottom>
          Create Activity
        </Typography>
        <Grid container spacing={3}>
          <Grid item xs={12} md={4}>
            <CreateActivity />
          </Grid>
          <Grid item xs={12} md={8}>

          </Grid>
        </Grid>
      </Container>
    </Layout>
  );
}

export const getServerSideProps: GetServerSideProps = async (context) => {

  return {
    props: {
    }
  }
}