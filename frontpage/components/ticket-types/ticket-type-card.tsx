import { Avatar, Button, Card, CardActions, CardContent, CardHeader, makeStyles, Typography } from "@material-ui/core";
import React from "react";
import { useDispatch } from "react-redux";
import { useOrderCommand } from "../../src/use-order-command.hook";
import { ADD_TICKETS, LOAD_ORDER } from "../../store/order.actions";
import { TicketType } from "./ticket-type.models";

interface TicketTypeProps {
  ticketType: TicketType;
}

const useStyles = makeStyles((theme) => ({
  icon: {
    marginRight: theme.spacing(2),
  },
  heroContent: {
    backgroundColor: theme.palette.background.paper,
    padding: theme.spacing(8, 0, 6),
  },
  heroButtons: {
    marginTop: theme.spacing(4),
  },
  orderCardAvatar: {

  }
}));

const TicketTypeCard: React.FC<TicketTypeProps> = ({ ticketType }) => {
  const classes = useStyles();

  const dispatch = useDispatch();
  
  const onClickBuyOrder = async (ticketTypeId: string) => {
    dispatch({
      type: ADD_TICKETS,
      payload: {
        ticketTypeId: ticketTypeId,
        quantity: 1
      }
    });
  }

  return <Card>
    <CardHeader
      avatar={
        <Avatar className={classes.orderCardAvatar}>{ticketType.name[0]}</Avatar>
      }
      title={ticketType.name} />
    <CardContent>
      <Typography variant="body2">
        A compelling text on why to buy this ticket type
      </Typography>
    </CardContent>
    <CardActions>
      <Button
        onClick={() => onClickBuyOrder(ticketType.ticketTypeId)}
        type="submit">Reserve</Button>
    </CardActions>
  </Card>;
};

export { TicketTypeCard };
