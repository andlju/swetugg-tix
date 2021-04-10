import { Button, Container, makeStyles, TextField, Typography } from '@material-ui/core';
import React from 'react';
import { Controller, useForm } from 'react-hook-form';

import { useActivityCommand } from '../../src/use-activity-command.hook';

interface AddTicketTypeProps {
  activityId: string;
  ownerId: string;
}

const useStyles = makeStyles((theme) => ({
  root: {
    marginTop: theme.spacing(4),
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
    marginLeft: theme.spacing(2),
  },
}));

type FormData = {
  ticketTypeName: string;
};

export function AddTicketType({ activityId, ownerId }: AddTicketTypeProps) {
  const classes = useStyles();

  const { register, handleSubmit, setValue, control } = useForm<FormData>({
    defaultValues: {
      ticketTypeName: '',
    },
  });

  const [addTicketTypeCommand, addTicketTypeStatus] = useActivityCommand(
    `/activities/${activityId}/ticket-types?ownerId=${ownerId}`
  );

  const onSubmit = async (data: FormData) => {
    addTicketTypeCommand({
      name: data.ticketTypeName,
    });
    setValue('ticketTypeName', '');
  };

  return (
    <Container className={classes.root}>
      <Typography variant="overline">Add New</Typography>
      <form className={classes.form} onSubmit={handleSubmit(onSubmit)}>
        <Controller
          control={control}
          name="ticketTypeName"
          render={({ field }) => (
            <TextField {...field} label="Name" size="small" className={classes.input} disabled={addTicketTypeStatus.processing} />
          )}
        />

        <Button
          type="submit"
          className={classes.button}
          variant="outlined"
          color="primary"
          disabled={addTicketTypeStatus.processing}>
          Add
        </Button>
      </form>
    </Container>
  );
}
