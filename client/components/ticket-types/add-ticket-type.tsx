import {
  makeStyles,
  TextField,
  Button, Typography, Container
} from "@material-ui/core";
import React from "react";
import { useState } from "react";
import { buildUrl } from "../../src/url-utils";

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


export default function AddTicketType({ activityId }: AddTicketTypeProps) {
  const classes = useStyles();
  const [ticketTypeName, setTicketTypeName] = useState<string>('');

  const addTicketType = async (evt: React.FormEvent) => {
    evt.preventDefault();
    const res = await fetch(buildUrl(`/activities/${activityId}/ticket-types`), {
      method: "POST",
      body: JSON.stringify({
        name: 'test'
      }),
      headers: {
        'Content-Type': 'application/json'
      }
    });
    const result = await res.json();
    setTicketTypeName('');
    console.log(result);
  }

  const handleChange = (event: React.ChangeEvent<any>) => {
    setTicketTypeName(event.target.value);
  }

  return (
    <Container className={classes.root}>
      <Typography variant="overline">Add New</Typography>
      <form className={classes.form} onSubmit={addTicketType}>
        <TextField id="name" className={classes.input} label="Name" value={ticketTypeName} size="small" onChange={handleChange}></TextField>
        <Button type="submit" className={classes.button} variant="outlined" color="primary">Add</Button>
      </form>
    </Container>
  );
}