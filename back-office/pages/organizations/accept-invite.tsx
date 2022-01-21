import { Fab, makeStyles, Toolbar } from '@material-ui/core';
import Container from '@material-ui/core/Container';
import Grid from '@material-ui/core/Grid';
import Paper from '@material-ui/core/Paper';
import Typography from '@material-ui/core/Typography';
import AddIcon from '@material-ui/icons/Add';
import clsx from 'clsx';
import { GetServerSideProps } from 'next';
import React, { useEffect, useMemo } from 'react';
import { useState } from 'react';
import { useDispatch, useSelector } from 'react-redux';

import { AcceptOrganizationInvite } from '../../components/organizations/accept-organization-invite';
import { CreateOrganizationModal } from '../../components/organizations/create-organization-modal';
import { OrganizationList } from '../../components/organizations/organization-list';
import { useAuthenticatedUser } from '../../src/use-authenticated-user.hook';
import { listVisible } from '../../store/common/list-state.models';
import { loadOrganizationInvite, loadOrganizations } from '../../store/organizations/organizations.actions';
import { RootState } from '../../store/store';

const useStyles = makeStyles((theme) => ({
  container: {
    paddingTop: theme.spacing(4),
    paddingBottom: theme.spacing(4),
  },
  paper: {
    padding: theme.spacing(1),
  },
}));

interface AcceptInvitePageParams {
  inviteToken: string;
}

export default function AcceptInvitePage({ inviteToken }: AcceptInvitePageParams) {
  const classes = useStyles();

  const { user } = useAuthenticatedUser(['https://swetuggtixdev.onmicrosoft.com/tix-api/access_as_backoffice']);

  const { invite } = useSelector((r: RootState) => r.organizations);

  const dispatch = useDispatch();

  useEffect(() => {
    if (user.current?.userId) {
      dispatch(loadOrganizationInvite(inviteToken));
    }
  }, [user, inviteToken]);

  return (
    <Container maxWidth={false} className={classes.container}>
      <Typography variant="h4" component="h1" gutterBottom>
        Accept Invite
      </Typography>
      <Grid container spacing={3}>
        <Grid item xs={12}>
          <Paper className={clsx(classes.paper)}>
            <AcceptOrganizationInvite invite={invite} inviteToken={inviteToken} />
          </Paper>
        </Grid>
      </Grid>
    </Container>
  );
}

export const getServerSideProps: GetServerSideProps = async (req) => {
  const inviteToken = req.query.token;

  return {
    props: {
      inviteToken,
    },
  };
};
