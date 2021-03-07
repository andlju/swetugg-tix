import {
  makeStyles,
  TextField,
  Button, Typography, Container
} from "@material-ui/core";
import React from "react";
import { useForm } from "react-hook-form";
import { useDispatch } from "react-redux";
import { sendActivityCommand } from "../activities/store/activities.actions";

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

  const dispatch = useDispatch();

  const { register, handleSubmit, setValue, formState } = useForm<FormData>({
    defaultValues: {
    }
  });

  const onSubmit = async (data: FormData) => {
    dispatch(sendActivityCommand(`/activities/${activityId}/ticket-types`, {
      name: data.ticketTypeName
    }));
    setValue("ticketTypeName", "");
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