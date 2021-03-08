import {
  makeStyles,
  TextField,
  Button, IconButton, Typography, Container, Grid, Box
} from "@material-ui/core";
import React from "react";
import { useForm } from "react-hook-form";
import { useDispatch } from "react-redux";
import { sendActivityCommand } from "../../store/activities/activities.actions";
import { TicketType } from "./ticket-type.models";

interface EditTicketTypeProps {
  ticketType: TicketType;
}

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
  buttonContainer: {
    verticalAlign: 'middle',
    display: 'flex',
    flexDirection: 'column',
    justifyContent: 'center'
  },
  button: {

  }
}));

type LimitFormData = {
  seats?: number;
};

export function ModifyLimits({ ticketType }: EditTicketTypeProps) {
  const classes = useStyles();

  const dispatch = useDispatch();

  const increaseLimitForm = useForm<LimitFormData>({
    defaultValues: {

    }
  });

  const decreaseLimitForm = useForm<LimitFormData>({
    defaultValues: {

    }
  });

  const onSubmitIncreaseLimit = async (data: LimitFormData) => {
    dispatch(sendActivityCommand(`/activities/${ticketType.activityId}/ticket-types/${ticketType.ticketTypeId}/increase-limit`, {
      seats: +(data.seats || 0)
    }));
    increaseLimitForm.setValue("seats", undefined);
  };

  const onSubmitDecreaseLimit = async (data: LimitFormData) => {
    dispatch(sendActivityCommand(`/activities/${ticketType.activityId}/ticket-types/${ticketType.ticketTypeId}/decrease-limit`, {
      seats: +(data.seats || 0)
    }));
    decreaseLimitForm.setValue("seats", undefined);
  };

  const onClickRemoveLimit = async () => {
    dispatch(sendActivityCommand(`/activities/${ticketType.activityId}/ticket-types/${ticketType.ticketTypeId}/limit`, {

    }));
  };

  return (
    <Container className={classes.root}>
      <Typography variant="overline">Limits</Typography>
      <Grid container spacing={2}>
        <Grid item xs={12} md={4}>
          <form className={classes.form} onSubmit={increaseLimitForm.handleSubmit(onSubmitIncreaseLimit)}>
            <TextField name="seats" label="Increase limit" type="number"
              className={classes.input} variant="outlined"
              inputRef={increaseLimitForm.register}
              disabled={increaseLimitForm.formState.isSubmitting} />
          </form>
        </Grid>
        <Grid item xs={12} md={4}>
          <form className={classes.form} onSubmit={decreaseLimitForm.handleSubmit(onSubmitDecreaseLimit)}>
            <TextField name="seats" label="Decrease limit" type="number"
              className={classes.input} variant="outlined"
              inputRef={decreaseLimitForm.register}
              disabled={decreaseLimitForm.formState.isSubmitting} />
          </form>
        </Grid>
        <Grid item xs={12} md={4} className={classes.buttonContainer}>
          <Button
            className={classes.button} color="secondary" variant="contained"
            size="medium"
            disabled={!ticketType.limit}
            onClick={onClickRemoveLimit}>Remove Limit</Button>
        </Grid>
      </Grid>
    </Container>
  );
}