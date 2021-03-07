import React from 'react';
import router from 'next/router';
import { CircularProgress, Container, makeStyles } from '@material-ui/core';
import {
  Typography,
  TextField,
  Button
} from '@material-ui/core';

import { useForm } from 'react-hook-form';
import { useActivityCommand } from '../../src/use-activity-command.hook';
import { sendActivityCommand } from './store/activities.actions';
import { useDispatch } from 'react-redux';

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

  },
  progressWrapper: {
    margin: theme.spacing(1),
    position: 'relative',
  },
  buttonProgress: {
    color: theme.palette.action.active,
    position: 'absolute',
    top: '50%',
    left: '50%',
    marginTop: -12,
    marginLeft: -12,
  }
}));

type FormData = {
  activityName: string
};


export function CreateActivity() {
  const classes = useStyles();

  const { register, handleSubmit, formState } = useForm<FormData>({
    defaultValues: {
    }
  });
  const dispatch = useDispatch();

  // const [createActivity] = useActivityCommand('Create activity');
  const onSubmit = async (data: FormData) => {
    try {
      dispatch(sendActivityCommand(`/activities`, {
        name: data.activityName
      }));
    } catch (err) {
      // Report error?
    }
  }

  return (<Container className={classes.root}>
    <Typography variant="overline">Activity</Typography>
    <form className={classes.form} onSubmit={handleSubmit(onSubmit)}>
      <TextField name="activityName" label="Name"
        inputRef={register}
        variant="outlined" className={classes.input}
        disabled={formState.isSubmitting} />

      <div className={classes.progressWrapper}>
        <Button type="submit"
          variant="outlined" className={classes.button}
          disabled={formState.isSubmitting}>
          Create
        </Button>
        {formState.isSubmitting && <CircularProgress size={24} className={classes.buttonProgress} />}
      </div>
    </form>
  </Container>);
}