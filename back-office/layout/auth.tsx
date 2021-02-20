import React, { useEffect, useState } from "react";
import { useIsAuthenticated, useMsal } from "@azure/msal-react";
import { loginRequest } from "../src/auth-config";
import Button from "@material-ui/core/Button";
import MenuItem from '@material-ui/core/MenuItem';
import Menu from '@material-ui/core/Menu';
import IconButton from '@material-ui/core/IconButton';
import AccountCircle from '@material-ui/icons/AccountCircle';
import { Typography } from "@material-ui/core";


const SignInButton = () => {
  const { instance } = useMsal();

  const [anchorEl, setAnchorEl] = React.useState(null);
  const open = Boolean(anchorEl);

  const handleLogin = (loginType: string) => {
      setAnchorEl(null);

      if (loginType === "popup") {
          instance.loginPopup(loginRequest);
      } else if (loginType === "redirect") {
          instance.loginRedirect(loginRequest);
      }
  }

  return (
      <div>
          <Button
              onClick={(event) => setAnchorEl(event.currentTarget)}
              color="inherit"
          >
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
              onClose={() => setAnchorEl(null)}
          >
              <MenuItem onClick={() => handleLogin("popup")} key="loginPopup">Sign in using Popup</MenuItem>
              <MenuItem onClick={() => handleLogin("redirect")} key="loginRedirect">Sign in using Redirect</MenuItem>
          </Menu>
      </div>
  )
};

const SignOutButton = () => {
  const { instance } = useMsal();

  const [anchorEl, setAnchorEl] = React.useState(null);
  const open = Boolean(anchorEl);

  const handleLogout = () => {
      setAnchorEl(null);
      instance.logout();
  }

  return (
      <div>
          <IconButton
              onClick={(event) => setAnchorEl(event.currentTarget)}
              color="inherit"
          >
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
              onClose={() => setAnchorEl(null)}
          >
              <MenuItem onClick={handleLogout}>Logout</MenuItem>
          </Menu>
      </div>
  )
};

export const SignInSignOutButton = () => {
  const isAuthenticated = useIsAuthenticated();

  if (isAuthenticated) {
      return <SignOutButton />;
  } else {
      return <SignInButton />;
  }
}

export const WelcomeName = () => {
  const { accounts } = useMsal();
  const [name, setName] = useState(null);

  useEffect(() => {
      if (accounts.length > 0) {
          console.log('Accounts', accounts);
          setName(accounts[0].name.split(" ")[0]);
      }
  }, [accounts]);

  if (name) {
      return <Typography variant="h6">Welcome, {name}</Typography>;
  } else {
      return null;
  }
}