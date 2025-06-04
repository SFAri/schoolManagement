// utils/auth.ts (tạo file này)
import {jwtDecode} from 'jwt-decode';

interface DecodedToken {
    role: string;
}

export const getRoleFromToken = (token: string | null): string | null => {
    if (!token) return null;

    try {
        const decoded = jwtDecode <DecodedToken>(token);
        return decoded.role; // Trả về role từ payload
    } catch (error) {
        console.error('Token không hợp lệ:', error);
        return null;
    }
};