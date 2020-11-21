import React from 'react';
import Head from 'next/head';
import { ThemeProvider } from '@material-ui/core/styles';
import CssBaseline from '@material-ui/core/CssBaseline';
import { SnackbarProvider } from 'notistack';
import theme from '../styles/theme';
import type { AppProps } from 'next/app';
import wrapper from '../store/store';
import BackOfficeLayout from '../layout/main-layout';

function MyApp(props: AppProps) {
  const { Component, pageProps } = props;

  React.useEffect(() => {
    // Remove the server-side injected CSS.
    const jssStyles = document.querySelector('#jss-server-side');
    if (jssStyles?.parentElement) {
      jssStyles.parentElement.removeChild(jssStyles);
    }
  }, []);

  return (
    <React.Fragment>
      <Head>
        <title>My page</title>
        <meta name="viewport" content="initial-scale=1, width=device-width" />
      </Head>
      <ThemeProvider theme={theme}>
        {/* CssBaseline kickstart an elegant, consistent, and simple baseline to build upon. */}
        <CssBaseline />
        <SnackbarProvider maxSnack={3} anchorOrigin={{
          vertical: 'bottom',
          horizontal: 'right',
        }}>
            <BackOfficeLayout>
              <Component {...pageProps} />
            </BackOfficeLayout>
        </SnackbarProvider>
      </ThemeProvider>
    </React.Fragment>
  );
}

export default wrapper.withRedux(MyApp);