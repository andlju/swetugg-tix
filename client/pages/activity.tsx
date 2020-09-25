import { NextPage } from 'next';
import Link from 'next/Link';
import { Container, Row, Col, Card } from 'react-bootstrap';
import axios from 'axios';
import Layout from '../components/Layout';
import { FunctionComponent } from 'react';

interface Activity {
    activityId: string,
    name: string,
}

interface ActivityProps {
    activity: Activity;
}

const ActivityCard: FunctionComponent<ActivityProps> = ({ activity }) => (
    <Card>
        <Card.Header>
            {activity.name}
        </Card.Header>
        <Card.Body>
            Hello
        </Card.Body>
    </Card>
);

const Page: NextPage<Activity> = (activity) => (
    <Layout>
        <Container>
            <Row>
                <Col md="8">
                    <ActivityCard activity={activity} />
                </Col>
            </Row>
        </Container>
    </Layout>
);

Page.getInitialProps = async ({ query: { activityId } }) => {
    const res = await axios.get<Activity>(`https://tix-dev-api68072f8e.azurewebsites.net/api/activity/${activityId}`);
    console.log(res.data)
    return res.data;
}

export default Page