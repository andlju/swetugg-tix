import React, { useEffect } from 'react';
import { CircularProgress, Container, makeStyles } from '@material-ui/core';
import {
  Typography,
  TextField,
  Button
} from '@material-ui/core';

import { useForm } from 'react-hook-form';
import { updateUser, User } from '../../store/auth/auth.actions';
import { useDispatch } from 'react-redux';

const useStyles = makeStyles((theme) => ({
  root: {
    padding: theme.spacing(0)
  },
  form: {
    display: 'flex',
    flexDirection: 'column',
  },
  input: {
    flex: '1',
  },
  button: {

  },
  progressWrapper: {
    margin: theme.spacing(1),
    position: 'relative',
  },
  buttonProgress: {
    color: theme.palette.action.active,
    position: 'absolute',
    top: '50%',
    left: '50%',
    marginTop: -12,
    marginLeft: -12,
  }
}));

type FormData = {
  name: string;
};

export interface EditProfileProps {
  user: User
}

export function EditProfile({ user }: EditProfileProps) {
  const classes = useStyles();

  const dispatch = useDispatch();

  const { register, handleSubmit, formState } = useForm<FormData>({
    defaultValues: {
      name: user?.name || ''
    }
  });

  const onSubmit = async (data: FormData) => {
    console.log("Updating user profile", data);
    user.name = data.name;
    dispatch(updateUser(user));
  };

  return (<Container className={classes.root}>
    <Typography variant="overline">Profile</Typography>
    <form className={classes.form} onSubmit={handleSubmit(onSubmit)}>
      <TextField name="name" label="Name"
        inputRef={register}
        variant="outlined" className={classes.input}
        disabled={formState.isSubmitting} />

      <div className={classes.progressWrapper}>
        <Button type="submit"
          variant="outlined" className={classes.button}
          disabled={formState.isSubmitting}>
          Update
        </Button>
        {
        //createActivityStatus.processing && <CircularProgress size="1.4rem" className={classes.buttonProgress} />
        }
      </div>
    </form>
  </Container>);
}