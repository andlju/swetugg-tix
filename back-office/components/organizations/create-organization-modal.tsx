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
import React, { useEffect, useMemo } from 'react';
import { Controller, useForm } from 'react-hook-form';
import { useDispatch } from 'react-redux';

import { useActivityCommand } from '../../src/use-activity-command.hook';
import { User } from '../../store/auth/auth.actions';
import { createOrganization } from '../../store/organizations/organizations.actions';
import { OrganizationsState } from '../../store/organizations/organizations.reducer';

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
  name: string;
};

interface CreateOrganizationProps {
  open: boolean;
  setOpen: (open: boolean) => void;
  organizations: OrganizationsState;
}

export function CreateOrganizationModal({ organizations, open, setOpen }: CreateOrganizationProps) {
  const classes = useStyles();

  const dispatch = useDispatch();
  const { editState } = organizations;

  const { handleSubmit, setValue, control } = useForm<FormData>({
    defaultValues: {
      name: '',
    },
  });

  const onSubmit = async (data: FormData) => {
    dispatch(createOrganization(data.name));
  };

  useEffect(() => {
    if (!editState.saving && !editState.errorCode) {
      setValue('name', '');
      setOpen(false);
    }
  }, [editState.saving]);

  const handleCancel = () => setOpen(false);

  return (
    <React.Fragment>
      <Dialog
        open={open}
        closeAfterTransition
        onBackdropClick={handleCancel}
        BackdropProps={{
          timeout: 500,
        }}>
        <DialogTitle>Create Organization</DialogTitle>
        <DialogContent>
          <DialogContentText>This will create a new Organization.</DialogContentText>
          <form className={classes.form} onSubmit={handleSubmit(onSubmit)}>
            <Grid container spacing={2}>
              <Grid item xs={12}>
                <Controller
                  control={control}
                  name="name"
                  render={(props) => (
                    <TextField
                      {...props}
                      label="Organization name"
                      variant="outlined"
                      fullWidth={true}
                      disabled={editState.saving}
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
                <Button
                  type="submit"
                  className={classes.saveButton}
                  color="primary"
                  disabled={editState.saving}>
                  Save
                </Button>
                {editState.saving && (
                  <CircularProgress size="1.4rem" className={classes.buttonProgress} />
                )}
              </div>
            </DialogActions>
          </form>
        </DialogContent>
      </Dialog>
    </React.Fragment>
  );
}
