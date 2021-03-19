import React, { useEffect } from 'react';
import router from 'next/router';
import { CircularProgress, Container, makeStyles } from '@material-ui/core';
import {
  Typography,
  TextField,
  Button
} from '@material-ui/core';

import { useForm } from 'react-hook-form';
import { useDispatch } from 'react-redux';
import { createOrganization } from '../../store/organizations/organizations.actions';

const useStyles = makeStyles((theme) => ({
  root: {
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


export function CreateOrganization() {
  const classes = useStyles();

  const dispatch = useDispatch();
  
  const { register, handleSubmit, formState } = useForm<FormData>({
    defaultValues: {
    }
  });

  const onSubmit = async (data: FormData) => {
    dispatch(createOrganization(data.name));
  }

  return (<Container className={classes.root}>
    <Typography variant="overline">Organization</Typography>
    <form className={classes.form} onSubmit={handleSubmit(onSubmit)}>
      <TextField name="name" label="Name"
        inputRef={register}
        variant="outlined" className={classes.input}
        disabled={formState.isSubmitting} />

      <div className={classes.progressWrapper}>
        <Button type="submit"
          variant="outlined" className={classes.button}
          disabled={formState.isSubmitting}>
          Create
        </Button>
        {formState.isSubmitting && <CircularProgress size="1.4rem" className={classes.buttonProgress} />}
      </div>
    </form>
  </Container>);
}