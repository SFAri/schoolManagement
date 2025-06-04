import { User } from '../hooks/useAuth';
import axiosInstance from './custom-axios'; // Nhập instance Axios đã cấu hình

export interface LoginResponse {
    token: string;
    user: any; // Thay thế bằng kiểu dữ liệu chính xác
}

export interface Product {
    id: number;
    name: string;
    price: number;
    // Bạn có thể thêm các thuộc tính khác ở đây
}

// Hàm đăng nhập
export const login = async (email: string, password: string): Promise<LoginResponse> => {
    const response = await axiosInstance.post('/User/login', { email, password });
    return response.data; 
};

// Hàm lấy thông tin người dùng
export const getUserInfo = async (token: string): Promise<User> => {
    const response = await axiosInstance.get('/User/me', {
        headers: {
            Authorization: `Bearer ${token}`,
        },
    });
    return response.data; // Trả về thông tin người dùng
};

// Hàm đăng xuất
export const logout = async (token: string): Promise<any> => {
    const response = await axiosInstance.post('/User/logout', {},{
        headers: {
            
            Authorization: `Bearer ${token}`,
        },
    });
    return response;
};