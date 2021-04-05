import { CircularProgress, makeStyles, Typography } from '@material-ui/core';
import Button from '@material-ui/core/Button';
import IconButton from '@material-ui/core/IconButton';
import Menu from '@material-ui/core/Menu';
import MenuItem from '@material-ui/core/MenuItem';
import AccountCircle from '@material-ui/icons/AccountCircle';
import React, { useState } from 'react';
import { useDispatch, useSelector } from 'react-redux';

import { InteractionKind, login, logout, User } from '../../store/auth/auth.actions';
import { TixState } from '../../store/common/state.models';
import { RootState } from '../../store/store';

const useStyles = makeStyles((theme) => ({
  wrapper: {
    margin: theme.spacing(1),
    position: 'relative',
  },
  buttonProgress: {
    position: 'absolute',
    top: '50%',
    left: '50%',
    marginTop: -12,
    marginLeft: -12,
  },
}));

const SignInButton = () => {
  const [anchorEl, setAnchorEl] = useState<Element | null>(null);
  const open = Boolean(anchorEl);
  const dispatch = useDispatch();

  const handleLogin = (loginType: string) => {
    setAnchorEl(null);

    if (loginType === 'popup') {
      dispatch(login(InteractionKind.POPUP));
    } else if (loginType === 'redirect') {
      dispatch(login(InteractionKind.REDIRECT));
    }
  };

  return (
    <div>
      <Button onClick={(event) => setAnchorEl(event.currentTarget)} color="inherit">
        Login
      </Button>
      <Menu
        id="menu-appbar"
        anchorEl={anchorEl}
        anchorOrigin={{
          vertical: 'top',
          horizontal: 'right',
        }}
        keepMounted
        transformOrigin={{
          vertical: 'top',
          horizontal: 'right',
        }}
        open={open}
        onClose={() => setAnchorEl(null)}>
        <MenuItem onClick={() => handleLogin('popup')} key="loginPopup">
          Sign in using Popup
        </MenuItem>
        <MenuItem onClick={() => handleLogin('redirect')} key="loginRedirect">
          Sign in using Redirect
        </MenuItem>
      </Menu>
    </div>
  );
};

const SignOutButton = () => {
  const [anchorEl, setAnchorEl] = useState<Element | null>(null);
  const open = Boolean(anchorEl);
  const dispatch = useDispatch();

  const handleLogout = () => {
    setAnchorEl(null);
    console.log('Logging out');
    dispatch(logout());
  };

  return (
    <div>
      <IconButton onClick={(event) => setAnchorEl(event.currentTarget)} color="inherit">
        <AccountCircle />
      </IconButton>
      <Menu
        id="menu-appbar"
        anchorEl={anchorEl}
        anchorOrigin={{
          vertical: 'top',
          horizontal: 'right',
        }}
        keepMounted
        transformOrigin={{
          vertical: 'top',
          horizontal: 'right',
        }}
        open={open}
        onClose={() => setAnchorEl(null)}>
        <MenuItem onClick={handleLogout}>Logout</MenuItem>
      </Menu>
    </div>
  );
};

interface SignInButtonState {
  user: TixState<User>;
  inProgress: boolean;
}

export const SignInSignOutButton = ({ user, inProgress }: SignInButtonState) => {
  const classes = useStyles();
  return (
    <div className={classes.wrapper}>
      {(user.fetching || user.saving || inProgress) && (
        <CircularProgress size="1.5rem" color="inherit" className={classes.buttonProgress} />
      )}
      {user.current ? <SignOutButton /> : <SignInButton />}
    </div>
  );
};

export const WelcomeName = ({ user }: { user: TixState<User> }) => {
  if (user.current) {
    return <Typography variant="h6">Welcome, {user.current.name}</Typography>;
  } else {
    return null;
  }
};
