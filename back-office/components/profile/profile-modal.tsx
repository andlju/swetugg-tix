import React, { useEffect, useRef } from 'react';
import { makeStyles } from '@material-ui/core/styles';
import { RootState } from '../../store/store';
import { logout, UserStatus } from '../../store/auth/auth.actions';
import { useDispatch, useSelector } from 'react-redux';
import { Button, Dialog, DialogActions, DialogContent, DialogContentText, DialogTitle } from '@material-ui/core';
import EditProfile, { EditProfileHandle } from './edit-profile';

const useStyles = makeStyles((theme) => ({
  logoutButton: {
    alignSelf: "left",
    marginRight: "auto"
  },
  saveButton: {

  },
}));

export function ProfileModal() {
  const classes = useStyles();
  const [open, setOpen] = React.useState(false);
  const dispatch = useDispatch();
  const { user } = useSelector((r: RootState) => r.auth);

  useEffect(() => {
    if (user?.status === UserStatus.None) {
      setOpen(true);
    } else {
      setOpen(false);
    }
  }, [user]);

  const handleClose = () => {
    setOpen(false);
  };

  const profileEditRef = useRef<EditProfileHandle>(null);
  const handleSave = () => {
    profileEditRef.current?.submit();
  }

  const handleLogout = () => {
    dispatch(logout());
  }

  return (
    <React.Fragment>
      { user &&
        <Dialog
          open={open}
          onClose={handleClose}
          disableBackdropClick={true}
          closeAfterTransition
          BackdropProps={{
            timeout: 500,
          }}
        >
          <DialogTitle>New user</DialogTitle>
          <DialogContent>
            <DialogContentText>
              Hello and welcome to Swetugg Tix. Since you are a new user we would like to know a little bit more about you. Please fill out the following form.
            </DialogContentText>
            <EditProfile user={user} ref={profileEditRef} />
            <DialogActions>
              <Button onClick={handleLogout}
                className={classes.logoutButton} color="default">
                Logout
              </Button>
              <Button onClick={handleSave}
                 className={classes.saveButton} color="primary">
                Save
              </Button>
            </DialogActions>
          </DialogContent>
        </Dialog>
      }
    </React.Fragment>);
}