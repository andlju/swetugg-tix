import {
  makeStyles,
  TextField,
  Button, Typography, Container
} from "@material-ui/core";
import React from "react";
import { useForm } from "react-hook-form";
import { useDispatch } from "react-redux";
import { useActivityCommand } from "../../src/user-activity-command.hook";
import { sendActivityCommand } from "../../store/activities/activities.actions";

interface AddTicketTypeProps {
  activityId: string
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

export function AddTicketType({ activityId }: AddTicketTypeProps) {
  const classes = useStyles();

  const { register, handleSubmit, setValue, formState } = useForm<FormData>({
    defaultValues: {
    }
  });

  const [addTicketTypeCommand, addTicketTypeStatus] = useActivityCommand(`/activities/${activityId}/ticket-types`);

  const onSubmit = async (data: FormData) => {
    addTicketTypeCommand({
      name: data.ticketTypeName
    });
    setValue("ticketTypeName", "");
  }

  return (
    <Container className={classes.root}>
      <Typography variant="overline">Add New</Typography>
      <form className={classes.form} onSubmit={handleSubmit(onSubmit)}>
        <TextField name="ticketTypeName" label="Name"
          size="small" className={classes.input}
          inputRef={register}
          disabled={addTicketTypeStatus.processing} />
        <Button type="submit" className={classes.button} variant="outlined" color="primary"
          disabled={addTicketTypeStatus.processing}>Add</Button>
      </form>
    </Container>
  );
}