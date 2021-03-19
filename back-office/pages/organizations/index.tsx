import React, { useEffect } from 'react';
import { GetServerSideProps } from 'next';
import { Accordion, AccordionDetails, AccordionSummary, FormControlLabel, makeStyles, Radio } from '@material-ui/core';
import ExpandMoreIcon from '@material-ui/icons/ExpandMore';
import ExpandLessIcon from '@material-ui/icons/ExpandLess';
import clsx from 'clsx';
import Typography from '@material-ui/core/Typography';
import Paper from '@material-ui/core/Paper';
import Container from '@material-ui/core/Container';
import Grid from '@material-ui/core/Grid';
import { WelcomeName } from '../../components/auth/auth';
import { useAuthenticatedUser } from '../../src/use-authenticated-user.hook';
import { useDispatch, useSelector } from 'react-redux';
import { loadOrganizations } from '../../store/organizations/organizations.actions';
import { RootState } from '../../store/store';
import { OrganizationList } from '../../components/organizations/organization-list';

const useStyles = makeStyles((theme) => ({
  container: {
    paddingTop: theme.spacing(4),
    paddingBottom: theme.spacing(4),
  },
  paper: {
    padding: theme.spacing(1),
  },
  activityList: {
    minHeight: theme.spacing(30),
    maxHeight: theme.spacing(60),
    overflow: 'auto',
  }
}));

export default function Index() {
  const classes = useStyles();

  const organizations = useSelector((r: RootState) => r.organizations && r.organizations.visibleOrganizations.ids.map(orgId => r.organizations.organizations[orgId]));

  const { user } = useAuthenticatedUser(["https://swetuggtixlocal.onmicrosoft.com/tix-api/access_as_admin"]);

  const dispatch = useDispatch();
  useEffect(() => {
    dispatch(loadOrganizations());
  }, []);

  return (
    <Container maxWidth={false} className={classes.container}>
      <Typography variant="h4" component="h1" gutterBottom>
        Organizations
      </Typography>
      <Grid container spacing={3}>
        {/* List of organizations for current user */}
        <Grid item xs={6}>

          {organizations && <OrganizationList organizations={organizations} />}

        </Grid>
      </Grid>

    </Container>
  );
}

export const getServerSideProps: GetServerSideProps = async () => {
  return {
    props: {

    }
  };
};