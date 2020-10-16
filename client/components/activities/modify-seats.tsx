import { Container, Grid, makeStyles, TextField, Typography } from "@material-ui/core";
import { ActivityDetailsProps } from "./activity.models";

import React from "react";
import { useForm } from "react-hook-form";
import { useCommand } from "../../src/use-command.hook";

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
}))

type FormData = {
  seats: Number
};

export default function ModifySeats({ activity, refreshActivityRevision }: ActivityDetailsProps) {
  const classes = useStyles();

  const increaseForm = useForm<FormData>({
    defaultValues: {
      seats: 0
    }
  });
  const decreaseForm = useForm<FormData>({
    defaultValues: {
      seats: 0
    }
  });

  const [addSeats, sendingAddSeats] = useCommand('Add seats');
  const [removeSeats, sendingRemoveSeats] = useCommand('Remove seats');

  const onSubmitAddSeats = async (data: FormData) => {
    try {
      const seats = +data.seats;
      const result = await addSeats(`/activities/${activity.activityId}/add-seats`, {
        seats: +data.seats
      });
      increaseForm.setValue("seats", 0);
      refreshActivityRevision(result.revision ?? 0);
    } catch (err) {

    }
  }

  const onSubmitRemoveSeats = async (data: FormData) => {
    try {
      const seats = +data.seats;
      const result = await removeSeats(`/activities/${activity.activityId}/remove-seats`, {
        seats: seats
      });
      decreaseForm.setValue("seats", 0);
      refreshActivityRevision(result.revision ?? 0);
    } catch (err) {

    }
  }

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
              disabled={increaseForm.formState.isSubmitting} />
          </form>
        </Grid>
        <Grid item xs={12} md={6}>
          <form className={classes.form} onSubmit={decreaseForm.handleSubmit(onSubmitRemoveSeats)}>
            <TextField
              name="seats" label="Seats to remove"
              type="number"
              inputRef={decreaseForm.register}
              variant="outlined" className={classes.input}
              disabled={decreaseForm.formState.isSubmitting} />
          </form>
        </Grid>
      </Grid>
    </Container>
  )
}