import { Typography } from '@material-ui/core';
import Button from '@material-ui/core/Button';
import IconButton from '@material-ui/core/IconButton';
import Menu from '@material-ui/core/Menu';
import MenuItem from '@material-ui/core/MenuItem';
import AccountCircle from '@material-ui/icons/AccountCircle';
import React, { useState } from 'react';
import { useDispatch, useSelector } from 'react-redux';

import { InteractionKind, login, logout } from '../../store/auth/auth.actions';
import { RootState } from '../../store/store';

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

export const SignInSignOutButton = () => {
  const { user } = useSelector((s: RootState) => s.auth);

  if (user.current) {
    return <SignOutButton />;
  } else {
    return <SignInButton />;
  }
};

export const WelcomeName = () => {
  const { user } = useSelector((s: RootState) => s.auth);

  if (user.current) {
    return <Typography variant="h6">Welcome, {user.current.name}</Typography>;
  } else {
    return null;
  }
};
