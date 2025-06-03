import { Navigate, Route, Routes } from "react-router-dom";
import Login from "./pages/Login/Login"
import {ROUTERS} from './utils/router'
import App from "./App";
import AllCoursesPage from "./pages/Admin/Courses/AllCourses";
import DetailCoursePage from "./pages/Admin/DetailCourse/DetailCourse";
import AllUsersPage from "./pages/Admin/AllUsers/AllUsers";
import DetailUser from "./pages/DetailUser/DetailUser";
import SchedulePage from "./pages/Schedule/schedule";
import { useAuth } from "./hooks/useAuth";
import AllScores from "./pages/Admin/Scores/AllScores";
import { RequiredRoute } from "./hooks/requiredRoute";
import { MissingPage } from "./pages/Missing/MissingPage";
import { RoleBasedRedirect } from "./utils/RouteBased";

const RenderUserRouter = () => {
    const {token} = useAuth(); 
    const adminRouters = [
        // {
        //     path: ROUTERS.HOME,
        //     component: <App role="admin" selected="1"/>
        // },
        {
            path: ROUTERS.ADMIN.ALLCOURSES,
            component: <AllCoursesPage role="admin"/>
        },
        {
            path: ROUTERS.ADMIN.ALLCOURSES + '/:id',
            component: <DetailCoursePage role="admin"/>
        },

        // User:
        {
            path: ROUTERS.ADMIN.ALLUSERS,
            component: <AllUsersPage/>
        },
        {
            path: ROUTERS.ADMIN.ALLUSERS + '/:id',
            component: <DetailUser role="admin" selected="2"/>
        },

        // Score:
        {
            path: ROUTERS.ADMIN.ALLSCORES,
            component: <AllScores role="admin"/>
        },

        // My profile:
        {
            path: ROUTERS.PROFILE,
            component: <DetailUser role="admin" selected="3"/>
        },
    ]

    const lecturerRouter = [
        {
            path: ROUTERS.LECTURER.MYCOURSE,
            component: <AllCoursesPage role="lecturer"/>
        },
        {
            path: ROUTERS.LECTURER.MYCOURSE + '/:id',
            component: <DetailCoursePage role="lecturer"/>
        },
        {
            path: ROUTERS.LECTURER.MYSCHEDULE,
            component: <SchedulePage role="lecturer"/>
        },
        {
            path: ROUTERS.LECTURER.MYPROFILE,
            component: <DetailUser role="lecturer" selected="3"/>
        }
    ]

    const studentRouter = [
        {
            path: ROUTERS.STUDENT.MYCOURSE,
            component: <AllCoursesPage role="student"/>
        },
        {
            path: ROUTERS.STUDENT.MYCOURSE + '/:id',
            component: <DetailCoursePage role="student"/>
        },
        {
            path: ROUTERS.STUDENT.MYSCHEDULE,
            component: <SchedulePage role="student"/>
        },
        {
            path: ROUTERS.STUDENT.MYPROFILE,
            component: <DetailUser role="student" selected="3"/>
        },
    ]
    return (
        <Routes>
            <Route path={ROUTERS.LOGIN} element={token === undefined ? <Login /> : <Navigate to={'/'+ROUTERS.HOME} replace />} />
            <Route path={ROUTERS.HOME} element={<RoleBasedRedirect />} />
            <Route element={<RequiredRoute allowedRoles={['Admin']}/>}>
                {
                    adminRouters.map((item, key) => (
                        <Route key={key} path={item.path} element={item.component} />
                    ))
                }
            </Route>
            <Route element={<RequiredRoute allowedRoles={['Lecturer']}/>}>
                {
                    lecturerRouter.map((item, key) => (
                        <Route key={key} path={item.path} element={item.component} />
                    ))
                }
            </Route>
            <Route element={<RequiredRoute allowedRoles={['Student']}/>}>
                {
                    studentRouter.map((item, key) => (
                        <Route key={key} path={item.path} element={item.component} />
                    ))
                }
            </Route>

            {/* Catch all */}
            <Route path="*" element={<MissingPage/>} />
        </Routes>
    );
}

const RouterCustom = () => {
    return RenderUserRouter();
};

export default RouterCustom;