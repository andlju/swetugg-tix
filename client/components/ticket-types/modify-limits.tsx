import {
  makeStyles,
  TextField,
  Button, Typography, Container, Grid
} from "@material-ui/core";
import React from "react";
import { useForm } from "react-hook-form";
import { useCommand } from "../../src/use-command.hook";
import { TicketType } from "./ticket-type.models";

interface EditTicketTypeProps {
  ticketType: TicketType,
  refreshTicketTypesRevision: (revision: number) => void
}

const useStyles = makeStyles((theme) => ({
  root: {
    marginTop: theme.spacing(4),
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

type LimitFormData = {
  seats: number
};

export default function ModifyLimits({ ticketType, refreshTicketTypesRevision }: EditTicketTypeProps) {
  const classes = useStyles();

  const [increaseLimit, sendingIncreaseLimit] = useCommand("Increase Ticket Type Limit");
  const [decreaseLimit, sendingDecreaseLimit] = useCommand("Decrease Ticket Type Limit");
  const [removeLimit, sendingRemoveLimit] = useCommand("Remove Ticket Type Limit");

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
    refreshTicketTypesRevision(res.revision ?? 0);
  }

  const onSubmitDecreaseLimit = async (data: LimitFormData) => {
    const res = await decreaseLimit(`/activities/${ticketType.activityId}/ticket-types/${ticketType.ticketTypeId}/decrease-limit`, {
      seats: +data.seats
    });
    decreaseLimitForm.setValue("seats", 0);
    refreshTicketTypesRevision(res.revision ?? 0);
  }

  return (
    <Container className={classes.root}>
      <Typography variant="overline">Limits</Typography>
      <Grid container spacing={2}>
        <Grid item xs={12} md={6}>
          <form className={classes.form} onSubmit={increaseLimitForm.handleSubmit(onSubmitIncreaseLimit)}>
            <TextField name="seats" label="Increase limit" type="number"
              size="small" className={classes.input}
              inputRef={increaseLimitForm.register}
              disabled={increaseLimitForm.formState.isSubmitting} />
          </form>
        </Grid>
        <Grid item xs={12} md={6}>
          <form className={classes.form} onSubmit={decreaseLimitForm.handleSubmit(onSubmitDecreaseLimit)}>
            <TextField name="seats" label="Decrease limit" type="number"
              size="small" className={classes.input}
              inputRef={decreaseLimitForm.register}
              disabled={decreaseLimitForm.formState.isSubmitting} />
          </form>
        </Grid>
      </Grid>
    </Container>
  );
}