import CssBaseline from '@material-ui/core/CssBaseline';
import { ThemeProvider } from '@material-ui/core/styles';
import App, { AppProps } from 'next/app';
import Head from 'next/head';
import { withApplicationInsights } from 'next-applicationinsights';
import { SnackbarProvider } from 'notistack';
import React from 'react';
import { Provider } from 'react-redux';

import routingEpics from '../globals/routing.epic';
import BackOfficeLayout from '../layout/main-layout';
import configureStore from '../store/store';
import theme from '../styles/theme';

const store = configureStore(routingEpics);

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
      <Provider store={store}>
        <ThemeProvider theme={theme}>
          <CssBaseline />
          <SnackbarProvider
            maxSnack={3}
            anchorOrigin={{
              vertical: 'bottom',
              horizontal: 'right',
            }}>
            <BackOfficeLayout>
              <Component {...pageProps} />
            </BackOfficeLayout>
          </SnackbarProvider>
        </ThemeProvider>
      </Provider>
    </React.Fragment>
  );
}

class WrapperApp extends App {
  render() {
    return <MyApp {...this.props} />;
  }
}

export default withApplicationInsights({
  instrumentationKey: process.env.NEXT_PUBLIC_APPINSIGHTS_INSTRUMENTATIONKEY,
  isEnabled: !!process.env.NEXT_PUBLIC_APPINSIGHTS_INSTRUMENTATIONKEY,
})(WrapperApp);
