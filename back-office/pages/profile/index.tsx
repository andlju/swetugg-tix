import { Button, CircularProgress, makeStyles } from '@material-ui/core';
import Container from '@material-ui/core/Container';
import Grid from '@material-ui/core/Grid';
import Paper from '@material-ui/core/Paper';
import Typography from '@material-ui/core/Typography';
import { GetServerSideProps } from 'next';
import React, { useEffect } from 'react';
import { useForm } from 'react-hook-form';
import { useDispatch, useSelector } from 'react-redux';

import { EditProfile, UserFormData } from '../../components/profile/edit-profile';
import { useAuthenticatedUser } from '../../src/use-authenticated-user.hook';
import { updateUser } from '../../store/auth/auth.actions';
import { RootState } from '../../store/store';

const useStyles = makeStyles((theme) => ({
  container: {
    paddingTop: theme.spacing(4),
    paddingBottom: theme.spacing(4),
  },
  paper: {
    padding: theme.spacing(2),
  },
  form: {
    display: 'flex',
    flexDirection: 'column',
  },
  wrapper: {
    margin: theme.spacing(1),
    position: 'relative',
  },
  saveButton: {},
  buttonProgress: {
    position: 'absolute',
    top: '50%',
    left: '50%',
    marginTop: -12,
    marginLeft: -12,
  },
}));

export default function Index() {
  const classes = useStyles();

  useAuthenticatedUser(['https://swetuggtixlocal.onmicrosoft.com/tix-api/access_as_admin']);

  const dispatch = useDispatch();
  const { user } = useSelector((r: RootState) => r.auth);

  const userForm = useForm<UserFormData>({
    defaultValues: {
      name: '',
    },
  });

  const { handleSubmit, formState, reset } = userForm;

  useEffect(() => {
    onReset();
  }, [user.current]);

  const onReset = () => {
    reset({ name: user.current?.name || '' });
  };

  const onSubmit = async (data: UserFormData) => {
    if (!user.current) return;
    user.current.name = data.name;
    dispatch(updateUser(user.current));
  };

  return (
    <Container maxWidth={false} className={classes.container}>
      <Typography variant="h4" component="h1" gutterBottom>
        Edit Profile
      </Typography>
      <Grid container spacing={3}>
        <Grid item xs={12} md={6}>
          <Paper className={classes.paper}>
            <form className={classes.form} onSubmit={handleSubmit(onSubmit)}>
              <Grid container>
                <Grid item xs={12}>
                  {user.current && <EditProfile userForm={userForm} />}
                </Grid>
                <Grid item>
                  <div className={classes.wrapper}>
                    <Button variant="contained" className={classes.saveButton} onClick={onReset}>
                      Reset
                    </Button>
                  </div>
                </Grid>
                <Grid item>
                  <div className={classes.wrapper}>
                    <Button
                      type="submit"
                      variant="contained"
                      color="primary"
                      className={classes.saveButton}
                      disabled={user.fetching || user.saving}>
                      Save
                    </Button>
                    {(user.fetching || user.saving) && (
                      <CircularProgress size="1.4rem" className={classes.buttonProgress} />
                    )}
                  </div>
                </Grid>
              </Grid>
            </form>
          </Paper>
        </Grid>
        <Grid item xs={12} md={6}></Grid>
      </Grid>
    </Container>
  );
}

export const getServerSideProps: GetServerSideProps = async () => {
  return {
    props: {},
  };
};
