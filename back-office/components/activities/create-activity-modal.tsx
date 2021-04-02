import {
  Button,
  CircularProgress,
  Dialog,
  DialogActions,
  DialogContent,
  DialogContentText,
  DialogTitle,
  FormControl,
  Grid,
  InputLabel,
  MenuItem,
  Select,
  TextField,
} from '@material-ui/core';
import { makeStyles } from '@material-ui/core/styles';
import router from 'next/router';
import React, { useEffect, useMemo } from 'react';
import { Controller, useForm } from 'react-hook-form';

import { useActivityCommand } from '../../src/use-activity-command.hook';
import { User } from '../../store/auth/auth.actions';
import { Organization } from '../../store/organizations/organizations.actions';

const useStyles = makeStyles((theme) => ({
  form: {
    display: 'flex',
    flexDirection: 'column',
  },
  wrapper: {
    margin: theme.spacing(1),
    position: 'relative',
  },
  input: {
    flex: 1,
  },
  cancelButton: {
    marginRight: 'auto',
  },
  saveButton: {},
  buttonProgress: {
    position: 'absolute',
    top: '50%',
    left: '50%',
    marginTop: -12,
    marginLeft: -12,
  },
}));

type FormData = {
  activityName: string;
  ownerId: string;
};

interface CreateActivityProps {
  open: boolean;
  setOpen: (open: boolean) => void;
  user: User;
  organizations: Organization[];
}

export function CreateActivityModal({ user, organizations, open, setOpen }: CreateActivityProps) {
  const classes = useStyles();

  const { handleSubmit, formState, setValue, control } = useForm<FormData>({
    defaultValues: {
      activityName: '',
      ownerId: '',
    },
  });

  const [createActivityCommand, createActivityStatus, createActivityState] = useActivityCommand(`/activities`);

  const onSubmit = async (data: FormData) => {
    if (data.ownerId) {
      createActivityCommand({
        name: data.activityName,
        ownerId: data.ownerId,
      });
    }
  };

  const handleCancel = () => setOpen(false);

  const options = useMemo(
    () => [{ ownerId: user.userId, name: user.name }, ...organizations.map((o) => ({ ownerId: o.organizationId, name: o.name }))],
    [user, organizations]
  );

  useEffect(() => {
    if (user.userId) {
      setValue('ownerId', user.userId);
    }
  }, [user.userId]);

  useEffect(() => {
    if (createActivityState?.aggregateId) {
      setOpen(false);
      const activityOwnerId = (createActivityState.body as any).ownerId as string;
      router.push(`/activities/${createActivityState?.aggregateId}?ownerId=${activityOwnerId}`);
    }
  }, [createActivityState?.aggregateId]);

  return (
    <React.Fragment>
      <Dialog
        open={open}
        closeAfterTransition
        onBackdropClick={handleCancel}
        BackdropProps={{
          timeout: 500,
        }}>
        <DialogTitle>Create Activity</DialogTitle>
        <DialogContent>
          <DialogContentText>
            This will create a new activity in one of the Organizations that you are allowed to manage.
          </DialogContentText>
          <form className={classes.form} onSubmit={handleSubmit(onSubmit)}>
            <Grid container spacing={2}>
              <Grid item xs={12}>
                {options[0] && (
                  <Controller
                    control={control}
                    name="ownerId"
                    render={(props) => (
                      <FormControl variant="outlined" fullWidth={true}>
                        <InputLabel id="select-ownerid-label">Organization</InputLabel>
                        <Select {...props} labelId="select-ownerid-label" label="Organization">
                          {options.map((o) => (
                            <MenuItem key={o.ownerId} value={o.ownerId}>
                              {o.name}
                            </MenuItem>
                          ))}
                        </Select>
                      </FormControl>
                    )}
                  />
                )}
              </Grid>
              <Grid item xs={12}>
                <Controller
                  control={control}
                  name="activityName"
                  render={(props) => (
                    <TextField
                      {...props}
                      label="Activity name"
                      variant="outlined"
                      fullWidth={true}
                      disabled={createActivityStatus.processing}
                    />
                  )}
                />
              </Grid>
            </Grid>

            <DialogActions>
              <Button onClick={handleCancel} className={classes.cancelButton} color="default">
                Cancel
              </Button>
              <div className={classes.wrapper}>
                <Button type="submit" className={classes.saveButton} color="primary" disabled={createActivityStatus.processing}>
                  Save
                </Button>
                {createActivityStatus.processing && <CircularProgress size="1.4rem" className={classes.buttonProgress} />}
              </div>
            </DialogActions>
          </form>
        </DialogContent>
      </Dialog>
    </React.Fragment>
  );
}
