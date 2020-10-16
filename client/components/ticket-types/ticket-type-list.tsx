import Link from 'next/link';
import {
  Button,
  TableContainer, Table, TableBody, TableHead, TableRow, TableCell,
  Paper,
  Typography,
  makeStyles
} from "@material-ui/core";
import { TicketType } from "./ticket-type.models";
import AddTicketType from "./add-ticket-type";
import { useEffect, useState } from 'react';
import { getView } from '../../src/services/view-fetcher.service';
import { buildUrl } from '../../src/url-utils';

interface TicketTypeListProps {
  activityId: string,
  initialTicketTypes: TicketType[]
}

const useStyles = makeStyles((theme) => ({
  paper: {
    padding: theme.spacing(2),
  },
  nameColumnHead: {
    width: "70%"
  },
  actionsColumnHead: {
    width: "30%"
  },
  identifier: {
    color: theme.palette.text.secondary,
    fontVariantCaps: "all-small-caps"
  },
  refreshing : {
    opacity: 0.25
  }
}));

export default function TicketTypeList({ initialTicketTypes, activityId }: TicketTypeListProps) {
  const classes = useStyles();
  const [ticketTypes, setTicketTypes] = useState(initialTicketTypes);
  const [refreshTicketTypes, setRefreshTicketTypes] = useState('');

  useEffect(() => {
    if (refreshTicketTypes) {
      const fetchData = async () => {
        const resp = await getView<TicketType[]>(
          buildUrl(`/activities/${activityId}/ticket-types`),
          v => refreshTicketTypes === "all" || !!v.find(tt => tt.ticketTypeId === refreshTicketTypes));
          setTicketTypes(resp);
        setRefreshTicketTypes('');
      };
      fetchData();
    }
  }, [refreshTicketTypes]);

  return (<Paper className={classes.paper}>
    <Typography variant="overline">
      Ticket Types
    </Typography>
    <TableContainer>
      <Table size="small">
        <TableHead>
          <TableRow>
            <TableCell className={classes.nameColumnHead}>Name</TableCell>
            <TableCell className={classes.actionsColumnHead}>Actions</TableCell>
          </TableRow>
        </TableHead>
        <TableBody className={!!refreshTicketTypes ? classes.refreshing : ''}>
          {ticketTypes.map(row => (
            <TableRow key={row.ticketTypeId} hover={true}>
              <TableCell>
                <Typography>{row.name}</Typography>
                <Typography className={classes.identifier}>{row.ticketTypeId}</Typography>
              </TableCell>
              <TableCell>
                <Link href={`/activities/${row.activityId}/ticket-types/${row.ticketTypeId}`}>
                  <Button variant="outlined">Edit</Button>
                </Link>
              </TableCell>
            </TableRow>
          ))}
        </TableBody>
      </Table>
    </TableContainer>

    <AddTicketType activityId={activityId} refreshTicketTypes={(tt) => setRefreshTicketTypes(tt)}/>
  </Paper>);
}

