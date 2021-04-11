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
import { useDispatch } from 'react-redux';

import { createOrganizationInvite } from '../../store/organizations/organizations.actions';
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
  reloadButton: {},
  buttonProgress: {
    position: 'absolute',
    top: '50%',
    left: '50%',
    marginTop: -12,
    marginLeft: -12,
  },
}));

type FormData = {
  inviteUrl: string;
  name: string;
};

interface InviteUserProps {
  open: boolean;
  setOpen: (open: boolean) => void;
  organizationId: string;
  organizations: OrganizationsState;
}

export function InviteUserModal({ organizations, organizationId, open, setOpen }: InviteUserProps) {
  const classes = useStyles();

  const dispatch = useDispatch();
  const { createInvite } = organizations;

  const { handleSubmit, setValue, control } = useForm<FormData>({
    defaultValues: {
      inviteUrl: '',
      name: '',
    },
  });

  const onSubmit = async (data: FormData) => {
    // dispatch(createOrganization(data.name));
  };

  const handleReload = () => {
    dispatch(createOrganizationInvite(organizationId));
  };

  useEffect(() => {
    if (open) {
      dispatch(createOrganizationInvite(organizationId));
    }
  }, [open, organizationId]);

  useEffect(() => {
    if (createInvite.current?.token) {
      const basePath = window && window.location.origin;
      setValue('inviteUrl', `${basePath}/organizations/accept-invite?token=${createInvite.current.token}`);
    }
  }, [createInvite.current]);

  const handleCancel = () => setOpen(false);

  return (
    <React.Fragment>
      <Dialog
        open={open}
        closeAfterTransition
        maxWidth="sm"
        fullWidth={true}
        onBackdropClick={handleCancel}
        BackdropProps={{
          timeout: 500,
        }}>
        <DialogTitle>Invite User</DialogTitle>
        <DialogContent>
          <DialogContentText>
            Share this URL with any user you want to invite to the organization. The URL is valid for 2 days.
          </DialogContentText>
          <form className={classes.form} onSubmit={handleSubmit(onSubmit)}>
            <Grid container spacing={2}>
              <Grid item xs={12}>
                <Controller
                  control={control}
                  name="inviteUrl"
                  render={({ field }) => (
                    <TextField
                      {...field}
                      label="Invitation Url"
                      variant="outlined"
                      multiline={true}
                      fullWidth={true}
                      disabled={true}
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
                <Button onClick={handleReload} className={classes.reloadButton} color="primary" disabled={createInvite.fetching}>
                  Reload
                </Button>
                {createInvite.saving && <CircularProgress size="1.4rem" className={classes.buttonProgress} />}
              </div>
            </DialogActions>
          </form>
        </DialogContent>
      </Dialog>
    </React.Fragment>
  );
}
