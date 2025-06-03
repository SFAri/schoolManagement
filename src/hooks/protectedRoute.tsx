import { Navigate } from "react-router-dom";
import { useAuth } from "./useAuth"
import { ROUTERS } from "../utils/router";

type Props = { children: React.ReactNode}

export const ProtectedRoute = ({ children } : Props) => {
    // const {user} = useAuth();
    const role = localStorage.getItem("role");
    return role ? (
        <>
            {children}
        </>
    ):
    (
        <Navigate to={'/' +ROUTERS.LOGIN} replace/>
    );
};