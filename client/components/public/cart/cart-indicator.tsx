import { IconButton } from "@material-ui/core";
import ShoppingCartIcon from "@material-ui/icons/ShoppingCart";
import React from "react";
import { useContext } from "react";
import Link from "../../Link";
import { MainStore } from "../store/store";

const CartIndicator: React.FC = () => {
  const { state } = useContext(MainStore);

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

export default CartIndicator;