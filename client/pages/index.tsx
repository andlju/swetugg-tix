import { NextPage } from 'next';
import Link from 'next/Link';
import { Container, Row, Col } from 'react-bootstrap';
import Layout from '../components/Layout';

interface Props {

}

const Page: NextPage<Props> = ( ) => (
  <Layout>
    <Container>
      <Link href="/create-activity"><a>Create a new activity</a></Link>
    </Container>
  </Layout>
);

Page.getInitialProps = async ({ req }) => {

  return { }
}

export default Page