import {
  Button,
  Link,
  makeStyles,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Typography,
} from '@material-ui/core';
import React from 'react';

import { User } from '../../store/auth/auth.actions';
import { Organization } from '../../store/organizations/organizations.actions';

export interface UserListProps {
  organization: Organization;
  users: User[];
}

const useStyles = makeStyles((theme) => ({
  nameColumnHead: {
    width: '60%',
    fontWeight: 'bold',
  },
  freeSeatsColumnHead: {
    width: '10%',
  },
  totalSeatsColumnHead: {
    width: '10%',
  },
  ticketTypesColumnHead: {
    width: '10%',
  },
  actionsColumnHead: {
    width: '10%',
  },
  numberCell: {
    textAlign: 'right',
  },
  identifier: {
    color: theme.palette.text.secondary,
    fontVariantCaps: 'all-small-caps',
  },
}));

export function UserList({ organization, users }: UserListProps) {
  const classes = useStyles();

  return (
    <TableContainer>
      <Table size="small">
        <TableHead>
          <TableRow>
            <TableCell className={classes.nameColumnHead}>Name</TableCell>
            <TableCell className={classes.ticketTypesColumnHead}>Ticket Types</TableCell>
            <TableCell className={classes.actionsColumnHead}>Actions</TableCell>
          </TableRow>
        </TableHead>
        <TableBody>
          {users.map((row) => (
            <TableRow hover={true} key={row.userId}>
              <TableCell>
                <Typography>{row.name}</Typography>
                <Typography className={classes.identifier}>{row.userId}</Typography>
              </TableCell>
              <TableCell className={classes.numberCell}>{0}</TableCell>
              <TableCell>
                <Button variant="contained" color="primary">
                  Remove
                </Button>
              </TableCell>
            </TableRow>
          ))}
        </TableBody>
      </Table>
    </TableContainer>
  );
}
