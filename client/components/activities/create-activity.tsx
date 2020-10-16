import React, { useState } from 'react';
import router from 'next/router';
import { Container, makeStyles } from '@material-ui/core';
import {
  Typography,
  TextField,
  Button
} from '@material-ui/core';

import { sendCommand } from '../../src/services/command.service';
import { useForm } from 'react-hook-form';
import { useSnackbar } from 'notistack';
import { useCommand } from '../../src/use-command.hook';

interface CreateActivityProps {

}

const useStyles = makeStyles((theme) => ({
  root: {
    padding: theme.spacing(0)
  },
  form: {
    display: 'flex',
    flexDirection: 'row',
  },
  input: {
    flex: '1',
  },
  button: {
    marginLeft: theme.spacing(2)
  }
}));

type FormData = {
  activityName: String
};


export default function CreateActivity({ }: CreateActivityProps) {
  const classes = useStyles();

  const { register, handleSubmit, setValue, errors, formState } = useForm<FormData>({
    defaultValues: {
    }
  });
  const [ createActivity, sending ] = useCommand('Create activity');
  const onSubmit = async (data: FormData) => {
    try {
      const result = await createActivity(`/activities`, {
        name: data.activityName
      });
      await router.push(`/activities/${result.aggregateId}`);
    } catch(err) {

    }
  }

  return (<Container className={classes.root}>
    <Typography variant="overline">Activity</Typography>
    <form className={classes.form} onSubmit={handleSubmit(onSubmit)}>
      <TextField name="activityName" label="Name"
        inputRef={register}
        variant="outlined" className={classes.input}
        disabled={formState.isSubmitting} />
      <Button type="submit"
        variant="outlined" className={classes.button}
        disabled={formState.isSubmitting}>
        Create
      </Button>
    </form>
  </Container>);
}