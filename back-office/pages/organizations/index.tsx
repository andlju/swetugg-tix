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
  
  const [selectedOrg, setSelectedOrg] = React.useState(0);

  const handleSelect = (event: React.ChangeEvent<HTMLInputElement>) => {
    setSelectedOrg(+event.target.value);
  }

  const { user } = useAuthenticatedUser(["https://swetuggtixlocal.onmicrosoft.com/tix-api/access_as_admin"]);

  return (
    <Container maxWidth={false} className={classes.container}>
      <Typography variant="h4" component="h1" gutterBottom>
        Organizations
        </Typography>
      <Grid container spacing={3}>
        {/* List of activities */}
        <Grid item xs={6}>
          <Accordion>
            <AccordionSummary
              expandIcon={<ExpandMoreIcon />}
              aria-label="Expand"
            >
              <FormControlLabel
                aria-label="Select"
                onClick={(event) => event.stopPropagation()}
                onFocus={(event) => event.stopPropagation()}
                value={1}
                control={<Radio 
                  checked={selectedOrg === 1}
                  onChange={handleSelect}
                  />}
                label="Swetugg Events"
              />
            </AccordionSummary>
            <AccordionDetails>
              <Typography color="textSecondary">
                Description of the Swetugg Events organization
              </Typography>
            </AccordionDetails>
          </Accordion>
          <Accordion>
            <AccordionSummary
              expandIcon={<ExpandMoreIcon />}
              aria-label="Expand"
            >
              <FormControlLabel
                aria-label="Select"
                onClick={(event) => event.stopPropagation()}
                onFocus={(event) => event.stopPropagation()}
                value={2}
                control={<Radio 
                  checked={selectedOrg === 2}
                  onChange={handleSelect}
                  />}
                label="Aptitud"
              />
            </AccordionSummary>
            <AccordionDetails>
              <Typography color="textSecondary">
              Description of the Aptitud organization
              </Typography>
            </AccordionDetails>
          </Accordion>
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