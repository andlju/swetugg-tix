import { Button, Grid, Typography } from '@material-ui/core';
import React from 'react';
import { useDispatch } from 'react-redux';

import { TixState } from '../../store/common/state.models';
import { acceptOrganizationInvite, OrganizationInvite } from '../../store/organizations/organizations.actions';

interface AcceptOrganizationInviteParams {
  inviteToken: string;
  invite: TixState<OrganizationInvite>;
}

export function AcceptOrganizationInvite({ invite, inviteToken }: AcceptOrganizationInviteParams) {
  const dispatch = useDispatch();

  const handleAccept = () => {
    dispatch(acceptOrganizationInvite(inviteToken));
  };

  return (
    <>
      {invite.current && (
        <Grid container>
          <Grid item xs={12}>
            <Typography>
              You have been invited to the {invite.current.organization.name} organization by {invite.current.invitedByUser.name}
            </Typography>
          </Grid>
          <Grid>
            <Button variant="contained" onClick={handleAccept} disabled={invite.fetching || invite.current.accepted}>
              {invite.current.accepted ? 'Accepted' : 'Accept'}
            </Button>
          </Grid>
        </Grid>
      )}
    </>
  );
}
