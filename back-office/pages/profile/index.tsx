import React, { useEffect, useRef } from 'react';
import { GetServerSideProps } from 'next';
import { Button, makeStyles } from '@material-ui/core';
import clsx from 'clsx';
import Typography from '@material-ui/core/Typography';
import Paper from '@material-ui/core/Paper';
import Container from '@material-ui/core/Container';
import Grid from '@material-ui/core/Grid';
import { useAuthenticatedUser } from '../../src/use-authenticated-user.hook';
import EditProfile, { EditProfileHandle } from '../../components/profile/edit-profile';

const useStyles = makeStyles((theme) => ({
  container: {
    paddingTop: theme.spacing(4),
    paddingBottom: theme.spacing(4),
  },
  paper: {
    padding: theme.spacing(2)
  }
}));

export default function Index() {
  const classes = useStyles();

  const { user } = useAuthenticatedUser(["https://swetuggtixlocal.onmicrosoft.com/tix-api/access_as_admin"]);

  const profileEditRef = useRef<EditProfileHandle>(null);
  const handleSave = () => {
    profileEditRef.current?.submit();
  }

  return (
    <Container maxWidth={false} className={classes.container}>
      <Typography variant="h4" component="h1" gutterBottom>
        Edit Profile
        </Typography>
      <Grid container spacing={3}>
        <Grid item xs={12} md={6}>
          <Paper className={classes.paper}>
            { user && <EditProfile user={user} ref={profileEditRef} /> }
            <Button onClick={handleSave}
              //className={classes.button}
              //disabled={formState.isSubmitting}
              >
              Update
            </Button>
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