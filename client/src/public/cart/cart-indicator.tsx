import { IconButton } from "@material-ui/core";
import ShoppingCartIcon from "@material-ui/icons/ShoppingCart";
import Link from "next/link";
import React from "react";
import { useContext } from "react";
import { PublicStore } from "../store/store";

const CartIndicator: React.FC = () => {
  const { state } = useContext(PublicStore);
  
  return (
    <React.Fragment>
      {state.order.orderId && (
        <Link href={`/public/order/${state.order.orderId}`}>
          <IconButton>
            <ShoppingCartIcon />
          </IconButton>
        </Link>)
      }
    </React.Fragment>);
};

export { CartIndicator };