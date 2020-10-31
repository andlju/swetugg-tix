import { IconButton } from "@material-ui/core";
import ShoppingCartIcon from "@material-ui/icons/ShoppingCart";
import { useContext } from "react";
import { store } from "../store";

const CartIndicator: React.FC = () => {
  const { state } = useContext(store);
  
  return (<IconButton>
    {state.orderId && <ShoppingCartIcon/>}    
  </IconButton>)
};

export default CartIndicator;