import { IconButton } from "@material-ui/core";
import ShoppingCartIcon from "@material-ui/icons/ShoppingCart";
import Link from "next/link";
import React from "react";
import { useSelector } from "react-redux";
import { OrderState } from "../../store/order.reducer";
import { RootState } from "../../store/root.reducer";

const CartIndicator: React.FC = () => {

  const { currentOrder: order  } = useSelector<RootState, OrderState>(state => state.order);
  
  return (
    <React.Fragment>
      {order && (
        <Link href={`/order/${order.orderId}`}>
          <IconButton>
            <ShoppingCartIcon />
          </IconButton>
        </Link>)
      }
    </React.Fragment>);
};

export { CartIndicator };