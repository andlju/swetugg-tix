import React from 'react';
import Head from 'next/head';
import { ThemeProvider } from '@material-ui/core/styles';
import CssBaseline from '@material-ui/core/CssBaseline';
import { SnackbarProvider } from 'notistack';
import { withApplicationInsights } from 'next-applicationinsights';
import theme from '../styles/theme';
import App, { AppProps } from 'next/app';
import wrapper from '../store/store';

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
            <Component {...pageProps} />
        </SnackbarProvider>
      </ThemeProvider>
    </React.Fragment>
  );
}


class WrapperApp extends App {
  render() {
    return (
      <MyApp {...this.props} />
    )
  }
}

export default wrapper.withRedux(withApplicationInsights({ 
  instrumentationKey: process.env.NEXT_PUBLIC_APPINSIGHTS_INSTRUMENTATIONKEY,
  isEnabled: !!process.env.NEXT_PUBLIC_APPINSIGHTS_INSTRUMENTATIONKEY
})(WrapperApp));
