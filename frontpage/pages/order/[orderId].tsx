
import { makeStyles } from "@material-ui/core";
import { GetServerSideProps } from "next";
import React from "react";
import { useSelector } from "react-redux";
import { END } from "redux-saga";
import { Cart } from "../../components/cart/cart";
import Layout from "../../layout/public-layout";
import { getView } from "../../src/services/view-fetcher.service";
import { buildUrl } from "../../src/url-utils";
import { LOAD_ORDER } from "../../store/order.actions";
import { Order } from "../../store/order.models";
import { OrderState } from "../../store/order.reducer";
import { RootState } from "../../store/root.reducer";
import wrapper, { SagaStore } from "../../store/store";


interface OrderProps {
  orderId: string
}

const useStyles = makeStyles((theme) => ({

}));


const PublicOrderPage: React.FC<OrderProps> = ({ orderId }) => {
  const classes = useStyles();
  
  const { currentOrder: order } = useSelector<RootState, OrderState>(state => state.order);

  return (<Layout title="Order">
    { order && <Cart orderId={order.orderId} tickets={order.tickets} activityId={order.activityId}></Cart> }
  </Layout>);
}

export default PublicOrderPage;

export const getServerSideProps: GetServerSideProps = wrapper.getServerSideProps(async ({ store, params }) => {
  if (!params) {
    return {
      props: {
      }
    }
  }
  const orderId = params?.orderId;

  store.dispatch({ type: LOAD_ORDER, payload: { orderId } });
  store.dispatch(END);

  await (store as SagaStore).sagaTask.toPromise();

  return {
    props: {
      orderId: orderId
    }
  };
});

