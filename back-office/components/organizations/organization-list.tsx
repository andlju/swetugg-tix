import {
  Accordion,
  AccordionDetails,
  AccordionSummary,
  Fab,
  FormControlLabel,
  Link,
  makeStyles,
  Radio,
  Toolbar,
  Typography,
} from '@material-ui/core';
import AddIcon from '@material-ui/icons/Add';
import ExpandLessIcon from '@material-ui/icons/ExpandLess';
import ExpandMoreIcon from '@material-ui/icons/ExpandMore';
import React from 'react';
import { useDispatch, useSelector } from 'react-redux';

import { Organization, selectOrganization } from '../../store/organizations/organizations.actions';
import { RootState } from '../../store/store';

export interface OrganizationListProps {
  organizations: Organization[];
}

const useStyles = makeStyles((theme) => ({
  identifier: {
    color: theme.palette.text.secondary,
    fontVariantCaps: 'all-small-caps',
  },
  organizationListTitle: {
    flex: '1 1 100%',
  },
  organizationListToolbar: {
    paddingLeft: theme.spacing(2),
    paddingRight: theme.spacing(2),
  },
}));

export function OrganizationList({ organizations }: OrganizationListProps) {
  const classes = useStyles();

  const { selectedOrganizationId, createInvite } = useSelector((r: RootState) => r.organizations);

  const dispatch = useDispatch();

  const handleSelect = (event: React.ChangeEvent<HTMLInputElement>) => {
    dispatch(selectOrganization(event.target.value));
  };

  return (
    <React.Fragment>
      {organizations.map((org) => (
        <Accordion key={org.organizationId}>
          <AccordionSummary expandIcon={<ExpandMoreIcon />} aria-label="Expand">
            <FormControlLabel
              aria-label="Select"
              onClick={(event) => event.stopPropagation()}
              onFocus={(event) => event.stopPropagation()}
              value={org.organizationId}
              control={
                <Radio
                  checked={selectedOrganizationId === org.organizationId}
                  onChange={handleSelect}
                />
              }
              label={org.name}
            />
          </AccordionSummary>
          <AccordionDetails>
            <Typography color="textSecondary">
              Description of the {org.name} organization
            </Typography>
          </AccordionDetails>
        </Accordion>
      ))}
    </React.Fragment>
  );
}
