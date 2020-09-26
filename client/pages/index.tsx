import React, { Props } from 'react';
import { GetServerSideProps } from 'next';
import Typography from '@material-ui/core/Typography';
import Link from '../components/Link';
import Layout from '../components/layout/main-layout';
import { buildUrl } from '../src/url-utils';
import ActivityList, { Activity } from '../components/activities/activity-list';

interface IndexProps {
  activities: Activity[]
}

export default function Index({ activities }: IndexProps) {
  return (
    <Layout>
      <Typography variant="h4" component="h1" gutterBottom>
        Next.js with TypeScript example
      </Typography>
      <ActivityList activities={ activities } ></ActivityList>
      <Link href="/about" color="secondary">
        Go to the about page
      </Link>
    </Layout>
  );
}


export const getServerSideProps: GetServerSideProps = async () => {
  // Get external data from the file system, API, DB, etc.
  const resp = await fetch(buildUrl('/activities'));
  const data = await resp.json() as Activity[];
  console.log(data);
  // The value of the `props` key will be
  //  passed to the `Home` component
  return {
    props: {
      activities: data
    }
  }
}