import React, { useState } from 'react';
import router from 'next/router';
import { makeStyles } from '@material-ui/core';
import clsx from 'clsx';
import {
  Typography,
  Paper,
  TextField,
  Button
} from '@material-ui/core';

import { buildUrl } from '../../src/url-utils';
import { sendCommand } from '../../src/services/command.service';

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
  const [creating, setCreating] = useState(false);

  const createActivity = async (evt : React.FormEvent) => {
    evt.preventDefault();
    setCreating(true);
    const result = await sendCommand('/activities',  {
        name: 'test'
      });
    await router.push(`/activities/${result.aggregateId}`);
  };

  return (<Paper className={classes.paper} component="form" onSubmit={createActivity}>
    <Typography variant="overline">Activity</Typography>
    <TextField id="name" className={classes.input} label="Name" variant="outlined"></TextField>
    <Button variant="outlined" className={classes.button} type="submit" disabled={creating}>Create</Button>
  </Paper>);
}