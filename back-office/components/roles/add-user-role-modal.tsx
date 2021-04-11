import {
  Button,
  CircularProgress,
  Dialog,
  DialogActions,
  DialogContent,
  DialogContentText,
  DialogTitle,
  FormControl,
  FormHelperText,
  Grid,
  InputLabel,
  MenuItem,
  Select,
  TextField,
} from '@material-ui/core';
import { makeStyles } from '@material-ui/core/styles';
import React, { useEffect, useMemo } from 'react';
import { Controller, useFieldArray, useForm, useWatch } from 'react-hook-form';
import { useDispatch, useSelector } from 'react-redux';

import { useActivityCommand } from '../../src/use-activity-command.hook';
import { User } from '../../store/auth/auth.actions';
import { listVisible } from '../../store/common/list-state.models';
import { TixState } from '../../store/common/state.models';
import { createOrganization, Organization } from '../../store/organizations/organizations.actions';
import { OrganizationsState } from '../../store/organizations/organizations.reducer';
import { Role } from '../../store/roles/roles.actions';
import { RootState } from '../../store/store';
import { addUserRole } from '../../store/users/users.actions';

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

type AddUserRoleFormData = {
  roleId: string;
  attributes: { name: string; value: string }[];
};

interface AddUserRoleModalProps {
  open: boolean;
  setOpen: (open: boolean) => void;
  userId: string;
  roles: Role[];
}

export function AddUserRoleModal({ userId, roles, open, setOpen }: AddUserRoleModalProps) {
  const classes = useStyles();

  const dispatch = useDispatch();

  const { handleSubmit, setValue, control, watch, register } = useForm<AddUserRoleFormData>({
    defaultValues: {
      roleId: '',
      attributes: [],
    },
  });

  const { fields, append, remove } = useFieldArray({ control, name: 'attributes' });

  const organizations = useSelector((r: RootState) => r.organizations);
  const visibleOrganizations = listVisible(organizations.organizations);
  const activities = useSelector((r: RootState) => r.activities);
  const visibleActivities = listVisible(activities.activities);

  const onSubmit = async (data: AddUserRoleFormData) => {
    dispatch(addUserRole(userId, data));
  };

  const roleId = watch('roleId');
  const selectedRole = useMemo(() => roles.find((r) => r.roleId === roleId), [roleId]);

  useEffect(() => {
    // When a role is selected - first remove all fields.
    remove();
  }, [selectedRole]);

  useEffect(() => {
    // Whenever fields change - if we have the wrong number of fields in place, let's add them
    // Need to do it this way since React Hook Form only allows one field method call per render.
    if (!selectedRole || selectedRole.attributes.length === fields.length) return;

    const newFields = selectedRole.attributes.map((a) => ({ name: a.name, value: '' }));
    append(newFields);
  }, [fields]);

  const { addUserRoleCommand } = useSelector((r: RootState) => r.users);
  useEffect(() => {
    if (addUserRoleCommand.result) {
      setOpen(false);
    }
  }, [addUserRoleCommand.result]);

  const handleCancel = () => setOpen(false);

  return (
    <React.Fragment>
      <Dialog
        open={open}
        maxWidth="sm"
        fullWidth={true}
        closeAfterTransition
        onBackdropClick={handleCancel}
        BackdropProps={{
          timeout: 500,
        }}>
        <DialogTitle>Add User Role</DialogTitle>
        <DialogContent>
          <DialogContentText>This will add a role to the selected user.</DialogContentText>
          <form className={classes.form} onSubmit={handleSubmit(onSubmit)}>
            <Grid container spacing={2}>
              <Grid item xs={12}>
                {roles[0] && (
                  <Controller
                    control={control}
                    name="roleId"
                    render={({ field }) => (
                      <FormControl variant="outlined" fullWidth={true}>
                        <InputLabel id="select-ownerid-label">Role</InputLabel>
                        <Select {...field} labelId="select-ownerid-label" label="Role" displayEmpty={true}>
                          {roles.map((r) => (
                            <MenuItem key={r.roleId} value={r.roleId}>
                              {r.name}
                            </MenuItem>
                          ))}
                        </Select>
                        {selectedRole && <FormHelperText>{selectedRole?.description}</FormHelperText>}
                      </FormControl>
                    )}
                  />
                )}
              </Grid>
              {fields.map((f, index) => (
                <Grid item xs={12} key={f.id}>
                  <Controller
                    control={control}
                    name={`attributes.${index}.value` as const}
                    defaultValue={''}
                    render={({ field }) => (
                      <FormControl variant="outlined" fullWidth={true}>
                        <InputLabel id={`select-attribute-${index}-label`}>{f.name}</InputLabel>
                        <Select {...field} labelId={`select-attribute-${index}-label`} label={f.name} displayEmpty={true}>
                          <MenuItem value="*">All</MenuItem>
                          {f.name === 'OrganizationId' &&
                            visibleOrganizations.map((o) => (
                              <MenuItem key={o.organizationId} value={o.organizationId}>
                                {o.name}
                              </MenuItem>
                            ))}
                          {f.name === 'ActivityId' &&
                            visibleActivities.map((a) => (
                              <MenuItem key={a.activityId} value={a.activityId}>
                                {a.name}
                              </MenuItem>
                            ))}
                        </Select>
                      </FormControl>
                    )}
                  />
                  <input type="hidden" {...register(`attributes.${index}.name` as const)} />
                </Grid>
              ))}
            </Grid>

            <DialogActions>
              <Button onClick={handleCancel} className={classes.cancelButton} color="default">
                Cancel
              </Button>
              <div className={classes.wrapper}>
                <Button type="submit" className={classes.saveButton} color="primary" disabled={addUserRoleCommand.sending}>
                  Save
                </Button>
                {addUserRoleCommand.sending && <CircularProgress size="1.4rem" className={classes.buttonProgress} />}
              </div>
            </DialogActions>
          </form>
        </DialogContent>
      </Dialog>
    </React.Fragment>
  );
}
