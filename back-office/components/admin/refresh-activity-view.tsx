import {
  Button,
  CircularProgress,
  Container,
  makeStyles,
  TextField,
  Typography,
} from '@material-ui/core';
import React from 'react';
import { useForm } from 'react-hook-form';
import { useDispatch } from 'react-redux';

import { sendActivityCommand } from '../../store/activities/activities.actions';

interface RefreshActivityViewProps {
  initialActivityId?: string;
}

type FormData = {
  activityId: string;
};

const useStyles = makeStyles((theme) => ({
  root: {
    padding: theme.spacing(0),
  },
  form: {
    display: 'flex',
    flexDirection: 'row',
  },
  input: {
    flex: '1',
  },
  button: {},
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
  },
}));

export function RefreshActivityView({ initialActivityId }: RefreshActivityViewProps) {
  const classes = useStyles();

  const { register, handleSubmit, setValue, errors, formState, setError } = useForm<FormData>({
    defaultValues: {
      activityId: initialActivityId,
    },
  });

  const dispatch = useDispatch();

  const onSubmit = async (data: FormData) => {
    dispatch(sendActivityCommand(`/activities-admin/${data.activityId}/rebuild`, {}));
    setValue('activityId', '');
  };

  return (
    <Container className={classes.root}>
      <Typography variant="overline">Refresh Views</Typography>
      <form className={classes.form} onSubmit={handleSubmit(onSubmit)}>
        <TextField
          name="activityId"
          label="Activity Id"
          variant="outlined"
          className={classes.input}
          inputRef={register({ required: 'Activity Id is required' })}
          disabled={formState.isSubmitting}
          error={!!errors.activityId}
          helperText={errors.activityId && errors.activityId.message}
        />
        <div className={classes.progressWrapper}>
          <Button
            type="submit"
            variant="outlined"
            className={classes.button}
            disabled={formState.isSubmitting}>
            Refresh
          </Button>
          {formState.isSubmitting && (
            <CircularProgress size={24} className={classes.buttonProgress} />
          )}
        </div>
      </form>
    </Container>
  );
}
