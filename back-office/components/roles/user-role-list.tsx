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
import { UserRole } from '../../store/users/users.actions';

export interface UserRoleListProps {
  organization: Organization;
  user: User;
  userRoles: UserRole[];
}

const useStyles = makeStyles((theme) => ({
  nameColumnHead: {
    width: '80%',
    fontWeight: 'bold',
  },
  attributesColumnHead: {
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

export function UserRoleList({ organization, user, userRoles }: UserRoleListProps) {
  const classes = useStyles();

  return (
    <TableContainer>
      <Table size="small">
        <TableHead>
          <TableRow>
            <TableCell className={classes.nameColumnHead}>Name</TableCell>
            <TableCell className={classes.attributesColumnHead}>Attributes</TableCell>
            <TableCell className={classes.actionsColumnHead}>Actions</TableCell>
          </TableRow>
        </TableHead>
        <TableBody>
          {userRoles.map((row) => (
            <TableRow hover={true} key={row.userRoleId}>
              <TableCell>
                <Typography>{row.roleName}</Typography>
                <Typography className={classes.identifier}>{row.userRoleId}</Typography>
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
