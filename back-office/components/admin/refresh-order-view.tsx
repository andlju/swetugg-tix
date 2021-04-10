import { Button, CircularProgress, Container, makeStyles, TextField, Typography } from '@material-ui/core';
import React from 'react';
import { Controller, useForm } from 'react-hook-form';

import { useOrderCommand } from '../../src/use-order-command.hook';

interface RefreshOrderViewProps {
  initialOrderId?: string;
}

type FormData = {
  orderId: string;
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

export function RefreshOrderView({ initialOrderId }: RefreshOrderViewProps) {
  const classes = useStyles();

  const [refreshView] = useOrderCommand('Refresh views');
  const { register, handleSubmit, setValue, formState, setError, control } = useForm<FormData>({
    defaultValues: {
      orderId: initialOrderId,
    },
  });

  const { errors } = formState;

  const onSubmit = async (data: FormData) => {
    try {
      await refreshView(`/orders-admin/${data.orderId}/rebuild`, {});
      setValue('orderId', '');
    } catch (err) {
      setError('orderId', { message: 'Invalid format' });
    }
  };

  return (
    <Container className={classes.root}>
      <Typography variant="overline">Refresh Views</Typography>
      <form className={classes.form} onSubmit={handleSubmit(onSubmit)}>
        <Controller
          control={control}
          name="orderId"
          render={({ field }) => (
            <TextField
              {...field}
              label="Order Id"
              variant="outlined"
              className={classes.input}
              disabled={formState.isSubmitting}
              error={!!errors.orderId}
              helperText={errors.orderId && errors.orderId.message}
            />
          )}
        />
        <div className={classes.progressWrapper}>
          <Button type="submit" variant="outlined" className={classes.button} disabled={formState.isSubmitting}>
            Refresh
          </Button>
          {formState.isSubmitting && <CircularProgress size={24} className={classes.buttonProgress} />}
        </div>
      </form>
    </Container>
  );
}
