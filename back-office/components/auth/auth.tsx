import React, { useState } from "react";
import Button from "@material-ui/core/Button";
import MenuItem from '@material-ui/core/MenuItem';
import Menu from '@material-ui/core/Menu';
import IconButton from '@material-ui/core/IconButton';
import AccountCircle from '@material-ui/icons/AccountCircle';
import { Typography } from "@material-ui/core";
import { useDispatch, useSelector } from "react-redux";
import { RootState } from "../../store/store";
import { login, logout } from "./store/auth.actions";


const SignInButton = () => {
    const [anchorEl, setAnchorEl] = useState<Element|null>(null);
    const open = Boolean(anchorEl);
    const dispatch = useDispatch();

    const handleLogin = (loginType: string) => {
        setAnchorEl(null);
        dispatch(login());

        if (loginType === "popup") {
            console.log("Auth with popup");
        } else if (loginType === "redirect") {
            console.log("Auth with redirect");
        }
    };

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
    );
};

const SignOutButton = () => {

    const [anchorEl, setAnchorEl] = useState<Element|null>(null);
    const open = Boolean(anchorEl);
    const dispatch = useDispatch();

    const handleLogout = () => {
        setAnchorEl(null);
        console.log("Logging out");
        dispatch(logout());
    };

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
    );
};

export const SignInSignOutButton = () => {
    const { user } = useSelector((s: RootState) => s.auth);

    if (user) {
        return <SignOutButton />;
    } else {
        return <SignInButton />;
    }
};

export const WelcomeName = () => {
    const { user } = useSelector((s: RootState) => s.auth);

    if (user) {
        return <Typography variant="h6">Welcome, {user.displayName}</Typography>;
    } else {
        return null;
    }
};