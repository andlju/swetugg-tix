import {
  makeStyles,
  TextField,
  Button, Typography, Container
} from "@material-ui/core";
import React from "react";
import { useState } from "react";
import { sendCommand } from "../../src/services/command.service";
import { buildUrl } from "../../src/url-utils";

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


export default function AddTicketType({ activityId, refreshTicketTypes }: AddTicketTypeProps) {
  const classes = useStyles();
  const [creating, setCreating] = useState(false);
  const [ticketTypeName, setTicketTypeName] = useState<string>('');

  const addTicketType = async (evt: React.FormEvent) => {
    evt.preventDefault();
    setTicketTypeName('');
    setCreating(true);
    const res = await sendCommand(`/activities/${activityId}/ticket-types`, {
      name: 'test'
    });
    setCreating(false);
    console.log("Command status", res.body);
    refreshTicketTypes(res.body.ticketTypeId);
  }

  const handleChange = (event: React.ChangeEvent<any>) => {
    setTicketTypeName(event.target.value);
  }

  return (
    <Container className={classes.root}>
      <Typography variant="overline">Add New</Typography>
      <form className={classes.form} onSubmit={addTicketType}>
        <TextField id="name" className={classes.input} label="Name" value={ticketTypeName} size="small" disabled={creating} onChange={handleChange}></TextField>
        <Button type="submit" className={classes.button} variant="outlined" color="primary" disabled={creating}>Add</Button>
      </form>
    </Container>
  );
}