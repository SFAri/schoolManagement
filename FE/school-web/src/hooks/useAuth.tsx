import { createContext, ReactNode, useContext, useEffect, useState } from "react";
import { login as apiLogin, getUserInfo , logout as apiLogout} from '../services/authService'; // Nhập các hàm từ apiService, hãy đảm bảo bạn có hàm getUserInfo
// import { User } from './types'; // Nhập định nghĩa kiểu User từ type file
export interface User {
    id: string;
    firstName: string;
    lastName: string;
    email: string;
    roleName: string;
  }
export type authContextType = {
    user: User | undefined;
    token : string | undefined;
    login: (email: string, password: string) => Promise<void>;
    logout: () => Promise<void>;
};

const authContextDefaultValues: authContextType = {
    user: undefined,
    token: undefined,
    login: async () => {},
    logout: async () => {},
};

const AuthContext = createContext<authContextType>(authContextDefaultValues);

export function useAuth() {
    return useContext(AuthContext);
}

type Props = {
    children: ReactNode;
};

export function AuthProvider({ children }: Props) {
    const [user, setUser] = useState<User | undefined>();
    const [token, setToken] = useState<string | undefined>();

    useEffect(() => {
        const token = localStorage.getItem('token');
        if (token) {
            fetchUserData(token);
        } else {
            
        }
    }, []); // Chạy một lần khi component mount

    const fetchUserData = async (token:string) => {
        const userInfo = await getUserInfo(token);
        if (userInfo) {
            setUser(userInfo);
            setToken(token);
        } else {
            setUser(undefined); // Xử lý khi không lấy được thông tin người dùng
            setToken(undefined);
        }
    };

    // Hàm đăng nhập
    const login = async (email: string, password: string) => {
        try {
            const response = await apiLogin(email, password); // Gọi hàm login từ apiService
            // const userData: User = response.user;
            const token = response.token;
            localStorage.setItem('token', token);

            const userResponse = await getUserInfo(token); // Hàm gọi API getMe, truyền theo token

            console.log('userResponse: ', userResponse)
            const userData: User = userResponse;

            console.log('userData: ', userData)

            // localStorage.setItem('token', token);
            // const roleFromToken = getRoleFromToken(token);
            if (userResponse !== null) {
                localStorage.setItem('user', JSON.stringify(userResponse))
                localStorage.setItem('role', userData.roleName);
            }
            setUser(userData);
            setToken(token);

        } catch (error) {
            console.error("Đăng nhập thất bại:", error);
            throw error; // Ném lỗi để xử lý bên ngoài nếu cần
        }
    };

    // Hàm đăng xuất
    const logout = async () => {
        try {
            if (token !== undefined){
                console.log("TOKEN NOT UNDEFINE!" + token);
                const result = await apiLogout(token);
                if (result.status === 200){
                    localStorage.clear();
                    setUser(undefined);
                    setToken(undefined); 
                }
            }
        }
        catch (error: any){
            console.error("Đăng xuất thất bại:", error);
            throw error; // Ném lỗi để xử lý bên ngoài nếu cần
        }
        
        // Bạn có thể thêm yêu cầu API để thông báo đã đăng xuất nếu cần
    };

    const value = {
        user,
        token,
        login,
        logout,
    };
    
    return (
        <AuthContext.Provider value={value}>
            {children}
        </AuthContext.Provider>
    );
}