import React from 'react';
import { Container, makeStyles } from '@material-ui/core';
import {
  Typography,
  TextField,
} from '@material-ui/core';

import { UseFormMethods } from 'react-hook-form';

const useStyles = makeStyles((theme) => ({
  root: {
    display: 'flex',
    flexDirection: 'column',
    padding: theme.spacing(0)
  },
  input: {
    flex: '1',
  }
}));

export type UserFormData = {
  name: string;
};

export interface NewProfileProps {
  userForm: UseFormMethods<UserFormData>,
}

export const NewProfile: React.FC<NewProfileProps> = ({ userForm }) => {
  const classes = useStyles();

  const { register, formState } = userForm;

  return (<Container className={classes.root}>
      <Typography variant="overline">Profile</Typography>
      <TextField name="name" label="Name"
        inputRef={register}
        variant="outlined" className={classes.input}
        disabled={formState.isSubmitting} />
  </Container>);
}
