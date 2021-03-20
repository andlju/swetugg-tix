import { Avatar, Button, Card, CardActions, CardContent, CardHeader, makeStyles, Typography } from "@material-ui/core";
import React from "react";
import { useDispatch } from "react-redux";
import { ADD_TICKETS } from "../../store/order.actions";
import { TicketType } from "../../store/ticket-type.models";

interface TicketTypeProps {
  ticketType: TicketType;
  orderId?: string;
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

const TicketTypeCard: React.FC<TicketTypeProps> = ({ ticketType, orderId }) => {
  const classes = useStyles();

  const dispatch = useDispatch();
  
  const onClickBuyOrder = async () => {
    const cmd = {
      type: ADD_TICKETS,
      payload: {
        orderId: orderId,
        activityId: ticketType.activityId,
        activityOwnerId: ticketType.ownerId,
        ticketTypeId: ticketType.ticketTypeId,
        quantity: 1
      }
    };
    console.log("Command", cmd);
    dispatch(cmd);
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
        onClick={() => onClickBuyOrder()}
        type="submit">Reserve</Button>
    </CardActions>
  </Card>;
};

export { TicketTypeCard };
