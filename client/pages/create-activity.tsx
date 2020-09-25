import { NextPage } from 'next';
import Router from 'next/router';
import axios from 'axios';

import { Form, Button } from 'react-bootstrap';
import { useForm } from 'react-hook-form';
import Layout from '../components/Layout';

const Page: NextPage<any> = () => {
    const { handleSubmit, register, errors } = useForm();

    const onSubmit = async (values) => {
        const res = await axios.post('https://tix-dev-api893ee6eb.azurewebsites.net/api/activity');
        if (res.status === 200) {
            const activityId = res.data.activityId;

            Router.push({ pathname: `/activity`, query: { activityId }});
        }
    };

    return (
        <Layout>
            <Form onSubmit={handleSubmit(onSubmit)}>
                <Form.Group>
                    <Form.Label>Name</Form.Label>
                    <Form.Control name="activityName" type="text" ref={register} />
                </Form.Group>
                <Button variant="primary" type="submit">
                    Submit
                </Button>
            </Form>
        </Layout>
    )
};

Page.getInitialProps = async ({ }) => {
    return {};
};

export default Page;