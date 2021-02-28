import {
  makeStyles,
  TextField,
  Button, IconButton, Typography, Container, Grid
} from "@material-ui/core";
import CheckIcon from "@material-ui/icons/Check";
import React from "react";
import { useForm } from "react-hook-form";
import { useDispatch } from "react-redux";
import { useActivityCommand } from "../../src/use-activity-command.hook";
import { loadActivity } from "../activities/store/activities.actions";
import { TicketType } from "./ticket-type.models";

interface EditTicketTypeProps {
  ticketType: TicketType
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
  button: {
    // marginLeft: theme.spacing(2)
  },
  limitButton: {

  },
}));

type LimitFormData = {
  seats: number
};

export function ModifyLimits({ ticketType }: EditTicketTypeProps) {
  const classes = useStyles();

  const [increaseLimit] = useActivityCommand("Increase Ticket Type Limit");
  const [decreaseLimit] = useActivityCommand("Decrease Ticket Type Limit");
  const [removeLimit] = useActivityCommand("Remove Ticket Type Limit", { method: "DELETE" });

  const dispatch = useDispatch();
  
  const increaseLimitForm = useForm<LimitFormData>({
    defaultValues: {
      seats: 0
    }
  });

  const decreaseLimitForm = useForm<LimitFormData>({
    defaultValues: {
      seats: 0
    }
  });

  const onSubmitIncreaseLimit = async (data: LimitFormData) => {
    const res = await increaseLimit(`/activities/${ticketType.activityId}/ticket-types/${ticketType.ticketTypeId}/increase-limit`, {
      seats: +data.seats
    });
    increaseLimitForm.setValue("seats", 0);
    dispatch(loadActivity(ticketType.activityId, res.revision));
  }

  const onSubmitDecreaseLimit = async (data: LimitFormData) => {
    const res = await decreaseLimit(`/activities/${ticketType.activityId}/ticket-types/${ticketType.ticketTypeId}/decrease-limit`, {
      seats: +data.seats
    });
    decreaseLimitForm.setValue("seats", 0);
    dispatch(loadActivity(ticketType.activityId, res.revision));
  }

  const onClickRemoveLimit = async () => {
    const res = await removeLimit(`/activities/${ticketType.activityId}/ticket-types/${ticketType.ticketTypeId}/limit`, {

    });
    dispatch(loadActivity(ticketType.activityId, res.revision));
  }

  return (
    <Container className={classes.root}>
      <Typography variant="overline">Limits</Typography>
      <Grid container spacing={2}>
        <Grid item xs={12} md={4}>
          <form className={classes.form} onSubmit={increaseLimitForm.handleSubmit(onSubmitIncreaseLimit)}>
            <TextField name="seats" label="Increase limit" type="number"
              size="small" className={classes.input}
              inputRef={increaseLimitForm.register}
              disabled={increaseLimitForm.formState.isSubmitting} />
              <IconButton
                type="submit"
                className={classes.limitButton} color="primary">
                  <CheckIcon />
              </IconButton>
          </form>
        </Grid>
        <Grid item xs={12} md={4}>
          <form className={classes.form} onSubmit={decreaseLimitForm.handleSubmit(onSubmitDecreaseLimit)}>
            <TextField name="seats" label="Decrease limit" type="number"
              size="small" className={classes.input}
              inputRef={decreaseLimitForm.register}
              disabled={decreaseLimitForm.formState.isSubmitting} />
              <IconButton
                type="submit"
                className={classes.limitButton} color="primary">
                  <CheckIcon />
              </IconButton>
          </form>
        </Grid>
        <Grid item xs={12} md={4}>
          <Button 
            className={classes.button} color="secondary" variant="contained"
            disabled={!ticketType.limit}
            onClick={onClickRemoveLimit}>Remove Limit</Button>
        </Grid>
      </Grid>
    </Container>
  );
}