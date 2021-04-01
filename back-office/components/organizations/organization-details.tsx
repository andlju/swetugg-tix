import { Button, makeStyles, Typography } from '@material-ui/core';
import { Container } from '@material-ui/core';
import { Grid, Table, TableBody, TableCell, TableRow } from '@material-ui/core';
import Link from 'next/link';
import React from 'react';

import { Organization } from '../../store/organizations/organizations.actions';

const useStyles = makeStyles((theme) => ({
  root: {
    padding: theme.spacing(0),
  },
}));

const frontpageRoot = process.env.NEXT_PUBLIC_FRONTPAGE_ROOT ?? 'http://localhost:3001';

export type OrganizationDetailsProps = {
  organization: Organization;
};

export function OrganizationDetails({ organization }: OrganizationDetailsProps) {
  const classes = useStyles();
  return (
    <Container className={classes.root}>
      <Typography variant="overline">Organization Information</Typography>
      <Grid container spacing={2}>
        <Grid item xs={12}>
          <Table size="small">
            <TableBody>
              <TableRow>
                <TableCell>Activities</TableCell>
                <TableCell>{0}</TableCell>
              </TableRow>
            </TableBody>
          </Table>
        </Grid>
        <Grid item xs={12}>
          <Link href={`${frontpageRoot}/${organization.organizationId}`}>
            <Button variant="contained" color="primary">
              Public page
            </Button>
          </Link>
        </Grid>
      </Grid>
    </Container>
  );
}
