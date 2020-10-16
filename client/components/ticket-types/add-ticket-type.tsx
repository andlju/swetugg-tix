import {
  makeStyles,
  TextField,
  Button, Typography, Container
} from "@material-ui/core";
import React from "react";
import { useForm } from "react-hook-form";
import { useCommand } from "../../src/use-command.hook";

interface AddTicketTypeProps {
  activityId: string,
  refreshTicketTypes: (ticketTypeId: string) => void
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

type FormData = {
  ticketTypeName: string
};

export default function AddTicketType({ activityId, refreshTicketTypes }: AddTicketTypeProps) {
  const classes = useStyles();

  const [addTicketType, sending] = useCommand("Add Ticket Type");

  const { register, handleSubmit, setValue, errors, formState } = useForm<FormData>({
    defaultValues: {
    }
  });

  const onSubmit = async (data: FormData) => {
    const res = await addTicketType(`/activities/${activityId}/ticket-types`, {
      name: data.ticketTypeName
    });
    setValue("ticketTypeName", "");
    refreshTicketTypes(res.body.ticketTypeId);
  }

  return (
    <Container className={classes.root}>
      <Typography variant="overline">Add New</Typography>
      <form className={classes.form} onSubmit={handleSubmit(onSubmit)}>
        <TextField name="ticketTypeName" label="Name"
          size="small" className={classes.input}
          inputRef={register}
          disabled={formState.isSubmitting} />
        <Button type="submit" className={classes.button} variant="outlined" color="primary"
          disabled={formState.isSubmitting}>Add</Button>
      </form>
    </Container>
  );
}