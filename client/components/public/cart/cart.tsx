import { Table, TableBody, TableCell, TableHead, TableRow } from "@material-ui/core";
import React from "react";

interface Ticket {
  ticketId: string,
  ticketTypeId: string;
  ticketReference?: string;
}

interface Cart {
  orderId: string,
  activityId: string,
  tickets: Ticket[];
}

const Cart: React.FC<Cart> = ({ tickets }) => {
  return (<React.Fragment>
    <Table>
      <TableHead>
        <TableRow>
          <TableCell>TicketType</TableCell>
          <TableCell>Reference</TableCell>
        </TableRow>
      </TableHead>
      <TableBody>
        {tickets.map(t => (
          <TableRow>
            <TableCell>{t.ticketTypeId}</TableCell>
            <TableCell>{t.ticketReference}</TableCell>
          </TableRow>
        ))}
      </TableBody>
    </Table>
  </React.Fragment>);
};

export default Cart;