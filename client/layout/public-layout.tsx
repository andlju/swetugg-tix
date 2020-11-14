import { makeStyles } from '@material-ui/core/styles';
import Link from '../components/Link';
import AppBar from '@material-ui/core/AppBar';
import Toolbar from '@material-ui/core/Toolbar';
import Typography from '@material-ui/core/Typography';
import React from 'react';
import CartIndicator from '../components/public/cart/cart-indicator';

const useStyles = makeStyles((theme) => ({
  toolbar: {
    justifyContent: 'space-between',
  },
  left: {
    flex: 1,
  },
  right: {
    flex: 1,
    display: 'flex',
    justifyContent: 'flex-end',
  },
  footer: {
    backgroundColor: theme.palette.background.paper,
    padding: theme.spacing(6),
  },
}));

function Copyright() {
  return (
    <Typography variant="body2" color="textSecondary" align="center">
      {'Copyright © '}
      <Link color="inherit" href="https://swetugg.se/">
        Swetugg Tix
      </Link>{' '}
      {new Date().getFullYear()}
      {'.'}
    </Typography>
  );
}

interface LayoutProps {
  title: string
}

export const Layout: React.FC<LayoutProps> = ({ children, title }) => {
  const classes = useStyles();

  return (<div>
      <AppBar position="relative">
        <Toolbar>
          <div className={classes.left} />
          <Typography variant="h6" color="inherit" noWrap>
            {title}
          </Typography>
          <div className={classes.right}>
            <CartIndicator />
          </div>
        </Toolbar>
      </AppBar>
      <main>
        {children}
      </main>
      {/* Footer */}
      <footer className={classes.footer}>
        <Typography variant="h6" align="center" gutterBottom>
          Footer
        </Typography>
        <Typography variant="subtitle1" align="center" color="textSecondary" component="p">
          Something here to give the footer a purpose!
        </Typography>
        <Copyright />
      </footer>
      {/* End footer */}
    </div>)
}

export default Layout