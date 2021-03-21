import { Accordion, AccordionDetails, AccordionSummary, Fab, FormControlLabel, Link, makeStyles, Radio, Toolbar, Typography } from '@material-ui/core';
import ExpandMoreIcon from '@material-ui/icons/ExpandMore';
import ExpandLessIcon from '@material-ui/icons/ExpandLess';
import AddIcon from '@material-ui/icons/Add';
import React from "react";
import { Organization } from "../../store/organizations/organizations.actions";

export interface OrganizationListProps {
  organizations: Organization[];
}


const useStyles = makeStyles((theme) => ({
  identifier: {
    color: theme.palette.text.secondary,
    fontVariantCaps: "all-small-caps"
  },
  organizationListTitle: {
    flex: '1 1 100%'
  },
  organizationListToolbar: {
    paddingLeft: theme.spacing(2),
    paddingRight: theme.spacing(2)
  }
}));

export function OrganizationList({ organizations }: OrganizationListProps) {

  const classes = useStyles();

  const [selectedOrg, setSelectedOrg] = React.useState('');

  const handleSelect = (event: React.ChangeEvent<HTMLInputElement>) => {
    setSelectedOrg(event.target.value);
  };

  return (<React.Fragment>
    <Toolbar className={classes.organizationListToolbar}>
      <Typography className={classes.organizationListTitle} variant="h6" component="div">
        Organizations
      </Typography>
    </Toolbar>
    {
      organizations.map(org =>
        <Accordion key={org.organizationId}>
          <AccordionSummary
            expandIcon={<ExpandMoreIcon />}
            aria-label="Expand"
          >
            <FormControlLabel
              aria-label="Select"
              onClick={(event) => event.stopPropagation()}
              onFocus={(event) => event.stopPropagation()}
              value={org.organizationId}
              control={<Radio
                checked={selectedOrg === org.organizationId}
                onChange={handleSelect}
              />}
              label={org.name}
            />
          </AccordionSummary>
          <AccordionDetails>
            <Typography color="textSecondary">
              Description of the {org.name} organization
            </Typography>
          </AccordionDetails>
        </Accordion>
      )
    }
  </React.Fragment>);
}