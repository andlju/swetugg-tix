import * as React from 'react';
import ListItem from '@material-ui/core/ListItem';
import ListItemIcon from '@material-ui/core/ListItemIcon';
import ListItemText from '@material-ui/core/ListItemText';
import ListSubheader from '@material-ui/core/ListSubheader';
import DashboardIcon from '@material-ui/icons/Dashboard';
import SettingsIcon from '@material-ui/icons/Settings';
import BusinessIcon from '@material-ui/icons/Business'
import EventIcon from '@material-ui/icons/Event'
import Link from 'next/link';

export const mainListItems = (
  <div>
    <Link href="/">
      <ListItem button>
        <ListItemIcon>
          <DashboardIcon />
        </ListItemIcon>
        <ListItemText primary="Dashboard" />
      </ListItem>
    </Link>
    <Link href="/activities">
      <ListItem button>
        <ListItemIcon>
          <EventIcon />
        </ListItemIcon>
        <ListItemText primary="Activities" />
      </ListItem>
    </Link>
    <Link href="/organizations">
      <ListItem button>
        <ListItemIcon>
          <BusinessIcon />
        </ListItemIcon>
        <ListItemText primary="Organizations" />
      </ListItem>
    </Link>
  </div>
);

export const adminListItems = (
  <div>
    <ListSubheader inset>Administration</ListSubheader>
    <Link href="/admin">
    <ListItem button>
      <ListItemIcon>
        <SettingsIcon />
      </ListItemIcon>
      <ListItemText primary="Views" />
    </ListItem>
    </Link>
  </div>
);
