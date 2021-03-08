import { Container, Grid, makeStyles, TextField, Typography } from "@material-ui/core";
import { Activity } from "./activity.models";

import React from "react";
import { useForm } from "react-hook-form";
import { useDispatch } from "react-redux";
import { sendActivityCommand } from "../../store/activities/activities.actions";
import { useActivityCommand } from "../../src/user-activity-command.hook";

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
  activity: Activity
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

  const [ addSeatsCommand, addSeatsState ] = useActivityCommand(`/activities/${activity.activityId}/add-seats`);
  const [ removeSeatsCommand, removeSeatsState ] = useActivityCommand(`/activities/${activity.activityId}/remove-seats`);

  const onSubmitAddSeats = async (data: FormData) => {
    const seats = +(data.seats || 0);
    addSeatsCommand({
      seats: seats
    });
    increaseForm.setValue("seats", undefined);
  };

  const onSubmitRemoveSeats = async (data: FormData) => {
      const seats = +(data.seats || 0);
      removeSeatsCommand({
        seats: seats
      });
  
      decreaseForm.setValue("seats", undefined);
  };

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
              disabled={increaseForm.formState.isSubmitting || addSeatsState?.isProcessing || false} />
          </form>
        </Grid>
        <Grid item xs={12} md={6}>
          <form className={classes.form} onSubmit={decreaseForm.handleSubmit(onSubmitRemoveSeats)}>
            <TextField
              name="seats" label="Seats to remove"
              type="number"
              inputRef={decreaseForm.register}
              variant="outlined" className={classes.input}
              disabled={decreaseForm.formState.isSubmitting || removeSeatsState?.isProcessing || false} />
          </form>
        </Grid>
      </Grid>
    </Container>
  );
}