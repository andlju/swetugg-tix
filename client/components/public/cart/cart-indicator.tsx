import { IconButton } from "@material-ui/core";
import ShoppingCartIcon from "@material-ui/icons/ShoppingCart";
import { useContext } from "react";
import { MainStore } from "../store/store";

const CartIndicator: React.FC = () => {
  const { state } = useContext(MainStore);
  
  return (<IconButton>
    {state.order.orderId && <ShoppingCartIcon/>}    
  </IconButton>)
};

export default CartIndicator;