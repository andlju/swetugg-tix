import {
  Button,
  CircularProgress,
  Dialog,
  DialogActions,
  DialogContent,
  DialogContentText,
  DialogTitle,
} from '@material-ui/core';
import { makeStyles } from '@material-ui/core/styles';
import React, { useEffect } from 'react';
import { useForm } from 'react-hook-form';
import { useDispatch, useSelector } from 'react-redux';

import { createUser, logout, User, UserStatus } from '../../store/auth/auth.actions';
import { TixState } from '../../store/common/state.models';
import { RootState } from '../../store/store';
import { NewProfile, UserFormData } from './new-profile';

const useStyles = makeStyles((theme) => ({
  form: {
    display: 'flex',
    flexDirection: 'column',
  },
  wrapper: {
    margin: theme.spacing(1),
    position: 'relative',
  },
  logoutButton: {
    marginRight: 'auto',
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

interface ProfileModalProps {
  user: TixState<User>;
}

export function ProfileModal({ user }: ProfileModalProps) {
  const classes = useStyles();
  const [open, setOpen] = React.useState(false);
  const dispatch = useDispatch();

  const userForm = useForm<UserFormData>({
    defaultValues: {
      name: '',
    },
  });

  const { handleSubmit, setValue, reset } = userForm;

  useEffect(() => {
    if (user.current && user.current?.status === UserStatus.None) {
      console.log('Setting user name to', user.current?.name);
      setOpen(true);
      reset({ name: user.current?.name || 'Your name here' });
    } else {
      setOpen(false);
    }
  }, [user.current]);

  const handleLogout = () => {
    dispatch(logout());
  };

  const onSubmit = async (data: UserFormData) => {
    if (!user.current) return;
    user.current.name = data.name;
    dispatch(createUser(user.current));
  };

  return (
    <React.Fragment>
      {user.current && (
        <Dialog
          open={open}
          disableBackdropClick={true}
          maxWidth="sm"
          fullWidth={true}
          closeAfterTransition
          BackdropProps={{
            timeout: 500,
          }}>
          <DialogTitle>New user</DialogTitle>
          <DialogContent>
            <DialogContentText>
              Hello and welcome to Swetugg Tix. Since you are a new user we would like to know a little bit more about you. Please
              fill out the following form.
            </DialogContentText>
            <form className={classes.form} onSubmit={handleSubmit(onSubmit)}>
              <NewProfile userForm={userForm} />
              <DialogActions>
                <Button onClick={handleLogout} className={classes.logoutButton} color="default">
                  Logout
                </Button>
                <div className={classes.wrapper}>
                  <Button type="submit" className={classes.saveButton} color="primary" disabled={user.fetching || user.saving}>
                    Save
                  </Button>
                  {(user.fetching || user.saving) && <CircularProgress size="1.4rem" className={classes.buttonProgress} />}
                </div>
              </DialogActions>
            </form>
          </DialogContent>
        </Dialog>
      )}
    </React.Fragment>
  );
}
