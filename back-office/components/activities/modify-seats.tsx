import { CircularProgress, Container, Grid, InputAdornment, makeStyles, TextField, Typography } from '@material-ui/core';
import React, { useEffect } from 'react';
import { Controller, useForm } from 'react-hook-form';

import { CommandLogSeverity } from '../../src/services/activity-command.service';
import { useActivityCommand } from '../../src/use-activity-command.hook';
import { Activity } from '../../store/activities/activity.models';

const useStyles = makeStyles((theme) => ({
  root: {
    padding: theme.spacing(0),
  },
  form: {
    display: 'flex',
    flexDirection: 'row',
  },
  input: {
    flex: '1',
  },
  button: {},
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
  },
}));

export type ModifySeatsProps = {
  activity: Activity;
};

type FormData = {
  seats: string;
};

export function ModifySeats({ activity }: ModifySeatsProps) {
  const classes = useStyles();

  const increaseForm = useForm<FormData>({
    defaultValues: {
      seats: '',
    },
  });
  const decreaseForm = useForm<FormData>({
    defaultValues: {
      seats: '',
    },
  });

  const [addSeatsCommand, addSeatsStatus, addSeatsState] = useActivityCommand(
    `/activities/${activity.activityId}/add-seats?ownerId=${activity.ownerId}`
  );
  const [removeSeatsCommand, removeSeatsStatus, removeSeatsState] = useActivityCommand(
    `/activities/${activity.activityId}/remove-seats?ownerId=${activity.ownerId}`
  );

  const addSeatsError = 
    addSeatsState?.messages && addSeatsState.messages.find((m) => m.severity === CommandLogSeverity.Error);
  const removeSeatsError =
    removeSeatsState?.messages && removeSeatsState.messages.find((m) => m.severity === CommandLogSeverity.Error);

  const onSubmitAddSeats = async (data: FormData) => {
    const seats = +(data.seats || 0);
    addSeatsCommand({
      seats: seats,
    });
  };

  const onSubmitRemoveSeats = async (data: FormData) => {
    const seats = +(data.seats || 0);
    removeSeatsCommand({
      seats: seats,
    });
  };

  useEffect(() => {
    if (addSeatsStatus.completed) {
      increaseForm.setValue('seats', '');
    }
    if (removeSeatsStatus.completed) {
      decreaseForm.setValue('seats', '');
    }
  }, [addSeatsStatus, removeSeatsStatus]);

  return (
    <Container className={classes.root}>
      <Typography variant="overline">Modify Seats</Typography>
      <Grid container spacing={3}>
        <Grid item xs={12} md={6}>
          <form className={classes.form} onSubmit={increaseForm.handleSubmit(onSubmitAddSeats)}>
            <Controller
              control={increaseForm.control}
              name="seats"
              render={(props) => (
                <TextField
                  {...props}
                  label="Seats to add"
                  type="number"
                  variant="outlined"
                  className={classes.input}
                  disabled={addSeatsStatus.processing}
                  error={addSeatsStatus.failed}
                  helperText={addSeatsError?.message}
                  InputProps={{
                    endAdornment: addSeatsStatus.processing && (
                      <InputAdornment position="end">
                        <CircularProgress color="inherit" size="1.5rem" />
                      </InputAdornment>
                    ),
                  }}
                />
              )}
            />
          </form>
        </Grid>
        <Grid item xs={12} md={6}>
          <form className={classes.form} onSubmit={decreaseForm.handleSubmit(onSubmitRemoveSeats)}>
            <Controller
              control={decreaseForm.control}
              name="seats"
              render={(props) => (
                <TextField
                  {...props}
                  label="Seats to remove"
                  type="number"
                  variant="outlined"
                  className={classes.input}
                  disabled={removeSeatsStatus.processing}
                  error={removeSeatsStatus.failed}
                  helperText={removeSeatsError?.message}
                  InputProps={{
                    endAdornment: removeSeatsStatus.processing && (
                      <InputAdornment position="end">
                        <CircularProgress color="inherit" size="1.5rem" />
                      </InputAdornment>
                    ),
                  }}
                />
              )}
            />
          </form>
        </Grid>
      </Grid>
    </Container>
  );
}
