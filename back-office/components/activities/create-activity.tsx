import React, { useEffect } from 'react';
import router from 'next/router';
import { CircularProgress, Container, makeStyles } from '@material-ui/core';
import {
  Typography,
  TextField,
  Input,
  Button
} from '@material-ui/core';

import { useForm, Controller } from 'react-hook-form';
import { useActivityCommand } from '../../src/user-activity-command.hook';
import { User } from '../../store/auth/auth.actions';

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
  activityName: string;
  ownerId: string;
};

interface CreateActivityProps {
  user: User;
}

export function CreateActivity({ user }: CreateActivityProps) {
  const classes = useStyles();

  const { handleSubmit, formState, setValue, control } = useForm<FormData>({
    defaultValues: {
      activityName: '',
      ownerId: ''
    }
  });

  const [createActivityCommand, createActivityStatus, createActivityState] = useActivityCommand(`/activities`);

  const onSubmit = async (data: FormData) => {
    if (data.ownerId) {
      createActivityCommand({
        name: data.activityName,
        ownerId: data.ownerId
      });
    }
  };

  useEffect(() => {
    if (user.userId) {
      setValue("ownerId", user.userId);
    }
  }, [user.userId]);

  useEffect(() => {
    if (createActivityState?.aggregateId) {
      router.push(`/activities/${createActivityState?.aggregateId}`);
    }
  }, [createActivityState?.aggregateId]);

  return (<Container className={classes.root}>
    <Typography variant="overline">Activity</Typography>
    <form className={classes.form} onSubmit={handleSubmit(onSubmit)}>
      <Controller 
        control={control}
        name="activityName" 
        render={(props) => (<TextField 
          {...props}
          label="Name"
          variant="outlined"
          className={classes.input}
          disabled={formState.isSubmitting} />)}
      />
      <Controller 
        control={control}
        name="ownerId"
        render={(props) => (<Input 
          {...props} 
          type="hidden" />)}
      />
      
      <div className={classes.progressWrapper}>
        <Button type="submit"
          variant="outlined" className={classes.button}
          disabled={formState.isSubmitting}>
          Create
        </Button>
        {createActivityStatus.processing && <CircularProgress size="1.4rem" className={classes.buttonProgress} />}
      </div>
    </form>
  </Container>);
}