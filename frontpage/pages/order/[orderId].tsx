
import { makeStyles } from "@material-ui/core";
import { GetServerSideProps } from "next";
import React from "react";
import Layout from "../../../layout/public-layout";
import { Cart } from "../../../src/public";
import { getView } from "../../../src/services/view-fetcher.service";
import { buildUrl } from "../../../src/url-utils";

interface OrderProps {
  order: any,
  //ticketTypes: TicketType[];
}

const useStyles = makeStyles((theme) => ({

}));


const PublicOrderPage: React.FC<OrderProps> = ({ order }) => {
  const classes = useStyles();
  console.log(order);
  return (<Layout title="Order">
    <Cart orderId={order.orderId} tickets={order.tickets} activityId={order.activityId}></Cart>
  </Layout>);
}

export default PublicOrderPage;

export const getServerSideProps: GetServerSideProps = async (context) => {
  const orderId = context.params?.orderId;

  const [orderResp] = await Promise.all([
    getView<any>(buildUrl(`/orders/${orderId}`))
  ]);

  return {
    props: {
      order: orderResp
    }
  };
};