import { Button, Grid, Paper, Typography } from '@material-ui/core';
import React, { useEffect } from 'react';
import { useDispatch } from 'react-redux';

import { acceptOrganizationInvite, OrganizationInvite } from '../../store/organizations/organizations.actions';

interface AcceptOrganizationInviteParams {
  inviteToken: string;
  invite: OrganizationInvite;
  accepted: boolean;
  loading: boolean;
}

export function AcceptOrganizationInvite({ invite, inviteToken, accepted, loading }: AcceptOrganizationInviteParams) {
  const dispatch = useDispatch();

  const handleAccept = () => {
    dispatch(acceptOrganizationInvite(inviteToken));
  };

  return (
    <Grid container>
      <Grid item xs={12}>
        <Typography>
          You have been invited to the {invite.organization.name} organization by {invite.invitedByUser.name}
        </Typography>
      </Grid>
      <Grid>
        <Button variant="contained" onClick={handleAccept} disabled={loading || accepted}>
          {accepted ? 'Accepted' : 'Accept'}
        </Button>
      </Grid>
    </Grid>
  );
}
