import * as React from 'react';
import ListItem from '@material-ui/core/ListItem';
import ListItemIcon from '@material-ui/core/ListItemIcon';
import ListItemText from '@material-ui/core/ListItemText';
import ListSubheader from '@material-ui/core/ListSubheader';
import DashboardIcon from '@material-ui/icons/Dashboard';
import SettingsIcon from '@material-ui/icons/Settings';
import EventIcon from '@material-ui/icons/Event'
import Link from '../../components/Link';

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
    <Link href="/back-office/activities">
      <ListItem button>
        <ListItemIcon>
          <EventIcon />
        </ListItemIcon>
        <ListItemText primary="Activities" />
      </ListItem>
    </Link>
  </div>
);

export const adminListItems = (
  <div>
    <ListSubheader inset>Administration</ListSubheader>
    <Link href="/back-office/admin">
    <ListItem button>
      <ListItemIcon>
        <SettingsIcon />
      </ListItemIcon>
      <ListItemText primary="Views" />
    </ListItem>
    </Link>
  </div>
);
