import React, { useEffect, useMemo } from 'react';
import router from 'next/router';
import { CircularProgress, Container, FormControl, InputLabel, makeStyles, MenuItem, Select } from '@material-ui/core';
import { Autocomplete } from '@material-ui/lab';
import {
  Typography,
  TextField,
  Input,
  Button
} from '@material-ui/core';

import { useForm, Controller } from 'react-hook-form';
import { useActivityCommand } from '../../src/use-activity-command.hook';
import { User } from '../../store/auth/auth.actions';
import { Organization } from '../../store/organizations/organizations.actions';

const useStyles = makeStyles((theme) => ({
  root: {
    padding: theme.spacing(0)
  },
  form: {
    display: 'flex',
    flexDirection: 'row',
  },
  input: {
    margin: theme.spacing(1),
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
  activityName: string;
  ownerId: string;
};

interface CreateActivityProps {
  user: User;
  organizations: Organization[];
}

export function CreateActivity({ user, organizations }: CreateActivityProps) {
  const classes = useStyles();

  const { handleSubmit, formState, setValue, control } = useForm<FormData>({
    defaultValues: {
      activityName: '',
      ownerId: ''
    }
  });

  const [createActivityCommand, createActivityStatus, createActivityState] = useActivityCommand(`/activities`);

  const onSubmit = async (data: FormData) => {
    if (data.ownerId) {
      createActivityCommand({
        name: data.activityName,
        ownerId: data.ownerId
      });
    }
  };

  const options = useMemo(() => ([{
    ownerId: user.userId, name: user.name
  },
  ...organizations.map(o => ({
    ownerId: o.organizationId, name: o.name
  }))]),
    [user, organizations]);

  useEffect(() => {
    if (user.userId) {
      setValue("ownerId", user.userId);
    }
  }, [user.userId]);

  useEffect(() => {
    if (createActivityState?.aggregateId) {
      const activityOwnerId = (createActivityState.body as any).ownerId as string;
      router.push(`/activities/${createActivityState?.aggregateId}?ownerId=${activityOwnerId}`);
    }
  }, [createActivityState?.aggregateId]);

  return (<Container className={classes.root}>
    <Typography variant="overline">Activity</Typography>
    <form className={classes.form} onSubmit={handleSubmit(onSubmit)}>
      {options[0] &&
        <Controller
          control={control}
          name="ownerId"
          render={(props) => (
            <FormControl className={classes.input} variant="outlined">
              <InputLabel id="select-ownerid-label">Organization</InputLabel>
              <Select
                {...props}
                labelId="select-ownerid-label"
                label="Organization">
                {options.map(o => <MenuItem key={o.ownerId} value={o.ownerId}>{o.name}</MenuItem>)}
              </Select>
            </FormControl>
          )} />
      }
      <Controller
        control={control}
        name="activityName"
        render={(props) => (<TextField
          {...props}
          label="Name"
          variant="outlined"
          className={classes.input}
          disabled={formState.isSubmitting} />)}
      />
      <Controller
        control={control}
        name="ownerId"
        render={(props) => (<Input
          {...props}
          type="hidden" />)}
      />

      <div className={classes.progressWrapper}>
        <Button type="submit"
          variant="outlined" className={classes.button}
          disabled={formState.isSubmitting}>
          Create
        </Button>
        {createActivityStatus.processing && <CircularProgress size="1.4rem" className={classes.buttonProgress} />}
      </div>
    </form>
  </Container>);
}