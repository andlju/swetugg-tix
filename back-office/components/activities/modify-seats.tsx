import { CircularProgress, Container, Grid, InputAdornment, makeStyles, TextField, Typography } from "@material-ui/core";
import { Activity } from "./activity.models";

import React, { useEffect } from "react";
import { useForm } from "react-hook-form";
import { useActivityCommand } from "../../src/user-activity-command.hook";
import { CommandLogSeverity } from "../../src/services/activity-command.service";

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

export type ModifySeatsProps = {
  activity: Activity;
};

type FormData = {
  seats?: number;
};

export function ModifySeats({ activity }: ModifySeatsProps) {
  const classes = useStyles();

  const increaseForm = useForm<FormData>({
    defaultValues: {

    }
  });
  const decreaseForm = useForm<FormData>({
    defaultValues: {
    }
  });

  const [addSeatsCommand, addSeatsStatus, addSeatsState] = useActivityCommand(`/activities/${activity.activityId}/add-seats`);
  const [removeSeatsCommand, removeSeatsStatus, removeSeatsState] = useActivityCommand(`/activities/${activity.activityId}/remove-seats`);

  const addSeatsError = addSeatsState?.messages && addSeatsState.messages.find(m => m.severity === CommandLogSeverity.Error);
  const removeSeatsError = removeSeatsState?.messages && removeSeatsState.messages.find(m => m.severity === CommandLogSeverity.Error);

  const onSubmitAddSeats = async (data: FormData) => {
    const seats = +(data.seats || 0);
    addSeatsCommand({
      seats: seats
    });
  };

  const onSubmitRemoveSeats = async (data: FormData) => {
    const seats = +(data.seats || 0);
    removeSeatsCommand({
      seats: seats
    });
  };

  useEffect(() => {
    if (addSeatsStatus.completed) {
      increaseForm.setValue("seats", undefined);
    }
    if (removeSeatsStatus.completed) {
      decreaseForm.setValue("seats", undefined);
    }
  }, [addSeatsStatus, removeSeatsStatus]);

  return (
    <Container className={classes.root}>
      <Typography variant="overline">
        Modify Seats
      </Typography>
      <Grid container spacing={3}>
        <Grid item xs={12} md={6}>
          <form className={classes.form} onSubmit={increaseForm.handleSubmit(onSubmitAddSeats)}>
            <TextField
              name="seats" label="Seats to add"
              type="number"
              inputRef={increaseForm.register}
              variant="outlined" className={classes.input}
              disabled={addSeatsStatus.processing}
              error={addSeatsStatus.failed}
              helperText={addSeatsError?.message}
              InputProps={{
                endAdornment: addSeatsStatus.processing && <InputAdornment position="end"><CircularProgress color="inherit" size="1.5rem" /></InputAdornment>
              }} />
          </form>
        </Grid>
        <Grid item xs={12} md={6}>
          <form className={classes.form} onSubmit={decreaseForm.handleSubmit(onSubmitRemoveSeats)}>
            <TextField
              name="seats" label="Seats to remove"
              type="number"
              inputRef={decreaseForm.register}
              variant="outlined" className={classes.input}
              disabled={removeSeatsStatus.processing}
              error={removeSeatsStatus.failed}
              helperText={removeSeatsError?.message}
              InputProps={{
                endAdornment: removeSeatsStatus.processing && <InputAdornment position="end"><CircularProgress color="inherit" size="1.5rem" /></InputAdornment>
              }} />
          </form>
        </Grid>
      </Grid>
    </Container>
  );
}