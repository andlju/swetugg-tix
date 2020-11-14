
import { makeStyles } from "@material-ui/core";
import React from "react";

interface OrderProps {
  order: Order,
  //ticketTypes: TicketType[];
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
}));


const PublicActivityPage: React.FC<OrderProps> = ({ initialActivity, ticketTypes }) => {

}