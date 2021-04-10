import { Container, makeStyles } from '@material-ui/core';
import { TextField, Typography } from '@material-ui/core';
import React from 'react';
import { Control, Controller, FormState } from 'react-hook-form';

const useStyles = makeStyles((theme) => ({
  root: {
    display: 'flex',
    flexDirection: 'column',
    padding: theme.spacing(0),
  },
  input: {
    flex: '1',
  },
}));

export type UserFormData = {
  name: string;
};

export interface EditProfileProps {
  userForm: { formState: FormState<UserFormData>; control: Control<UserFormData> };
}

export const EditProfile: React.FC<EditProfileProps> = ({ userForm }) => {
  const classes = useStyles();

  const { formState, control } = userForm;

  return (
    <Container className={classes.root}>
      <Typography variant="overline">Profile</Typography>
      <Controller
        control={control}
        name="name"
        render={({ field }) => (
          <TextField
            {...field}
            name="name"
            label="Name"
            variant="outlined"
            className={classes.input}
            disabled={formState.isSubmitting}
          />
        )}
      />
    </Container>
  );
};
