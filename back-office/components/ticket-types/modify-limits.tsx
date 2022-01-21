import {
  Box,
  Button,
  CircularProgress,
  Container,
  Grid,
  IconButton,
  InputAdornment,
  makeStyles,
  TextField,
  Typography,
} from '@material-ui/core';
import React, { useEffect } from 'react';
import { Controller, useForm } from 'react-hook-form';

import { CommandLogSeverity } from '../../src/services/activity-command.service';
import { useActivityCommand } from '../../src/use-activity-command.hook';
import { TicketType } from '../../store/activities/ticket-type.models';

interface EditTicketTypeProps {
  ticketType: TicketType;
}

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
  buttonContainer: {
    verticalAlign: 'middle',
    display: 'flex',
    flexDirection: 'column',
    justifyContent: 'center',
  },
  button: {},
}));

type LimitFormData = {
  seats: string;
};

export function ModifyLimits({ ticketType }: EditTicketTypeProps) {
  const classes = useStyles();

  const increaseLimitForm = useForm<LimitFormData>({
    defaultValues: {
      seats: '',
    },
  });

  const decreaseLimitForm = useForm<LimitFormData>({
    defaultValues: {
      seats: '',
    },
  });

  const [increaseLimitCommand, increaseLimitStatus, increaseLimitState] = useActivityCommand(
    `/activities/${ticketType.activityId}/ticket-types/${ticketType.ticketTypeId}/increase-limit?ownerId=${ticketType.ownerId}`
  );
  const [decreaseLimitCommand, decreaseLimitStatus, decreaseLimitState] = useActivityCommand(
    `/activities/${ticketType.activityId}/ticket-types/${ticketType.ticketTypeId}/decrease-limit?ownerId=${ticketType.ownerId}`
  );
  const [removeLimitCommand, removeLimitStatus, removeLimitState] = useActivityCommand(
    `/activities/${ticketType.activityId}/ticket-types/${ticketType.ticketTypeId}/limit?ownerId=${ticketType.ownerId}`,
    { method: 'DELETE' }
  );

  const increaseLimitError =
    increaseLimitState?.messages && increaseLimitState.messages.find((m) => m.severity === CommandLogSeverity.Error);
  const decreaseLimitError =
    decreaseLimitState?.messages && decreaseLimitState.messages.find((m) => m.severity === CommandLogSeverity.Error);
  const removeLimitError =
    removeLimitState?.messages && removeLimitState.messages.find((m) => m.severity === CommandLogSeverity.Error);

  const onSubmitIncreaseLimit = async (data: LimitFormData) => {
    increaseLimitCommand({
      seats: +(data.seats || 0),
    });
  };

  const onSubmitDecreaseLimit = async (data: LimitFormData) => {
    decreaseLimitCommand({
      seats: +(data.seats || 0),
    });
  };

  const onClickRemoveLimit = async () => {
    removeLimitCommand({});
  };

  useEffect(() => {
    if (increaseLimitStatus.completed) {
      increaseLimitForm.setValue('seats', '');
    }
    if (decreaseLimitStatus.completed) {
      decreaseLimitForm.setValue('seats', '');
    }
  }, [increaseLimitStatus, decreaseLimitStatus]);

  return (
    <Container className={classes.root}>
      <Typography variant="overline">Limits</Typography>
      <Grid container spacing={2}>
        <Grid item xs={12} md={4}>
          <form className={classes.form} onSubmit={increaseLimitForm.handleSubmit(onSubmitIncreaseLimit)}>
            <Controller
              control={increaseLimitForm.control}
              name="seats"
              render={({ field }) => (
                <TextField
                  {...field}
                  label="Increase limit"
                  type="number"
                  className={classes.input}
                  variant="outlined"
                  disabled={increaseLimitStatus.processing}
                  error={increaseLimitStatus.failed}
                  helperText={increaseLimitError?.message}
                  InputProps={{
                    endAdornment: increaseLimitStatus.processing && (
                      <InputAdornment position="end">
                        <CircularProgress color="inherit" size="1.5rem" />
                      </InputAdornment>
                    ),
                  }}
                />
              )}
            />
          </form>
        </Grid>
        <Grid item xs={12} md={4}>
          <form className={classes.form} onSubmit={decreaseLimitForm.handleSubmit(onSubmitDecreaseLimit)}>
            <Controller
              control={decreaseLimitForm.control}
              name="seats"
              render={({ field }) => (
                <TextField
                  {...field}
                  label="Decrease limit"
                  type="number"
                  className={classes.input}
                  variant="outlined"
                  disabled={decreaseLimitStatus.processing}
                  error={decreaseLimitStatus.failed}
                  helperText={decreaseLimitError?.message}
                  InputProps={{
                    endAdornment: decreaseLimitStatus.processing && (
                      <InputAdornment position="end">
                        <CircularProgress color="inherit" size="1.5rem" />
                      </InputAdornment>
                    ),
                  }}
                />
              )}
            />
          </form>
        </Grid>
        <Grid item xs={12} md={4} className={classes.buttonContainer}>
          <Button
            className={classes.button}
            color="secondary"
            variant="contained"
            size="medium"
            disabled={ticketType.limit == undefined}
            onClick={onClickRemoveLimit}>
            Remove Limit
          </Button>
        </Grid>
      </Grid>
    </Container>
  );
}
