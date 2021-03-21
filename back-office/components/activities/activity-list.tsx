import React from 'react';
import Link from 'next/link';
import {
  Table, TableBody, TableHead, TableRow, TableCell, TableContainer,
  Typography,
  Toolbar,
  makeStyles, Fab, Button
} from '@material-ui/core';
import AddIcon from '@material-ui/icons/Add';

import { Activity } from '../../store/activities/activity.models';

interface ActivityListProps {
  activities: Activity[]
}

const useStyles = makeStyles((theme) => ({
  nameColumnHead: {
    width: "60%",
    fontWeight: "bold"
  },
  freeSeatsColumnHead: {
    width: "10%"
  },
  totalSeatsColumnHead: {
    width: "10%"
  },
  ticketTypesColumnHead: {
    width: "10%"
  },
  actionsColumnHead: {
    width: "10%"
  },
  numberCell: {
    textAlign: "right"
  },
  identifier: {
    color: theme.palette.text.secondary,
    fontVariantCaps: "all-small-caps"
  },
  activityListTitle: {
    flex: '1 1 100%'
  },
  activityListToolbar: {
    paddingLeft: theme.spacing(2),
    paddingRight: theme.spacing(2)
  }
}));

export function ActivityList({ activities }: ActivityListProps) {

  const classes = useStyles();

  return (<React.Fragment>
    <Toolbar className={classes.activityListToolbar}>
      <Typography className={classes.activityListTitle} variant="h6" component="div">
        Activities
      </Typography>
      <Link href="/activities/create">
        <Fab size="small" color="primary">
          <AddIcon />
        </Fab>
      </Link>
    </Toolbar>

    <TableContainer>
      <Table size="small">
        <TableHead>
          <TableRow>
            <TableCell className={classes.nameColumnHead}>Name</TableCell>
            <TableCell className={classes.freeSeatsColumnHead}>Free Seats</TableCell>
            <TableCell className={classes.totalSeatsColumnHead}>Total Seats</TableCell>
            <TableCell className={classes.ticketTypesColumnHead}>Ticket Types</TableCell>
            <TableCell className={classes.actionsColumnHead}>Actions</TableCell>
          </TableRow>
        </TableHead>
        <TableBody>
          {activities.map(row => (
            <TableRow hover={true} key={row.activityId} >
              <TableCell>
                <Typography>{row.name}</Typography>
                <Typography className={classes.identifier}>{row.activityId}</Typography>
              </TableCell>
              <TableCell className={classes.numberCell}>{row.freeSeats}</TableCell>
              <TableCell className={classes.numberCell}>{row.totalSeats}</TableCell>
              <TableCell className={classes.numberCell}>{row.ticketTypes?.length ?? 0}</TableCell>
              <TableCell>
                <Link href={`/activities/${row.activityId}?ownerId=${row.ownerId}`}>
                  <Button
                    variant="contained" color="secondary">
                    Details
                    </Button>
                </Link>
              </TableCell>
            </TableRow>)
          )}
        </TableBody>
      </Table>
    </TableContainer>
  </React.Fragment>);
}

