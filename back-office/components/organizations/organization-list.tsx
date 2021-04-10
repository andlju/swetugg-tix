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

import { Organization } from '../../store/organizations/organizations.actions';

export interface OrganizationListProps {
  organizations: Organization[];
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
  activitiesColumnHead: {
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

export function OrganizationList({ organizations }: OrganizationListProps) {
  const classes = useStyles();

  return (
    <TableContainer>
      <Table size="small">
        <TableHead>
          <TableRow>
            <TableCell className={classes.nameColumnHead}>Name</TableCell>
            <TableCell className={classes.activitiesColumnHead}>Activities</TableCell>
            <TableCell className={classes.actionsColumnHead}>Actions</TableCell>
          </TableRow>
        </TableHead>
        <TableBody>
          {organizations.map((row) => (
            <TableRow hover={true} key={row.organizationId}>
              <TableCell>
                <Typography>{row.name}</Typography>
                <Typography className={classes.identifier}>{row.organizationId}</Typography>
              </TableCell>
              <TableCell className={classes.numberCell}>{0}</TableCell>
              <TableCell>
                <Link href={`/organizations/${row.organizationId}`}>
                  <Button variant="contained" color="secondary">
                    Details
                  </Button>
                </Link>
              </TableCell>
            </TableRow>
          ))}
        </TableBody>
      </Table>
    </TableContainer>
  );
}
