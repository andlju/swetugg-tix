import React, { useEffect } from 'react';
import router from 'next/router';
import { CircularProgress, Container, makeStyles } from '@material-ui/core';
import {
  Typography,
  TextField,
  Button
} from '@material-ui/core';

import { Controller, useForm } from 'react-hook-form';
import { useDispatch, useSelector } from 'react-redux';
import { createOrganization } from '../../store/organizations/organizations.actions';
import { RootState } from '../../store/store';

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
  const { editState } = useSelector((r: RootState) => r.organizations);

  const { handleSubmit, setValue, control } = useForm<FormData>({
    defaultValues: {
      name: ''
    }
  });

  const onSubmit = async (data: FormData) => {
    dispatch(createOrganization(data.name));
  };

  useEffect(() => {
    if (!editState.saving && !editState.errorCode) {
      setValue("name", '');
    }
  }, [editState.saving]);

  return (<Container className={classes.root}>
    <Typography variant="overline">Organization</Typography>
    <form className={classes.form} onSubmit={handleSubmit(onSubmit)}>
      <Controller
        control={control}
        name="name"
        render={(props) => (
          <TextField
            {...props}
            label="Name"
            variant="outlined" className={classes.input}
            disabled={editState.saving}
            error={!!editState.errorCode}
            helperText={editState.errorMessage}
            />
        )}
      />

      <div className={classes.progressWrapper}>
        <Button type="submit"
          variant="outlined" className={classes.button}
          disabled={editState.saving}>
          Create
        </Button>
        {editState.saving && <CircularProgress size="1.4rem" className={classes.buttonProgress} />}
      </div>
    </form>
  </Container>);
}