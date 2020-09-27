import Link from 'next/link';
import { 
  Button, 
  Table, TableBody, TableHead, TableRow, TableCell,
  Paper,
  Typography,
  makeStyles } from "@material-ui/core";
import { TicketTypeListProps } from "./ticket-type.models";

const useStyles = makeStyles((theme) => ({
  paper: {
    padding: theme.spacing(2),
  },
  ticketTypeIdColumnHead: {
    width: "30%"
  },
  nameColumnHead: {
    width: "40%"
  },
  actionsColumnHead: {
    width: "40%"
  },
}));

export default function TicketTypeList({ ticketTypes }: TicketTypeListProps) {
  const classes = useStyles();
  return (<Paper className={classes.paper}>
    <Typography variant="overline">
      Activity Information
    </Typography>
    <Table size="small">
      <TableHead>
        <TableRow>
          <TableCell className={classes.ticketTypeIdColumnHead}>TicketTypeId</TableCell>
          <TableCell className={classes.nameColumnHead}>Name</TableCell>
          <TableCell className={classes.actionsColumnHead}>Actions</TableCell>
        </TableRow>
      </TableHead>
      <TableBody>
        {ticketTypes.map(row => (
          <TableRow key={row.ticketTypeId} hover={true}>
            <TableCell>{row.ticketTypeId}</TableCell>
            <TableCell>{row.name}</TableCell>
            <TableCell>
              <Link href={`/activities/${row.activityId}/ticket-types/${row.ticketTypeId}`}>
                <Button>Edit...</Button>
              </Link>
            </TableCell>
          </TableRow>
        ))}
      </TableBody>
    </Table>
  </Paper>);
}

