import { Navigate } from "react-router-dom";
import { ROUTERS } from "../utils/router";

export const RoleBasedRedirect = () => {
    const user = localStorage.getItem('user');
    const userJSON = user ? JSON.parse(user) : null;

    const role = userJSON?.roleName;

    if (role === 'Admin') {
        return <Navigate to={'/' + ROUTERS.ADMIN.ALLCOURSES} />;
    }
    if (role === 'Lecturer') {
        return <Navigate to={'/' + ROUTERS.LECTURER.MYCOURSE} />;
    }
    if (role === 'Student') {
        return <Navigate to={'/' + ROUTERS.STUDENT.MYCOURSE} />;
    }

    return <Navigate to={'/' + ROUTERS.LOGIN} />;
}
