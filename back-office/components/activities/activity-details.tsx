import { Button, makeStyles, Typography } from "@material-ui/core";
import { Activity } from "../../store/activities/activity.models";

import { Container } from "@material-ui/core";
import { Table, TableBody, TableRow, TableCell, Grid } from "@material-ui/core";
import React from "react";
import Link from "next/link";

const useStyles = makeStyles((theme) => ({
  root: {
    padding: theme.spacing(0),
  },
}))

const frontpageRoot = process.env.NEXT_PUBLIC_FRONTPAGE_ROOT ?? 'http://localhost:3001';

export type ActivityDetailsProps = {
  activity: Activity,
}

export function ActivityDetails({ activity }: ActivityDetailsProps) {
  const classes = useStyles();
  return (
    <Container className={classes.root}>
      <Typography variant="overline">
        Activity Information
      </Typography>
      <Grid container spacing={2}>
        <Grid item xs={12}>
          <Table size="small">
            <TableBody>
              <TableRow>
                <TableCell>Free Seats</TableCell>
                <TableCell>{activity.freeSeats}</TableCell>
              </TableRow>
              <TableRow>
                <TableCell>Total Seats</TableCell>
                <TableCell>{activity.totalSeats}</TableCell>
              </TableRow>
              <TableRow>
                <TableCell>Owner</TableCell>
                <TableCell>{activity.ownerId}</TableCell>
              </TableRow>
              
            </TableBody>
          </Table>
        </Grid>
        <Grid item xs={12}>
          <Link href={`${frontpageRoot}/${activity.activityId}`}>
            <Button
              variant="contained" color="primary">
              Public page
            </Button>
          </Link>
        </Grid>
      </Grid>
    </Container>
  )
}