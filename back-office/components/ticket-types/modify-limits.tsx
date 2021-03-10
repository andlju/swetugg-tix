import {
  makeStyles,
  TextField,
  Button, IconButton, Typography, Container, Grid, Box, InputAdornment, CircularProgress
} from "@material-ui/core";
import React from "react";
import { useForm } from "react-hook-form";
import { useDispatch } from "react-redux";
import { CommandLogSeverity } from "../../src/services/activity-command.service";
import { useActivityCommand } from "../../src/user-activity-command.hook";
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

  const [increaseLimitCommand, increaseLimitStatus, increaseLimitState] = useActivityCommand(`/activities/${ticketType.activityId}/ticket-types/${ticketType.ticketTypeId}/increase-limit`);
  const [decreaseLimitCommand, decreaseLimitStatus, decreaseLimitState] = useActivityCommand(`/activities/${ticketType.activityId}/ticket-types/${ticketType.ticketTypeId}/decrease-limit`);
  const [removeLimitCommand, removeLimitStatus, removeLimitState] = useActivityCommand(`/activities/${ticketType.activityId}/ticket-types/${ticketType.ticketTypeId}/limit`, { method: "DELETE"});

  const increaseLimitError = increaseLimitState?.messages && increaseLimitState.messages.find(m => m.severity === CommandLogSeverity.Error);
  const decreaseLimitError = decreaseLimitState?.messages && decreaseLimitState.messages.find(m => m.severity === CommandLogSeverity.Error);
  const removeLimitError = removeLimitState?.messages && removeLimitState.messages.find(m => m.severity === CommandLogSeverity.Error);

  const onSubmitIncreaseLimit = async (data: LimitFormData) => {
    increaseLimitCommand({
      seats: +(data.seats || 0)
    });
    increaseLimitForm.setValue("seats", undefined);
  };

  const onSubmitDecreaseLimit = async (data: LimitFormData) => {
    decreaseLimitCommand({
      seats: +(data.seats || 0)
    });
    decreaseLimitForm.setValue("seats", undefined);
  };

  const onClickRemoveLimit = async () => {
    removeLimitCommand({});
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
              disabled={increaseLimitStatus.processing} 
              error={increaseLimitStatus.failed}
              helperText={increaseLimitError?.message}
              InputProps={{
                endAdornment: increaseLimitStatus.processing && <InputAdornment position="end"><CircularProgress color="inherit" size="1.5rem" /></InputAdornment>
              }}/>
          </form>
        </Grid>
        <Grid item xs={12} md={4}>
          <form className={classes.form} onSubmit={decreaseLimitForm.handleSubmit(onSubmitDecreaseLimit)}>
            <TextField name="seats" label="Decrease limit" type="number"
              className={classes.input} variant="outlined"
              inputRef={decreaseLimitForm.register}
              disabled={decreaseLimitStatus.processing} 
              error={decreaseLimitStatus.failed}
              helperText={decreaseLimitError?.message}
              InputProps={{
                endAdornment: decreaseLimitStatus.processing && <InputAdornment position="end"><CircularProgress color="inherit" size="1.5rem" /></InputAdornment>
              }}/>
          </form>
        </Grid>
        <Grid item xs={12} md={4} className={classes.buttonContainer}>
          <Button
            className={classes.button} color="secondary" variant="contained"
            size="medium"
            disabled={ticketType.limit == undefined}
            onClick={onClickRemoveLimit}>Remove Limit</Button>
        </Grid>
      </Grid>
    </Container>
  );
}