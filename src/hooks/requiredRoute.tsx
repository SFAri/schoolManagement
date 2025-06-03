import { Navigate, Outlet, useLocation } from "react-router-dom";
import { ROUTERS } from "../utils/router";

type Props = { allowedRoles: string[]}

export const RequiredRoute = ({ allowedRoles }: Props) => {
    const location = useLocation();
    const user = localStorage.getItem('user');
    const userJSON = user? JSON.parse(user) : null;
    const hasAccess = userJSON?.roleName && allowedRoles.includes(userJSON?.roleName);
    return hasAccess
                    ? <Outlet />
                    : user 
                        ? <Navigate to={'/' + ROUTERS.UNAUTHORIZE} state={{from: location}} replace />
                        : <Navigate to={'/' + ROUTERS.LOGIN} state={{from : location}} replace />

};