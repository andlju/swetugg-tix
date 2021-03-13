import { useEffect } from "react";
import { useDispatch, useSelector } from "react-redux";
import { validateLogin, User } from "../store/auth/auth.actions";
import { RootState } from "../store/store";

interface AuthenticatedUserResult {
  user: User | undefined;
}

export function useAuthenticatedUser(scopes: string[]): AuthenticatedUserResult {

  const { user } = useSelector((s: RootState) => s.auth);

  const dispatch = useDispatch();

  useEffect(() => {
    // Validate that we have a currently logged in user with correct scopes
    console.log(`Hook dispatching validateUser`);
    dispatch(validateLogin(scopes));

  }, []);

  return {
    user
  };
}