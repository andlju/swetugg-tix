import React from 'react';
import { router } from 'next';
import { makeStyles } from '@material-ui/core';
import clsx from 'clsx';
import {
  Typography,
  Paper,
  TextField,
  Button
} from '@material-ui/core';

import { buildUrl } from '../../src/url-utils';

interface CreateActivityProps {

}

const useStyles = makeStyles((theme) => ({
  paper: {
    padding: theme.spacing(2),
    display: 'flex',
    flexDirection: 'column'
  },
  input: {
    flex: '1',
    marginTop: theme.spacing(2)
  },
  button: {
    marginTop: theme.spacing(2)
  }
}));

export default function CreateActivity({ }: CreateActivityProps) {
  const classes = useStyles();

  const createActivity = async (evt : React.FormEvent) => {
    evt.preventDefault();
    const res = await fetch(buildUrl('/activities'), {
      method: 'POST',
      body: JSON.stringify({
        name: 'test'
      }),
      headers: {
        'Content-Type': 'application/json'
      }
    });

    const result = await res.json();
    router.push(`/activities/${result.activityId}`);
  };
  return (<Paper className={classes.paper} component="form" onSubmit={createActivity}>
    <Typography variant="overline">Activity</Typography>
    <TextField id="name" className={classes.input} label="Name" variant="outlined"></TextField>
    <Button variant="outlined" className={classes.button} type="submit">Create</Button>
  </Paper>);
}