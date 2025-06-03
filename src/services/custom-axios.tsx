import axios from "axios"
import { jwtDecode, JwtPayload } from "jwt-decode";

const instance = axios.create({
    baseURL: 'https://localhost:7122/api',
    timeout: 10000,
    headers: {
        "Content-Type": 'application/json',
        'Accept': '*/*'
    }
});

// Function to check token expiration
const isTokenExpired = (token: string | null): boolean => {
    if (!token) return true;
    const decoded = jwtDecode<JwtPayload>(token);
    return (decoded.exp ? decoded.exp * 1000 < Date.now() : true);
};

// Request interceptor
instance.interceptors.request.use(async (config) => {
    const token = localStorage.getItem('token');
    if (config.url === '/User/login') {
        return config; // Skip adding the Authorization header
    }
    console.log('token: ', token);
    if (isTokenExpired(token)) {
        localStorage.clear();
        window.location.reload();
    } else {
        config.headers['Authorization'] = `Bearer ${token}`;
    }

    return config;
}, (error) => {
    return Promise.reject(error);
});

// Response interceptor
// instance.interceptors.response.use(
//     response => response, // Handle successful responses
//     async (error) => {
//         const originalRequest = error.config;

//         // Check if it's a 401 error and attempt to refresh the token
//         if (error.response?.status === 401 && !originalRequest._retry) {
//             originalRequest._retry = true;

//             try {
//                 const newToken = await refreshToken();
//                 localStorage.setItem('token', newToken);
//                 originalRequest.headers['Authorization'] = `Bearer ${newToken}`;
//                 return instance(originalRequest); // Retry the original request
//             } catch (refreshError) {
//                 console.error('Failed to refresh token:', refreshError);
//                 // Handle logout or redirect
//                 return Promise.reject(refreshError);
//             }
//         }

//         return Promise.reject(error);
//     }
// );


export default instance;